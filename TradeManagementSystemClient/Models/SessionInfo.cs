using System;
namespace TradeManagementSystemClient.Models
{
    public struct SessionInfo
    {
        public string XsrfToken { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string ClientId { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
