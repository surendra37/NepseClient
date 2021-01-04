using Newtonsoft.Json;

namespace NepseService.TradeManagementSystem.Models.Requests
{

    public class MeroshareAuthRequest
    {
        [JsonProperty("clientId")]
        public int ClientId { get; }

        [JsonProperty("username")]
        public string Username { get; }

        [JsonProperty("password")]
        public string Password { get; }

        public MeroshareAuthRequest(int clientId, string username, string password)
        {
            ClientId = clientId;
            Username = username;
            Password = password;
        }
    }
}