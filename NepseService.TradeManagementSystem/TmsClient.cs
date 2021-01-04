using System.Net;
using System.Security.Authentication;

using NepseService.TradeManagementSystem.Models.Requests;
using NepseService.TradeManagementSystem.Utils;

using Newtonsoft.Json.Serialization;

using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;
using System.IO;
using NepseService.TradeManagementSystem.Models.Responses;

namespace NepseService.TradeManagementSystem
{
    public class TmsClient
    {
        public string CookiePath { get; init; } = "cookies.dat";
        public bool UseCookies { get; init; } = true;
        public IRestClient Client { get; }

        public TmsClient(string baseUrl)
        {
            Client = new RestClient(baseUrl)
            {
                CookieContainer = UseCookies ? CookieUtils.ReadCookiesFromDisk(CookiePath) : new CookieContainer(),
                Authenticator = new TmsAuthenticator(),
            };
            Client.UseNewtonsoftJson(new Newtonsoft.Json.JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy(),
                }
            });
        }

        public void SignIn(string username, string password)
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

            if (UseCookies)
            {
                CookieUtils.WriteCookiesToDisk(CookiePath, Client.CookieContainer);
            }
        }

        public void SignOut()
        {
            Log.Debug("Signing out");
            var request = new RestRequest("/tmsapi/authenticate/logout");
            var response = Client.Post<ResponseBase>(request);
            Log.Information(response.Data.Message);

            if (UseCookies && File.Exists(CookiePath))
            {
                File.Delete(CookiePath);
            }
            Client.CookieContainer = new CookieContainer();
        }
    }
}
