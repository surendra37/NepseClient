using NepseClient.Commons.Contracts;
using Newtonsoft.Json;

namespace TradeManagementSystemClient.Models.Responses
{

    public class MarketType : IMarketType
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("marketType")]
        public string MyMarketType { get; set; }
    }
}