using NepseClient.Commons.Contracts;
using NepseClient.Commons.Extensions;
using NepseClient.Commons.Utils;
using NepseClient.Libraries.TradeManagementSystem.Models;
using NepseClient.Libraries.TradeManagementSystem.Models.Requests;
using NepseClient.Libraries.TradeManagementSystem.Models.Responses;
using NepseClient.Libraries.TradeManagementSystem.Responses;

using RestSharp;
using RestSharp.Authenticators;

using Serilog;

using System.Security.Authentication;
using System.Threading.Tasks;

namespace NepseClient.Libraries.TradeManagementSystem
{
    public class TmsClient : IAuthorizable
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
        public virtual void Authorize()
        {

        }

        public void SignIn(string url, string username, string password)
        {
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

        public WebSocketResponse<WsSecurityResponse> GetSecurities()
        {
            Log.Debug("Getting securities");
            var request = new RestRequest("/tmsapi/ws/top25securities");
            var response = this.AuthorizedGet<WebSocketResponse<WsSecurityResponse>>(request);
            return response.Data;
        }

        #region Graph 
        public GraphDataResponse[] GetGraphData(params string[] isins)
        {
            var request = new RestRequest("/tmsapi/graph-data/fetch/1"); // NPE019A00007 
            foreach (var isin in isins)
            {
                request.AddParameter("ISIN", isin);
            }
            var data = this.AuthorizedGet<GraphDataResponse[]>(request);
            return data.Data;
        }
        #endregion

        #region Top
        public TopGainerResponse[] GetTopGainers(int count = 10)
        {
            var api = new RestRequest($"/tmsapi/stock/top/gainer/{count}");

            var response= this.AuthorizedGet<TopGainerResponse[]>(api);
            return response.Data;
        }
        public TopGainerResponse[] GetTopLosers(int count = 8)
        {
            var api = new RestRequest($"/tmsapi/stock/top/loser/{count}");

            var response= this.AuthorizedGet<TopGainerResponse[]>(api);
            return response.Data;
        }

        public TopTurnoverResponse[] GetTopTurnover(int count = 9)
        {
            var api = new RestRequest($"/tmsapi/stock/top-securities/turnover/{count}");

            var response= this.AuthorizedGet<TopTurnoverResponse[]>(api);
            return response.Data;
        }
        public TopTurnoverResponse[] GetTopTransaction(int count = 9)
        {
            var api = new RestRequest($"/tmsapi/stock/top-securities/transaction/{count}");

            var response= this.AuthorizedGet<TopTurnoverResponse[]>(api);
            return response.Data;
        }
        public TopTurnoverResponse[] GetTopVolume(int count = 9)
        {
            var api = new RestRequest($"/tmsapi/stock/top-securities/volume/{count}");

            var response= this.AuthorizedGet<TopTurnoverResponse[]>(api);
            return response.Data;
        }
        #endregion

        #region Broker Back Office
        public Task<PortfolioResponse[]> GetPortfolioAsync()
        {
            var id = AuthData.User.Id;
            var request = new RestRequest($"/tmsapi/dp-holding/client/freebalance/{1979933}/CLI");
            return Client.GetAsync<PortfolioResponse[]>(request);
        }
        #endregion
    }
}
