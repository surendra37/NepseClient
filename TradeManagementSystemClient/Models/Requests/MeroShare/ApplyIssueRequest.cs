namespace TradeManagementSystemClient.Models.Requests.MeroShare
{
    public class ApplyIssueRequest
    {
        public string Demat { get; set; }
        public string Boid { get; set; }
        public string AppliedKitta { get; set; }
        public string AccountNumber { get; set; }
        public int CustomerId { get; set; }
        public int AccountBranchId { get; set; }
        public string CrnNumber { get; set; }
        public string CompanyShareId { get; set; }
        public int BankId { get; set; }
    }
}