using Dapper;

using NepseClient.Commons.Interfaces;
using NepseClient.Libraries.NepalStockExchange.Interfaces;

using System.Data;
using System.Threading.Tasks;

namespace NepseClient.Libraries.NepalStockExchange.Contexts
{
    public class WaccContext : IDatabaseContext
    {
        private const string TableName = "wacc";
        private const string InsertQuery =
            @"INSERT INTO wacc (average_buy_rate, isin, scrip_name, total_cost, total_quantity) VALUES (@AverageBuyRate, @Isin, @ScripName, @TotalCost, @TotalQuantity)";
        private const string DeleteQuery = "DELETE FROM wacc";
        private const string SelectBuyRateQuery = "SELECT average_buy_rate FROM wacc WHERE scrip_name=@Name";

        private readonly IDatabaseContext _context;

        public IDbConnection GetConnection() => _context.GetConnection();
        public IDbConnection GetConnectionReadOnly() => _context.GetConnectionReadOnly();

        public WaccContext(IDatabaseContext context)
        {
            _context = context;
        }

        public async Task<bool> Insert(IWaccItem item)
        {
            using var connection = GetConnection();
            var inserted = await connection.ExecuteAsync(InsertQuery, item);
            return inserted > 0;
        }

        public async Task<double> GetBuyRate(string scrip)
        {
            using var connection = GetConnectionReadOnly();
            return await connection.QueryFirstOrDefaultAsync<double>(SelectBuyRateQuery, new { Name = scrip });
        }
    }
}
