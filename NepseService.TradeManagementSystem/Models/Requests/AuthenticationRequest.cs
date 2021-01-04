using NepseService.TradeManagementSystem.Utils;

using Newtonsoft.Json;

namespace NepseService.TradeManagementSystem.Models.Requests
{
    public class AuthenticationRequest
    {
        [JsonProperty("userName")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("otp")]
        public string OneTimePassword { get; set; }

        [JsonProperty("jwt")]
        public string JsonWebToken { get; set; }

        public AuthenticationRequest(string username, string password)
        {
            Username = username;
            Password = EncodingUtils.Base64Encode(password);
            OneTimePassword = JsonWebToken = string.Empty;
        }
    }
}
