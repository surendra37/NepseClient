namespace TradeManagementSystemClient.Models.Responses.Tms
{
    public class GraphDataResponse
    {
        public object SecurityId { get; set; }
        public GraphDataDTO[] GraphDataDTOS { get; set; }
        public string Isin { get; set; }
    }
    public class GraphDataDTO
    {
        public double? Open { get; set; }
        public double Close { get; set; }
        public double? High { get; set; }
        public double? Low { get; set; }
        public int? Volume { get; set; }
        public string Date { get; set; }
    }
    public class TopGainerResponse
    {
        public string Symbol { get; set; }
        public double Ltp { get; set; }
        public double PointChange { get; set; }
        public double PerChange { get; set; }
        public string SecurityName { get; set; }
    }
    public class TopTurnoverResponse
    {
        public string SecuritySymbol { get; set; }
        public string SecurityName { get; set; }
        public double TotalTurnover { get; set; }
        public int Volume { get; set; }
        public int TotalTransaction { get; set; }
        public double LastTradePrice { get; set; }
        public double PerChange { get; set; }
        public double PrevClosePrice { get; set; }
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class TopVolume    {
        public string SecuritySymbol { get; set; } 
        public string SecurityName { get; set; } 
        public double TotalTurnover { get; set; } 
        public int Volume { get; set; } 
        public int TotalTransaction { get; set; } 
        public double LastTradePrice { get; set; } 
        public double PerChange { get; set; } 
        public double PrevClosePrice { get; set; } 
    }






}