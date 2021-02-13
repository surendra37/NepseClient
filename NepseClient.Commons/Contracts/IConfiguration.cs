namespace NepseClient.Commons.Contracts
{
    public interface IConfiguration
    {
        IMeroShareConfiguration Meroshare { get; }
        ITmsConfiguration Tms { get; }
        bool ShowNepseNotice { get; set; }
        bool AutoRefreshOnLoad { get; set; }
        bool ShowFloorsheet { get; set; }
    }
}