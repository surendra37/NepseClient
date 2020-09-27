using NepseClient.Commons;
using Newtonsoft.Json;

namespace TradeManagementSystemClient.Models.Requests
{
    public class AuthenticationRequest
    {
        [JsonProperty("userName")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("otp")]
        public string OTP { get; set; }

        [JsonProperty("jwt")]
        public string JWT { get; set; }

        public AuthenticationRequest(string username, string password)
        {
            Username = username;
            Password = EncodingUtils.Base64Encode(password);
        }
    }
}
