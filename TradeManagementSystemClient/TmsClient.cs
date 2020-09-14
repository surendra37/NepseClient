using System;
using NepseClient.Commons;
using RestSharp;
using TradeManagementSystemClient.Models.Responses;
using RestSharp.Serializers.NewtonsoftJson;
using Serilog;
using TradeManagementSystemClient.Models.Requests;
using TradeManagementSystemClient.Models;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace TradeManagementSystemClient
{
    public class TmsClient : INepseClient
    {
        private const string _sessionFilePath = "tms.session";
        private RestClient _client;
        private SessionInfo _session;
        public bool IsAuthenticated { get; private set; }
        public TmsClient(string baseUrl)
        {
            _client = new RestClient(baseUrl);
            _client.UseNewtonsoftJson();
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
            var serialized = JsonConvert.SerializeObject(_session);
            File.WriteAllText(_sessionFilePath, serialized);
        }

        public void RestoreSession()
        {
            if (!File.Exists(_sessionFilePath))
                return;

            var session = File.ReadAllText(_sessionFilePath);
            _session = JsonConvert.DeserializeObject<SessionInfo>(session);
            _client.Authenticator = new TmsAuthenticator(_session);

            IsAuthenticated = true;
            Log.Debug("Session restored [{0}]", _session.LastUpdated);
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

            if (!File.Exists(_sessionFilePath))
                return;

            File.Delete(_sessionFilePath);
            _session = default;
            IsAuthenticated = false;
            _client.Authenticator = null;
        }

        public void Authenticate(string username, string password)
        {
            Log.Debug("Authenticating...");
            var request = new RestRequest("/tmsapi/authenticate");
            request.AddJsonBody(new AuthenticationRequest(username, password));
            _client.Authenticator = null;
            var response = _client.Post<AuthenticationResponse>(request);
            if (!response.IsSuccessful)
            {
                throw new Exception(response.Content);
            }
            if (response.Data.Data.IsCookieEnabled)
            {
                var cookies = response.Headers.FirstOrDefault(x => x.Name.Equals("Set-Cookie"));
                if (cookies is null)
                {
                    throw new Exception("Cookies Not Found");
                }
                var parts = cookies.Value.ToString().Split(',');
                _session = GetSessionInfo(cookies.Value.ToString(), response.Data.Data);
            }
            else
            {
                _session = GetSessionInfo(null, response.Data.Data);
            }

            _client.Authenticator = new TmsAuthenticator(_session);
            Log.Debug("Authentication Complete");
            Log.Debug("Authentication Response Message: {0}", response.Data?.Message);
        }
        #endregion

        public IEnumerable<IScripResponse> GetMyPortfolio()
        {
            var request = new RestRequest($"/tmsapi/dp-holding/client/freebalance/{_session.ClientId}/CLI");
            _client.Authenticator = new TmsAuthenticator(_session);
            var response = _client.Get<List<ScripResponse>>(request);
            return response.Data;
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
        #endregion
    }
}
