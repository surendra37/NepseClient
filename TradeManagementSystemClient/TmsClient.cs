using NepseClient.Commons;
using NepseClient.Commons.Contracts;

using Newtonsoft.Json;

using RestSharp;

using Serilog;

using System;
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
        private readonly string _cookiPath = Path.Combine(Constants.AppDataPath.Value, "tms-cookies.dat");
        private readonly string _dataPath = "data.dat";

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
            if (AuthData is null) IsAuthenticated = false;
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
            SignOut();
        }

        public WebSocketResponse<WsSecurityResponse> GetSecurities()
        {
            Log.Debug("Getting securities");
            var request = new RestRequest("/tmsapi/ws/top25securities");
            var response = this.AuthorizedGet<WebSocketResponse<WsSecurityResponse>>(request);
            return response.Data;
        }
    }
}
