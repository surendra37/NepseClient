namespace NepseClient.Commons.Contracts
{
    public interface IScripResponse
    {
        string Scrip { get; set; }
        string Name { get; set; }
        float FreeBalance { get; set; }
        float TotalBalance { get; set; }
        float CurrentBalance { get; set; }
        float LastTransactionPrice { get; set; }
        float PreviousClosePrice { get; set; }
        float LTPTotal { get; set; }
        float PreviousTotal { get; set; }
        float WaccValue { get; set; }
        float DailyGain { get; }
        float TotalGain { get; }
    }
}
