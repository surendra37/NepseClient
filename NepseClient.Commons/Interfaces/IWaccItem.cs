namespace NepseClient.Commons.Interfaces
{
    public interface IWaccItem
    {
        double AverageBuyRate { get; }
        string Isin { get; }
        string ScripName { get; }
        double TotalCost { get; }
        int TotalQuantity { get; }
    }
}
