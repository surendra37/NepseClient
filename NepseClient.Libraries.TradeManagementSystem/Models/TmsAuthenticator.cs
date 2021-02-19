using System.Linq;
using System.Net;

using RestSharp;
using RestSharp.Authenticators;

using Serilog;

namespace NepseClient.Libraries.TradeManagementSystem.Models
{
    public class TmsAuthenticator : IAuthenticator
    {
        public TmsAuthenticator(IRestClient client)
        {
            // Trim cookies
            var cookies = client.CookieContainer.GetCookies(client.BaseUrl);
            client.CookieContainer = new CookieContainer();
            var newColl = new CookieCollection();
            foreach (Cookie cookie in cookies)
            {
                Log.Verbose("Name: {0}, Value:{1}", cookie.Name, cookie.Value);
                newColl.Add(new Cookie(cookie.Name, cookie.Value));
            }
            client.CookieContainer.Add(client.BaseUrl, newColl);
        }
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
