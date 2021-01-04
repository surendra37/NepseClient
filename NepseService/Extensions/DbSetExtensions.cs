
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace NepseService.Extensions
{
    public static class DbSetExtensions
    {
        public static void Truncate(this DatabaseFacade database, string tableName)
        {
            database.ExecuteSqlRaw($"DELETE FROM {tableName}");
        }
    }
}
