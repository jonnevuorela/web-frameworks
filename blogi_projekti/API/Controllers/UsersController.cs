// UsersController

using API.CustomExceptions;
using API.Dtos;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // Voimme injektoida IMapper-tyyppisenä _mapperin mihin tahansa controlleriin
    public class UsersController(IUserService _userService, IMapper _mapper) : ControllerBase
    {
        [HttpGet]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
        {
            var users = await _userService.GetAll();

            return Ok(_mapper.Map<IEnumerable<UserDto>>(users));
        }

        [HttpPost("register")]
        public async Task<ActionResult<RegisterRes>> Register(RegisterReq requestData)
        {
            try
            {
                var user = await _userService.Register(requestData.UserName, requestData.Password);
                return new RegisterRes
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Role = user.Role,
                };
            }
            // toistaiseksi missään ei vielä heitetä tätä omaa poikkeusta
            // otetaan se käyttöön myöhemmin
            catch (UserRegistrationException e)
            {
                return Problem(
                    title: "Error creating user",
                    detail: e.Message,
                    statusCode: StatusCodes.Status400BadRequest
                );
            }
            catch (Exception e)
            {
                return Problem(
                    title: "Error creating user",
                    detail: e.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        [HttpGet("account/rewards")]
        // tähän routeen ei pääse, jos Xp-avain puuttuu jwt-tokenista
        // tai sen arvo on alle 1000
        [Authorize(Policy = "Require1000Xp")]
        public ActionResult<string> GetRewards()
        {
            return Ok("");
        }
    }
}
