using NepseClient.Commons.Contracts;
using Newtonsoft.Json;

namespace TradeManagementSystemClient.Models.Responses
{

    public class TopResponse : ITopResponse
    {
        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("ltp")]
        public double Ltp { get; set; }

        [JsonProperty("pointChange")]
        public double PointChange { get; set; }

        [JsonProperty("perChange")]
        public double PerChange { get; set; }

        [JsonProperty("securityName")]
        public string SecurityName { get; set; }
    }
}