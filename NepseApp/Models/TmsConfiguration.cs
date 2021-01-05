using NepseClient.Commons.Contracts;

namespace NepseApp.Models
{

    public class TmsConfiguration : ITmsConfiguration
    {
        public string BaseUrl { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}