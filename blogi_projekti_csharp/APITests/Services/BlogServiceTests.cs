using API.Data;
using API.Models;
using API.Services;
using Microsoft.EntityFrameworkCore;

public class BlogServiceTests
{
    // Luodaan jokaiselle testille inmemorydatabase

    // Koska käytämme EF Corea, Servicen metodeja ei kannata mockata tai feikata,
    // vaan käytetään "oikean" tietokannan sijasta muistiin tallennettavaa dataa, jota voimme
    // käyttää samoin kuin EFCorea

    private DataContext CreateInMemoryDbContext(string dbName)
    {
        // Käytetään samaa DataContextia kuin API:ssakin.
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        var context = new DataContext(options);
        return context;
    }

    private async Task SeedData(DataContext context)
    {
        var blogs = new List<Blog>
        {
            new Blog
            {
                Id = 1,
                Title = "Test Blog 1",
                Content = "Test Blog 1 Content",
            },
            new Blog
            {
                Id = 2,
                Title = "Test Blog 2",
                Content = "Test Blog 2 Content",
            },
            new Blog
            {
                Id = 3,
                Title = "Test Blog 3",
                Content = "Test Blog 3 Content",
            },
        };

        await context.Blogs.AddRangeAsync(blogs);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task GetAll_ShouldReturnAllBlogs()
    {
        // Arrange
        // 1. Luodan tälle testille oma tietokanta
        var context = CreateInMemoryDbContext(nameof(GetAll_ShouldReturnAllBlogs));

        // 2. Laitetaan testidataa sisään
        await SeedData(context);

        // 3. Koska käytämme dependency injectionia, voimme korvata contextin testiin käytettävällä in-memory databasella

        var service = new BlogService(context);

        // Act
        var result = await service.GetAll();

        // Assert

        Assert.Equal(3, result.Count());

        Assert.Contains(result, b => b.Title == "Test Blog 2");
    }

    [Fact]
    public async Task GetAll_EmptyDb_ShouldReturnEmptyList()
    {
        // Arrange
        var context = CreateInMemoryDbContext(nameof(GetAll_EmptyDb_ShouldReturnEmptyList));

        // Jätetään tietokanta seedamatta, jotta saadaan tyhjä tulos
        var service = new BlogService(context);

        // Act
        var result = await service.GetAll();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
