using BusinessLogicLayer.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Model.Models.DTO;
using Model.Models.DTO.Product;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("get-all-products")]
        public async Task<IActionResult> GetAllProducts()
        {
            var result = await _productService.GetAllProductAsync();
            if (!result.IsSucceed)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("get-product-by/{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var result = await _productService.GetProductByIdAsync(id);
            if (!result.IsSucceed)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("create-product")]
        public async Task<IActionResult> CreateProduct(CreateProductDTO createProductDTO)
        {
            var result = await _productService.CreateProductAsync(createProductDTO);
            if (!result.IsSucceed)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("update-product/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, UpdateProductDTO updateProductDTO)
        {
            var result = await _productService.UpdateProductAsync(id, updateProductDTO);
            if (!result.IsSucceed)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("delete-product/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _productService.DeleteProductAsync(id);
            if (!result.IsSucceed)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
