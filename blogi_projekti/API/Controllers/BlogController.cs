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
    public class BlogsController(IBlogService _blogService, IMapper _mapper) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BlogDto>>> GetAll()
        {
            try
            {
                var blogs = await _blogService.GetAll();
                return Ok(_mapper.Map<List<BlogDto>>(blogs));
            }
            catch (Exception ex)
            {
                return Problem(title: "error fetching blogs", detail: ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BlogDto>> GetById(int id)
        {
            try
            {
                // täällä taas kannattaa palauttaa erillinen dto-objekti, koska mukana on käyttäjätietoja
                var blog = await _blogService.GetById(id);
                return Ok(_mapper.Map<BlogDto>(blog));
            }
            catch (NotFoundException)
            {
                // HUOM! Koska NotFound ottaa parametrikseen object?-tietotyypin, sen voi jättää pois tai luoda oman lennosta
                return NotFound(
                    new { Title = "blog not found", Detail = $"blog with the id of {id} not found" }
                );
            }
            catch (Exception ex)
            {
                return Problem(title: "error fetching blog", detail: ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<BlogDto>> Create(CreateBlogReq req)
        {
            try
            {
                // koska käytämme AuthorizeAttribuuttia routehandlerin yläpuolella, sisäänkirjautuneen
                // käyttäjän jwt:hen tallennetut tiedot löytyvät User.Claimsista automaattisesti

                // koska sisäänkirjautumisen yhteydessä
                // tallennamme subiin käyttäjän id:n, saamme id:n jwt:stä näin
                var idClaim = User.Claims.First(c => c.Type == "sub");
                var id = int.Parse(idClaim.Value);
                var blog = await _blogService.Create(req, id);
                return Ok(_mapper.Map<BlogDto>(blog));
            }
            catch (Exception e)
            {
                return Problem(title: "error creating blog post", detail: e.Message);
            }
        }
    }
}
