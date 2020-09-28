namespace NepseClient.Commons.Contracts
{
    public interface IIndexResponse
    {
        float Change { get; set; }
        string IndexCode { get; set; }
        float IndexValue { get; set; }
        float PercentageChange { get; set; }
        float PrevCloseIndex { get; set; }
    }
}