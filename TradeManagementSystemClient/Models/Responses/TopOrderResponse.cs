using NepseClient.Commons.Contracts;
using Newtonsoft.Json;

namespace TradeManagementSystemClient.Models.Responses
{

    public class TopOrderResponse : ITopOrderResponse
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("sequenceId")]
        public int SequenceId { get; set; }

        [JsonProperty("buySell")]
        public int BuySell { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("price")]
        public double Price { get; set; }

        [JsonProperty("totalOrders")]
        public int TotalOrders { get; set; }

        [JsonProperty("reserved")]
        public string Reserved { get; set; }
    }
}