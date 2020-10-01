namespace NepseClient.Commons.Contracts
{
    public interface IOrderType
    {
        int Id { get; set; }
        string OrderTypeCode { get; set; }
    }
}