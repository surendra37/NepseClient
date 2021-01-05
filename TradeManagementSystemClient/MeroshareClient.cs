using NepseClient.Commons.Contracts;

using Newtonsoft.Json.Serialization;

using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;

using Serilog;

using SuperSocket.ClientEngine;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;

using TradeManagementSystemClient.Extensions;
using TradeManagementSystemClient.Models.Requests;
using TradeManagementSystemClient.Models.Responses;

namespace TradeManagementSystemClient
{
    public class MeroshareClient : IDisposable
    {
        private readonly IConfiguration _configuration;
        public IRestClient Client { get; set; }
        public Func<MeroshareAuthRequest> PromptCredential { get; set; }
        public MeroshareClient(IConfiguration configuration)
        {
            Client = new RestClient(configuration.Meroshare.BaseUrl)
            {
                CookieContainer = new System.Net.CookieContainer(),
            };
            Client.UseNewtonsoftJson(new Newtonsoft.Json.JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy(),
                },
            });
            _configuration = configuration;
        }

        #region Authentication
        public virtual void Authorize()
        {
            Log.Debug("Authorizing...");
            var cred = PromptCredential?.Invoke();
            if (cred is null) throw new AuthenticationException("Not Authorized");
            SignIn(cred);
            Log.Debug("Authorized");
        }
        private void SignIn(MeroshareAuthRequest credentials)
        {
            Log.Debug("Signing in...");
            var request = new RestRequest("/api/meroShare/auth/");
            request.AddJsonBody(credentials);

            var response = Client.Post(request);
            if (!response.IsSuccessful) throw new AuthenticationException(response.Content);

            var authHeader = response.Headers.FirstOrDefault(x => x.Name.Equals("Authorization"));
            if (authHeader is null) throw new AuthenticationException("Authorization header not found");
            var value = authHeader.Value?.ToString();
            Client.Authenticator = new MeroshareAuthenticator(value);

            // Save values
            Log.Debug("Signed In");
        }
        private void SignOut()
        {
            var request = new RestRequest("/api/meroShare/auth/logout/");
            var response = Client.Get(request);
            Client.Authenticator = null;
            Log.Debug(response.Content);
        }
        #endregion

        /// <summary>
        /// Get Depository Participants (Capitals)
        /// </summary>
        public MeroshareCapitalResponse[] GetCapitals()
        {
            Log.Debug("Getting capitals");
            var request = new RestRequest("/api/meroShare/capital");
            var response = Client.Get<MeroshareCapitalResponse[]>(request);
            return response.Data;
        }

        public MeroshareOwnDetailResponse GetOwnDetails()
        {
            Log.Debug("Getting own details");
            var request = new RestRequest("/api/meroShare/ownDetail/");
            var response = AuthorizedGet<MeroshareOwnDetailResponse>(request);
            return response.Data;
        }

        public string[] GetMyShares()
        {
            Log.Debug("Getting my shares");
            var request = new RestRequest("/api/myPurchase/myShare/");
            var response = AuthorizedGet<string[]>(request);
            return response.Data;
        }

        public MeroshareViewMyPurchaseResponse ViewMyPurchase(MeroshareViewMyPurchaseRequest body)
        {
            Log.Debug("View my purchase");
            var request = new RestRequest("/api/myPurchase/view/");
            request.AddJsonBody(body);
            var response = AuthorizedPost<MeroshareViewMyPurchaseResponse>(request);
            return response.Data;
        }

        public MeroshareSearchMyPurchaseRespose[] SearchMyPurchase(MeroshareViewMyPurchaseRequest body)
        {
            var request = new RestRequest("/api/myPurchase/search/");
            request.AddJsonBody(body);
            var response = AuthorizedPost<MeroshareSearchMyPurchaseRespose[]>(request);
            return response.Data;
        }

        public IEnumerable<MeroshareViewMyPurchaseResponse> GetWaccs(IEnumerable<string> scrips)
        {
            Log.Debug("Getting waccs");
            var me = GetOwnDetails();
            var request = new MeroshareViewMyPurchaseRequest
            {
                Demat = me.Demat,
            };
            foreach (var scrip in scrips)
            {
                request.Scrip = scrip;
                var view = ViewMyPurchase(request);
                var searches = SearchMyPurchase(request);
                if (searches.Length == 0)
                {
                    yield return view;
                }
                else
                {
                    var first = searches.First();
                    var myView = new MeroshareViewMyPurchaseResponse
                    {
                        Isin = first.Isin,
                        ScripName = first.Scrip,
                        TotalCost = searches.Sum(x => x.UserPrice * x.TransactionQuantity),
                        TotalQuantity = searches.Sum(x => x.TransactionQuantity),
                        //AverageBuyRate
                    };
                    myView.TotalCost += view.TotalCost;
                    myView.TotalQuantity += view.TotalQuantity;
                    myView.AverageBuyRate = view.TotalCost / view.TotalQuantity;

                    yield return myView;
                }

            }
        }

        public void Dispose()
        {
            SignOut();
        }

        private IRestResponse<T> AuthorizedGet<T>(IRestRequest request)
        {
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
    }
}
