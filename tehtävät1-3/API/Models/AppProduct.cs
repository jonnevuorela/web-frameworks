using Microsoft.Data.Sqlite;

namespace API.Models
{
    public class AppProduct
    {
        public long? Id { get; set; } = null;

        public required string Name { get; set; }
    }
}
