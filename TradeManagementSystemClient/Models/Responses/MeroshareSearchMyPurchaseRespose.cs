namespace TradeManagementSystemClient.Models.Responses
{
    public class MeroshareSearchMyPurchaseRespose
    {
        public string Demat { get; set; }
        public string HistoryDescription { get; set; }
        public int Id { get; set; }
        public bool IsEdit { get; set; }
        public string Isin { get; set; }
        public string KnownPrice { get; set; }
        public object PurchasePrice { get; set; }
        public string PurchaseSource { get; set; }
        public float Rate { get; set; }
        public string Remarks { get; set; }
        public string Scrip { get; set; }
        public object TotalCost { get; set; }
        public long TransactionDate { get; set; }
        public int TransactionQuantity { get; set; }
        public double UserPrice { get; set; }
    }
}
