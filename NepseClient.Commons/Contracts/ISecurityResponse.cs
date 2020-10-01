namespace NepseClient.Commons.Contracts
{
    public interface ISecurityResponse
    {
        int Id { get; set; }
        string Symbol { get; set; }
        string SecurityName { get; set; }
        string CompanyName { get; set; }
        double FiftyTwoWeekhigh { get; set; }
        double FiftyTwoWeekLow { get; set; }
        string DisplayName { get; }
    }
}