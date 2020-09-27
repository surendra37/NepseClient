using NepseClient.Commons.Contracts;
using System;
using System.Collections.Generic;
using System.Security.Authentication;

namespace NepseClient.Commons
{
    public class ProxyNepseClient : INepseClient
    {
        private readonly INepseClient _client;

        public bool IsAuthenticated => _client.IsAuthenticated;
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

        public IEnumerable<ISecurityItem> GetLiveMarket()
        {
            return Retry(_client.GetLiveMarket);
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

        public string GetSocketUrl() => Retry(_client.GetSocketUrl);

        public List<KeyValuePair<string, string>> GetCookies() => _client.GetCookies();

        public bool IsLive() => _client.IsLive();

        public IEnumerable<IMarketWatchResponse> GetMarketWatch()
        {
            return Retry(_client.GetMarketWatch);
        }

        public IEnumerable<ITopResponse> GetTopGainers()
        {
            return Retry(_client.GetTopGainers);
        }

        public IEnumerable<ITopResponse> GetTopLosers()
        {
            return Retry(_client.GetTopLosers);
        }

        public IEnumerable<ITopSecuritiesResponse> GetTopTurnovers()
        {
            return Retry(_client.GetTopTurnovers);
        }

        public IEnumerable<ITopSecuritiesResponse> GetTopTransactions()
        {
            return Retry(_client.GetTopTransactions);
        }

        public IEnumerable<ITopSecuritiesResponse> GetTopVolumes()
        {
            return Retry(_client.GetTopVolumes);
        }
    }
}
