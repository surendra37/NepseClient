using NepseClient.Commons;
using RestSharp;
using RestSharp.Authenticators;

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
            if (_session == null) return;

            if (_session.CookieEnabled)
            {
                request.AddHeader("X-XSRF-TOKEN", _session.XsrfToken);
                request.AddCookie("accessToken", _session.AccessToken);
                request.AddCookie("refreshToken", _session.RefreshToken);
                request.AddCookie("XSRF-TOKEN", _session.XsrfToken);
            }
            else
            {
                request.AddHeader("Authorization", $"Bearer {_session.JsonWebToken}");
            }

        }
    }
}
