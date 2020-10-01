using NepseClient.Commons.Contracts;
using Newtonsoft.Json;

namespace TradeManagementSystemClient.Models.Responses
{

    public class OperatingParameter : IOperatingParameter
    {
        [JsonProperty("paramCode")]
        public string ParamCode { get; set; }

        [JsonProperty("paramValue")]
        public string ParamValue { get; set; }
    }
}