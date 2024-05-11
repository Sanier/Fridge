using Fridge.DAL.Interfaces;
using Fridge.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fridge.DAL.Repositories
{
    public class UserRepositories : IBaseRepositories<UserEntity>
    {
        private FridgeDbContext _fridgeDbContext;
        public UserRepositories(FridgeDbContext fridgeDbContext)
        {
            _fridgeDbContext = fridgeDbContext;
        }

        public async Task Create(UserEntity entity)
        {
            await _fridgeDbContext.Users.AddAsync(entity);
            await _fridgeDbContext.SaveChangesAsync();
        }

        public async Task Delete(UserEntity entity)
        {
            _fridgeDbContext.Users.Remove(entity);
            await _fridgeDbContext.SaveChangesAsync();
        }

        public IQueryable<UserEntity> Get()
        {
            return _fridgeDbContext.Users.AsNoTracking();
        }

        public async Task<UserEntity> Update(UserEntity entity)
        {
            _fridgeDbContext.Users.Update(entity);
            await _fridgeDbContext.SaveChangesAsync();

            return entity;
        }
    }
}
