namespace NepseService.TradeManagementSystem.Models.Responses
{

    public class ScripReponse
    {
        public string Scrip { get; set; }
        public double CurrentBalance { get; set; }
        public double PreviousCloseprice { get; set; }
        public double ValueAsOfPreviousClosePrice { get; set; }
        public double Ltp { get; set; }
        public double ValueAsOfLTP { get; set; }
        public int CdsFreeBalance { get; set; }
        public int CdsTotalBalance { get; set; }
        public string SymbolName { get; set; }
    }
}