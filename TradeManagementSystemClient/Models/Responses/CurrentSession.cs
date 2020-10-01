using NepseClient.Commons.Contracts;
using Newtonsoft.Json;

namespace TradeManagementSystemClient.Models.Responses
{

    public class CurrentSession : ICurrentSession
    {
        [JsonProperty("activeStatus")]
        public string ActiveStatus { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("sessionName")]
        public string SessionName { get; set; }

        [JsonProperty("exchangeMarketSessionId")]
        public int ExchangeMarketSessionId { get; set; }

        [JsonProperty("sessionStartTime")]
        public string SessionStartTime { get; set; }

        [JsonProperty("sessionEndTime")]
        public string SessionEndTime { get; set; }
    }
}