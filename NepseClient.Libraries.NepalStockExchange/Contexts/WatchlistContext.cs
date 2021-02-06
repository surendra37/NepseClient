using Dapper;

using NepseClient.Libraries.NepalStockExchange.Interfaces;
using NepseClient.Libraries.NepalStockExchange.Models;

using System.Collections.Generic;
using System.Data;

namespace NepseClient.Libraries.NepalStockExchange.Contexts
{
    public class WatchlistContext : IDatabaseContext
    {
        public const string TableName = "watchlist";
        const string SelectBySymbol = "SELECT is_watching FROM " + TableName + " WHERE symbol=@Symbol";
        const string SelectQueryForWatching = "SELECT symbol FROM " + TableName + " WHERE is_watching=@IsWatching";
        const string UpdateQuery = "REPLACE INTO " + TableName +
            " (symbol, is_watching) values (@Symbol, @IsWatching)";

        private readonly IDatabaseContext _context;
        public IDbConnection GetConnection() => _context.GetConnection();
        public IDbConnection GetConnectionReadOnly() => _context.GetConnectionReadOnly();
        public WatchlistContext(IDatabaseContext context)
        {
            _context = context;
        }

        public int Update(WatchlistItem item)
        {
            using var connection = GetConnection();
            return connection.Execute(UpdateQuery, item);
        }
        public bool Update(string name, bool isWatching)
        {
            return Update(new WatchlistItem
            {
                IsWatching = isWatching,
                Symbol = name,
            }) > 0;
        }

        public IEnumerable<string> Get(bool isWatching = true)
        {
            using var connection = GetConnectionReadOnly();
            return connection.Query<string>(SelectQueryForWatching, new WatchlistItem { IsWatching = isWatching });
        }

        public bool Find(string name)
        {
            using var connection = GetConnectionReadOnly();
            return connection.QueryFirstOrDefault<bool>(SelectBySymbol, new { Symbol = name });
        }
    }
}
