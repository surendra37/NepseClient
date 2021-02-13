using NepseClient.Commons.Contracts;

namespace NepseApp.Models
{
    public class Configuration : IConfiguration
    {
        public bool ShowNepseNotice
        {
            get => Settings.Default.ShowNepseNotice;
            set
            {
                Settings.Default.ShowNepseNotice = value;
                Settings.Default.Save();
            }
        }
        public bool ShowFloorsheet
        {
            get => Settings.Default.ShowFloorsheet;
            set
            {
                Settings.Default.ShowFloorsheet = value;
                Settings.Default.Save();
            }
        }
        public bool AutoRefreshOnLoad
        {
            get => Settings.Default.AutoRefreshOnLoad;
            set
            {
                Settings.Default.AutoRefreshOnLoad = value;
                Settings.Default.Save();
            }
        }
        public ITmsConfiguration Tms { get; } = new TmsConfiguration();
        public IMeroShareConfiguration Meroshare { get; } = new MeroShareConfiguration();
    }
}
