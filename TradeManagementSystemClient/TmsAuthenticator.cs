using System;
using TradeManagementSystemClient.Models;
using RestSharp.Authenticators;
using RestSharp;
namespace TradeManagementSystemClient
{
    public class TmsAuthenticator : IAuthenticator
    {
        private readonly SessionInfo _session;

        public TmsAuthenticator(SessionInfo session)
        {
            _session = session;
        }

        public void Authenticate(IRestClient client, IRestRequest request)
        {
            request.AddHeader("X-XSRF-TOKEN", _session.XsrfToken);
            request.AddCookie("accessToken", _session.AccessToken);
            request.AddCookie("refreshToken", _session.RefreshToken);
            request.AddCookie("XSRF-TOKEN", _session.XsrfToken);
        }
    }
}
