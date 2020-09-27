using System.Collections.Generic;
namespace NepseClient.Commons
{
    public interface INepseClient
    {
        #region Session
        SessionInfo Session { get; }
        bool IsAuthenticated { get; }

        void Authenticate(string url, string username, string password);
        void SaveSession();
        void RestoreSession();
        void Logout(); 
        #endregion

        IEnumerable<IScripResponse> GetMyPortfolio();
        IEnumerable<ISecurityItem> GetLiveMarket();
        string GetSocketUrl();
        List<KeyValuePair<string, string>> GetCookies();
        bool IsLive();
    }
}
