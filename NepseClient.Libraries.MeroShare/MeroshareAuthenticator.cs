using RestSharp;
using RestSharp.Authenticators;

namespace NepseClient.Libraries.MeroShare
{
    public class MeroshareAuthenticator : IAuthenticator
    {
        private readonly string _authorization;

        public MeroshareAuthenticator(string authorization)
        {
            _authorization = authorization;
        }

        public void Authenticate(IRestClient client, IRestRequest request)
        {
            request.AddHeader("Authorization", _authorization);
        }
    }
}
