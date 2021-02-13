using System;

namespace NepseClient.Libraries.NepalStockExchange.Responses
{
    public class GraphResponse
    {
        public DateTime BusinessDate { get; set; }
        public double OpenPrice { get; set; }
        public double HighPrice { get; set; }
        public double LowPrice { get; set; }
        public double PreviousDayClosePrice { get; set; }
        public double FiftyTwoWeekHigh { get; set; }
        public double LastTradedPrice { get; set; }
        public int TotalTradedQuantity { get; set; }
        public double ClosePrice { get; set; }
    }
}
