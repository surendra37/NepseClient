namespace NepseClient.Libraries.NepalStockExchange.Responses
{
    public class PageableResponse<T>
    {
        public T[] Content { get; set; }
        public Pageable Pageable { get; set; }
        public int TotalPages { get; set; }
        public int TotalElements { get; set; }
        public bool Last { get; set; }
        public int Number { get; set; }
        public int Size { get; set; }
        public int NumberOfElements { get; set; }
        public Sort Sort { get; set; }
        public bool First { get; set; }
        public bool Empty { get; set; }
    }


}
