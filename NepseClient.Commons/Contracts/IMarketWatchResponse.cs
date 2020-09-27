using System.Collections.Generic;

namespace NepseClient.Commons.Contracts
{
    public interface IMarketWatchResponse
    {
        int MarketWatchID { get; }
        string MarketWatchName { get; }
        ISecurityResponse[] Securities { get; }
        int UserId { get; }
    }
}