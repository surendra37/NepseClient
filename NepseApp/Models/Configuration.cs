using NepseClient.Commons.Contracts;

namespace NepseApp.Models
{
    public class Configuration : IConfiguration
    {
        public ITmsConfiguration Tms { get; } = new TmsConfiguration();
        public IMeroShareConfiguration Meroshare { get; } = new MeroShareConfiguration();
    }
}
