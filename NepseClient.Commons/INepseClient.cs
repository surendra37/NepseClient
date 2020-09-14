using System.Collections.Generic;
namespace NepseClient.Commons
{
    public interface INepseClient
    {
        void Authenticate(string username, string password);
        IEnumerable<IScripResponse> GetMyPortfolio();

        void SaveSession();
        void RestoreSession();
        void Logout();
    }
}
