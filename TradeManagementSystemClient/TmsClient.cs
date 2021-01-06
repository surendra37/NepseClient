using NepseClient.Commons;
using NepseClient.Commons.Contracts;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;

using Serilog;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private readonly ITmsConfiguration _config;
        private IDictionary<string, float> _waccDict;
        private AuthenticationDataResponse _authData;

        public RestClient Client { get; }
        public Func<AuthenticationRequest> PromptCredentials { get; set; }

        public TmsClient(IConfiguration config)
        {
            _config = config.Tms;
            Client = new RestClient(_config.BaseUrl)
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
            LoadWacc();
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

        public ScripResponse[] GetMyPortfolios()
        {
            if (!_isAuthenticated) Authorize();

            var clientId = _authData.ClientDealerMember.Client.Id;
            var request = new RestRequest($"/tmsapi/dp-holding/client/freebalance/{clientId}/CLI");
            var response = AuthorizedGet<ScripResponse[]>(request);
            // Update Wacc Value
            foreach (var scrip in response.Data)
            {
                var key = scrip.Scrip;
                if (_waccDict.ContainsKey(key))
                {
                    scrip.WaccValue = _waccDict[key];
                    if (scrip.LastTransactionPrice == 0)
                    {
                        scrip.LastTransactionPrice = scrip.WaccValue;
                        scrip.PreviousClosePrice = scrip.LastTransactionPrice;
                        scrip.LTPTotal = scrip.WaccValue * scrip.TotalBalance;
                        scrip.PreviousTotal = scrip.LTPTotal;
                    }
                }
                else
                {
                    _waccDict.Add(key, scrip.WaccValue);
                }
            }
            return response.Data;
        }

        public ISecurityItem[] GetLiveMarket()
        {
            var request = new RestRequest("/tmsapi/ws/top25securities");

            var response = AuthorizedGet<SocketResponse<SecurityItem2>>(request);
            return response.Data.Payload.Data
                .OrderByDescending(x => x.LastTradedDateTime)
                .ToArray();
        }

        public MarketWatchResponse[] GetMarketWatch()
        {
            if(!_isAuthenticated) Authorize();

            var request = new RestRequest($"/tmsapi/market-watch/user/{_authData.User.Id}");
            var response = AuthorizedGet<MarketWatchResponse[]>(request);
            return response.Data;
        }

        #region Top
        public ITopResponse[] GetTopGainers()
        {
            var request = new RestRequest("/tmsapi/stock/top/gainer/8");
            var response = AuthorizedGet<TopResponse[]>(request);
            return response.Data;
        }
        public ITopResponse[] GetTopLosers()
        {
            var request = new RestRequest("/tmsapi/stock/top/loser/8");
            var response = AuthorizedGet<TopResponse[]>(request);
            return response.Data;
        }
        public ITopSecuritiesResponse[] GetTopTurnovers()
        {
            var request = new RestRequest("/tmsapi/stock/top-securities/turnover/9");
            var response = AuthorizedGet<TopSecuritiesResponse[]>(request);
            return response.Data;
        }
        public ITopSecuritiesResponse[] GetTopTransactions()
        {
            var request = new RestRequest("/tmsapi/stock/top-securities/transaction/9");
            var response = AuthorizedGet<TopSecuritiesResponse[]>(request);
            return response.Data;
        }
        public ITopSecuritiesResponse[] GetTopVolumes()
        {
            var request = new RestRequest("/tmsapi/stock/top-securities/volume/9");
            var response = AuthorizedGet<TopSecuritiesResponse[]>(request);
            return response.Data;
        }
        #endregion

        #region WebSocket WS
        public IStockQuoteResponse[] GetStockQuote(string id)
        {
            var request = new RestRequest($"/tmsapi/ws/stockQuote/{id}");
            var response = AuthorizedGet<SocketResponse<StockQuoteResponse>>(request);
            return response.Data.Payload.Data;
        }
        public IIndexResponse[] GetIndices()
        {
            var request = new RestRequest("/tmsapi/ws/index");
            var response = AuthorizedGet<SocketResponse<IndexResponse>>(request);
            return response.Data.Payload.Data;
        }
        public IOHLCDataResponse[] GetOHLCPortfolio()
        {
            if(_isAuthenticated) Authorize();
            var request = new RestRequest($"/tmsapi/ws/clientPortfolio/{_authData.ClientDealerMember.Client.Id}");
            var response = AuthorizedGet<SocketResponse<OHLCDataResponse>>(request);
            return response.Data.Payload.Data;
        }
        #endregion

        public void LoadWacc()
        {
            var path = Path.Combine(Constants.AppDataPath.Value, "wacc.json");
            if (!File.Exists(path)) return;
            var json = File.ReadAllText(path);
            var waccs = JsonConvert.DeserializeObject<MeroshareViewMyPurchaseResponse[]>(json);
            _waccDict = waccs.ToDictionary(x => x.ScripName, x => (float)x.AverageBuyRate);
        }

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

        public string GetSocketUrl()
        {
            return null;
            //if (Session.CookieEnabled)
            //{
            //    return $"wss://{Client.BaseUrl.Host}/tmsapi/socketEnd?memberId={Session.MemberId}&clientId={Session.ClientId}&dealerId={Session.DealerId}&userId={Session.UserId}";
            //}
            //else
            //{
            //    return $"wss://{Client.BaseUrl.Host}/tmsapi/socketEnd?memberId={Session.MemberId}&clientId={Session.ClientId}&dealerId={Session.DealerId}&userId={Session.UserId}&access_token={Session.AccessToken}";
            //}
        }

        public List<KeyValuePair<string, string>> GetCookies()
        {
            return null;
            //if (!Session.CookieEnabled) return null;

            //var output = new List<KeyValuePair<string, string>>();
            //if (!string.IsNullOrEmpty(Session.XsrfToken))
            //{
            //    output.Add(new KeyValuePair<string, string>("XSRF-TOKEN", Session.XsrfToken));
            //}

            //if (!string.IsNullOrEmpty(Session.AccessToken))
            //{
            //    output.Add(new KeyValuePair<string, string>("accessToken", Session.AccessToken));
            //}

            //if (!string.IsNullOrEmpty(Session.RefreshToken))
            //{
            //    output.Add(new KeyValuePair<string, string>("refreshToken", Session.RefreshToken));
            //}

            //return output.Count > 0 ? output : null;
        }
    }
}
