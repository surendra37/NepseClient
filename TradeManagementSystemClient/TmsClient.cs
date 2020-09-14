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
            if (!File.Exists(_sessionFilePath))
                return;

            File.Delete(_sessionFilePath);
            _session = default;
            IsAuthenticated = false;
            _client.Authenticator = null;
        }

        public string GetBusinessDate()
        {
            var request = new RestRequest("/tmsapi/dashboard/businessDate");
            var response = _client.Get<string>(request);
            return response.Data;
        }

        public void Authenticate(string username, string password)
        {
            Log.Debug("Authenticating...");
            var request = new RestRequest("/tmsapi/authenticate");
            request.AddJsonBody(new AuthenticationRequest(username, password));
            var response = _client.Post<AuthenticationResponse>(request);
            if (!response.IsSuccessful)
            {
                throw new Exception(response.Content);
            }
            Log.Debug("Authentication Response Message: {0}", response.Data?.Message);
            // {Set-Cookie=XSRF-TOKEN=9c361ef0-5397-4762-ab97-0def566a627e; Version=1; Path=/; Domain=; Max-Age=4200, accessToken=eyJhbGciOiJIUzI1NiJ9.eyJqdGkiOiI3MTYyMCIsImlhdCI6MTYwMDA3NjIxMiwic3ViIjoiQ1MtQTJDQTFVREEzUixPTS1DVVIsU1NJLUEyQ0ExVURBM1IsTUFELVIsTEYtQTJDQTFVREEzUixTVExCSS1DVVIsV1MtUixHTC1SLFRCLUNVUixTUS1SLEROQVMtUixHUkFQSC1SLE1XLVIsUEYtQTJDQTFVREEzUixNV0RDLVIsT0ItQTJDQTFVREEzUixESC1DVVIsT0JILVIsVFItQ1VSLEZNLUEyQ0ExVURBM1IsTUlORk8tUixDRC1SLFNCSS1BMkNBMVVEQTNSLENGVC1BMkNBMVVEQTNSLEZXLUEyQ0ExVURBM1IsV1MtQTJDQTFVREEzUixBUy1SLE1MTldTLVIsTUwtUixNTEVNLVIsU0ktUixDTFRTLVIsTUNPRS1BMkNBMVVEQTNSLEROQUwtUixUQkgtUixCUy1SLENDTS1BMkNBMVVEQTNSIiwiaXNzIjoiU1M0ODIxNjciLCJleHAiOjE2MDAxMDYyMTIsImlzQ2xpZW50IjoiMSIsImlzTWVtYmVyIjoiMCIsImlzRGVhbGVyIjoiMCJ9.Qe42EDhpQk8Vc03xoSV1ZWNvYpaE5a0HlayZSvu5OVY; Version=1; Path=/; Domain=; Secure; HttpOnly; Max-Age=4200, refreshToken=eyJhbGciOiJIUzI1NiJ9.eyJqdGkiOiI3MTYyMCIsImlhdCI6MTYwMDA3NjIxMiwic3ViIjoiQ1MtQTJDQTFVREEzUixPTS1DVVIsU1NJLUEyQ0ExVURBM1IsTUFELVIsTEYtQTJDQTFV…
            var cookies = response.Headers.FirstOrDefault();

            ////_session = new SessionInfo
            //{
            //    AccessToken = "eyJhbGciOiJIUzI1NiJ9.eyJqdGkiOiI3MTYyMCIsImlhdCI6MTYwMDA3MzI2Miwic3ViIjoiQ1MtVUEyUkExQ0RBMyxPTS1VUkMsU1NJLVVBMlJBMUNEQTMsTUFELVIsTEYtVUEyUkExQ0RBMyxTVExCSS1VUkMsV1MtUixHTC1SLFRCLVVSQyxTUS1SLEROQVMtUixHUkFQSC1SLE1XLVIsUEYtVUEyUkExQ0RBMyxNV0RDLVIsT0ItVUEyUkExQ0RBMyxESC1VUkMsT0JILVIsVFItVVJDLEZNLVVBMlJBMUNEQTMsTUlORk8tUixDRC1SLFNCSS1VQTJSQTFDREEzLENGVC1VQTJSQTFDREEzLEZXLVVBMlJBMUNEQTMsV1MtVUEyUkExQ0RBMyxBUy1SLE1MTldTLVIsTUwtUixNTEVNLVIsU0ktUixDTFRTLVIsTUNPRS1VQTJSQTFDREEzLEROQUwtUixUQkgtUixCUy1SLENDTS1VQTJSQTFDREEzIiwiaXNzIjoiU1M0ODIxNjciLCJleHAiOjE2MDAxMDMyNjIsImlzQ2xpZW50IjoiMSIsImlzTWVtYmVyIjoiMCIsImlzRGVhbGVyIjoiMCJ9._fb3OtlXmdcMcduerBwXwhbRfHA9stRV9fY_WUZatDg",
            //    RefreshToken = "eyJhbGciOiJIUzI1NiJ9.eyJqdGkiOiI3MTYyMCIsImlhdCI6MTYwMDA3MzI2Miwic3ViIjoiQ1MtVUEyUkExQ0RBMyxPTS1VUkMsU1NJLVVBMlJBMUNEQTMsTUFELVIsTEYtVUEyUkExQ0RBMyxTVExCSS1VUkMsV1MtUixHTC1SLFRCLVVSQyxTUS1SLEROQVMtUixHUkFQSC1SLE1XLVIsUEYtVUEyUkExQ0RBMyxNV0RDLVIsT0ItVUEyUkExQ0RBMyxESC1VUkMsT0JILVIsVFItVVJDLEZNLVVBMlJBMUNEQTMsTUlORk8tUixDRC1SLFNCSS1VQTJSQTFDREEzLENGVC1VQTJSQTFDREEzLEZXLVVBMlJBMUNEQTMsV1MtVUEyUkExQ0RBMyxBUy1SLE1MTldTLVIsTUwtUixNTEVNLVIsU0ktUixDTFRTLVIsTUNPRS1VQTJSQTFDREEzLEROQUwtUixUQkgtUixCUy1SLENDTS1VQTJSQTFDREEzIiwiaXNzIjoiU1M0ODIxNjciLCJleHAiOjE2MDAwNzY4NjIsImlzQ2xpZW50IjoiMSIsImlzTWVtYmVyIjoiMCIsImlzRGVhbGVyIjoiMCJ9.cYxJY4tRM3mgX_aZcGIIvyfnQePCTrtIcWBcW2LyBhA",
            //   XsrfToken = "47177bf0-970b-4c4e-a095-bc54ee5827bc",
            //   ClientId = "1979933",
            //    LastUpdated = DateTime.Now,
            // };
        }

        public IEnumerable<IScripResponse> GetMyPortfolio()
        {
            var request = new RestRequest($"/tmsapi/dp-holding/client/freebalance/{_session.ClientId}/CLI");
            var response = _client.Get<List<ScripResponse>>(request);
            return response.Data;
        }
    }
}
