using NepseClient.Libraries.NepalStockExchange.Interfaces;

using System;
using System.Data;

namespace NepseClient.Libraries.NepalStockExchange.Contexts
{
    public abstract class DatabaseContext : IDatabaseContext
    {
        public DatabaseContext()
        {
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            TodayPrice = new TodayPriceContext(this);
            Watchlist = new WatchlistContext(this);
        }

        public TodayPriceContext TodayPrice { get; set; }
        public WatchlistContext Watchlist { get; set; }
        public abstract IDbConnection GetConnection();
        public abstract IDbConnection GetConnectionReadOnly();
    }
}
