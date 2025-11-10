using API.Models;
using API.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        [HttpGet(Name = "GetAllProducts")]
        public async Task<ActionResult<IEnumerable<AppProduct>>> GetAllProducts()
        {
            try
            {
                using (var repo = new ProductsSQLiteRepository())
                {
                    var products = await repo.GetAll();
                    return Ok(products);
                }
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpGet("{id}", Name = "GetProductById")]
        public async Task<ActionResult<AppProduct>> GetProductById(long id)
        {
            try
            {
                using (var repo = new ProductsSQLiteRepository())
                {
                    var product = await repo.GetById(id);
                    if (product == null)
                    {
                        return NotFound("Product not found");
                    }
                    return Ok(product);
                }
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
        public async Task<ActionResult<AppProduct>> AddNewProduct(string name)
        {
            try
            {
                using (var repo = new ProductsSQLiteRepository())
                {
                    var product = await repo.Save(name);
                    if (product == null)
                    {
                        return Problem(
                            $"error creating product",
                            statusCode: StatusCodes.Status500InternalServerError
                        );
                    }

                    return Ok(product);
                }
            }
            catch (Exception ex)
            {
                return Problem(ex.Message, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPatch("{id}", Name = "UpdateProductById")]
        public async Task<ActionResult<AppProduct>> UpdateProduct(long id, string name)
        {
            try
            {
                using (var repo = new ProductsSQLiteRepository())
                {
                    var product = await repo.Save(name, id);
                    if (product == null)
                    {
                        return Problem(
                            $"Error updating user",
                            statusCode: StatusCodes.Status500InternalServerError
                        );
                    }
                    return product;
                }
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
                using (var repo = new ProductsSQLiteRepository())
                {
                    bool removed = await repo.Remove(id);
                    if (removed)
                    {
                        return NoContent();
                    }
                    return NotFound("product not found");
                }
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
