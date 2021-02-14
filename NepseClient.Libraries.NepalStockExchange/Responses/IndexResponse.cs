namespace NepseClient.Libraries.NepalStockExchange.Responses
{
    public class IndexResponse
    {
        public int Id { get; set; }
        public string Index { get; set; }
        public double Close { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double PreviousClose { get; set; }
        public double Change { get; set; }
        public double PerChange { get; set; }
        public double FiftyTwoWeekHigh { get; set; }
        public double FiftyTwoWeekLow { get; set; }
        public double CurrentValue { get; set; }
    }
}
