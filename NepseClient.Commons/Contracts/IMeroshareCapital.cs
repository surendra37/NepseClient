namespace NepseClient.Commons.Contracts
{
    public interface IMeroshareCapital
    {
        string Code { get; set; }
        int Id { get; set; }
        string Name { get; set; }
        object StatusDto { get; set; }
        string DisplayName { get; }
    }
}