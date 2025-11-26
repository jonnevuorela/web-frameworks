using API.CustomExceptions;
using API.Data;
using API.Dtos;
using API.Interfaces;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class TagService(DataContext _repository) : ITagService
    {
        public async Task<IEnumerable<Tag>> GetAll()
        {
            var tags = await _repository.Tags.Include(t => t.Blogs).ToListAsync();
            return tags;
        }

        public async Task<Tag> GetById(int id)
        {
            var tag = await _repository
                .Tags.Include(t => t.Blogs)
                .FirstOrDefaultAsync(t => t.Id == id);
            if (tag == null)
            {
                throw new NotFoundException("Tag not found");
            }

            return tag;
        }

        public async Task<Tag> Create(CreateTagReq requestData, int? ownerId)
        {
            var blogs = new List<Blog>();
            if (blogs != null) // lsp ei luota, että constructori palauttaa tyhjän blogi listan. siksi tarkistus.
            {
                if (ownerId != null)
                {
                    var associatedBlog = await _repository
                        .Blogs.Include(b => b.Id)
                        .FirstOrDefaultAsync(b => b.Id == ownerId);
                    blogs.Add(associatedBlog);
                }

                var tag = new Tag { TagText = requestData.TagText, Blogs = blogs };

                await _repository.Tags.AddAsync(tag);
                await _repository.SaveChangesAsync();
                return tag;
            }
            else
            {
                throw new TagCreationException("Failed to create new tag");
            }
        }

        public async Task<Tag> Edit(int id, UpdateTagReq requestData)
        {
            var tag = await GetById(id);

            tag.TagText = requestData.TagText;

            await _repository.SaveChangesAsync();

            return tag;
        }

        public async Task Delete(int id)
        {
            var tag = await GetById(id);
            _repository.Tags.Remove(tag);
            await _repository.SaveChangesAsync();
        }
    }
}
