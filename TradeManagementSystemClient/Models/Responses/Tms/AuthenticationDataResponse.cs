using Newtonsoft.Json;

namespace TradeManagementSystemClient.Models.Responses
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