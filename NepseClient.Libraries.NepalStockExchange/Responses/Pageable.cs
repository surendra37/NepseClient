namespace NepseClient.Libraries.NepalStockExchange.Responses
{

    public class Pageable
    {
        public Sort Sort { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public int Offset { get; set; }
        public bool Paged { get; set; }
        public bool Unpaged { get; set; }
    }
}