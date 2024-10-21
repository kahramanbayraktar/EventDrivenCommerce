using InventoryService.Domain.Entities;
using InventoryService.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Infrastructure.Data.Context
{
    public class InventoryItemDbContext : DbContext
    {
        public InventoryItemDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new InventoryItemConfiguration());
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<InventoryItem> InventoryItems { get; set; }
    }
}
