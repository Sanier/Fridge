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
        public async Task<IActionResult> Create(ProductModel createProductModel)
        {
            var response = await _productService.Create(createProductModel);

            if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
                return Ok(new { description = response.Description });

            return BadRequest(new { description = response.Description });
        }

        [HttpGet]
        public async Task<IActionResult> GetProductsInFridge()
        {
            var response = await _productService.GetProductsInFridge();

            return Json(new { data = response.Data });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteProductsInFridge(long id)
        {
            var response = await _productService.DeleteProductsInFridge(id);

            if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
                return Ok(new { description = response.Description });

            return BadRequest(new { description = response.Description });
        }

        [HttpPut]
        public async Task<IActionResult> ChangeProductsInFridge(ProductModel model)
        {
            var response = await _productService.ChangeProductsInFridge(model);

            if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
                return Ok(new { description = response.Description });

            return BadRequest(new { description = response.Description });
        }
    }
}
