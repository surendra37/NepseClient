using NepseClient.Commons.Utils;
using NepseClient.Libraries.NepalStockExchange.Contexts;

using Serilog;

using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace NepseApp.Service
{
    public class SQLiteDatabaseContext : DatabaseContext
    {
        private readonly string _connectionString;
        private readonly string _connectionStringReadOnly;
        public SQLiteDatabaseContext()
        {
            var source = "template.db3";
            var dataSource = PathUtils.ReplacePathHolders(ConfigurationManager.AppSettings["data_source"]);
            if (!File.Exists(dataSource))
            {
                File.Copy(source, dataSource);
            }

            Log.Debug("Using DataSource: {0}", dataSource);
            var builder = new SQLiteConnectionStringBuilder
            {
                DataSource = dataSource,
                Version = 3,
            };
            _connectionString = builder.ConnectionString;

            builder.ReadOnly = true;
            _connectionStringReadOnly = builder.ConnectionString;
        }

        public override IDbConnection GetConnection() => new SQLiteConnection(_connectionString);

        public override IDbConnection GetConnectionReadOnly() => new SQLiteConnection(_connectionStringReadOnly);
    }
}
