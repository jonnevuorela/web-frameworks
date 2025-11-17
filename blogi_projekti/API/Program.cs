using System.Text;
using API.Data;
using API.Interfaces;
using API.Services;
using API.Tools;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// yläpuolella: var builder = WebApplication.CreateBuilder(args);
builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        // varmistetaan, että TokenKey löytyy
        var tokenKey =
            builder.Configuration["TokenKey"] ?? throw new Exception("token key not found");
        // konfataan tässä, mitä tarkistetaan
        o.TokenValidationParameters = new TokenValidationParameters
        {
            // varmistaa allekirjoituksen
            ValidateIssuerSigningKey = true,
            // jotta allekirjoituksen voi tarkistaa,
            // pitää kertoa, mitä avainta allekirjoituksessa käytetään
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
            // issuerin tarkistus on pois päältä

            ValidateIssuer = false,
            // myös audiencen tarkistus on pois päältä
            ValidateAudience = false,
        };

        o.MapInboundClaims = false;
    });

// Nyt softamme tietää, mitä tauluja tietokantamme sisältää (ja myös mitä kenttiä taulut sisältävät)
// sekä pstymme yhdistämään siihen.
builder.Services.AddDbContext<DataContext>(opt =>
{
    // AddDbContextille pitää kertoa, mistä tietokantayhteyden speksit löytyvät

    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<ITokenTool, SymmetricTokenTool>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IBlogService, BlogService>();

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddAuthorization(option =>
{
    option.AddPolicy("RequireAdminRole", policy => policy.RequireRole("admin"));

    option.AddPolicy("Require1000Xp", policy => policy.Requirements.Add(new XpRequirement(1000)));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
