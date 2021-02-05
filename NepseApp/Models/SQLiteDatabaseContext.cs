using NepseClient.Libraries.NepalStockExchange.Contexts;

using System.Data;
using System.Data.SQLite;

namespace NepseApp.Models
{
    public class SQLiteDatabaseContext : DatabaseContext
    {
        private readonly string _connectionString;
        private readonly string _connectionStringReadOnly;

        public SQLiteDatabaseContext()
        {
            var builder = new SQLiteConnectionStringBuilder
            {
                DataSource = @"C:\Program Files\DB Browser for SQLite\finance.db",
                Version = 3,
            };
            _connectionString = builder.ConnectionString;

            builder.ReadOnly = true;
            _connectionStringReadOnly = builder.ConnectionString;
        }

        public override IDbConnection GetConnection() => new SQLiteConnection(_connectionString);

        public override IDbConnection GetConnectionReadOnly()=> new SQLiteConnection(_connectionString);
    }
}
