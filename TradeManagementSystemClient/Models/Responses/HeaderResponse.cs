using Newtonsoft.Json;

namespace TradeManagementSystemClient.Models.Responses
{

    public class HeaderResponse
    {

        [JsonProperty("channel")]
        public string Channel { get; set; }

        [JsonProperty("transaction")]
        public string Transaction { get; set; }

        [JsonProperty("tnxCode")]
        public object TransactionCode { get; set; }
    }
}