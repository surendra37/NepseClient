using NepseClient.Commons.Contracts;
using Newtonsoft.Json;

namespace TradeManagementSystemClient.Models.Responses
{

    public class OHLCDataResponse : IOHLCDataResponse
    {
        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("securityName")]
        public string SecurityName { get; set; }

        [JsonProperty("perChange")]
        public float PerChange { get; set; }

        [JsonProperty("lastTradePrice")]
        public float LastTradePrice { get; set; }

        [JsonProperty("openPrice")]
        public float OpenPrice { get; set; }

        [JsonProperty("highPrice")]
        public float HighPrice { get; set; }

        [JsonProperty("lowPrice")]
        public float LowPrice { get; set; }

        [JsonProperty("closePrice")]
        public float ClosePrice { get; set; }
    }
}