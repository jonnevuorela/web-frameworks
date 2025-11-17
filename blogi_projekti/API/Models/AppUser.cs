using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace API.Models
{
    [Index(nameof(UserName), IsUnique = true)]
    public class AppUser
    {
        // Muista, käyttää Id-propertysta juuri tällaista nimeä
        // asennamme EntityFrameWorkCore-riippvuuden Nugetista
        // EF Core tekee autom. Id-attribuutista tietokannan
        // taulun perusavaimen
        public int Id { get; set; }
        public required string UserName { get; set; }
        public required string Role { get; set; }

        // näiden pitää olla byte[]-tietotyyppiä
        public required byte[] PasswordSalt { get; set; }
        public required byte[] HashedPassword { get; set; }

        // tämä on uusi
        // tämä kertoo, että kyseessä blogs<->users-taulujen välillä on 1:n-relaatio
        public ICollection<Blog> Blogs { get; set; } = new List<Blog>();
    }
}
