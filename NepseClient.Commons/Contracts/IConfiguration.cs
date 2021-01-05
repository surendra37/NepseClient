namespace NepseClient.Commons.Contracts
{
    public interface IConfiguration
    {
        IMeroShareConfiguration Meroshare { get; }
        ITmsConfiguration Tms { get; }
    }
}