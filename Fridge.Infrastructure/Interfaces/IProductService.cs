using Fridge.Domain.Entities;
using Fridge.Domain.Models;
using Fridge.Domain.Response;

namespace Fridge.Infrastructure.Interfaces
{
    public interface IProductService
    {
        Task<IBaseResponse<IEnumerable<ProductModel>>> GetProductsInFridge();
        Task<IBaseResponse<ProductEntity>> Create(ProductModel createFridgeModel);
        Task<IBaseResponse<IEnumerable<ProductModel>>> DeleteProductsInFridge(long id);
        Task<IBaseResponse<IEnumerable<ProductModel>>> ChangeProductsInFridge(ProductModel model);
    }
}
