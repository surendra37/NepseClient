namespace NepseClient.Commons.Contracts
{
    public interface IMeroshareSearchMyPurchaseRespose
    {
        string Demat { get; set; }
        string HistoryDescription { get; set; }
        int Id { get; set; }
        bool IsEdit { get; set; }
        string Isin { get; set; }
        string KnownPrice { get; set; }
        object PurchasePrice { get; set; }
        string PurchaseSource { get; set; }
        float Rate { get; set; }
        string Remarks { get; set; }
        string Scrip { get; set; }
        object TotalCost { get; set; }
        long TransactionDate { get; set; }
        int TransactionQuantity { get; set; }
        double UserPrice { get; set; }
    }
}