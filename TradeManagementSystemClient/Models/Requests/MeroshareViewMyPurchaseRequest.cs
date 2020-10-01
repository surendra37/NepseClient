using Newtonsoft.Json;
namespace TradeManagementSystemClient.Models.Requests
{
    public class MeroshareViewMyPurchaseRequest
    {
        [JsonProperty("demat")]
        public string Demat { get; set; }

        [JsonProperty("scrip")]
        public string Scrip { get; set; }
    }


}
