using System;

namespace NepseClient.Libraries.NepalStockExchange.Responses
{
    public class TodayPriceContent
    {
        public int Id { get; set; }
        public string BusinessDate { get; set; }
        public int SecurityId { get; set; }
        public string Symbol { get; set; }
        public string SecurityName { get; set; }
        public double OpenPrice { get; set; }
        public double HighPrice { get; set; }
        public double LowPrice { get; set; }
        public double ClosePrice { get; set; }
        public int TotalTradedQuantity { get; set; }
        public double TotalTradedValue { get; set; }
        public double PreviousDayClosePrice { get; set; }
        public double FiftyTwoWeekHigh { get; set; }
        public double FiftyTwoWeekLow { get; set; }
        public DateTime LastUpdatedTime { get; set; }
        public double LastUpdatedPrice { get; set; }
        public int TotalTrades { get; set; }
        public double AverageTradedPrice { get; set; }
        public double? MarketCapitalization { get; set; }

        public double AverageVolume => TotalTradedValue/TotalTradedQuantity;

        public double ActualMarketCapitalization
        {
            get
            {
                if (double.IsNaN(MarketCapitalization ?? double.NaN))
                    return double.NaN;

                return (MarketCapitalization ?? double.NaN) * 1000000;
            }
        }
    }

    public class WatchableTodayPrice : TodayPriceContent
    {
        public bool IsWatching { get; set; }
    }
}