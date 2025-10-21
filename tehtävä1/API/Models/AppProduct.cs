using Microsoft.Data.Sqlite;

namespace API.Models
{
    public class AppProduct
    {
        public long? Id { get; set; } = null;

        public required string Name { get; set; }

        /// <summary>
        /// Retrieves all products from the database.
        /// </summary>

        /// <returns>A list of all products from the database.</returns>
        /// <exception cref="SqliteException">Thrown when an error occurs while connecting to or querying the database.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the connection is already open when trying to open it again.</exception>
        public static async Task<IEnumerable<AppProduct>> GetAll()
        {
            var products = new List<AppProduct>();
            using var connection = new SqliteConnection("Data source=tuntiharjoitus1.db");

            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = $"SELECT * FROM products";

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                products.Add(new AppProduct
                {
                    Id = reader.GetInt64(0),
                    Name = reader.GetString(1),
                });
            }
            return products;
        }

        public static async Task<AppProduct?> GetById(long id)
        {
            AppProduct? product;
            using var connection = new SqliteConnection("Data source=tuntiharjoitus1.db");

            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = $"SELECT * FROM products WHERE id = @id";
            command.Parameters.AddWithValue("@id", id);

            using var reader = await command.ExecuteReaderAsync();

            if (!reader.HasRows)
            {
                return null;
            }

            await reader.ReadAsync();

            product = new AppProduct
            {
                Id = reader.GetInt64(0),
                Name = reader.GetString(1),
            };

            return product;
        }

        public static async Task<bool> Add(string name)
        {
            using var connection = new SqliteConnection("Data source=tuntiharjoitus1.db");
            connection.Open();

            var insertCommand = connection.CreateCommand();
            insertCommand.CommandText = @"
                INSERT INTO products(name)
                VALUES(@Name);
                SELECT last_insert_rowid();";
            insertCommand.Parameters.AddWithValue("@Name", name);

            var newId = await insertCommand.ExecuteScalarAsync();
            if (newId == null)
            {
                return false;
            }

            return true;
        }

        public static async Task<bool> Update(string name, long id)
        {
            using var connection = new SqliteConnection("Data source=tuntiharjoitus1.db");
            connection.Open();

            var updateCommand = connection.CreateCommand();
            updateCommand.CommandText = @"
                UPDATE products SET
                    name = @newName
                WHERE id = @id;";

            updateCommand.Parameters.AddWithValue("@newName", name);
            updateCommand.Parameters.AddWithValue("@id", id);

            int rowsAffected = await updateCommand.ExecuteNonQueryAsync();

            return rowsAffected == 1;
        }

        public static async Task<bool> Remove(long id)
        {
            using var connection = new SqliteConnection("Data source=tuntiharjoitus1.db");

            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM products WHERE id = @id;";
            command.Parameters.AddWithValue("@id", id);

            int rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected == 1;
        }
    }
}
