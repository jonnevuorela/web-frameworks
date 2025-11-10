using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

// Models/AppUser.cs

namespace API.Models
{
    [Index(nameof(UserName), IsUnique = true)]
    public class AppUser
    {
        public int Id { get; set; }
        public required string UserName { get; set; }
        public required string Role { get; set; }

        public required byte[] PasswordSalt { get; set; }
        public required byte[] HashedPassword { get; set; }
    }
}
