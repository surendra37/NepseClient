using Dapper;

using NepseClient.Libraries.NepalStockExchange.Interfaces;
using NepseClient.Libraries.NepalStockExchange.Responses;

using Serilog;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace NepseClient.Libraries.NepalStockExchange.Contexts
{
    public class TodayPriceContext : IDatabaseContext
    {
        public const string TableName = "today_prices";
        const string SelectQuery = "SELECT * FROM today_prices";
        const string SelectWatchableQuery = "SELECT t.*, w.is_watching FROM " + TableName + " t LEFT JOIN watchlist w on t.symbol=w.symbol";
        const string SelectQueryBySymbols = "SELECT * FROM today_prices WHERE symbol IN @symbols";
        const string SelectQueryBySymbol = "SELECT * FROM today_prices WHERE symbol=@Symbol";
        const string CreateQuery = "INSERT INTO today_prices (id, business_date, security_id, symbol, security_name, open_price, high_price, low_price, close_price, total_traded_quantity, total_traded_value, previous_day_close_price, fifty_two_week_high, fifty_two_week_low, last_updated_time, last_updated_price, total_trades," +
            "average_traded_price, market_capitalization) VALUES (@Id, @BusinessDate, @SecurityId, @Symbol, @SecurityName, @OpenPrice, @HighPrice, @LowPrice, @ClosePrice, @TotalTradedQuantity, @TotalTradedValue, @PreviousDayClosePrice, @FiftyTwoWeekHigh, @FiftyTwoWeekLow, @LastUpdatedTime, @LastUpdatedPrice, @TotalTrades, @AverageTradedPrice, @MarketCapitalization)";
        const string DeleteQuery = "DELETE FROM today_prices";

        private readonly IDatabaseContext _context;
        private readonly LastUpdatedContext _lastUpdatedContext;

        public IDbConnection GetConnection() => _context.GetConnection();
        public IDbConnection GetConnectionReadOnly() => _context.GetConnectionReadOnly();

        public TodayPriceContext(IDatabaseContext context, LastUpdatedContext lastUpdatedContext)
        {
            _context = context;
            _lastUpdatedContext = lastUpdatedContext;
        }

        public int Add(TodayPriceContent price)
        {
            using var connection = GetConnection();
            return connection.Execute(CreateQuery, price);
        }
        public int AddRange(IEnumerable<TodayPriceContent> prices)
        {
            using var connection = GetConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                var output = connection.Execute(CreateQuery, prices, transaction);
                transaction.Commit();
                return output;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to add today prices, Rolling back..");
                transaction.Rollback();
                throw;
            }
        }
        public int Clear()
        {
            using var connection = GetConnection();
            return connection.Execute(DeleteQuery);
        }

        public IEnumerable<TodayPriceContent> Find(IEnumerable<string> symbols)
        {
            using var connection = GetConnectionReadOnly();
            return connection.Query<TodayPriceContent>(SelectQueryBySymbols, new { symbols }) ?? Enumerable.Empty<TodayPriceContent>();
        }

        public WatchableTodayPrice Find(string symbol)
        {
            using var connection = GetConnectionReadOnly();
            return connection.QueryFirstOrDefault<WatchableTodayPrice>(SelectQueryBySymbol, new { Symbol= symbol });
        }
        public IEnumerable<TodayPriceContent> Get()
        {
            using var connection = GetConnectionReadOnly();
            return connection.Query<TodayPriceContent>(SelectQuery);
        }

        public IEnumerable<WatchableTodayPrice> GetWatchables()
        {
            using var connection = GetConnectionReadOnly();
            return connection.Query<WatchableTodayPrice>(SelectWatchableQuery);
        }

        public DateTime GetLastUpdatedOn()
        {
            return _lastUpdatedContext.Find(TableName);
        }

        public bool SetLastUpdated()
        {
            return _lastUpdatedContext.AddOrReplace(TableName);
        }
    }
}
