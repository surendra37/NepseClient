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
            LastUpdated = new LastUpdatedContext(this);
            TodayPrice = new TodayPriceContext(this, LastUpdated);
            Watchlist = new WatchlistContext(this);
        }

        public TodayPriceContext TodayPrice { get; set; }
        public WatchlistContext Watchlist { get; set; }
        public LastUpdatedContext LastUpdated { get; set; }
        public abstract IDbConnection GetConnection();
        public abstract IDbConnection GetConnectionReadOnly();
    }
}
