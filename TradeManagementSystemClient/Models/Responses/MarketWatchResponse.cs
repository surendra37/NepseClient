using NepseClient.Commons.Contracts;

using Newtonsoft.Json;

namespace TradeManagementSystemClient.Models.Responses
{

    public class MarketWatchResponse
    {
        [JsonProperty("marketWatchID")]
        public int MarketWatchID { get; set; }
        public string MarketWatchName { get; set; }
        public int UserId { get; set; }

        public SecurityResponse[] Security { get; set; }
        public ISecurityResponse[] Securities => Security;
    }
}