using Fridge.DAL.Configuration;
using Fridge.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fridge.DAL
{
    public class FridgeDbContext : DbContext
    {
        public DbSet<ProductEntity> Products { get; set; }
        public DbSet<UserEntity> Users { get; set; }

        public FridgeDbContext(DbContextOptions<FridgeDbContext> options) : base(options) 
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
        }
    }
}
