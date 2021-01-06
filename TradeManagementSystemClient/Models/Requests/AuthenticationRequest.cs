using System;

using NepseClient.Commons;

using Newtonsoft.Json;

namespace TradeManagementSystemClient.Models.Requests
{
    public class AuthenticationRequest
    {
        [JsonIgnore]
        public Uri BaseUrl { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Otp { get; set; }
        public string Jwt { get; set; }

        public AuthenticationRequest(string username, string password)
        {
            UserName = username;
            Password = EncodingUtils.Base64Encode(password);
        }
    }
}
