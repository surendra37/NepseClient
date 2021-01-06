namespace NepseClient.Commons.Contracts
{
    public interface IScripResponse
    {
        string Scrip { get; }
        string Name { get; }
        float FreeBalance { get; }
        float TotalBalance { get; }
        float CurrentBalance { get; }
        float LastTransactionPrice { get; }
        float PreviousClosePrice { get; }
        float LTPTotal { get; }
        float PreviousTotal { get; }
        float TotalCost { get; }
        float WaccValue { get; }
        float DailyGain { get; }
        float TotalGain { get; }
    }
}
