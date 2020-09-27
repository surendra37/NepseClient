using NepseClient.Commons;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Authentication;
using TradeManagementSystemClient.Models.Requests;
using TradeManagementSystemClient.Models.Responses;

namespace TradeManagementSystemClient
{
    public class TmsClient : INepseClient
    {
        private const string _sessionFilePath = "tms.session";
        private RestClient _client;

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

        private RestClient CreateClient(string baseUrl)
        {
            var client = new RestClient(baseUrl);
            client.ThrowOnAnyError = true;
            client.UseNewtonsoftJson();

            return client;
        }

        #region UnAuthorized Access
        public string GetBusinessDate()
        {
            var request = new RestRequest("/tmsapi/dashboard/businessDate");
            var response = _client.Get<string>(request);
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
            _client = CreateClient(Session.Host);
            _client.Authenticator = new TmsAuthenticator(Session);

            IsAuthenticated = true;
            Log.Debug("Session restored [{0}]", Session.LastUpdated);
        }

        public void Logout()
        {
            var request = new RestRequest("/tmsapi/authenticate/logout");
            try
            {
                var response = _client.Post(request);
            }
            catch (Exception ex)
            {
                Log.Debug(ex, "Failed to logout");
            }

            Session = null;
            IsAuthenticated = false;
            _client.Authenticator = null;

            if (!File.Exists(_sessionFilePath))
                return;

            File.Delete(_sessionFilePath);
        }

        public void Authenticate(string url, string username, string password)
        {
            Log.Debug("Authenticating...");
            _client = CreateClient(url);

            var request = new RestRequest("/tmsapi/authenticate");
            request.AddJsonBody(new AuthenticationRequest(username, password));
            _client.Authenticator = null;
            var response = _client.Post<AuthenticationResponse>(request);
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
            _client.Authenticator = new TmsAuthenticator(Session);
            Log.Debug("Authentication Complete");
            Log.Debug("Authentication Response Message: {0}", response.Data?.Message);

            IsAuthenticated = true;
        }
        #endregion

        public IEnumerable<IScripResponse> GetMyPortfolio()
        {
            var request = new RestRequest($"/tmsapi/dp-holding/client/freebalance/{Session.ClientId}/CLI");
            _client.Authenticator = new TmsAuthenticator(Session);
            var response = _client.Get<List<ScripResponse>>(request);
            CheckAuthenticated(response);
            return response.Data;
        }

        public IEnumerable<ISecurityItem> GetLiveMarket()
        {
            var request = new RestRequest("/tmsapi/ws/top25securities");

            var response = _client.Get<SocketResponse<SecurityItem2>>(request);
            CheckAuthenticated(response);

            return response.Data.Payload.Data.OrderByDescending(x => x.LastTradedDateTime);
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

        private void CheckAuthenticated(IRestResponse response)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                throw new AuthenticationException(response.Content);
        }

        public string GetSocketUrl()
        {
            if (Session.CookieEnabled)
            {
                return $"wss://{_client.BaseUrl.Host}/tmsapi/socketEnd?memberId={Session.MemberId}&clientId={Session.ClientId}&dealerId={Session.DealerId}&userId={Session.UserId}";
            }
            else
            {
                return $"wss://{_client.BaseUrl.Host}/tmsapi/socketEnd?memberId={Session.MemberId}&clientId={Session.ClientId}&dealerId={Session.DealerId}&userId={Session.UserId}&access_token={Session.AccessToken}";
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
        #endregion
    }
}
