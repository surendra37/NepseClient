namespace NepseClient.Commons.Contracts
{
    public interface IMeroshareViewMyPurchase
    {
        float AverageBuyRate { get; set; }
        string Isin { get; set; }
        string ScripName { get; set; }
        double TotalCost { get; set; }
        int TotalQuantity { get; set; }
    }
}