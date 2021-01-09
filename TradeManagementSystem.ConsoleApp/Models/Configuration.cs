using System;
using NepseClient.Commons.Contracts;

namespace TradeManagementSystem.ConsoleApp.Models
{
    public class Configuration : IConfiguration
    {
        public Configuration()
        {
        }

        public IMeroShareConfiguration Meroshare { get; } = new MeroShareConfiguration();

        public ITmsConfiguration Tms { get; } = new TmsConfiguration();
    }

    public class MeroShareConfiguration : IMeroShareConfiguration
    {
        public string BaseUrl { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Username { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Password { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string ClientId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string[] Demat { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool RememberPassword { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string AuthHeader { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Save()
        {
            throw new NotImplementedException();
        }
    }

    public class TmsConfiguration : ITmsConfiguration
    {
        public string BaseUrl { get; set; } = "https://tms49.nepsetms.com.np";
        public string Password { get; set; } = "Pass@word1";
        public string Username { get; set; } = "SS482167";
        public bool RememberPassword { get; set; } = true;

        public void Save()
        {
            
        }
    }
}
