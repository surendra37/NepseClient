using Dapper;

using NepseClient.Libraries.NepalStockExchange.Interfaces;
using NepseClient.Libraries.NepalStockExchange.Models;
using NepseClient.Libraries.NepalStockExchange.Responses;

using Serilog;

using System;
using System.Collections.Generic;
using System.Data;

namespace NepseClient.Libraries.NepalStockExchange.Contexts
{
    public class TodayPriceContext : IDatabaseContext
    {
        const string SelectQuery = "SELECT * FROM today_prices";
        const string SelectQueryBySymbols = "SELECT * FROM today_prices WHERE symbol IN @symbols";
        const string CreateQuery = "INSERT INTO today_prices (id, business_date, security_id, symbol, security_name, open_price, high_price, low_price, close_price, total_traded_quantity, total_traded_value, previous_day_close_price, fifty_two_week_high, fifty_two_week_low, last_updated_time, last_updated_price, total_trades," +
            "average_traded_price, market_capitalization) VALUES (@Id, @BusinessDate, @SecurityId, @Symbol, @SecurityName, @OpenPrice, @HighPrice, @LowPrice, @ClosePrice, @TotalTradedQuantity, @TotalTradedValue, @PreviousDayClosePrice, @FiftyTwoWeekHigh, @FiftyTwoWeekLow, @LastUpdatedTime, @LastUpdatedPrice, @TotalTrades, @AverageTradedPrice, @MarketCapitalization)";
        const string DeleteQuery = "DELETE FROM today_prices";

        private readonly IDatabaseContext _context;

        public IDbConnection GetConnection() => _context.GetConnection();
        public IDbConnection GetConnectionReadOnly() => _context.GetConnectionReadOnly();

        public TodayPriceContext(IDatabaseContext context)
        {
            _context = context;
        }

        public int AddTodayPrice(TodayPriceContent price)
        {
            using var connection = GetConnection();
            return connection.Execute(CreateQuery, price);
        }
        public int AddTodayPrice(IEnumerable<TodayPriceContent> prices)
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
        public int Delete()
        {
            using var connection = GetConnection();
            return connection.Execute(DeleteQuery);
        }

        public IEnumerable<TodayPriceContent> Get(IEnumerable<string> symbols)
        {
            using var connection = GetConnectionReadOnly();
            return connection.Query<TodayPriceContent>(SelectQueryBySymbols, new { symbols });
        }
        public IEnumerable<TodayPriceContent> Get()
        {
            using var connection = GetConnectionReadOnly();
            return connection.Query<TodayPriceContent>(SelectQuery);
        }
    }

    public class WatchlistContext : IDatabaseContext
    {
        const string SelectQuery = "SELECT symbol FROM watchlist";
        const string SelectQueryForWatching = "SELECT symbol FROM watchlist WHERE is_watching=@IsWatching";
        const string UpdateQuery = "REPLACE INTO watchlist (symbol, is_watching) values (@Symbol, @IsWatching)";

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

        public IEnumerable<string> Get(bool isWatching = true)
        {
            using var connection = GetConnectionReadOnly();
            return connection.Query<string>(SelectQueryForWatching, new WatchlistItem { IsWatching = isWatching });
        }
    }
}
