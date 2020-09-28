namespace NepseClient.Commons.Contracts
{
    public interface ITopOrderResponse
    {
        int BuySell { get; set; }
        int Id { get; set; }
        double Price { get; set; }
        int Quantity { get; set; }
        string Reserved { get; set; }
        int SequenceId { get; set; }
        int TotalOrders { get; set; }
    }
}