using RestSharp;
using RestSharp.Authenticators;

namespace NepseClient.Libraries.TradeManagementSystem.Models
{
    public class XsrfTokenAuthenticator : IAuthenticator
    {
        private string _authHeader;

        public XsrfTokenAuthenticator(string accessToken)
        {
            _authHeader = accessToken;
        }

        public void Authenticate(IRestClient client, IRestRequest request)
        {
            request.AddOrUpdateParameter("X-XSRF-TOKEN", _authHeader, ParameterType.HttpHeader);
        }
    }
}
