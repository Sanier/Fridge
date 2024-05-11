using Fridge.Domain.Entities;
using Fridge.Domain.Models;
using Fridge.Domain.Response;

namespace Fridge.Infrastructure.Interfaces
{
    public interface IProductService
    {
        Task<IBaseResponse<IEnumerable<ProductModel>>> GetProductsInFridge(long userId);
        Task<IBaseResponse<ProductEntity>> Create(ProductModel createFridgeModel, long userId);
        Task<IBaseResponse<IEnumerable<ProductModel>>> DeleteProductsInFridge(string productName, long userId);
        Task<IBaseResponse<IEnumerable<ProductModel>>> ChangeProductsInFridge(ProductModel model, long userId);
    }
}
