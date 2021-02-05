using System.Data;

namespace NepseClient.Libraries.NepalStockExchange.Interfaces
{
    public interface IDatabaseContext
    {
        IDbConnection GetConnection();
        IDbConnection GetConnectionReadOnly();
    }
}
