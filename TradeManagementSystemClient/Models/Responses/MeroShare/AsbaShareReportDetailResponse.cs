using System;

namespace TradeManagementSystemClient.Models.Responses.MeroShare
{
    public class AsbaShareReportDetailResponse
    {
        public string ClientName { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public int CompanyShareId { get; set; }
        public DateTime MaxIssueCloseDate { get; set; }
        public int MaxUnit { get; set; }
        public DateTime MinIssueOpenDate { get; set; }
        public int MinUnit { get; set; }
        public int MultipleOf { get; set; }
        public string ProspectusPath { get; set; }
        public string ProspectusRemarks { get; set; }
        public string Scrip { get; set; }
        public string ShareGroupName { get; set; }
        public double SharePerUnit { get; set; }
        public string ShareTypeName { get; set; }
        public double ShareValue { get; set; }
        public string SubGroup { get; set; }
    }



}
