using RestSharp;
using RestSharp.Authenticators;

namespace TradeManagementSystemClient.Models
{
    public class BearerAuthenticator : IAuthenticator
    {
        private readonly string _jsonWebToken;

        public BearerAuthenticator(string jsonWebToken)
        {
            _jsonWebToken = jsonWebToken;
        }

        public void Authenticate(IRestClient client, IRestRequest request)
        {
            request.AddHeader("Authorization", $"Bearer {_jsonWebToken}");
        }
    }
}
