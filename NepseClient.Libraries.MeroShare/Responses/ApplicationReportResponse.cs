namespace NepseClient.Libraries.MeroShare.Models.Responses
{
    public class ApplicationReportItem
    {
        public int CompanyShareId { get; set; }
        public string? SubGroup { get; set; }
        public string? Scrip { get; set; }
        public string? CompanyName { get; set; }
        public string? ShareTypeName { get; set; }
        public string? ShareGroupName { get; set; }
        public string? StatusName { get; set; }
        public int ApplicantFormId { get; set; }
        public string Action { get; set; } = "apply";
    }

    public class ApplicationReportResponse
    {
        public ApplicationReportItem[] Object { get; set; }
        public int TotalCount { get; set; }
    }


}
