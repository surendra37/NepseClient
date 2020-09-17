using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Text;

namespace NepseClient.Commons
{
    public class ProxyNepseClient : INepseClient
    {
        private readonly INepseClient _client;

        public SessionInfo Session => _client.Session;
        public Action<INepseClient> ShowAuthenticationDialog { get; }
        public ProxyNepseClient(INepseClient client, Action<INepseClient> showAuthDialog)
        {
            _client = client;
            ShowAuthenticationDialog = showAuthDialog;
        }

        #region Session
        public void Authenticate(string url, string username, string password) => _client.Authenticate(url, username, password);
        public void Logout() => _client.Logout();
        public void RestoreSession() => _client.RestoreSession();
        public void SaveSession() => _client.SaveSession();
        #endregion

        public IEnumerable<IScripResponse> GetMyPortfolio()
        {
            return Retry(_client.GetMyPortfolio);
        }

        private T Retry<T>(Func<T> body, bool retry = true)
        {
            try
            {
                return body();

            }
            catch (AuthenticationException)
            {
                if (retry)
                {
                    ShowAuthenticationDialog?.Invoke(_client);
                    return Retry(body, false);
                }
                throw;
            }
        }
    }
}
