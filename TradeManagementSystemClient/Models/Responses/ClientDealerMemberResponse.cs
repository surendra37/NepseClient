using Newtonsoft.Json;

namespace TradeManagementSystemClient.Models.Responses
{

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