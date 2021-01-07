using NepseClient.Commons.Contracts;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;

using Serilog;

using System;
using System.Security.Authentication;

using TradeManagementSystemClient.Extensions;
using TradeManagementSystemClient.Models;
using TradeManagementSystemClient.Models.Requests;
using TradeManagementSystemClient.Models.Responses;
using TradeManagementSystemClient.Utils;

namespace TradeManagementSystemClient
{
    public class TmsClient : IDisposable
    {
        private bool _isAuthenticated;
        private AuthenticationDataResponse _authData;

        public RestClient Client { get; }
        public Func<AuthenticationRequest> PromptCredentials { get; set; }

        public TmsClient(IConfiguration config)
        {
            Client = new RestClient(config.Tms.BaseUrl)
            {
                CookieContainer = new System.Net.CookieContainer(),
            };
            Client.UseNewtonsoftJson(new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy(),
                },
            });
        }

        #region UnAuthorized Access
        public string GetBusinessDate()
        {
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
            Client.BaseUrl = body.BaseUrl;
            var request = new RestRequest("/tmsapi/authenticate");
            request.AddJsonBody(body);

            var response = Client.Post<ResponseBase<AuthenticationDataResponse>>(request);
            if (!response.IsSuccessful)
            {
                _isAuthenticated = false;
                throw new AuthenticationException(response.Content);
            }

            _authData = response.Data.Data;
            CookieUtils.ParseCookies(response, Client.CookieContainer, Client.BaseUrl);
            Client.Authenticator = new TmsAuthenticator();
            Log.Debug("Signed In");
            _isAuthenticated = true;
        }
        public void SignOut()
        {
            Log.Debug("Signing out from Tms");
            if (!_isAuthenticated)
            {
                Log.Debug("Not authenticated. No sign out required.");
                return;
            }
            Log.Debug("Signing out");
            var request = new RestRequest("/tmsapi/authenticate/logout");
            var response = Client.Post<ResponseBase>(request);
            Log.Information(response.Data.Message);
            _isAuthenticated = false;
            Log.Debug("Signed out from Tms");
        }
        #endregion
        public void Dispose()
        {
            SignOut();
        }

        #region Helper Methods
        private IRestResponse<T> AuthorizedGet<T>(IRestRequest request)
        {
            if (!_isAuthenticated)
            {
                Authorize();
            }
            var response = Client.Get<T>(request);
            if (response.IsUnAuthorized())
            {
                Authorize();
                return Client.Get<T>(request);
            }
            return response;
        }
        private IRestResponse<T> AuthorizedPost<T>(IRestRequest request)
        {
            var response = Client.Post<T>(request);
            if (response.IsUnAuthorized())
            {
                Authorize();
                return Client.Post<T>(request);
            }
            return response;
        } 
        #endregion
    }
}
