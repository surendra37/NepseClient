namespace NepseClient.Libraries.NepalStockExchange.Responses
{
    public class MarketDepthContent
    {
        public int StockId { get; set; }
        public double OrderBookOrderPrice { get; set; }
        public int Quantity { get; set; }
        public int OrderCount { get; set; }
        public int IsBuy { get; set; }
    }

    public class MarketDepth
    {
        public MarketDepthContent[] BuyMarketDepthList { get; set; }
        public MarketDepthContent[] SellMarketDepthList { get; set; }
    }

    public class MarketDepthResonse
    {
        public int TotalBuyQty { get; set; }
        public MarketDepth MarketDepth { get; set; }
        public int TotalSellQty { get; set; }
    }
}
