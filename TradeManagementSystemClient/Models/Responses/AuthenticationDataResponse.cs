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
}