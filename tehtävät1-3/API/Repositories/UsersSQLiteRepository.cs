// Repositories/UsersSQLiteRepository.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Interfaces;
using API.Models;
using Microsoft.Data.Sqlite;

namespace API.Repositories
{
    // IDisposable-interfacen käyttö
    // pakottaa implementoimaan Dispose-metodin
    // jossa voimme sulkea tietokantayhteyden

    /*
    
    OTA HUOMIOON: constructor ei ole missään nimessä
    paras paikka tietokantayhteyden avaamiselle Repositoryssa
    mutta katsotaan parempi tapa Dependency Injection-osiossa
    
    */

    public class UsersSQLiteRepository : IUsersRepository
    {
        private readonly SqliteConnection _connection;

        public UsersSQLiteRepository()
        {
            _connection = new SqliteConnection("Data Source=tuntiharjoitus1.db");
            _connection.Open();
        }

        // Tämä metodi suoritetaan automaattisesti,
        // kun Controllerissa mennään using-blokin ulkoupuolelle
        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.Close();
                _connection.Dispose();
            }
        }

        public async Task<IEnumerable<AppUser>> GetAll()
        {
            // luodaan tyhjä lista tietokannan rivejä varten
            var users = new List<AppUser>();
            // luodaan yhteys ja avataan se

            // luodaan sql-komento ja suoritetaan se
            var command = _connection.CreateCommand();
            command.CommandText = $"SELECT * FROM users";

            // Jälkeen ExecuteReadeAsync, koska odotetaan rivejä vataukseksi tietokannasta
            using var reader = await command.ExecuteReaderAsync();

            // luetaan luupissa kaikki rivit, jotka kyselyllä saadaan ulos
            // tietokannasta
            while (await reader.ReadAsync())
            {
                // lisätään ne ylempänä luotuun listaan
                users.Add(
                    new AppUser
                    {
                        Id = reader.GetInt64(0),
                        Firstname = reader.GetString(1),
                        Lastname = reader.GetString(2),
                        Username = reader.GetString(3),
                    }
                );
            }

            return users;
        }

        public async Task<AppUser?> GetById(long id)
        {
            // luodaan tyhjä lista tietokannan rivejä varten
            AppUser? user;
            // luodaan yhteys ja avataan se

            // luodaan sql-komento ja suoritetaan se
            var command = _connection.CreateCommand();
            command.CommandText = $"SELECT * FROM users WHERE id = @id";
            command.Parameters.AddWithValue("@id", id);

            // Jälkeen ExecuteReadeAsync, koska odotetaan rivejä vataukseksi tietokannasta
            using var reader = await command.ExecuteReaderAsync();

            if (!reader.HasRows)
            {
                return null;
            }

            // luetaan luupissa kaikki rivit, jotka kyselyllä saadaan ulos
            // tietokannasta
            await reader.ReadAsync();

            // lisätään ne ylempänä luotuun listaan
            user = new AppUser
            {
                Id = reader.GetInt64(0),
                Firstname = reader.GetString(1),
                Lastname = reader.GetString(2),
                Username = reader.GetString(3),
            };

            return user;
        }

        private async Task<AppUser?> Add(string firstname, string lastname, string username)
        {
            var insertCommand = _connection.CreateCommand();
            insertCommand.CommandText =
                @"
                INSERT INTO users(first_name, last_name, username)
                VALUES(@firstName, @lastName, @username);
                SELECT last_insert_rowid();";
            insertCommand.Parameters.AddWithValue("@firstName", firstname);
            insertCommand.Parameters.AddWithValue("@lastName", lastname);
            insertCommand.Parameters.AddWithValue("@username", username);

            var newId = await insertCommand.ExecuteScalarAsync();
            if (newId == null)
            {
                return null;
            }

            var id = (long)newId;

            return new AppUser
            {
                Firstname = firstname,
                Lastname = lastname,
                Username = username,
                Id = id,
            };
        }

        // HUOM!!!
        // kun pääseemme testaukseen asti, testiframework antaa
        // tälle metodille korkeat kompleksisuuspisteet
        // mutta jätetään se toistaiseksi näin,
        // jotta näemme testauksen hyötyjä käytännössä
        private async Task<AppUser?> Update(
            string firstname,
            string lastname,
            string username,
            long? id
        )
        {
            if (id == null)
            {
                return null;
            }
            var user = await GetById((long)id);
            if (user == null)
            {
                return null;
            }
            // luodaan sql-komento päivitystä varten ja asetetaan sille yo. parametrit
            var updateCommand = _connection.CreateCommand();
            updateCommand.CommandText =
                @"
                UPDATE users SET
                    first_name = @newFirstName,
                    last_name = @newLastName,
                    username = @newUsername
                WHERE id = @id;";

            // Koska pääsemme modelin metodin sisällä luokan instanssin propertyihin käsiksi,
            // metodille ei tarvitse lähettää erillisiä parametrejä
            updateCommand.Parameters.AddWithValue("@newFirstName", firstname);
            updateCommand.Parameters.AddWithValue("@newLastName", lastname);
            updateCommand.Parameters.AddWithValue("@newUsername", username);
            updateCommand.Parameters.AddWithValue("@id", id);

            int rowsAffected = await updateCommand.ExecuteNonQueryAsync();

            user.Username = username;
            user.Firstname = firstname;
            user.Lastname = lastname;
            if (rowsAffected == 1)
            {
                return user;
            }

            return null;
        }

        public async Task<AppUser?> Save(
            string firstname,
            string lastname,
            string username,
            long? id = null
        )
        {
            AppUser? user;
            if (id == null)
            {
                user = await Add(firstname, lastname, username);
            }
            else
            {
                user = await Update(firstname, lastname, username, id);
            }

            return user;
        }

        public async Task<bool> Remove(long id)
        {
            // luodaan sql-komento poistoa varten
            var command = _connection.CreateCommand();
            command.CommandText = "DELETE FROM users WHERE id = @id;";
            command.Parameters.AddWithValue("@id", id);

            // tässä voidaan käyttää ExecuteNonQueryAsyncia,
            // koska DELETE-kysely ei palauta varsinaista tulosta tietokannasta
            int rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected == 1;
        }
    }
}
