using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Interfaces;
using API.Models;
using Microsoft.IdentityModel.Tokens;

namespace API.Tools
{
    public class SymmetricTokenTool(IConfiguration _configuration) : ITokenTool
    {
        public string CreateToken(AppUser user)
        {
            // Jos käyttäjä löytyy ja salasana on oikein,
            // luodaan JWT käyttäjän tunnistamista varten

            var tokenKey = _configuration["TokenKey"];
            if (tokenKey == null)
            {
                throw new Exception("token key not set");
            }

            // varmista myös, että TokenKey on tarpeeksi pitkä
            if (tokenKey.Length < 64)
            {
                throw new Exception("invalid token key, must be more than 64 characters in length");
            }

            // SymmetricSecurityKey on tyyppi, jossa jwt:n luonnissa
            // ja jwtn tarkistuksessa käytetään samaa avainta (TokenKey)
            // string pitää muuttaa byte[] arrayksi
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
            // jwt:n allekirjoitus tehdään avainta käyttäen
            // HMACSha512-algoritimillä
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            // tässä jwt:n sisään laitettavat käyttäjää koskevat asiat
            // Sub = käyttäjän id
            // Name = käyttäjänimi
            // Jti = random merkkijono
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, user.Role),
            };
            // luodaan token claimit on yo. tiedot, jotka menevät sisään
            // expires on voimassaoloaika (tässä viikko)
            // SigningCredentials on allekirjoitus joka luotiin ylempänä
            var _token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: credentials
            );
            // tässä tehdään tokenista base64 enkoodattu merkkijono
            // tämä voidaan antaa käyttäjälle kirjautumisen jälkeen
            return new JwtSecurityTokenHandler().WriteToken(_token);
        }
    }
}
