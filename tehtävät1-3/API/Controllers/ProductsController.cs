using API.Dtos;
using API.Factories;
using API.Interfaces;
using API.Models;
using API.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController(IProductService _productService) : ControllerBase
    {
        [HttpGet(Name = "GetAllProducts")]
        public async Task<ActionResult<IEnumerable<ProductRes>>> GetAllProducts()
        {
            try
            {
                var res = await _productService.FetchAll();
                if (res == null)
                {
                    return Problem($"error fetching all products");
                }

                return Ok(res);
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpGet("{id}", Name = "GetProductById")]
        public async Task<ActionResult<ProductRes>> GetProductById(long id)
        {
            try
            {
                var res = await _productService.Fetch(id);
                if (res == null)
                {
                    return NotFound("Product not found");
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                return Problem(
                    $"Database error: {ex.Message}",
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        [HttpPost(Name = "AddProduct")]
        public async Task<ActionResult<ProductRes>> AddNewProduct(ProductReq req)
        {
            try
            {
                var res = await _productService.Add(req);
                if (res == null)
                {
                    return Problem(
                        $"error creating product",
                        statusCode: StatusCodes.Status500InternalServerError
                    );
                }

                return Ok(res);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPatch("{id}", Name = "UpdateProductById")]
        public async Task<ActionResult<ProductRes>> UpdateProduct(long id, ProductReq req)
        {
            try
            {
                var res = await _productService.Edit(id, req);
                if (res == null)
                {
                    return Problem(
                        $"Error updating user",
                        statusCode: StatusCodes.Status500InternalServerError
                    );
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                return Problem(
                    $"Database error: {ex.Message}",
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        [HttpDelete("{id}", Name = "RemoveProductById")]
        public async Task<ActionResult> RemoveProduct(long id)
        {
            try
            {
                bool removed = await _productService.Remove(id);
                if (removed)
                {
                    return NoContent();
                }
                return NotFound("product not found");
            }
            catch (Exception ex)
            {
                return Problem(
                    $"Database error: {ex.Message}",
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }
    }
}
