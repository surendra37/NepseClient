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
            AddCookie(cookies, request, "accessToken");
            AddCookie(cookies, request, "refreshToken");
            AddCookie(cookies, request, "XSRF-TOKEN");

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

    public class CachedTmsAuthenticator : IAuthenticator
    {
        public void Authenticate(IRestClient client, IRestRequest request)
        {
            request.AddHeader("X-XSRF-TOKEN", "5932a3c0-9d38-4283-acc0-f867171b0c16");
            request.AddHeader("XSRF-TOKEN", "5932a3c0-9d38-4283-acc0-f867171b0c16");
            request.AddHeader("accessToken", "eyJhbGciOiJIUzI1NiJ9.eyJqdGkiOiI3MTYyMCIsImlhdCI6MTYxMzY1MzAxOSwic3ViIjoiQ1MtQTJEQTFDVVJBMyxPTS1DVVIsU1NJLUEyREExQ1VSQTMsTUFELVIsTEYtQTJEQTFDVVJBMyxTVExCSS1DVVIsV1MtQoJB7UfVAxrJ1pHGyULxz9zLA2nQ3gROQVMtUixHUkFQSC1SLE1XLVIsUEYtQTJEQTFDVVJBMyxNV0RDLVIsT0ItQTJEQTFDVVJBMyxESC1DVVIsT0JILVIsVFItQ1VSLEZNLUEyREExQ1VSQTMsTUlORk8tUixDRC1SLFNCSS1BMkRBMUNVUkEzLENGVC1BMkRBMUNVUkEzLEZXLUEyREExQ1VSQTMsV1MtQTJEQTFDVVJBMyxBUy1SLE1MTldTLVIsTUwtUixNTEVNLVIsU0ktUixDTFRTLVIsTUNPRS1BMkRBMUNVUkEzLEROQUwtUixUQkgtUixCUy1SLENDTS1BMkRBMUNVUkEzIiwiaXNzIjoiU1M0ODIxNjciLCJleHAiOjE2MTM2ODMwMTksImlzQ2xpZW50IjoiMSIsImlzTWVtYmVyIjoiMCIsImlzRGVhbGVyIjoiMCJ9.DMBLhxQJbnBCcLFwna7t2hiuwlr9ZdU9co-wWOHMl2A");
            request.AddHeader("refreshToken", "eyJhbGciOiJIUzI1NiJ9.eyJqdGkiOiI3MTYyMCIsImlhdCI6MTYxMzY1MzAxOSwic3ViIjoiQ1MtQTJEQTFDVVJBMyxPTS1DVVIsU1NJLUEyREExQ1VSQTMsTUFELVIsTEYtQTJEQTFDVVJBMyxTVExCSS1DVVIsV1Mtic9fAYmTGBNSixRAULhut9vZodZThZROQVMtUixHUkFQSC1SLE1XLVIsUEYtQTJEQTFDVVJBMyxNV0RDLVIsT0ItQTJEQTFDVVJBMyxESC1DVVIsT0JILVIsVFItQ1VSLEZNLUEyREExQ1VSQTMsTUlORk8tUixDRC1SLFNCSS1BMkRBMUNVUkEzLENGVC1BMkRBMUNVUkEzLEZXLUEyREExQ1VSQTMsV1MtQTJEQTFDVVJBMyxBUy1SLE1MTldTLVIsTUwtUixNTEVNLVIsU0ktUixDTFRTLVIsTUNPRS1BMkRBMUNVUkEzLEROQUwtUixUQkgtUixCUy1SLENDTS1BMkRBMUNVUkEzIiwiaXNzIjoiU1M0ODIxNjciLCJleHAiOjE2MTM2NjUwMTksImlzQ2xpZW50IjoiMSIsImlzTWVtYmVyIjoiMCIsImlzRGVhbGVyIjoiMCJ9.E5GOAzmkLpKDS3zk814m_xCd84Rb5jWob3RJdWSEQP4");
        }
    }
}
