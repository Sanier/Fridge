using Fridge.DAL.Configuration;
using Fridge.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fridge.DAL
{
    public class FridgeDbContext : DbContext
    {
        public DbSet<ProductEntity> Products { get; set; }

        public FridgeDbContext(DbContextOptions<FridgeDbContext> options) : base(options) 
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductEntity>()
                .HasKey(e => e.ProductId);
        }
    }
}
