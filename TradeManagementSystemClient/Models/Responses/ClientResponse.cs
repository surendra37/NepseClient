

using Newtonsoft.Json;

namespace TradeManagementSystemClient.Models.Responses
{

    public class ClientResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
