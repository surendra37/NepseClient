namespace NepseClient.Commons.Contracts
{
    public interface ITmsConfiguration
    {
        string BaseUrl { get; set; }
        string Password { get; set; }
        string Username { get; set; }
        bool RememberPassword { get; set; }

        void Save();
    }
}