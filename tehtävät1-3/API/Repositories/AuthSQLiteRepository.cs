using API.DTOs;
using API.Interfaces;
using API.Models;
using Microsoft.Data.Sqlite;

namespace API.Repositories
{
    // ILogRepo-riippuvuus on poistettu constructorista
    public class AuthSQLiteRepository(SqliteConnection _connection) : IAuthRepo
    {
        public async Task<AppUser?> Login(string username, string password)
        {
            AppUser? user;
            // luodaan yhteys ja avataan se

            // luodaan sql-komento ja suoritetaan se
            var command = _connection.CreateCommand();
            command.CommandText =
                $"SELECT id, first_name, last_name, username, password FROM users WHERE username = @username";
            command.Parameters.AddWithValue("@username", username);

            // Jälkeen ExecuteReadeAsync, koska odotetaan rivejä vataukseksi tietokannasta
            using var reader = await command.ExecuteReaderAsync();

            if (!reader.HasRows)
            {
                return null;
            }

            // salasanan tarkistusta ei enää tehdä täällä

            // luetaan luupissa kaikki rivit, jotka kyselyllä saadaan ulos
            // tietokannasta
            await reader.ReadAsync();

            user = new AppUser
            {
                Id = reader.GetInt64(0),
                Firstname = reader.GetString(1),
                Lastname = reader.GetString(2),
                Username = reader.GetString(3),
                Password = reader.GetString(4),
            };

            return user;
        }

        public async Task<AppUser?> Create(RegisterReq req)
        {
            var insertCommand = _connection.CreateCommand();
            insertCommand.CommandText =
                @"
                INSERT INTO users(first_name, last_name, username, password)
                VALUES(@firstName, @lastName, @username, @password);
                SELECT last_insert_rowid();";
            insertCommand.Parameters.AddWithValue("@firstName", req.Firstname);
            insertCommand.Parameters.AddWithValue("@lastName", req.Lastname);
            insertCommand.Parameters.AddWithValue("@username", req.UserName);
            insertCommand.Parameters.AddWithValue("@password", req.Password);

            var newId = await insertCommand.ExecuteScalarAsync();
            if (newId == null)
            {
                return null;
            }

            var id = (long)newId;

            return new AppUser
            {
                Firstname = req.Firstname,
                Lastname = req.Lastname,
                Username = req.UserName,
                Id = id,
                Password = req.Password,
            };
        }
    }
}
