using Dapper;

using NepseClient.Libraries.NepalStockExchange.Interfaces;

using System;
using System.Data;

namespace NepseClient.Libraries.NepalStockExchange.Contexts
{
    public class LastUpdatedContext : IDatabaseContext
    {
        const string FindQuery = "SELECT updated_on FROM last_updated WHERE name=@Name";
        const string UpdateQuery = "REPLACE INTO last_updated (name, updated_on) values (@Name, @UpdatedOn)";

        private readonly IDatabaseContext _context;
        public IDbConnection GetConnection() => _context.GetConnection();
        public IDbConnection GetConnectionReadOnly() => _context.GetConnectionReadOnly();
        public LastUpdatedContext(IDatabaseContext context)
        {
            _context = context;
        }

        public bool AddOrReplace(string name)
        {
            using var connection = GetConnection();
            return connection.Execute(UpdateQuery, new { Name = name, UpdatedOn = DateTime.Now }) > 0;
        }

        public DateTime Find(string name)
        {
            using var connection = GetConnectionReadOnly();
            return connection.QueryFirstOrDefault<DateTime>(FindQuery, new { Name = name });
        }
    }
}
