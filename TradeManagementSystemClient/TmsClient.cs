using NepseClient.Commons.Contracts;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;

using Serilog;

using System;
using System.IO;
using System.Security.Authentication;

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
        private readonly string _cookiPath = "cookies.dat";
        private readonly string _dataPath = "data.dat";

        private AuthenticationDataResponse _authData;
        private ITmsConfiguration _config;

        public IRestClient Client { get; private set; }
        public Func<AuthenticationRequest> PromptCredentials { get; set; }

        public TmsClient(IConfiguration config)
        {
            _config = config.Tms;
            Client = CreateNewClient(_config.BaseUrl);
            RestoreSession();
        }
        private void RestoreSession()
        {
            Log.Debug("Restoring session");
            Client.CookieContainer = CookieUtils.ReadCookiesFromDisk(_cookiPath);
            if (File.Exists(_dataPath))
            {
                var json = File.ReadAllText(_dataPath);
                _authData = JsonConvert.DeserializeObject<AuthenticationDataResponse>(json);
                Client.Authenticator = new TmsAuthenticator();
            }
        }
        private void SaveSession()
        {
            Log.Debug("Saving session");
            if (Client is not null)
                CookieUtils.WriteCookiesToDisk(_cookiPath, Client.CookieContainer);
            if (_authData is not null)
            {
                var json = JsonConvert.SerializeObject(_authData);
                File.WriteAllText(_dataPath, json);
            }
        }
        private void ClearSession()
        {
            Log.Debug("Clearing session");
            Client = null;
            _authData = null;
        }

        private IRestClient CreateNewClient(string baseUrl)
        {
            Log.Debug("Creating new client");
            var output = new RestClient(baseUrl)
            {
                CookieContainer = new System.Net.CookieContainer(),
            };
            output.UseNewtonsoftJson(new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy(),
                },
            });
            return output;
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
            SignIn(cred);
            Log.Debug("Authorized");
        }
        private void SignIn(AuthenticationRequest body)
        {
            Log.Debug("Signing in");
            ClearSession();
            Client = CreateNewClient(_config.BaseUrl);
            var request = new RestRequest("/tmsapi/authenticate");
            request.AddJsonBody(body);

            var response = Client.Post<ResponseBase<AuthenticationDataResponse>>(request);
            if (!response.IsSuccessful)
            {
                ClearSession();
                throw new AuthenticationException(response.Content);
            }

            _authData = response.Data.Data;
            CookieUtils.ParseCookies(response, Client.CookieContainer, Client.BaseUrl);
            Client.Authenticator = new TmsAuthenticator();
            SaveSession();
            Log.Debug("Signed In");
        }
        public void SignOut()
        {
            Log.Debug("Signing out from Tms");
            if (Client is not null)
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
