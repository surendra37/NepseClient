using NepseClient.Commons.Interfaces;

namespace NepseClient.Libraries.MeroShare.Models.Responses
{
    public class MeroshareViewMyPurchaseResponse : IWaccItem
    {
        public double AverageBuyRate { get; set; }
        public string Isin { get; set; }
        public string ScripName { get; set; }
        public double TotalCost { get; set; }
        public int TotalQuantity { get; set; }
    }
}