using NepseClient.Commons.Contracts;
using Newtonsoft.Json;

namespace TradeManagementSystemClient.Models.Responses
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class InstrumentType : IInstrumentType
    {
        [JsonProperty("activeStatus")]
        public string ActiveStatus { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }
}