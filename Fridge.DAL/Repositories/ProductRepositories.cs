using Fridge.DAL.Interfaces;
using Fridge.Domain.Entities;

namespace Fridge.DAL.Repositories
{
    public class ProductRepositories : IBaseRepositories<ProductEntity>
    {
        private FridgeDbContext _fridgeDbContext;

        public ProductRepositories(FridgeDbContext fridgeDbContext)
        {
            _fridgeDbContext = fridgeDbContext;
        }
        public async Task Create(ProductEntity entity)
        {
            await _fridgeDbContext.Products.AddAsync(entity);
            await _fridgeDbContext.SaveChangesAsync();
        }

        public async Task Delete(ProductEntity entity)
        {
            _fridgeDbContext.Products.Remove(entity);
            await _fridgeDbContext.SaveChangesAsync();
        }

        public IQueryable<ProductEntity> Get()
        {
            return _fridgeDbContext.Products;
        }

        public async Task<ProductEntity> Update(ProductEntity entity)
        {
            _fridgeDbContext.Products.Update(entity);
            await _fridgeDbContext.SaveChangesAsync();

            return entity;
        }
    }
}
