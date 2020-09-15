namespace NepseClient.Commons
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
    }
}
