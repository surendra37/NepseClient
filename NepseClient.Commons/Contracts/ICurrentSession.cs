namespace NepseClient.Commons.Contracts
{
    public interface ICurrentSession
    {
        string ActiveStatus { get; set; }
        int ExchangeMarketSessionId { get; set; }
        int Id { get; set; }
        string SessionEndTime { get; set; }
        string SessionName { get; set; }
        string SessionStartTime { get; set; }
    }
}