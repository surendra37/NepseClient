using NepseClient.Commons.Contracts;
using Newtonsoft.Json;

namespace NepseService.TradeManagementSystem.Models.Responses
{
    public class MeroshareSearchMyPurchaseRespose : IMeroshareSearchMyPurchaseRespose
    {
        [JsonProperty("demat")]
        public string Demat { get; set; }

        [JsonProperty("historyDescription")]
        public string HistoryDescription { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("isEdit")]
        public bool IsEdit { get; set; }

        [JsonProperty("isin")]
        public string Isin { get; set; }

        [JsonProperty("knownPrice")]
        public string KnownPrice { get; set; }

        [JsonProperty("purchasePrice")]
        public object PurchasePrice { get; set; }

        [JsonProperty("purchaseSource")]
        public string PurchaseSource { get; set; }

        [JsonProperty("rate")]
        public float Rate { get; set; }

        [JsonProperty("remarks")]
        public string Remarks { get; set; }

        [JsonProperty("scrip")]
        public string Scrip { get; set; }

        [JsonProperty("totalCost")]
        public object TotalCost { get; set; }

        [JsonProperty("transactionDate")]
        public long TransactionDate { get; set; }

        [JsonProperty("transactionQuantity")]
        public int TransactionQuantity { get; set; }

        [JsonProperty("userPrice")]
        public double UserPrice { get; set; }
    }
}
