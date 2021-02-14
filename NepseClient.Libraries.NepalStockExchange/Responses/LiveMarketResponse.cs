﻿namespace NepseClient.Libraries.NepalStockExchange.Responses
{
    public class LiveMarketResponse
    {
        public string SecurityId { get; set; }
        public string SecurityName { get; set; }
        public string Symbol { get; set; }
        public int IndexId { get; set; }
        public double OpenPrice { get; set; }
        public double HighPrice { get; set; }
        public double LowPrice { get; set; }
        public int TotalTradeQuantity { get; set; }
        public double LastTradedPrice { get; set; }
        public double PercentageChange { get; set; }
        public string LastUpdatedDateTime { get; set; }
        public int LastTradedVolume { get; set; }
        public double PreviousClose { get; set; }
        public double PointChange => LastTradedPrice - PreviousClose;
    }



}