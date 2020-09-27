using NepseClient.Commons.Contracts;
using Newtonsoft.Json;

namespace TradeManagementSystemClient.Models.Responses
{

    public class MarketWatchResponse : IMarketWatchResponse
    {
        [JsonProperty("marketWatchID")]
        public int MarketWatchID { get; set; }

        [JsonProperty("marketWatchName")]
        public string MarketWatchName { get; set; }

        [JsonProperty("userId")]
        public int UserId { get; set; }

        [JsonProperty("security")]
        public SecurityResponse[] Security { get; set; }
        public ISecurityResponse[] Securities => Security;
    }
}