using API.CustomExceptions;
using API.Data;
using API.Dtos;
using API.Interfaces;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class BlogService(DataContext _repository) : IBlogService
    {
        public async Task<IEnumerable<Blog>> GetAll()
        {
            var blogs = await _repository.Blogs.ToListAsync();
            return blogs;
        }

        public async Task<Blog> GetById(int id)
        {
            var blogWithOwner = await _repository
                .Blogs.Include(b => b.Owner)
                .Include(b => b.Tags)
                .FirstOrDefaultAsync(b => b.Id == id);
            if (blogWithOwner == null)
            {
                throw new NotFoundException("blog not found");
            }

            return blogWithOwner;
        }

        public async Task<Blog> Create(CreateBlogReq requestData, int loggedInUser)
        {
            // Haetaan jo oo. tagit, jotka löytyvät Tags-taulusta

            var existingTags = await _repository
                .Tags.Where(tag => requestData.Tags.Contains(tag.TagText))
                .ToListAsync();

            // katsotaan, onko joukossa uusia tageja, joita ei ole vielä tallennettu tietokantaan

            var existingTagTexts = existingTags.Select(tag => tag.TagText).ToList();
            var newTagTexts = requestData.Tags.Except(existingTagTexts).ToList();

            var newTagEntities = new List<Tag>();

            // lisätään uudet tagit listaan
            foreach (var text in newTagTexts)
            {
                newTagEntities.Add(new Tag { TagText = text });
            }
            // addataan kaikki uudet tagit kerralla
            await _repository.Tags.AddRangeAsync(newTagEntities);

            // yhdistetään uudet ja vanhat tägit yhteen listaan
            var allTags = existingTags.Union(newTagEntities).ToList();

            var blog = new Blog
            {
                Title = requestData.Title,
                Content = requestData.Content,
                AppUserId = loggedInUser,
                // lisätään tägit blogiin
                Tags = allTags,
            };

            // EfCoressa kaikki tietokantaan lisättävät rivit pitää ensin lisätä commitoitavien listaan käyttäen addia
            await _repository.Blogs.AddAsync(blog);
            // kun rivit on ensin lisätty listaan, muutokset voidaan tallentaa
            await _repository.SaveChangesAsync();
            return blog;
        }

        public async Task<Blog> Edit(int id, UpdateBlogReq requestData)
        {
            var blog = await GetById(id);

            blog.Title = requestData.Title;
            blog.Content = requestData.Content;

            await _repository.SaveChangesAsync();

            return blog;
        }

        public async Task Delete(int id)
        {
            var blog = await GetById(id);
            _repository.Blogs.Remove(blog);
            await _repository.SaveChangesAsync();
        }
    }
}
