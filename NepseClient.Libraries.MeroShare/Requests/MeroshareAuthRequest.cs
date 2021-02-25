namespace NepseClient.Libraries.MeroShare.Models.Requests
{

    public class MeroshareAuthRequest
    {
        public string ClientId { get; }
        public string Username { get; }
        public string Password { get; }

        public MeroshareAuthRequest(string clientId, string username, string password)
        {
            ClientId = clientId;
            Username = username;
            Password = password;
        }
    }
}