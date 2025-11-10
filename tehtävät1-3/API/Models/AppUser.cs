using Microsoft.Data.Sqlite;

namespace API.Models
{
    public class AppUser
    {
        public long? Id { get; set; } = null;

        public required string Firstname { get; set; }

        public required string Lastname { get; set; }
        public required string Username { get; set; }

        /// <summary>
        /// Retrieves all users from the database.
        /// </summary>
        /// <returns>A list of all users from the database.</returns>
        /// <exception cref="SqliteException">Thrown when an error occurs while
        /// connecting to or querying the database.</exception> <exception
        /// cref="InvalidOperationException">Thrown if the connection is already open
        /// when trying to open it again.</exception>
        public static async Task<IEnumerable<AppUser>> GetAll()
        {
            // luodaan tyhjä lista tietokannan rivejä varten
            var users = new List<AppUser>();
            // luodaan yhteys ja avataan se
            using var connection = new SqliteConnection("Data source=tuntiharjoitus1.db");

            connection.Open();

            // luodaan sql-komento ja suoritetaan se
            var command = connection.CreateCommand();
            command.CommandText = $"SELECT * FROM users";

            // Jälkeen ExecuteReadeAsync, koska odotetaan rivejä vataukseksi
            // tietokannasta
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

        public static async Task<AppUser?> GetById(long id)
        {
            // luodaan tyhjä lista tietokannan rivejä varten
            AppUser? user;
            // luodaan yhteys ja avataan se
            using var connection = new SqliteConnection("Data source=tuntiharjoitus1.db");

            connection.Open();

            // luodaan sql-komento ja suoritetaan se
            var command = connection.CreateCommand();
            command.CommandText = $"SELECT * FROM users WHERE id = @id";
            command.Parameters.AddWithValue("@id", id);

            // Jälkeen ExecuteReadeAsync, koska odotetaan rivejä vataukseksi
            // tietokannasta
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

        private async Task<bool> Add()
        {
            using var connection = new SqliteConnection("Data source=tuntiharjoitus1.db");
            connection.Open();

            var insertCommand = connection.CreateCommand();
            insertCommand.CommandText =
                @"
                INSERT INTO users(first_name, last_name, username)
                VALUES(@firstName, @lastName, @username);
                SELECT last_insert_rowid();";
            insertCommand.Parameters.AddWithValue("@firstName", Firstname);
            insertCommand.Parameters.AddWithValue("@lastName", Lastname);
            insertCommand.Parameters.AddWithValue("@username", Username);

            var newId = await insertCommand.ExecuteScalarAsync();
            if (newId == null)
            {
                return false;
            }

            Id = (long)newId;

            return true;
        }

        private async Task<bool> Update()
        {
            using var connection = new SqliteConnection("Data source=tuntiharjoitus1.db");
            connection.Open();

            // luodaan sql-komento päivitystä varten ja asetetaan sille yo. parametrit
            var updateCommand = connection.CreateCommand();
            updateCommand.CommandText =
                @"
                UPDATE users SET
                    first_name = @newFirstName,
                    last_name = @newLastName,
                    username = @newUsername
                WHERE id = @id;";

            // Koska pääsemme modelin metodin sisällä luokan instanssin propertyihin
            // käsiksi, metodille ei tarvitse lähettää erillisiä parametrejä
            updateCommand.Parameters.AddWithValue("@newFirstName", Firstname);
            updateCommand.Parameters.AddWithValue("@newLastName", Lastname);
            updateCommand.Parameters.AddWithValue("@newUsername", Username);
            updateCommand.Parameters.AddWithValue("@id", Id);

            int rowsAffected = await updateCommand.ExecuteNonQueryAsync();

            return rowsAffected == 1;
        }

        public async Task<bool> Save()
        {
            bool success;
            if (Id == null)
            {
                success = await Add();
            }
            else
            {
                success = await Update();
            }

            return success;
        }

        public async Task<bool> Remove()
        {
            using var connection = new SqliteConnection("Data source=tuntiharjoitus1.db");

            connection.Open();

            // luodaan sql-komento poistoa varten
            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM users WHERE id = @id;";
            command.Parameters.AddWithValue("@id", Id);

            // tässä voidaan käyttää ExecuteNonQueryAsyncia,
            // koska DELETE-kysely ei palauta varsinaista tulosta tietokannasta
            int rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected == 1;
        }
    }
}
