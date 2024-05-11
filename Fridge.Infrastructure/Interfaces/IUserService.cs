using Fridge.Domain.Entities;
using Fridge.Domain.Models;
using Fridge.Domain.Response;

namespace Fridge.Infrastructure.Interfaces
{
    public interface IUserService
    {
        Task<IBaseResponse<UserEntity>> Create(UserModel user);
        Task<IBaseResponse<string>> Authenticate(UserModel user);
        Task<IBaseResponse<UserModel>> CheckIn(long userId);
        Task<IBaseResponse<IEnumerable<UserModel>>> GetUsers();
    }
}
