using System.Net;
using System.Security.Authentication;

using NepseService.TradeManagementSystem.Models.Requests;
using NepseService.TradeManagementSystem.Utils;

using Newtonsoft.Json.Serialization;

using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;
using NepseService.TradeManagementSystem.Models.Responses;
using System;
using Microsoft.Extensions.Configuration;

namespace NepseService.TradeManagementSystem
{
    public class TmsClient : IDisposable
    {
        private readonly string _username;
        private readonly string _password;
        public IRestClient Client { get; }
        public TmsClient(IConfiguration config)
            : this(config["Tms:Host"], config["Tms:Username"], config["Tms:Password"])
        {

        }

        public TmsClient(string baseUrl, string username, string password)
        {
            Client = new RestClient(baseUrl)
            {
                CookieContainer = new CookieContainer(),
                Authenticator = new TmsAuthenticator(),
            };
            Client.UseNewtonsoftJson(new Newtonsoft.Json.JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy(),
                }
            });
            _username = username;
            _password = password;
        }

        #region Authorization
        private void SignIn(string username, string password)
        {
            Log.Debug("Signing in");
            var request = new RestRequest("/tmsapi/authenticate");
            var body = new AuthenticationRequest(username, password);
            request.AddJsonBody(body);

            var response = Client.Post<ResponseBase>(request);
            if (!response.IsSuccessful)
            {
                throw new AuthenticationException(response.Content);
            }

            CookieUtils.ParseCookies(response, Client.CookieContainer, Client.BaseUrl);
            Log.Debug("Signed In");
        }

        public void SignOut()
        {
            Log.Debug("Signing out");
            var request = new RestRequest("/tmsapi/authenticate/logout");
            var response = Client.Post<ResponseBase>(request);
            Log.Information(response.Data.Message);
        }
        #endregion

        public void Dispose()
        {
            SignOut();
        }

        public string GetBusinessDate()
        {
            Log.Debug("Getting business date");
            var request = new RestRequest("/tmsapi/dashboard/businessDate");
            var response = Client.Get<string>(request);
            return response.Data;
        }

        public ScripReponse[] GetMyPortfolio(string clientId, bool retry = true)
        {
            Log.Debug("Getting my portfolio");
            var request = new RestRequest($"/tmsapi/dp-holding/client/freebalance/{clientId}/CLI");

            var response = Client.Get<ScripReponse[]>(request);
            if (retry && IsUnAuthorized(response))
            {
                Authorize();
                Log.Debug("Retrying after authorization get my portfolio");
                return GetMyPortfolio(clientId, false);
            }
            return response.Data;
        }

        private static bool IsUnAuthorized(IRestResponse response)
        {
            return response.StatusCode == HttpStatusCode.Unauthorized;
        }

        public virtual void Authorize()
        {
            Log.Debug("Authorizing...");
            SignIn(_username, _password);
            Log.Debug("Authorized");
        }
    }
}
