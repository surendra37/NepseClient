using System.Data;

namespace NepseClient.Libraries.Database
{
    public abstract class DatabaseContext
    {
        public abstract IDbConnection GetConnection();
        public abstract IDbConnection GetConnectionReadOnly();

        public virtual void PrepareDatabase()
        {

        }
    }
}
