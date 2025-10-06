using Microsoft.Data.Sqlite;

// Models/AppUser.cs



namespace API.Models
{
    public class AppUser
    {

        public long? Id { get; set; } = null;

        public required string Firstname { get; set; }

        public required string Lastname { get; set; }
        public required string Username { get; set; }


        
    }
}
