using System;

namespace NepseClient.Commons
{

    public class SessionInfo
    {
        public string Host { get; set; }
        public string Username { get; set; }
        //public string Password { get; set; }

        public bool CookieEnabled { get; set; }
        public string JsonWebToken { get; set; }
        public string XsrfToken { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string ClientId { get; set; }
        public string DealerId { get; set; }
        public string MemberId { get; set; }
        public string UserId { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}