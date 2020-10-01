using NepseClient.Commons.Contracts;
using Newtonsoft.Json;

namespace TradeManagementSystemClient.Models.Responses
{

    public class CurrentMarketSession : ICurrentMarketSession
    {
        [JsonProperty("currentSessions")]
        [JsonConverter(typeof(ConcreteArraryConverter<CurrentSession>))]
        public ICurrentSession[] CurrentSessions { get; set; }

        [JsonProperty("nonSessions")]
        [JsonConverter(typeof(ConcreteArraryConverter<NonSession>))]
        public INonSession[] NonSessions { get; set; }

        [JsonProperty("activeStatus")]
        public bool ActiveStatus { get; set; }
    }
}