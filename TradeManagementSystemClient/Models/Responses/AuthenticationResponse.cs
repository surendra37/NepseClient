using System;
using Newtonsoft.Json;

namespace TradeManagementSystemClient.Models.Responses
{
    public class AuthenticationResponse
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("level")]
        public string Level { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public object Data { get; set; }

        public AuthenticationResponse()
        {
            
        }
    }
}
