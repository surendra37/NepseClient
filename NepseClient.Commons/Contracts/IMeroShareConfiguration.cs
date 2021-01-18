namespace NepseClient.Commons.Contracts
{
    public interface IMeroShareConfiguration
    {
        string Username { get; set; }
        string Password { get; set; }
        string ClientId { get; set; }
        bool RememberPassword { get; set; }
        string AuthHeader { get; set; }

        void Save();
    }
}