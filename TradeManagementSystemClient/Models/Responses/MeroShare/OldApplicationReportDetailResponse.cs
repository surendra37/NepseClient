using System;

namespace TradeManagementSystemClient.Models.Responses.MeroShare
{
    public class OldApplicationReportDetailResponse
    {
        public string AccountNumber { get; set; }
        public string Action { get; set; }
        public double Amount { get; set; }
        public int ApplicantFormId { get; set; }
        public DateTime AppliedDate { get; set; }
        public int AppliedKitta { get; set; }
        public string ClientName { get; set; }
        public DateTime MaxIssueCloseDate { get; set; }
        public int MeroShareId { get; set; }
        public string MeroshareRemark { get; set; }
        public string ReasonOrRemark { get; set; }
        public int ReceivedKitta { get; set; }
        public string RegisteredBranchName { get; set; }
        public string StageName { get; set; }
        public string StatusDescription { get; set; }
        public string StatusName { get; set; }
        public string SuspectStatusName { get; set; }
    }
}