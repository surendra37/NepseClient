namespace NepseClient.Commons.Contracts
{
    public interface ISession
    {
        void SaveSession();
        void RestoreSession();
    }
}
