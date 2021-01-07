namespace NepseClient.Commons.Contracts
{
    public interface IMeroShareConfiguration
    {
        string BaseUrl { get; set; }
        string Username { get; set; }
        string Password { get; set; }
        string ClientId { get; set; }
        string[] Demat { get; set; }
        bool RememberPassword { get; set; }
        string AuthHeader { get; set; }

        void Save();
    }
}