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

        public IEnumerable<IMarketWatchResponse> GetMarketWatch() => Retry(_client.GetMarketWatch);

        public IEnumerable<ITopResponse> GetTopGainers() => Retry(_client.GetTopGainers);

        public IEnumerable<ITopResponse> GetTopLosers() => Retry(_client.GetTopLosers);

        public IEnumerable<ITopSecuritiesResponse> GetTopTurnovers() => Retry(_client.GetTopTurnovers);

        public IEnumerable<ITopSecuritiesResponse> GetTopTransactions() => Retry(_client.GetTopTransactions);

        public IEnumerable<ITopSecuritiesResponse> GetTopVolumes() => Retry(_client.GetTopVolumes);

        public IEnumerable<IIndexResponse> GetIndices() => Retry(_client.GetIndices);

        public IEnumerable<IStockQuoteResponse> GetStockQuote(string id) => Retry(() => _client.GetStockQuote(id));
    }
}
