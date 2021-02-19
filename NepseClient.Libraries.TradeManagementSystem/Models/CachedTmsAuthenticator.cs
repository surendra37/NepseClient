using System.Net;
using RestSharp;
using RestSharp.Authenticators;

namespace NepseClient.Libraries.TradeManagementSystem.Models
{

    public class CachedTmsAuthenticator : IAuthenticator
    {
        private const string RefreshToken = "eyJhbGciOiJIUzI1NiJ9.eyJqdGkiOiI3MTYyMCIsImlhdCI6MTYxMzcxNTI1NCwic3ViIjoiQ1MtQTFVQTNEQTJSQyxPTS1VUkMsU1NJLUExVUEzREEyUkMsTUFELVIsTEYtQTFVQTNEQTJSQyxTVExCSS1VUkMsV1MtUixHTC1SLFRCLVVSQyxTUS1SLEROQVMtUixHUkFQSC1SLE1XLVIsUEYtQTFVQTNEQTJSQyxNV0RDLVIsT0ItQTFVQTNEQTJSQyxESC1VUkMsT0JILVIsVFItVVJDLEZNLUExVUEzREEyUkMsTUlORk8tUixDRC1SLFNCSS1BMVVBM0RBMlJDLENGVC1BMVVBM0RBMlJDLEZXLUExVUEzREEyUkMsV1MtQTFVQTNEQTJSQyxBUy1SLE1MTldTLVIsTUwtUixNTEVNLVIsU0ktUixDTFRTLVIsTUNPRS1BMVVBM0RBMlJDLEROQUwtUixUQkgtUixCUy1SLENDTS1BMVVBM0RBMlJDIiwiaXNzIjoiU1M0ODIxNjciLCJleHAiOjE2MTM3MjcyNTQsImlzQ2xpZW50IjoiMSIsImlzTWVtYmVyIjoiMCIsImlzRGVhbGVyIjoiMCJ9.5OtYmJGKq03tIC0LOhnZca27VxduS2Ia0eXMC649rwA";
        private const string AccessToken = "eyJhbGciOiJIUzI1NiJ9.eyJqdGkiOiI3MTYyMCIsImlhdCI6MTYxMzcxNTI1NCwic3ViIjoiQ1MtQTFVQTNEQTJSQyxPTS1VUkMsU1NJLUExVUEzREEyUkMsTUFELVIsTEYtQTFVQTNEQTJSQyxTVExCSS1VUkMsV1MtUixHTC1SLFRCLVVSQyxTUS1SLEROQVMtUixHUkFQSC1SLE1XLVIsUEYtQTFVQTNEQTJSQyxNV0RDLVIsT0ItQTFVQTNEQTJSQyxESC1VUkMsT0JILVIsVFItVVJDLEZNLUExVUEzREEyUkMsTUlORk8tUixDRC1SLFNCSS1BMVVBM0RBMlJDLENGVC1BMVVBM0RBMlJDLEZXLUExVUEzREEyUkMsV1MtQTFVQTNEQTJSQyxBUy1SLE1MTldTLVIsTUwtUixNTEVNLVIsU0ktUixDTFRTLVIsTUNPRS1BMVVBM0RBMlJDLEROQUwtUixUQkgtUixCUy1SLENDTS1BMVVBM0RBMlJDIiwiaXNzIjoiU1M0ODIxNjciLCJleHAiOjE2MTM3NDUyNTQsImlzQ2xpZW50IjoiMSIsImlzTWVtYmVyIjoiMCIsImlzRGVhbGVyIjoiMCJ9.BdsLIgPyV-GyL859kz5NxMly0NRPu5leD-hm3clsGPk";
        private const string XsrfToken = "35ce69d7-b8c3-4b4c-bfec-1a2628ceab0e";

        public void Authenticate(IRestClient client, IRestRequest request)
        {
            var cookies = client.CookieContainer.GetCookies(client.BaseUrl);
            if (cookies.Count < 3)
            {
                cookies.Add(new Cookie { Name = "accessToken", Value = AccessToken });
                cookies.Add(new Cookie { Name = "refreshToken", Value = RefreshToken });
                cookies.Add(new Cookie { Name = "XSRF-TOKEN", Value = XsrfToken });
                client.CookieContainer.Add(client.BaseUrl, cookies);
            }
            request.AddHeader("X-XSRF-TOKEN", XsrfToken);
            //request.AddCookie("XSRF-TOKEN", XsrfToken);
            //request.AddCookie("accessToken", AccessToken);
            //request.AddCookie("refreshToken", RefreshToken);
        }
    }
}