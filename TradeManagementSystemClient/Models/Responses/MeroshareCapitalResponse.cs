namespace TradeManagementSystemClient.Models.Responses
{
    public class MeroshareCapitalResponse
    {
        public string Code { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public object StatusDto { get; set; }
        public string DisplayName => $"{Name}({Code})";
    }
}