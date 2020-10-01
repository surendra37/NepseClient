namespace NepseClient.Commons.Contracts
{
    public interface IInstrumentType
    {
        string ActiveStatus { get; }
        string Code { get; }
        string Description { get; }
        int Id { get; }
    }
}