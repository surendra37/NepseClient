using Newtonsoft.Json;

namespace NepseClient.Libraries.TradeManagementSystem.Requests
{
    public class SocketRequest
    {
        [JsonProperty("header")]
        public SocketHeaderRequest Header { get; }

        [JsonProperty("payload")]
        public SocketPayloadRequest Payload { get; } = new SocketPayloadRequest();

        public SocketRequest(bool startOrStop, string name)
        {
            Header = new SocketHeaderRequest(startOrStop, name);
        }
    }

    public class SocketHeaderRequest
    {

        [JsonProperty("channel")]
        public string Channel { get; } = "@control";

        [JsonProperty("transaction")]
        public string Transaction { get; }

        public SocketHeaderRequest(bool startOrStop, string name)
        {
            // start_index, start_top25securities, start_index, start_ticker
            var startText = startOrStop ? "start" : "stop";
            Transaction = $"{startText}_{name}";
        }
    }

    public class SocketPayloadRequest
    {

        [JsonProperty("argument")]
        public string Argument { get; } = "undefined";
    }

}
