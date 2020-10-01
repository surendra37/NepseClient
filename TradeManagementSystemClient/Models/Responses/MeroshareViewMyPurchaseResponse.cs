using NepseClient.Commons.Contracts;
using Newtonsoft.Json;

namespace TradeManagementSystemClient.Models.Responses
{
    public class MeroshareViewMyPurchaseResponse : IMeroshareViewMyPurchase
    {
        [JsonProperty("averageBuyRate")]
        public float AverageBuyRate { get; set; }

        [JsonProperty("isin")]
        public string Isin { get; set; }

        [JsonProperty("scripName")]
        public string ScripName { get; set; }

        [JsonProperty("totalCost")]
        public double TotalCost { get; set; }

        [JsonProperty("totalQuantity")]
        public int TotalQuantity { get; set; }
    }
}