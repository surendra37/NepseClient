
using RestSharp;

namespace TradeManagementSystemClient.Extensions
{
    public static class RestResponseExtensions
    {
        public static bool IsUnAuthorized(this IRestResponse response)
        {
            if (response.IsSuccessful || response.StatusCode == System.Net.HttpStatusCode.OK) 
                return false;

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return true;

            return false;
            //foreach (var header in response.Headers)
            //{
            //    if (header.Name.Equals("Access-Control-Expose-Headers"))
            //    {
            //        if (header.Value.Equals("Authorization"))
            //        {
            //            return true;
            //        }
            //    }
            //}
            //return false;
        }
    }
}
