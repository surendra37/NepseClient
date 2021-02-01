using System.Collections.Generic;

namespace NepseClient.Libraries.MeroShare.Models.Responses
{
    public class MeroShareMyPortfolio
    {
        public double CurrentBalance { get; set; }
        public double LastTransactionPrice { get; set; }
        public double PreviousClosingPrice { get; set; }
        public string Script { get; set; }
        public string ScriptDesc { get; set; }
        public string ValueAsOfLastTransactionPrice { get; set; }
        public string ValueAsOfPreviousClosingPrice { get; set; }
        public double ValueOfLastTransPrice { get; set; }
        public double ValueOfPrevClosingPrice { get; set; }
    }

    public class MerosharePortfolioResponse
    {
        public List<MeroShareMyPortfolio> MeroShareMyPortfolio { get; set; }
        public int TotalItems { get; set; }
        public string TotalValueAsOfLastTransactionPrice { get; set; }
        public string TotalValueAsOfPreviousClosingPrice { get; set; }
        public double TotalValueOfLastTransPrice { get; set; }
        public double TotalValueOfPrevClosingPrice { get; set; }
    }


}
