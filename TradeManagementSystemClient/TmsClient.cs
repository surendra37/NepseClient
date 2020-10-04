using NepseClient.Commons;
using NepseClient.Commons.Contracts;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using TradeManagementSystemClient.Models.Requests;
using TradeManagementSystemClient.Models.Responses;

namespace TradeManagementSystemClient
{
    public class TmsClient : INepseClient
    {
        private readonly string _sessionFilePath;
        private IDictionary<string, float> _waccDict;

        private RestClient _client;
        public RestClient Client
        {
            get
            {
                if (_client is null)
                {
                    throw new AuthenticationException("Not Authorized");
                }
                return _client;
            }
            set { _client = value; }
        }

        private SessionInfo _session;
        public SessionInfo Session
        {
            get
            {
                if (_session is null)
                    throw new AuthenticationException("Not Authorized.");
                return _session;
            }
            private set { _session = value; }
        }
        public bool IsAuthenticated { get; private set; }

        public Action ShowAuthenticationDialog { get; set; }

        public TmsClient()
        {
            _sessionFilePath = Path.Combine(Constants.AppDataPath.Value, "tms.session");

            LoadWacc();
        }

        private RestClient CreateClient(string baseUrl)
        {
            var client = new RestClient(baseUrl);
            //client.ThrowOnDeserializationError = true;
            client.UseNewtonsoftJson();

            return client;
        }

        #region UnAuthorized Access
        public string GetBusinessDate()
        {
            var request = new RestRequest("/tmsapi/dashboard/businessDate");
            var response = Client.Get<string>(request);
            return response.Data;
        }
        #endregion

        #region Session
        public void SaveSession()
        {
            if (Session is null) return;

            var serialized = JsonConvert.SerializeObject(Session);
            File.WriteAllText(_sessionFilePath, serialized);
        }

        public void RestoreSession()
        {
            if (!File.Exists(_sessionFilePath))
                return;

            var session = File.ReadAllText(_sessionFilePath);
            Session = JsonConvert.DeserializeObject<SessionInfo>(session);
            Client = CreateClient(Session.Host);
            Client.Authenticator = new TmsAuthenticator(Session);

            IsAuthenticated = true;
            Log.Debug("Session restored [{0}]", Session.LastUpdated);
        }

        public void Logout()
        {
            var request = new RestRequest("/tmsapi/authenticate/logout");
            try
            {
                var response = Client.Post(request);
            }
            catch (Exception ex)
            {
                Log.Debug(ex, "Failed to logout");
            }

            Session = null;
            IsAuthenticated = false;
            Client.Authenticator = null;

            if (!File.Exists(_sessionFilePath))
                return;

            File.Delete(_sessionFilePath);
        }

        public Task AuthenticateAsync(string url, string username, string password)
        {
            Log.Debug("Authenticating...");
            Client = CreateClient(url);

            var request = new RestRequest("/tmsapi/authenticate");
            request.AddJsonBody(new AuthenticationRequest(username, password));
            Client.Authenticator = null;
            return Client.ExecutePostAsync<AuthenticationResponse>(request)
                 .ContinueWith(responseTask =>
                 {
                     var response = responseTask.Result;
                     if (!response.IsSuccessful)
                     {
                         throw new AuthenticationException(response.Content);
                     }
                     if (response.Data.Data.IsCookieEnabled)
                     {
                         var cookies = response.Headers.FirstOrDefault(x => x.Name.Equals("Set-Cookie"));
                         if (cookies is null)
                         {
                             throw new Exception("Cookies Not Found");
                         }
                         var parts = cookies.Value.ToString().Split(',');
                         Session = GetSessionInfo(cookies.Value.ToString(), response.Data.Data);
                     }
                     else
                     {
                         Session = GetSessionInfo(null, response.Data.Data);
                     }
                     Session.Host = url;
                     Session.Username = username;
                     Client.Authenticator = new TmsAuthenticator(Session);
                     Log.Debug("Authentication Complete");
                     Log.Debug("Authentication Response Message: {0}", response.Data?.Message);

                     IsAuthenticated = true;
                 });

        }
        #endregion

        public Task<IEnumerable<IScripResponse>> GetMyPortfolioAsync(CancellationToken ct = default)
        {
            if (!IsAuthenticated)
            {
                ShowAuthenticationDialog?.Invoke();
            }
            var request = new RestRequest($"/tmsapi/dp-holding/client/freebalance/{Session.ClientId}/CLI");

            return Client.ExecuteGetAsync<ScripResponse[]>(request, ct)
                .ContinueWith(EnsureAuthenticated, ct)
                .ContinueWith<IEnumerable<IScripResponse>>(task =>
                {
                    var response = task.Result;
                    // Update Wacc Value
                    foreach (var scrip in response)
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
                    return response;
                }, ct);
        }

        public Task<IEnumerable<ISecurityItem>> GetLiveMarketAsync(CancellationToken ct = default)
        {
            var request = new RestRequest("/tmsapi/ws/top25securities");

            return Client.ExecuteGetAsync<SocketResponse<SecurityItem2>>(request, ct)
                .ContinueWith(EnsureAuthenticated, ct)
                .ContinueWith<IEnumerable<ISecurityItem>>(task =>
                {
                    var response = task.Result;
                    return response.Payload.Data
                    .OrderByDescending(x => x.LastTradedDateTime)
                    .ToArray();
                }, ct);
        }

        public Task<IEnumerable<IMarketWatchResponse>> GetMarketWatchAsync(CancellationToken ct = default)
        {
            var request = new RestRequest($"/tmsapi/market-watch/user/{Session.UserId}");
            return Client.ExecuteGetAsync<MarketWatchResponse[]>(request, ct)
                .ContinueWith<IEnumerable<IMarketWatchResponse>>(EnsureAuthenticated, ct);
        }

        #region Top
        public Task<IEnumerable<ITopResponse>> GetTopGainersAsync(CancellationToken ct = default)
        {
            var request = new RestRequest("/tmsapi/stock/top/gainer/8");
            return Client.ExecuteGetAsync<TopResponse[]>(request, ct)
                .ContinueWith<IEnumerable<ITopResponse>>(EnsureAuthenticated, ct);
        }
        public Task<IEnumerable<ITopResponse>> GetTopLosersAsync(CancellationToken ct = default)
        {
            var request = new RestRequest("/tmsapi/stock/top/loser/8");
            return Client.ExecuteGetAsync<TopResponse[]>(request, ct)
                .ContinueWith<IEnumerable<ITopResponse>>(EnsureAuthenticated, ct);
        }

        public Task<IEnumerable<ITopSecuritiesResponse>> GetTopTurnoversAsync(CancellationToken ct = default)
        {
            var request = new RestRequest("/tmsapi/stock/top-securities/turnover/9");
            return Client.ExecuteGetAsync<TopSecuritiesResponse[]>(request, ct)
                .ContinueWith<IEnumerable<ITopSecuritiesResponse>>(EnsureAuthenticated, ct);
        }
        public Task<IEnumerable<ITopSecuritiesResponse>> GetTopTransactionsAsync(CancellationToken ct = default)
        {
            var request = new RestRequest("/tmsapi/stock/top-securities/transaction/9");
            return Client.ExecuteGetAsync<TopSecuritiesResponse[]>(request, ct)
                .ContinueWith<IEnumerable<ITopSecuritiesResponse>>(EnsureAuthenticated, ct);
        }
        public Task<IEnumerable<ITopSecuritiesResponse>> GetTopVolumesAsync(CancellationToken ct = default)
        {
            var request = new RestRequest("/tmsapi/stock/top-securities/volume/9");
            return Client.ExecuteGetAsync<TopSecuritiesResponse[]>(request, ct)
                .ContinueWith<IEnumerable<ITopSecuritiesResponse>>(EnsureAuthenticated, ct);
        }
        #endregion

        public Task<IEnumerable<IStockQuoteResponse>> GetStockQuoteAsync(string id, CancellationToken ct = default)
        {
            var request = new RestRequest($"/tmsapi/ws/stockQuote/{id}");
            return Client.ExecuteGetAsync<SocketResponse<StockQuoteResponse>>(request, ct)
                .ContinueWith(EnsureAuthenticated, ct)
                .ContinueWith<IEnumerable<IStockQuoteResponse>>(task =>
                {
                    var response = task.Result;
                    return response.Payload.Data;
                });
        }

        public Task<IEnumerable<IIndexResponse>> GetIndicesAsync(CancellationToken ct = default)
        {
            var request = new RestRequest("/tmsapi/ws/index");
            return Client.ExecuteGetAsync<SocketResponse<IndexResponse>>(request, ct)
                .ContinueWith(EnsureAuthenticated, ct)
                .ContinueWith<IEnumerable<IIndexResponse>>(task =>
                {
                    var response = task.Result;
                    return response.Payload.Data;
                });
        }

        public Task<ICachedDataResponse> GetCachedData(CancellationToken ct = default)
        {
            var request = new RestRequest("/tmsapi/external/cache");
            return Client.ExecuteGetAsync<CachedDataResponse>(request, ct)
                .ContinueWith<ICachedDataResponse>(EnsureAuthenticated, ct);
        }

        #region Helpers
        private SessionInfo GetSessionInfo(string cookie, AuthenticationDataResponse authData)
        {
            var session = new SessionInfo
            {
                XsrfToken = string.Empty,
                RefreshToken = string.Empty,
                AccessToken = string.Empty,
                LastUpdated = DateTime.Now,
                ClientId = authData.ClientDealerMember.Client.Id,
                DealerId = authData.ClientDealerMember.Dealer.Id,
                MemberId = authData.ClientDealerMember.Member.Id,
                UserId = authData.User.Id,
                CookieEnabled = authData.IsCookieEnabled,
                JsonWebToken = authData.JsonWebToken,
            };

            if (string.IsNullOrEmpty(cookie))
                return session;

            var parts = cookie.Split(',');
            foreach (var part in parts)
            {
                var innerParts = part.Split(';');
                var firstParts = innerParts[0].Split('=');
                var key = firstParts[0].Trim();
                var value = firstParts[1];
                switch (key)
                {
                    case "XSRF-TOKEN":
                        session.XsrfToken = value;
                        break;
                    case "accessToken":
                        session.AccessToken = value;
                        break;
                    case "refreshToken":
                        session.RefreshToken = value;
                        break;
                    default:
                        break;
                }
            }
            return session;
        }

        public string GetSocketUrl()
        {
            if (Session.CookieEnabled)
            {
                return $"wss://{Client.BaseUrl.Host}/tmsapi/socketEnd?memberId={Session.MemberId}&clientId={Session.ClientId}&dealerId={Session.DealerId}&userId={Session.UserId}";
            }
            else
            {
                return $"wss://{Client.BaseUrl.Host}/tmsapi/socketEnd?memberId={Session.MemberId}&clientId={Session.ClientId}&dealerId={Session.DealerId}&userId={Session.UserId}&access_token={Session.AccessToken}";
            }
        }

        public List<KeyValuePair<string, string>> GetCookies()
        {
            if (!Session.CookieEnabled) return null;

            var output = new List<KeyValuePair<string, string>>();
            if (!string.IsNullOrEmpty(Session.XsrfToken))
            {
                output.Add(new KeyValuePair<string, string>("XSRF-TOKEN", Session.XsrfToken));
            }

            if (!string.IsNullOrEmpty(Session.AccessToken))
            {
                output.Add(new KeyValuePair<string, string>("accessToken", Session.AccessToken));
            }

            if (!string.IsNullOrEmpty(Session.RefreshToken))
            {
                output.Add(new KeyValuePair<string, string>("refreshToken", Session.RefreshToken));
            }

            return output.Count > 0 ? output : null;
        }

        public bool IsLive()
        {
            var today = DateTime.Now;
            var offDay = today.DayOfWeek == DayOfWeek.Friday || today.DayOfWeek == DayOfWeek.Saturday;
            if (offDay) return false;

            if (today.Hour < 11) return false;

            if (today.Hour > 6) return false;

            return true;
        }

        public void LoadWacc()
        {
            var costDict = new Dictionary<string, float>();
            var quantityDict = new Dictionary<string, int>();

            if (File.Exists(Path.Combine(Constants.AppDataPath.Value, "view.jl")))
            {
                foreach (var line in File.ReadAllLines(Path.Combine(Constants.AppDataPath.Value, "view.jl")))
                {
                    var view = JsonConvert.DeserializeObject<MeroshareViewMyPurchaseResponse>(line);
                    if (string.IsNullOrEmpty(view.ScripName)) continue;

                    costDict.Add(view.ScripName, view.AverageBuyRate * view.TotalQuantity);
                    quantityDict.Add(view.ScripName, view.TotalQuantity);
                }
            }

            if (File.Exists(Path.Combine(Constants.AppDataPath.Value, "search.jl")))
            {
                foreach (var line in File.ReadAllLines(Path.Combine(Constants.AppDataPath.Value, "search.jl")))
                {
                    var searches = JsonConvert.DeserializeObject<MeroshareSearchMyPurchaseRespose[]>(line);
                    if (searches.Length == 0) continue;
                    var scrip = searches[0].Scrip;
                    if (costDict.ContainsKey(scrip))
                    {
                        // average out
                        costDict[scrip] += searches.Sum(x => x.Rate * x.TransactionQuantity);
                        quantityDict[scrip] += searches.Sum(x => x.TransactionQuantity);
                    }
                    else
                    {
                        costDict.Add(scrip, searches.Sum(x => x.Rate * x.TransactionQuantity));
                        quantityDict.Add(scrip, searches.Sum(x => x.TransactionQuantity));
                    }
                }
            }

            _waccDict = costDict.ToDictionary(x => x.Key, x => x.Value / quantityDict[x.Key]);
        }

        public T EnsureAuthenticated<T>(Task<IRestResponse<T>> task)
        {
            var response = task.Result;
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                IsAuthenticated = false;
                throw new AuthenticationException(response.Content);
            }

            return response.Data;
        }

        public void HandleAuthException(AggregateException ex, ICommand command, object commandParameter = null)
        {
            foreach (var innerEx in ex.InnerExceptions)
            {
                if (innerEx is AuthenticationException)
                {
                    Log.Error(ex, "Not Authorized. Requesting credentials");
                    ShowAuthenticationDialog?.Invoke();
                    if (IsAuthenticated)
                    {
                        command?.Execute(commandParameter);
                    }
                }
            }
        }
        #endregion
    }
}
