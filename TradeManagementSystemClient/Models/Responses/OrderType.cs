using NepseClient.Commons.Contracts;
using Newtonsoft.Json;

namespace TradeManagementSystemClient.Models.Responses
{

    public class OrderType : IOrderType
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("orderTypeCode")]
        public string OrderTypeCode { get; set; }
    }
}