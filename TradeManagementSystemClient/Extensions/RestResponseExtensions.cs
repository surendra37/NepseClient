
using RestSharp;

namespace TradeManagementSystemClient.Extensions
{
    public static class RestResponseExtensions
    {
        public static bool IsUnAuthorized(this IRestResponse response)
        {
            return response.StatusCode == System.Net.HttpStatusCode.Unauthorized;
        }
    }
}
