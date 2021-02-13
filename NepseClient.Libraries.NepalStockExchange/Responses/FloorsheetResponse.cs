namespace NepseClient.Libraries.NepalStockExchange.Responses
{
    public class FloorsheetContent
    {
        public object Id { get; set; }
        public string ContractId { get; set; }
        public object ContractType { get; set; }
        public string StockSymbol { get; set; }
        public string BuyerMemberId { get; set; }
        public string SellerMemberId { get; set; }
        public int ContractQuantity { get; set; }
        public double ContractRate { get; set; }
        public double ContractAmount { get; set; }
        public string BusinessDate { get; set; }
        public int TradeBookId { get; set; }
        public int StockId { get; set; }
        public string BuyerBrokerName { get; set; }
        public string SellerBrokerName { get; set; }
        public object TradeTime { get; set; }
        public string SecurityName { get; set; }
    }

    public class Floorsheets
    {
        public FloorsheetContent[] Content { get; set; }
        public Pageable Pageable { get; set; }
        public int TotalPages { get; set; }
        public int TotalElements { get; set; }
        public bool Last { get; set; }
        public int Number { get; set; }
        public int Size { get; set; }
        public int NumberOfElements { get; set; }
        public Sort Sort { get; set; }
        public bool First { get; set; }
        public bool Empty { get; set; }
    }
    public class FloorsheetResponse
    {
        public double TotalAmount { get; set; }
        public long TotalQty { get; set; }
        public Floorsheets Floorsheets { get; set; }
        public long TotalTrades { get; set; }
    }
}
