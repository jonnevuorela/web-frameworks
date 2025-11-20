using API.Dtos;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // nyt on vain yksi dependency controllerissa
    public class AuthController(IAuthService _authService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<ActionResult<RegisterRes>> Register(RegisterReq request)
        {
            try
            {
                var res = await _authService.Register(request);
                if (res == null)
                {
                    return Problem($"error registering user");
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                return Problem($"error registering user {ex.Message}");
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginRes>> Login(LoginReq request)
        {
            try
            {
                // kaikki ylimääräinen businesslogiikka on piilossa servicelayerilla,
                // ja koodarin tarvitsee muistaa kutsua vain yhtä metodia
                var res = await _authService.Login(request);
                if (res == null)
                {
                    return NotFound(new { Message = "user not found" });
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                return Problem($"error registering user {ex.Message}");
            }
        }
    }
}
