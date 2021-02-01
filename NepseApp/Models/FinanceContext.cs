using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using NepseApp.DataModels;

namespace NepseApp.Models
{
    public class FinanceContext : DbContext
    {
        public DbSet<LiveDataModel> Live { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = new SqliteConnectionStringBuilder
            {
                DataSource = @"C:\Users\surendra\Desktop\finance.db",
            }.ConnectionString;

            optionsBuilder.UseSqlite(connectionString)
                .UseSnakeCaseNamingConvention();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<LiveDataModel>()
                .HasKey(x => x.Symbol);
        }
    }
}
