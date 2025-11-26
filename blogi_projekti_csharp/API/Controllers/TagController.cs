using API.CustomExceptions;
using API.Dtos;
using API.Interfaces;
using API.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TagsController(ITagService _tagService, IMapper _mapper) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TagBriefDto>>> GetAll()
        {
            try
            {
                var tags = await _tagService.GetAll();
                return Ok(_mapper.Map<List<TagBriefDto>>(tags));
            }
            catch (Exception ex)
            {
                return Problem(title: "error fetching tags", detail: ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TagDto>> GetById(int id)
        {
            try
            {
                var tag = await _tagService.GetById(id);
                return Ok(_mapper.Map<TagDto>(tag));
            }
            catch (TagCreationException)
            {
                return NotFound(
                    new { Title = "Tag not found", Detail = $"Tag with the id of {id} not found" }
                );
            }
            catch (Exception ex)
            {
                return Problem(title: "error fetching Tag", detail: ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<TagBriefDto>> Create(CreateTagReq req)
        {
            try
            {
                var tag = await _tagService.Create(req, null);
                return Ok(_mapper.Map<TagBriefDto>(tag));
            }
            catch (Exception e)
            {
                return Problem(title: "error creating Tag post", detail: e.Message);
            }
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<TagBriefDto>> UpdateTag(int id, UpdateTagReq req)
        {
            try
            {
                var tag = await _tagService.Edit(id, req);
                return Ok(_mapper.Map<TagBriefDto>(tag));
            }
            catch (Exception e)
            {
                return Problem(title: "error updating Tag", detail: e.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> RemoveTag(int id)
        {
            try
            {
                await _tagService.Delete(id);
                return NoContent();
            }
            catch (Exception e)
            {
                return Problem(title: "error removing Tag", detail: e.Message);
            }
        }
    }
}
