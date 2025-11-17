using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using API.CustomExceptions;
using API.Data;
using API.Interfaces;
using API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    // kun nyt käytämme EF Corea, pystymme injektoimaan serviceen
    // suoraan DataContextin ja käyttää sitä repositoryna
    // DataContext on jo rekisteröity aiemmin käyttäen AddDbContext-metodia Program.cs-filussa

    // IConfiguration on rajapinta, jonka configuraatio-luokka implementoi
    // Suomeksi sanottuna injektoimalla tämän rajapinnan pystymme lukemaan mm. appSettings.json-tiedon avaimet

    public class UserService(DataContext _repository, ITokenTool _tokenTool) : IUserService
    {
        public async Task<IEnumerable<AppUser>> GetAll()
        {
            // tämä tekee sql-kyselyn SELECT * FROM Users;
            var users = await _repository.Users.ToListAsync();
            return users;
        }

        public async Task<AppUser?> GetByUserName(string username)
        {
            // FirstOrDefaultAsync palauttaa null, jos hakuehdolla ei löydy yhtään riviä tietokannasta
            // muuten sen rivin tiedot, joka niillä löytyy

            // FirstAsync puolestaan heittää poikkeuksen, jos hakuehdolla ei löydy osumia tietokannasta

            // EF Core luo tästä sql-kyselyn SELECT * FROM Users WHERE LOWER(UserName) = ? LIMIT 1;
            // jossa kysymysmerkin paikalle tulee username-muuttujan arvo

            var user = await _repository.Users.FirstOrDefaultAsync(u =>
                u.UserName.ToLower() == username.ToLower()
            );
            return user;
        }

        public async Task<string> Login(string username, string password)
        {
            var user = await GetByUserName(username);

            if (user == null)
            {
                throw new NotFoundException("user not found");
            }

            // tässä sama hmac kuin rekisteröitymisessä
            // mutta nyt constructorille on annettu parametrina
            // löydetyn käyttäjän salt
            // saltin pitää nyt olla käyttäjälle rekisteröitymisessä tehty sama salt
            // jos salt ei ole sama, salasanat eivät koskaan täsmää
            using var hmac = new HMACSHA512(user.PasswordSalt);
            // tässä tehdään hash selkokielisestä salasanasta
            var computedPassword = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            // computedHash on byte[] array, joten se voidaan luupata läpi
            for (int i = 0; i < computedPassword.Length; i++)
            {
                // jos salasanat eivät täsmää, palautetaan null
                if (computedPassword[i] != user.HashedPassword[i])
                {
                    throw new NotFoundException("user not found");
                }
            }

            // tokenin luonti on tämän takana
            var token = _tokenTool.CreateToken(user);
            return token;
        }

        public async Task<AppUser> Register(string username, string password)
        {
            var existingUser = await GetByUserName(username);
            if (existingUser != null)
            {
                throw new UserRegistrationException("username must be unique");
            }

            using var hmac = new HMACSHA512();

            var user = new AppUser
            {
                UserName = username,
                Role = "user",
                // hmac.Key on randomi salt, jonka loit automaattisesti
                // kun teit hmac-instanssin
                PasswordSalt = hmac.Key,
                // ComputeHash tekee hashin selkokielisestä salasanasta
                HashedPassword = hmac.ComputeHash(
                    // Encoding.UTF8.GetBytes?
                    // req.Password on string-tyyypiä, mutta ComputeHash
                    // haluaa parametring byte[]-arrayna
                    // GetBytes siis palauttaa merkkijonosta byte[] arrayn.

                    Encoding.UTF8.GetBytes(password)
                ),
            };
            // tämä tekee käytännössä insertin Users-tietokantatauluun
            // koska olemme määrittäneet DbSetiksi Usersin, pystymme käyttämään sitä näin
            await _repository.Users.AddAsync(user);
            // insert pitää vahvistaa, jotta rivi oikeasti tallennetaan
            await _repository.SaveChangesAsync();

            return user;
        }

        public async Task<AppUser> GetAccount(int id)
        {
            var user = await _repository.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                throw new NotFoundException("user not found");
            }

            return user;
        }
    }
}
