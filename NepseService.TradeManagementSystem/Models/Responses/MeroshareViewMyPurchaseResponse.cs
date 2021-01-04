namespace NepseService.TradeManagementSystem.Models.Responses
{

    public class MeroshareViewMyPurchaseResponse
    {
        public float AverageBuyRate { get; set; }
        public string Isin { get; set; }
        public string ScripName { get; set; }
        public double TotalCost { get; set; }
        public int TotalQuantity { get; set; }
    }
}