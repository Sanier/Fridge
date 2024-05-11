using System.Net;
using Fridge.DAL.Interfaces;
using Fridge.Domain.Entities;
using Fridge.Domain.Models;
using Fridge.Domain.Response;
using Fridge.Infrastructure.Interfaces;

namespace Fridge.Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly IBaseRepositories<ProductEntity> _productRepository;

        public ProductService(IBaseRepositories<ProductEntity> productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IBaseResponse<IEnumerable<ProductModel>>> ChangeProductsInFridge(ProductModel productModel)
        {
            try
            {
                var list = _productRepository.Get()
                    .FirstOrDefault(l => l.ProductId == productModel.ProductId && l.UserId == productModel.UserId);

                if (list is null)
                    throw new ArgumentNullException();

                list = new ProductEntity()
                {
                    ProductId = productModel.ProductId,
                    Name = productModel.Name,
                    Count = productModel.Count,
                    Price = productModel.Price
                };

                await _productRepository.Update(list);

                return OutputProcessing<IEnumerable<ProductModel>>("The task has been changed", HttpStatusCode.Accepted);
            }
            catch (Exception ex)
            {
                return HandleException<IEnumerable<ProductModel>>(ex, "FridgeService.ChangeProductsInFridge");
            }
        }

        public async Task<IBaseResponse<ProductEntity>> Create(ProductModel productModel)
        {
            try
            {
                var list = _productRepository.Get()
                    .FirstOrDefault(l => l.Name == productModel.Name);

                list = new ProductEntity()
                {
                    ProductId = productModel.ProductId,
                    Name = productModel.Name,
                    Count = productModel.Count,
                    Price = productModel.Price,
                    UserId = productModel.UserId
                };

                await _productRepository.Create(list);
                await _productRepository.Update(list);

                return OutputProcessing<ProductEntity>("The task has been created", HttpStatusCode.Accepted);
            }
            catch (Exception ex)
            {
                return HandleException<ProductEntity>(ex, "FridgeService.Create");
            }
        }

        public async Task<IBaseResponse<IEnumerable<ProductModel>>> DeleteProductsInFridge(long id)
        {
            try
            {
                var list = _productRepository.Get()
                    .FirstOrDefault(l => l.ProductId == id);

                if (list is null)
                    throw new ArgumentNullException();

                await _productRepository.Delete(list);
                await _productRepository.Update(list);

                return OutputProcessing<IEnumerable<ProductModel>>("The task has been deleted", HttpStatusCode.Accepted);
            }
            catch (Exception ex)
            {
                return HandleException<IEnumerable<ProductModel>>(ex, "FridgeService.DeleteProductsInFridge");
            }
        }

        public async Task<IBaseResponse<IEnumerable<ProductModel>>> GetProductsInFridge()
        {
            try
            {
                var list = _productRepository.Get()
                    .Select(l => new ProductModel
                    {
                        ProductId = l.ProductId,
                        Name = l.Name,
                        Count = l.Count,
                        Price = l.Price,
                        UserId = l.UserId,
                    })
                    .ToList();

                if (list is null)
                    throw new ArgumentNullException();

                return OutputProcessing<IEnumerable<ProductModel>>(list, HttpStatusCode.Accepted);
            }
            catch (Exception ex)
            {
                return HandleException<IEnumerable<ProductModel>>(ex, "FridgeService.GetProductsInFridge");
            }
        }

        private BaseResponse<TResponse> OutputProcessing<TResponse>(string description, HttpStatusCode statusCode)
        {
            return new BaseResponse<TResponse>()
            {
                Description = $"{description}",
                StatusCode = statusCode
            };
        }

        private BaseResponse<TResponse> OutputProcessing<TResponse>(TResponse task, HttpStatusCode statusCode)
        {
            return new BaseResponse<TResponse>()
            {
                Data = task,
                StatusCode = statusCode
            };
        }

        private BaseResponse<TResponse> HandleException<TResponse>(Exception ex, string nameMethod)
        {
            return OutputProcessing<TResponse>(ex.Message, HttpStatusCode.InternalServerError);
        }
    }
}
