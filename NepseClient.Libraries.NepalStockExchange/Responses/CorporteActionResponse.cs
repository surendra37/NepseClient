using System;

namespace NepseClient.Libraries.NepalStockExchange.Responses
{
    public class CorporteActionResponse
    {
        public string ActiveStatus { get; set; }
        public string AuthorizationComments { get; set; }
        public DateTime SubmittedDate { get; set; }
        public string FilePath { get; set; }
        public double? DocumentId { get; set; }
        public int? RatioNum { get; set; }
        public int? RatioDen { get; set; }
        public double? CashDividend { get; set; }
        public string FiscalYear { get; set; }

        public double? Percentage
        {
            get
            {
                if (RatioNum is null || RatioDen is null || RatioDen is null) return null;

                return (double)RatioNum / RatioDen;
            }
        }
    }
}
