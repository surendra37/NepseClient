namespace NepseClient.Commons.Contracts
{
    public interface ICurrentMarketSession
    {
        bool ActiveStatus { get; set; }
        ICurrentSession[] CurrentSessions { get; set; }
        INonSession[] NonSessions { get; set; }
    }
}