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
}

