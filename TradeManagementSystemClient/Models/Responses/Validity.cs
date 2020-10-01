using NepseClient.Commons.Contracts;
using Newtonsoft.Json;

namespace TradeManagementSystemClient.Models.Responses
{

    public class Validity : IValidity
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("orderValidityCode")]
        public string OrderValidityCode { get; set; }
    }
}