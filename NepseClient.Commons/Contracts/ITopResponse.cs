namespace NepseClient.Commons.Contracts
{
    public interface ITopResponse
    {
        double Ltp { get; set; }
        double PerChange { get; set; }
        double PointChange { get; set; }
        string SecurityName { get; set; }
        string Symbol { get; set; }
    }
}