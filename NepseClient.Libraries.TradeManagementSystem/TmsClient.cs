using NepseClient.Commons.Contracts;
using NepseClient.Commons.Utils;
using NepseClient.Libraries.TradeManagementSystem.Models;
using NepseClient.Libraries.TradeManagementSystem.Requests;
using NepseClient.Libraries.TradeManagementSystem.Responses;

using RestSharp;
using RestSharp.Authenticators;

using Serilog;

using System;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace NepseClient.Libraries.TradeManagementSystem
{
    public class TmsClient
    {
        private readonly ITmsConfiguration _configuration;
        public AuthenticationDataResponse AuthData { get; private set; }
        public bool IsAuthenticated { get; set; }
        public IRestClient Client { get; private set; }

        public TmsClient(IConfiguration config)
        {
            _configuration = config.Tms;
            Client = RestClientUtils.CreateNewClient(config.Tms.BaseUrl);
        }

        private void Authorize()
        {
            IsAuthenticated = false;

        }

        private IAuthenticator GetAuthenticator(AuthenticationDataResponse authData)
        {
            if (authData is null || authData.IsCookieEnabled) return new TmsAuthenticator(Client);

            return new BearerAuthenticator(authData.JsonWebToken);
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

        public async Task SignInAsync()
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
                    await SignInAsync(url, username, password);
                    dialog.ConfirmCredentials(true);
                }
            }
        }
        private async Task SignInAsync(string url, string username, string password)
        {
            SignOut();
            Log.Debug("Signing in");

            var request = new RestRequest("/tmsapi/authenticate");
            var json = new TmsAuthenticationRequest(username, password);
            request.AddJsonBody(json);

            Client = RestClientUtils.CreateNewClient(url);
            var response = await Client.ExecutePostAsync<ResponseBase<AuthenticationDataResponse>>(request);
            if (!response.IsSuccessful)
            {
                Log.Warning(response.Content);
                throw new AuthenticationException(response.Data.Message);
            }
            AuthData = response.Data.Data;
            IsAuthenticated = true;
            Client.Authenticator = GetAuthenticator(AuthData);
            Log.Debug("Signed In");
        }

        public void SignOut()
        {
            Log.Debug("Signing out from Tms");
            if (IsAuthenticated)
            {
                Log.Debug("Signing out");
                var request = new RestRequest("/tmsapi/authenticate/logout");
                var response = Client.Post<ResponseBase>(request);
                Log.Information(response.Data.Message);
            }
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
        public Task<WebSocketResponse<WsSecurityResponse>> GetLiveMarketAsync()
        {
            var request = new RestRequest("/tmsapi/rtApi/ws/top25securities");
            return AuthorizeGet<WebSocketResponse<WsSecurityResponse>>(request);
        }

        private async Task<T> AuthorizeGet<T>(IRestRequest request)
        {
            var response = await Client.ExecuteGetAsync<T>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                Authorize();
                response = await Client.ExecuteGetAsync<T>(request);
            }
            if (!response.IsSuccessful)
            {
                throw new Exception(response.Content, response.ErrorException);
            }

            return response.Data;
        }
        #endregion
    }
}
