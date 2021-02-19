using NepseClient.Commons.Utils;
using NepseClient.Libraries.TradeManagementSystem.Models;
using NepseClient.Libraries.TradeManagementSystem.Requests;
using NepseClient.Libraries.TradeManagementSystem.Responses;

using RestSharp;
using RestSharp.Authenticators;

using Serilog;

using System.Security.Authentication;
using System.Threading.Tasks;

namespace NepseClient.Libraries.TradeManagementSystem
{
    public class TmsClient
    {
        public AuthenticationDataResponse AuthData { get; private set; }
        public bool IsAuthenticated { get; set; }

        public IRestClient Client { get; private set; }

        private static IAuthenticator GetAuthenticator(AuthenticationDataResponse authData)
        {
            if (authData is null || authData.IsCookieEnabled) return new TmsAuthenticator();

            return new BearerAuthenticator(authData.JsonWebToken);
        }

        #region UnAuthorized Access
        public string GetBusinessDate()
        {
            Log.Debug("Getting business date");
            var request = new RestRequest("/tmsapi/dashboard/businessDate");
            var response = Client.Get<string>(request);
            return response.Data;
        }
        #endregion

        #region Authentication

        public void SignIn(string url, string username, string password)
        {
            // Use cached
            Client = RestClientUtils.CreateNewClient(url);
            IsAuthenticated = true;
            Client.Authenticator = new CachedTmsAuthenticator();
            return;

            SignOut();
            Log.Debug("Signing in");

            var request = new RestRequest("/tmsapi/authenticate");
            var json = new TmsAuthenticationRequest(username, password);
            request.AddJsonBody(json);

            Client = RestClientUtils.CreateNewClient(url);
            var response = Client.Post<ResponseBase<AuthenticationDataResponse>>(request);
            if (!response.IsSuccessful)
            {
                Log.Warning(response.Content);
                throw new AuthenticationException(response.Data.Message);
            }
            AuthData = response.Data.Data;
            IsAuthenticated = true;
            Client.Authenticator = GetAuthenticator(AuthData);
            Log.Debug("Signed In");
        }
        public void SignOut()
        {
            return;
            Log.Debug("Signing out from Tms");
            if (IsAuthenticated)
            {
                Log.Debug("Signing out");
                var request = new RestRequest("/tmsapi/authenticate/logout");
                var response = Client.Post<ResponseBase>(request);
                Log.Information(response.Data.Message);
            }
            Log.Debug("Signed out from Tms");
        }
        #endregion

        #region Broker Back Office
        public Task<PortfolioResponse[]> GetPortfolioAsync()
        {
            //var id = AuthData.User.Id;
            //var request = new RestRequest($"/tmsapi/dp-holding/client/freebalance/{id}/CLI");
            var request = new RestRequest("/tmsapi/dp-holding/client/freebalance/1979933/CLI");

            //var res = Client.Get(request);
            //var client = RestClientUtils.CreateNewClient("https://tms49.nepsetms.com.np/");
            //client.Authenticator = new CachedTmsAuthenticator();

            //var response = client.Get(request);


            return Client.GetAsync<PortfolioResponse[]>(request);
        }
        #endregion

        #region Market
        public Task<WebSocketResponse<WsSecurityResponse>> GetLiveMarketAsync()
        {
            var request = new RestRequest("/tmsapi/rtApi/ws/top25securities");
            return Client.GetAsync<WebSocketResponse<WsSecurityResponse>>(request);
        }
        #endregion
    }
}
