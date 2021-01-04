using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Authentication;

using NepseService.TradeManagementSystem.Models.Requests;
using NepseService.TradeManagementSystem.Models.Responses;

using Newtonsoft.Json.Serialization;

using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;

namespace NepseService.TradeManagementSystem
{
    public class MeroshareClient : IDisposable
    {
        private readonly string _clientId;
        private readonly string _username;
        private readonly string _password;
        public IRestClient Client { get; }
        public MeroshareClient(string baseUrl, string clientId, string username, string password)
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
            _clientId = clientId;
            _username = username;
            _password = password;
        }

        #region Authorization
        public void SignIn(string clientId, string username, string password)
        {
            Log.Debug("Signing In");
            var request = new RestRequest("/api/meroShare/auth/");
            request.AddJsonBody(new MeroshareAuthRequest(clientId, username, password));

            var response = Client.Post(request);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new AuthenticationException(response.Content);
            }
            else
            {
                var authHeader = response.Headers.FirstOrDefault(x => x.Name.Equals("Authorization"));
                if (authHeader == null)
                {
                    throw new AuthenticationException(response.Content);
                }
                else
                {
                    var header = authHeader.Value?.ToString();
                    var authenticator = new MeroshareAuthenticator(header);
                    Client.Authenticator = authenticator;
                }
            }
            Log.Debug("Signed In");
        }

        public void SignOut()
        {
            Log.Debug("Signing out");
            var request = new RestRequest("/api/meroShare/auth/logout/");
            var response = Client.Get(request);
            Log.Debug(response.Content);
            Log.Debug("Signed out");
        }
        #endregion

        public void Dispose()
        {
            SignOut();
        }

        public IEnumerable<string> GetMyShares()
        {
            Log.Debug("Getting my shares");
            var request = new RestRequest("/api/myPurchase/myShare/");
            var response = Client.Get<string[]>(request);
            if (IsUnAuthorized(response))
            {
                Authorize();
                response = Client.Get<string[]>(request);
            }
            return response.Data;
        }

        public IEnumerable<MeroshareViewMyPurchaseResponse> ViewMyPurchase(string demat, IEnumerable<string> scrips)
        {
            Log.Debug("View purchase for demat: {0}", demat);
            foreach (var scrip in scrips)
            {
                var request = new RestRequest("/api/myPurchase/view");
                request.AddJsonBody(new { Demat = demat, Scrip = scrip });

                var response = Client.Post<MeroshareViewMyPurchaseResponse>(request);
                if (IsUnAuthorized(response))
                {
                    Authorize();
                    response = Client.Post<MeroshareViewMyPurchaseResponse>(request);
                }
                if (response.Data is null || string.IsNullOrEmpty(response.Data.ScripName))
                {
                    Log.Debug("Data not found for {0}", scrip);
                    continue;
                }
                else
                {
                    yield return response.Data;
                }
            }

        }

        public IEnumerable<MeroshareSearchMyPurchaseRespose[]> SearchMyPurchase(string demat, IEnumerable<string> scrips)
        {
            Log.Debug("Search purchase for demat: {0}", demat);
            foreach (var scrip in scrips)
            {
                var request = new RestRequest("/api/myPurchase/search");
                request.AddJsonBody(new { Demat = demat, Scrip = scrip });

                var response = Client.Post<MeroshareSearchMyPurchaseRespose[]>(request);
                if (IsUnAuthorized(response))
                {
                    Authorize();
                    response = Client.Post<MeroshareSearchMyPurchaseRespose[]>(request);
                }
                if (response.Data is null)
                {
                    Log.Debug("Data not found for {0}", scrip);
                    continue;
                }
                else
                {
                    yield return response.Data;
                }
            }

        }

        private static bool IsUnAuthorized(IRestResponse response)
        {
            return response.StatusCode == HttpStatusCode.Unauthorized;
        }

        public virtual void Authorize()
        {
            Log.Debug("Authorizing...");
            SignIn(_clientId, _username, _password);
            Log.Debug("Authorized");
        }
    }
}
