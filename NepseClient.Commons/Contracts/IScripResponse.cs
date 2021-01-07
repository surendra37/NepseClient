namespace NepseClient.Commons.Contracts
{
    public interface IScripResponse
    {
        string Scrip { get; }
        string Name { get; }
        double CurrentBalance { get; }
        double LastTransactionPrice { get; }
        double PreviousClosePrice { get; }
        double LTPTotal { get; }
        double PreviousTotal { get; }
        double WaccValue { get; }
        double TotalCost { get; }
        double DailyGain { get; }
        double TotalGain { get; }
    }
}
