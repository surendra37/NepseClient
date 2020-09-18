using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TradeManagementSystemClient.Models.Responses
{
    public class SocketResponse
    {

        [JsonProperty("header")]
        public HeaderResponse Header { get; set; }

        [JsonProperty("payload")]
        public JObject Payload { get; set; }
    }

    public class SocketResponse<T>
    {

        [JsonProperty("header")]
        public HeaderResponse Header { get; set; }

        [JsonProperty("payload")]
        public PayloadResponse<T> Payload { get; set; }
    }

}
