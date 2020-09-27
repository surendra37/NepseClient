namespace NepseClient.Commons.Contracts
{
    public interface ITopSecuritiesResponse
    {
        double LastTradePrice { get; set; }
        double PerChange { get; set; }
        double PrevClosePrice { get; set; }
        string SecurityName { get; set; }
        string SecuritySymbol { get; set; }
        int TotalTransaction { get; set; }
        double TotalTurnover { get; set; }
        int Volume { get; set; }
    }
}