using API.Dtos;
using API.Interfaces;
using API.Models;
using Microsoft.Data.Sqlite;

namespace API.Repositories
{
    public class ProductsSQLiteRepository : IProductsRepo
    {
        private SqliteConnection _connection;

        public ProductsSQLiteRepository()
        {
            _connection = new SqliteConnection("Data Source=tuntiharjoitus1.db");
            _connection.Open();
        }

        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.Close();
                _connection.Dispose();
            }
        }

        public async Task<IEnumerable<AppProduct>> GetAll()
        {
            var products = new List<AppProduct>();

            var command = _connection.CreateCommand();
            command.CommandText = $"SELECT * FROM products";

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                products.Add(
                    new AppProduct { Id = reader.GetInt64(0), Name = reader.GetString(1) }
                );
            }

            return products;
        }

        public async Task<AppProduct?> GetById(long id)
        {
            AppProduct? product;
            var command = _connection.CreateCommand();
            command.CommandText = $"SELECT * FROM products WHERE id = @id";
            command.Parameters.AddWithValue("@id", id);

            using var reader = await command.ExecuteReaderAsync();

            if (!reader.HasRows)
            {
                return null;
            }

            await reader.ReadAsync();

            product = new AppProduct { Id = reader.GetInt64(0), Name = reader.GetString(1) };

            return product;
        }

        public async Task<bool> Remove(long id)
        {
            var command = _connection.CreateCommand();
            command.CommandText = "DELETE FROM products WHERE id = @id";
            command.Parameters.AddWithValue("@id", id);

            var rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected == 1;
        }

        public async Task<AppProduct?> Save(string name, long? id)
        {
            AppProduct? product;
            if (id != null)
            {
                product = await Update(name, id.Value);
            }
            else
            {
                product = await Add(name);
            }

            return product;
        }

        private async Task<AppProduct?> Add(string name)
        {
            var insertCommand = _connection.CreateCommand();
            insertCommand.CommandText =
                @"INSERT INTO products(name) VALUES(@name); SELECT last_insert_rowid();";
            insertCommand.Parameters.AddWithValue("@name", name);

            var newId = await insertCommand.ExecuteScalarAsync();
            if (newId == null)
            {
                return null;
            }

            var id = (long)newId;

            return new AppProduct { Name = name, Id = id };
        }

        private async Task<AppProduct?> Update(string name, long id)
        {
            var product = await GetById(id);
            if (product == null)
            {
                return null;
            }
            else
            {
                var updateCommand = _connection.CreateCommand();
                updateCommand.CommandText = @"UPDATE products SET name = @newName WHERE id = @id;";

                updateCommand.Parameters.AddWithValue("@newName", name);
                updateCommand.Parameters.AddWithValue("@id", id);

                int rowsAffected = await updateCommand.ExecuteNonQueryAsync();

                product.Name = name;
                if (rowsAffected == 1)
                {
                    return product;
                }
            }

            return null;
        }

        private async Task<long?> GetIdByName(string name)
        {
            var command = _connection.CreateCommand();
            command.CommandText = $"SELECT * FROM products WHERE name = @name";
            command.Parameters.AddWithValue("@name", name);

            using var reader = await command.ExecuteReaderAsync();
            if (!reader.HasRows)
            {
                return null;
            }
            else
            {
                await reader.ReadAsync();
                return reader.GetInt64(0);
            }
        }
    }
}
