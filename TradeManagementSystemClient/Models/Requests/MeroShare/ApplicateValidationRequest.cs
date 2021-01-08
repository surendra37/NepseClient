namespace TradeManagementSystemClient.Models.Requests.MeroShare
{
    public class ApplicateValidationRequest
    {
        public string TransactionPIN { get; set; } 
        public string CompanyShareId { get; set; } 
        public int ApplicantFormId { get; set; } 
    }
}