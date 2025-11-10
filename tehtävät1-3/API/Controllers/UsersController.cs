using API.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppUser>>> GetAll()
    {
        try
        {
            var users = await AppUser.GetAll();
            return Ok(users);
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
    }
}

