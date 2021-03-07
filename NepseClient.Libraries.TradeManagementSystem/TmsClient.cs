using NepseClient.Commons.Contracts;
using NepseClient.Commons.Extensions;
using NepseClient.Commons.Interfaces;
using NepseClient.Commons.Utils;
using NepseClient.Libraries.TradeManagementSystem.Models;
using NepseClient.Libraries.TradeManagementSystem.Requests;
using NepseClient.Libraries.TradeManagementSystem.Responses;

using Newtonsoft.Json;

using RestSharp;
using RestSharp.Authenticators;

using Serilog;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;

namespace NepseClient.Libraries.TradeManagementSystem
{
    public class TmsClient : IRestAuthorizableAsync
    {
        private readonly ITmsConfiguration _configuration;
        private readonly IStorage _storage;
        private readonly string _sessionKey = "__usrsession__";

        public AuthenticationDataResponse AuthData
        {
            get
            {
                var success = _storage.LocalStorage.TryGetValue(_sessionKey, out var value);
                if (success) return JsonConvert.DeserializeObject<AuthenticationDataResponse>(value);

                IsAuthenticated = false;
                return null;
            }
        }
        public bool IsAuthenticated { get; set; }
        public IRestClient Client { get; private set; }

        public TmsClient(IConfiguration config, IStorage storage)
        {
            _configuration = config.Tms;
            _storage = storage;
            Client = RestClientUtils.CreateNewClient(config.Tms.BaseUrl);
            Client.CookieContainer = _storage.Container;
            IsAuthenticated = AuthData is not null;
            if(IsAuthenticated)
                Client.Authenticator = GetAuthenticator(AuthData);
        }

        private IAuthenticator GetAuthenticator(AuthenticationDataResponse authData)
        {
            if (authData is null || authData.IsCookieEnabled)
                return new XsrfTokenAuthenticator(GetXsrfToken(Client));

            return new JwtAuthenticator(authData.JsonWebToken);
        }

        private static string GetXsrfToken(IRestClient client)
        {
            // TODO: manage cookies
            var collection = client.CookieContainer.GetCookies(client.BaseUrl);
            var cookies = collection.FirstOrDefault(x => x.Name.Equals("XSRF-TOKEN"));
            return cookies?.Value;
        }

        public void UpdateBaseUrl()
        {
            IsAuthenticated = false;
            Client = RestClientUtils.CreateNewClient(_configuration.BaseUrl);
        }

        #region UnAuthorized Access
        public string GetBusinessDate()
        {
            Log.Debug("Getting business date");
            var request = new RestRequest("/tmsapi/dashboard/businessDate");
            var response = Client.Get<string>(request);
            return response.Data;
        }

        public DateTime GetLastUpdatedDate()
        {
            var request = new RestRequest("/tmsapi/orderApi/stock/last-updated-time");
            var response = Client.Get<LastUpdatedDateResponse>(request);
            return response.Data.Date;
        }
        #endregion

        #region Authentication

        public async Task SignInAsync(CancellationToken ct = default)
        {
            var url = _configuration.BaseUrl;
            var dialog = new Ookii.Dialogs.Wpf.CredentialDialog
            {
                MainInstruction = $"Please provide tms credentials for {url}",
                Content = "Enter your username and password provided by your broker",
                WindowTitle = "Input TMS Credentials",
                Target = url,
                UseApplicationInstanceCredentialCache = false,
                ShowSaveCheckBox = true,
                ShowUIForSavedCredentials = true,
            };
            using (dialog)
            {
                if (dialog.ShowDialog())
                {
                    var username = dialog.UserName;
                    var password = dialog.Password;
                    await SignInAsync(url, username, password, ct);
                    dialog.ConfirmCredentials(true);
                }
            }
        }
        private async Task SignInAsync(string url, string username, string password, CancellationToken ct)
        {
            await SignOutAsync(ct);
            Log.Debug("Signing in");

            var request = new RestRequest("/tmsapi/authenticate");
            var json = new TmsAuthenticationRequest(username, password);
            request.AddJsonBody(json);

            Client = RestClientUtils.CreateNewClient(url);
            var response = await Client.ExecutePostAsync<ResponseBase<AuthenticationDataResponse>>(request, ct);
            if (!response.IsSuccessful)
            {
                Log.Warning(response.Content);
                throw new AuthenticationException(response.Data.Message);
            }
            _storage.LocalStorage[_sessionKey] = JsonConvert.SerializeObject(response.Data.Data);
            foreach (var cookie in response.Cookies)
            {
                _storage.Container.Add(Client.BaseUrl, new System.Net.Cookie(cookie.Name, cookie.Value));
            }
            Client.CookieContainer = _storage.Container;
            IsAuthenticated = true;
            _storage.Save();

            Client.Authenticator = GetAuthenticator(AuthData);
            Log.Debug("Signed In");
        }

        public async Task SignOutAsync(CancellationToken ct = default)
        {
            Log.Debug("Signing out from Tms");
            if (IsAuthenticated && AuthData.IsCookieEnabled)
            {
                Log.Debug("Signing out");
                var request = new RestRequest("/tmsapi/authenticate/logout");
                var response = await Client.ExecutePostAsync<ResponseBase>(request, ct);
                Log.Information(response.Data.Message);
            }
            IsAuthenticated = false;
            Log.Debug("Signed out from Tms");
        }
        #endregion

        #region Broker Back Office
        public Task<PortfolioResponse[]> GetPortfolioAsync()
        {
            //var id = AuthData.User.Id;
            //var request = new RestRequest($"/tmsapi/dp-holding/client/freebalance/{id}/CLI");
            var request = new RestRequest("/tmsapi/dp-holding/client/freebalance/1979933/CLI");

            //var res = Client.Get(request);
            //var client = RestClientUtils.CreateNewClient("https://tms49.nepsetms.com.np/");
            //client.Authenticator = new CachedTmsAuthenticator();

            //var response = client.Get(request);


            return Client.GetAsync<PortfolioResponse[]>(request);
        }
        #endregion

        #region Market
        public Task<WebSocketResponse<WsSecurityResponse>> GetLiveMarketAsync(CancellationToken ct = default)
        {
            var request = new RestRequest("/tmsapi/rtApi/ws/top25securities");
            return this.AuthorizeGetAsync<WebSocketResponse<WsSecurityResponse>>(request, ct);
        }
        #endregion
    }
}
