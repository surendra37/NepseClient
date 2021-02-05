using System;

namespace NepseClient.Libraries.NepalStockExchange.Responses
{
    public class MarketStatusResponse
    {
        public string IsOpen { get; set; }
        public DateTime AsOf { get; set; }

        public bool IsClosed => "CLOSE".Equals(IsOpen, StringComparison.OrdinalIgnoreCase);
    }
}
