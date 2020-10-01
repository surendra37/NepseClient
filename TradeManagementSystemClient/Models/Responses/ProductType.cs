using NepseClient.Commons.Contracts;
using Newtonsoft.Json;

namespace TradeManagementSystemClient.Models.Responses
{

    public class ProductType : IProductType
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("productCode")]
        public string ProductCode { get; set; }
    }
}