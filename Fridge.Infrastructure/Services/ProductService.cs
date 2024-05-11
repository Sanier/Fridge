using System.Net;
using Fridge.DAL.Interfaces;
using Fridge.Domain.Entities;
using Fridge.Domain.Models;
using Fridge.Domain.Response;
using Fridge.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fridge.Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly IBaseRepositories<ProductEntity> _productRepository;

        public ProductService(IBaseRepositories<ProductEntity> productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IBaseResponse<IEnumerable<ProductModel>>> ChangeProductsInFridge(ProductModel productModel, long userId)
        {
            try
            {
                var list = await _productRepository.Get()
                    .FirstOrDefaultAsync(l => l.Name == productModel.Name && l.UserId == userId);

                if (list is null)
                    throw new ArgumentNullException();

                list = new ProductEntity()
                {
                    Name = productModel.Name,
                    Count = productModel.Count,
                    Price = productModel.Price,
                    UserId = userId
                };

                await _productRepository.Update(list);

                return OutputProcessing<IEnumerable<ProductModel>>("The task has been changed", HttpStatusCode.Accepted);
            }
            catch (Exception ex)
            {
                return HandleException<IEnumerable<ProductModel>>(ex, "FridgeService.ChangeProductsInFridge");
            }
        }

        public async Task<IBaseResponse<ProductEntity>> Create(ProductModel productModel, long userId)
        {
            try
            {
                if (await _productRepository.Get().AnyAsync(l => l.Name == productModel.Name && l.UserId == userId))
                    throw new ArgumentException("Such a product already exists");

                var product = new ProductEntity()
                {
                    Name = productModel.Name,
                    Count = productModel.Count,
                    Price = productModel.Price,
                    UserId = userId
                };

                await _productRepository.Create(product);
                await _productRepository.Update(product);

                return OutputProcessing<ProductEntity>("The task has been created", HttpStatusCode.Accepted);
            }
            catch (Exception ex)
            {
                return HandleException<ProductEntity>(ex, "FridgeService.Create");
            }
        }

        public async Task<IBaseResponse<IEnumerable<ProductModel>>> DeleteProductsInFridge(string productName, long userId)
        {
            try
            {
                var product = await _productRepository.Get()
                    .FirstOrDefaultAsync(l => l.Name == productName && l.UserId == userId);

                if (product is null)
                    throw new ArgumentNullException();

                await _productRepository.Delete(product);

                return OutputProcessing<IEnumerable<ProductModel>>("The task has been deleted", HttpStatusCode.Accepted);
            }
            catch (Exception ex)
            {
                return HandleException<IEnumerable<ProductModel>>(ex, "FridgeService.DeleteProductsInFridge");
            }
        }

        public async Task<IBaseResponse<IEnumerable<ProductModel>>> GetProductsInFridge(long userId)
        {
            try
            {
                var list = await _productRepository.Get()
                    .Where(l => l.UserId == userId)
                    .Select(l => new ProductModel
                    {
                        Name = l.Name,
                        Count = l.Count,
                        Price = l.Price
                    })
                    .ToListAsync();

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
