using System;

using NepseClient.Commons;

using Newtonsoft.Json;

namespace TradeManagementSystemClient.Models.Requests
{
    public class TmsAuthenticationRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Otp { get; set; }
        public string Jwt { get; set; }

        public TmsAuthenticationRequest(string username, string password)
        {
            UserName = username;
            Password = EncodingUtils.Base64Encode(password);
            Otp = Jwt = string.Empty;
        }
    }
}
