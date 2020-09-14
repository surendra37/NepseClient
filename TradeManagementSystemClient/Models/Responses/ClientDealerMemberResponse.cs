using Newtonsoft.Json;

namespace TradeManagementSystemClient.Models.Responses
{

    public class ClientDealerMemberResponse
    {
        [JsonProperty("client")]
        public ClientResponse Client { get; set; }
    }
}