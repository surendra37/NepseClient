using NepseClient.Commons;
using NepseClient.Commons.Constants;
using NepseClient.Commons.Contracts;

using Newtonsoft.Json;

using RestSharp;
using RestSharp.Authenticators;

using Serilog;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using TradeManagementSystemClient.Extensions;
using TradeManagementSystemClient.Interfaces;
using TradeManagementSystemClient.Models;
using TradeManagementSystemClient.Models.Requests;
using TradeManagementSystemClient.Models.Responses;
using TradeManagementSystemClient.Models.Responses.Tms;
using TradeManagementSystemClient.Utils;

namespace TradeManagementSystemClient
{
    public class TmsClient : IAuthorizable, IDisposable
    {
        private readonly string _cookiPath = Path.Combine(PathConstants.AppDataPath.Value, "tms-cookies.dat");
        private readonly string _dataPath = Path.Combine(PathConstants.AppDataPath.Value, "tms-data.dat");

        public AuthenticationDataResponse AuthData { get; private set; }
        public bool IsAuthenticated { get; set; }
        private ITmsConfiguration _config;

        public IRestClient Client { get; private set; }
        public Func<TmsAuthenticationRequest> PromptCredentials { get; set; }

        public TmsClient(IConfiguration config)
        {
            _config = config.Tms;

            ValidateBaseUrl();
            Client = RestClientUtils.CreateNewClient(_config.BaseUrl);
            Client.Authenticator = new TmsAuthenticator();

            RestoreSession();
        }

        private void ValidateBaseUrl()
        {
            if (string.IsNullOrEmpty(_config.BaseUrl))
            {
                Log.Error("Tms Base url is empty. Settings 'https://tms49.nepsetms.com.np' as default. Update the base url in settings and restart the application");
                _config.BaseUrl = "https://tms49.nepsetms.com.np";
            }
        }
        private void RestoreSession()
        {
            Log.Debug("Restoring session");

            Client.CookieContainer = CookieUtils.ReadCookiesFromDisk(_cookiPath);
            AuthData = AuthenticationDataResponse.NewInstance(_dataPath);
            IsAuthenticated = AuthData is not null;

            Client.Authenticator = GetAuthenticator(AuthData);
        }
        private void SaveSession()
        {
            Log.Debug("Saving session");
            if (Client is not null)
                CookieUtils.WriteCookiesToDisk(_cookiPath, Client.CookieContainer);
            if (AuthData is not null)
            {
                var json = JsonConvert.SerializeObject(AuthData);
                File.WriteAllText(_dataPath, json);

            }
            Client.Authenticator = GetAuthenticator(AuthData);
        }

        private static IAuthenticator GetAuthenticator(AuthenticationDataResponse authData)
        {
            if (authData is null || authData.IsCookieEnabled) return new TmsAuthenticator();

            return new BearerAuthenticator(authData.JsonWebToken);
        }

        private void ClearSession()
        {
            Log.Debug("Clearing session");
            Client.CookieContainer = new System.Net.CookieContainer();
            AuthData = null;
            IsAuthenticated = false;
        }

        #region UnAuthorized Access
        public string GetBusinessDate()
        {
            Log.Debug("Getting business date");
            var request = new RestRequest("/tmsapi/dashboard/businessDate");
            var response = Client.Get<string>(request);
            return response.Data;
        }
        #endregion

        #region Authentication
        public virtual void Authorize()
        {
            Log.Debug("Authorizing");
            var cred = PromptCredentials?.Invoke();
            if (cred is null) throw new AuthenticationException("Authentication cancelled");
            SignIn(cred);
            Log.Debug("Authorized");
        }
        private void SignIn(TmsAuthenticationRequest body)
        {
            Log.Debug("Signing in");
            ClearSession();
            var request = new RestRequest("/tmsapi/authenticate");
            request.AddJsonBody(body);

            var response = Client.Post<ResponseBase<AuthenticationDataResponse>>(request);
            if (!response.IsSuccessful)
            {
                Log.Error(response.Content);
                throw new AuthenticationException(response.Data.Message);
            }
            AuthData = response.Data.Data;
            CookieUtils.ParseCookies(response, Client.CookieContainer, Client.BaseUrl);
            SaveSession();
            IsAuthenticated = true;
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
            ClearSession();
            SaveSession();
            Log.Debug("Signed out from Tms");
        }
        #endregion
        public void Dispose()
        {
#if DEBUG
            //SignOut();
#else
            SignOut();
#endif
        }

        public WebSocketResponse<WsSecurityResponse> GetSecurities()
        {
            Log.Debug("Getting securities");
            var request = new RestRequest("/tmsapi/ws/top25securities");
            var response = this.AuthorizedGet<WebSocketResponse<WsSecurityResponse>>(request);
            return response.Data;
        }

        #region Graph 
        public GraphDataResponse[] GetGraphData(params string[] isins)
        {
            var request = new RestRequest("/tmsapi/graph-data/fetch/1"); // NPE019A00007 
            foreach (var isin in isins)
            {
                request.AddParameter("ISIN", isin);
            }
            var data = this.AuthorizedGet<GraphDataResponse[]>(request);
            return data.Data;
        }
        #endregion

        #region Top
        public TopGainerResponse[] GetTopGainers(int count = 10)
        {
            var api = new RestRequest($"/tmsapi/stock/top/gainer/{count}");

            var response= this.AuthorizedGet<TopGainerResponse[]>(api);
            return response.Data;
        }
        public TopGainerResponse[] GetTopLosers(int count = 8)
        {
            var api = new RestRequest($"/tmsapi/stock/top/loser/{count}");

            var response= this.AuthorizedGet<TopGainerResponse[]>(api);
            return response.Data;
        }

        public TopTurnoverResponse[] GetTopTurnover(int count = 9)
        {
            var api = new RestRequest($"/tmsapi/stock/top-securities/turnover/{count}");

            var response= this.AuthorizedGet<TopTurnoverResponse[]>(api);
            return response.Data;
        }
        public TopTurnoverResponse[] GetTopTransaction(int count = 9)
        {
            var api = new RestRequest($"/tmsapi/stock/top-securities/transaction/{count}");

            var response= this.AuthorizedGet<TopTurnoverResponse[]>(api);
            return response.Data;
        }
        public TopTurnoverResponse[] GetTopVolume(int count = 9)
        {
            var api = new RestRequest($"/tmsapi/stock/top-securities/volume/{count}");

            var response= this.AuthorizedGet<TopTurnoverResponse[]>(api);
            return response.Data;
        }
        #endregion
    }
}
