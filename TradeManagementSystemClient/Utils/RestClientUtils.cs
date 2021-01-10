using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;

namespace TradeManagementSystemClient.Utils
{
    public static class RestClientUtils
    {
        public static IRestClient CreateNewClient(string baseUrl)
        {
            var output = new RestClient(baseUrl)
            {
                CookieContainer = new System.Net.CookieContainer(),
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.141 Safari/537.36",
            };
            output.UseNewtonsoftJson(new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy(),
                },
            });
            return output;
        }
    }
}
