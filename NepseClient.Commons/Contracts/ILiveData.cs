namespace NepseClient.Commons.Contracts
{
    public interface ILiveData
    {
        string Symbol { get; }
        double Ltp { get; }
        double Ltv { get; }
        double PointChange { get; }
        double PercentChange { get; }
        double Open { get; }
        double High { get; }
        double Low { get; }
        double Close { get; }
        double Volume { get; }
    }
}
