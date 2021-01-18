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
}