using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NepseClient.Commons.Contracts
{
    public interface INepseClient
    {
        Action ShowAuthenticationDialog { get; set; }

        #region Session
        SessionInfo Session { get; }
        bool IsAuthenticated { get; }

        Task AuthenticateAsync(string url, string username, string password);
        void SaveSession();
        void RestoreSession();
        void Logout();
        #endregion

        Task<IEnumerable<IScripResponse>> GetMyPortfolioAsync(CancellationToken ct = default);
        string GetSocketUrl();
        List<KeyValuePair<string, string>> GetCookies();
        bool IsLive();
        Task<IEnumerable<ITopResponse>> GetTopGainersAsync(CancellationToken ct = default);
        Task<IEnumerable<ITopResponse>> GetTopLosersAsync(CancellationToken ct = default);
        Task<IEnumerable<ITopSecuritiesResponse>> GetTopTurnoversAsync(CancellationToken ct = default);
        Task<IEnumerable<ITopSecuritiesResponse>> GetTopTransactionsAsync(CancellationToken ct = default);
        Task<IEnumerable<ITopSecuritiesResponse>> GetTopVolumesAsync(CancellationToken ct = default);
        Task<IEnumerable<IIndexResponse>> GetIndicesAsync(CancellationToken ct = default);
        Task<IEnumerable<IStockQuoteResponse>> GetStockQuoteAsync(string id, CancellationToken ct = default);
        void HandleAuthException(AggregateException ex, ICommand command, object commandParameter = null);
        Task<IEnumerable<ISecurityItem>> GetLiveMarketAsync(CancellationToken ct = default);
        Task<IEnumerable<IMarketWatchResponse>> GetMarketWatchAsync(CancellationToken ct = default);
        Task<ICachedDataResponse> GetCachedData(CancellationToken ct = default);
    }
}
