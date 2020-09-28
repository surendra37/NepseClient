using NepseClient.Commons.Contracts;
using Newtonsoft.Json;

namespace TradeManagementSystemClient.Models.Responses
{
    public class IndexResponse : IIndexResponse
    {
        [JsonProperty("indexCode")]
        public string IndexCode { get; set; }

        [JsonProperty("indexValue")]
        public float IndexValue { get; set; }

        [JsonProperty("prevCloseIndex")]
        public float PrevCloseIndex { get; set; }

        [JsonProperty("change")]
        public float Change { get; set; }

        [JsonProperty("percentageChange")]
        public float PercentageChange { get; set; }
    }
}
