
using System.Security.Cryptography.X509Certificates;

using Microsoft.EntityFrameworkCore;

using NepseService.TradeManagementSystem.Models.Responses;

namespace NepseService.Models
{
    public class DatabaseContext : DbContext
    {
        public DbSet<ScripReponse> Scrips { get; set; }
        public DbSet<WaccItem> Waccs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=./nepse.sqlite");
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ScripReponse>()
                .HasKey(x => x.Scrip);

            modelBuilder.Entity<WaccItem>()
                .HasKey(x => x.Scrip);
            base.OnModelCreating(modelBuilder);
        }
    }
}
