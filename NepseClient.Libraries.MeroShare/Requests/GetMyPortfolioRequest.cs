namespace NepseClient.Libraries.MeroShare.Models.Requests
{
    public class GetMyPortfolioRequest
    {
        public string SortBy { get; set; } 
        public string[] Demat { get; set; } 
        public string ClientCode { get; set; } 
        public int Page { get; set; } 
        public int Size { get; set; } 
        public bool SortAsc { get; set; } 
    }
}
