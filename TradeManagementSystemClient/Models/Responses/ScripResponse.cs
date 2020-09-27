using NepseClient.Commons;
using Newtonsoft.Json;
namespace TradeManagementSystemClient.Models.Responses
{
    public class ScripResponse : IScripResponse
    {
        [JsonProperty("cdsFreeBalance")]
        public float FreeBalance { get; set; }

        [JsonProperty("cdsTotalBalance")]
        public float TotalBalance { get; set; }

        [JsonProperty("currentBalance")]
        public float CurrentBalance { get; set; }

        [JsonProperty("ltp")]
        public float LastTransactionPrice { get; set; }

        [JsonProperty("previousCloseprice")]
        public float PreviousClosePrice { get; set; }

        [JsonProperty("scrip")]
        public string Scrip { get; set; }

        [JsonProperty("symbolName")]
        public string Name { get; set; }

        [JsonProperty("valueAsOfLTP")]
        public float LTPTotal { get; set; }

        [JsonProperty("valueAsOfPreviousClosePrice")]
        public float PreviousTotal { get; set; }

        public ScripResponse()
        {
        }
    }
}
