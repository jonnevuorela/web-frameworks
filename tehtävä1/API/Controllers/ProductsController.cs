using API.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppProduct>>> GetAll()
    {
        try
        {
            var products = await AppProduct.GetAll();
            return Ok(products);
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
            var product = await AppProduct.GetById(id);
            if (product == null)
            {
                return NotFound("Product not found");
            }
            return Ok(product);
        }
        catch (Exception ex)
        {
            return Problem($"Database error: {ex.Message}", statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPost(Name = "AddProduct")]
    public async Task<ActionResult<AppProduct>> AddNewProduct(string name)
    {
        try
        {
            var success = await AppProduct.Add(name);
            if (!success)
            {
                return Problem($"error creating product", statusCode: StatusCodes.Status500InternalServerError);
            }

            return Ok(success);
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

            var success = await AppProduct.Update(name, id);
            if (!success)
            {
                return NotFound();
            }

            return Ok(success);

        }
        catch (Exception ex)
        {
            return Problem($"Database error: {ex.Message}", statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    [HttpDelete("{id}", Name = "RemoveProductById")]
    public async Task<ActionResult> RemoveProduct(long id)
    {
        try
        {
            bool removed = await AppProduct.Remove(id);

            if (removed)
            {
                return NoContent();
            }

            return NotFound("product not found");
        }
        catch (Exception ex)
        {
            return Problem($"Database error: {ex.Message}", statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}

