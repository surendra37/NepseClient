using NepseClient.Commons.Contracts;
using Newtonsoft.Json;

namespace TradeManagementSystemClient.Models.Responses
{

    public class MeroshareCapitalResponse : IMeroshareCapital
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("statusDto")]
        public object StatusDto { get; set; }

        public string DisplayName => $"{Name}({Code})";
    }
}