using System.IO;

using Newtonsoft.Json;

namespace NepseClient.Libraries.TradeManagementSystem.Responses
{
    public class AuthenticationDataResponse
    {
        public ClientDealerMemberResponse ClientDealerMember { get; set; }
        [JsonProperty("jwt")]
        public string JsonWebToken { get; set; }
        public string CookieEnabled { get; set; }
        [JsonIgnore]
        public bool IsCookieEnabled => !CookieEnabled.Equals("0");
        public ClientResponse User { get; set; }

        public static AuthenticationDataResponse NewInstance(string filePath)
        {
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<AuthenticationDataResponse>(json);
            }

            return null;
        }
    }

    public class ClientResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }

    public class ClientDealerMemberResponse
    {
        [JsonProperty("client")]
        public ClientResponse Client { get; set; }

        [JsonProperty("dealer")]
        public ClientResponse Dealer { get; set; }

        [JsonProperty("member")]
        public ClientResponse Member { get; set; }
    }
}