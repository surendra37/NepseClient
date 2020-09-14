using Newtonsoft.Json;

namespace TradeManagementSystemClient.Models.Responses
{

    public class AuthenticationDataResponse
    {
        [JsonProperty("clientDealerMember")]
        public ClientDealerMemberResponse ClientDealerMember { get; set; }

        [JsonProperty("jwt")]
        public string JsonWebToken { get; set; }

        [JsonProperty("cookieEnabled")]
        public string CookieEnabled { get; set; }

        public bool IsCookieEnabled => !CookieEnabled.Equals("0");
    }
}