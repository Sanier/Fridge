using Fridge.Domain.Models;
using Fridge.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fridge.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductModel createProductModel, long userId)
        {
            var response = await _productService.Create(createProductModel, userId);

            if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
                return Ok(new { description = response.Description });

            return BadRequest(new { description = response.Description });
        }

        [HttpGet]
        public async Task<IActionResult> GetProductsInFridge(long userId)
        {
            var response = await _productService.GetProductsInFridge(userId);

            return Json(new { data = response.Data });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteProductsInFridge(string productName, long userId)
        {
            var response = await _productService.DeleteProductsInFridge(productName, userId);

            if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
                return Ok(new { description = response.Description });

            return BadRequest(new { description = response.Description });
        }

        [HttpPut]
        public async Task<IActionResult> ChangeProductsInFridge(ProductModel model, long userId)
        {
            var response = await _productService.ChangeProductsInFridge(model, userId);

            if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
                return Ok(new { description = response.Description });

            return BadRequest(new { description = response.Description });
        }
    }
}
