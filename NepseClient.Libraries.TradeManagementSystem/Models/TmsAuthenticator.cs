using System.Linq;
using System.Net;

using RestSharp;
using RestSharp.Authenticators;

namespace NepseClient.Libraries.TradeManagementSystem.Models
{
    public class TmsAuthenticator : IAuthenticator
    {
        public void Authenticate(IRestClient client, IRestRequest request)
        {
            var cookies = client.CookieContainer.GetCookies(client.BaseUrl);
            AddHeader(cookies, request, "XSRF-TOKEN", "X-XSRF-TOKEN");
        }

        private static void AddHeader(CookieCollection cookies, IRestRequest request, string header, string headerToAdd)
        {
            var c = cookies.FirstOrDefault(x => x.Name.Equals(header));
            if (c is null) return;

            request.AddHeader(headerToAdd, c.Value);
        }

        private static void AddCookie(CookieCollection cookies, IRestRequest request, string name)
        {
            var cookie = cookies.FirstOrDefault(x => x.Name.Equals(name));
            if (cookie is null) return;

            request.AddCookie(name, cookie.Value);
        }
    }
}
