// Program.cs
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;


// tarvitaan SQLite-ajuria varten
using SQLitePCL;


// builder on objekti, 
// jonka avulla voimme kiinnittää erilaisia liitännäisiä web-palveluumme
// args parametri ilmestyy 'tyhjästä', 
// mutta se tarkoittaa terminaalissa palvelimen käynnistyksen yhteydessä annettuja 
// parametrejä (eivät ole pakollisia)
var builder = WebApplication.CreateBuilder(args);


// nämä kolme liitännäistä tarvitaan siihen, että swagger 
// OpenAPI-dokumentaatio toimii automaattisesti
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



// Tämä rivi liittyy SQLite-ajurin käyttöönottoon
Batteries.Init();

var app = builder.Build();


// Swagger-dokumentaatio on käytössä vain, kun serveri on kehitystilassa
// Serveri on devimoodissa, kun launchSettings.json-tiedostossa on

/*
"environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }

*/
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        // tämän pitäisi näyttää /swagger-osoitteessa
        // WithName-metodin poarametrin routen vieressä

        // materiaalia kirjoittaessa tämä ei toiminut
        options.DisplayOperationId();
    });
}

// Jos meillä olisi kehitystä vaten konffattu ja luotettu SSL-sertifikaatti, 
// tämä middleware ohjaisi automaattisesti liikenteen http->https
// eli lähettäessämme pyynnön osoitteeseen http://localhost:5175/api/users
// se uudelleenohjattaisiin autom. https://localhost:5175/api/users
app.UseHttpsRedirection();


// tämä rivi lukee tietokannan nimen apsettings.Development.json-tiedostosta

/*
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "SqliteConnection": "Data source=tuntiharjoitus1.db"
  }
}


*/
var sqliteConnString = builder.Configuration.GetConnectionString("SqliteConnection");

// jos SqliteConnection-ympäristömuuttujaa ei ole annettu konffeissa,
// palvelin antaa virheen
if (string.IsNullOrEmpty(sqliteConnString))
{
    Console.WriteLine("Error: SqliteConnection string is missing in configuration.");
    return;
}

// tämä on vain testiä varten, jotta näet, mistä
// kansiopolusta palvelin hakee tuntiharjoitus1.db-tietokantaa
Console.WriteLine($"Application Base Directory: {AppContext.BaseDirectory}");
Console.WriteLine($"Current Working Directory: {Environment.CurrentDirectory}");


string dbFileName = "tuntiharjoitus1.db";
string fullPathUsed = Path.Combine(Environment.CurrentDirectory, dbFileName);
Console.WriteLine($"Full path to DB file (expected): {fullPathUsed}");

// tätä routehandler-funktiota käytetään kahdessa routessa alempana


// sqliteConnString on tietokannan osoite
// AddUserRequest on class joka löytyy alempana. Sitä käytetään oo. käyttäjän tietojen muokkaamiseen
static async Task<IResult> UpdateUserHandler(string sqliteConnString, long id, AddUserRequest request)
{

    // yhdistetään tietokantaan
    // using var connection vastaa Pythonin with connect() as con:
    using var connection = new SqliteConnection(sqliteConnString);
    try
    {
        // avataan tietokantayhteys
        // sitä ei tarvitse sulkea käytön jälkeen itse,
        // koska käytämme using statementia ylempänä 
        connection.Open();

        // User? kysymysmerkki tietotyypin perässä tarkoittaa, että existingUser-muuttujan arvo voi olla null
        // se on null nyt alussa, mutta myös, jos haluttua käyttäjää ei löydy tietokannasta
        User? existingUser = null;

        // luodaan sql-kysely ja annetaan sille paramtrit
        var selectCommand = connection.CreateCommand();
        // @id vastaa pythonissa kysymysmerkkiä -> ...WHERE id = ?
        selectCommand.CommandText = "SELECT id, first_name, last_name, username FROM users WHERE id = @id;";
        // tässä @id-parametrille annetaan arvoksi id-muuttuja
        selectCommand.Parameters.AddWithValue("@id", id);

        // suoritetaan sql-komento ja luetaan tiedot tietokannasta
        // ExecuteReaderAsyncia käytetään, kun kysely on sellainen, josta odotetaan yhtä tai useampaa riviä
        using (var reader = await selectCommand.ExecuteReaderAsync())
        {

            if (reader.HasRows)
            {
                await reader.ReadAsync();
                existingUser = new User
                {
                    Id = reader.GetInt64(0),
                    FirstName = reader.GetString(1),
                    LastName = reader.GetString(2),
                    Username = reader.GetString(3)
                };
            }
        }
        // jos käyttäjää ei halutulla id:n arvolla löydy tietokannasta
        // palautetaan NOT FOUND 404
        if (existingUser == null)
        {
            return Results.NotFound(new { error = $"User with ID {id} not found." });
        }

        // jos request body ei sisällä first_name-avainta ja sen arvoa, käytetään käyttäjän oo. tietokannasta löytyvää etunimeä
        string newFirstName = string.IsNullOrWhiteSpace(request.FirstName) ? existingUser.FirstName : request.FirstName;
        // jos request body ei sisällä last_name-avainta ja sen arvoa, käytetään käyttäjän oo. tietokannasta löytyvää sukunimeä
        string newLastName = string.IsNullOrWhiteSpace(request.LastName) ? existingUser.LastName : request.LastName;
        // jos request body ei sisällä username-avainta ja sen arvoa, käytetään käyttäjän oo. tietokannasta löytyvää käyttäjänimeä
        string newUsername = string.IsNullOrWhiteSpace(request.Username) ? existingUser.Username : request.Username;



        // luodaan sql-komento päivitystä varten ja asetetaan sille yo. parametrit
        var updateCommand = connection.CreateCommand();
        updateCommand.CommandText = @"
            UPDATE users SET
                first_name = @newFirstName,
                last_name = @newLastName,
                username = @newUsername
            WHERE id = @id;";
        updateCommand.Parameters.AddWithValue("@newFirstName", newFirstName);
        updateCommand.Parameters.AddWithValue("@newLastName", newLastName);
        updateCommand.Parameters.AddWithValue("@newUsername", newUsername);
        updateCommand.Parameters.AddWithValue("@id", id);

        // suoritetaan yo. kysely
        // ExecuteNonQueryAsyncia käytetään kyselyissä, joista ei odotate tulosta
        // esim. UPDATE, DELETE, INSERT
        // paluuarvona tulee kokonaisluku, joka kertoo,
        // kuinka moneen riviin kysely on vaikuttanut
        int rowsAffected = await updateCommand.ExecuteNonQueryAsync();

        // jos päivitettyjen rivien määrä on 0, päivityksessä meni jotakin vikaan 
        if (rowsAffected == 0)
        {

            return Results.NotFound(new { error = $"User with ID {id} could not be updated (possibly not found after check)." });
        }

        // luodaan palautusta varten päivitetystä käyttäjästä json-vastaus 
        var updatedUser = new User
        {
            Id = id,
            FirstName = newFirstName,
            LastName = newLastName,
            Username = newUsername
        };

        // Results.Ok palauttaa käyttäjän tiedot ja 
        // Ok tarkoittaa HTTP-statuskoodia 200

        return Results.Ok(updatedUser);
    }
    // käsitellään mahdolliset virheet
    catch (SqliteException ex)
    {
        if (ex.SqliteErrorCode == 19) // SQLite error code for UNIQUE constraint violation
        {
            return Results.Conflict(new { error = "The provided username already exists. Please choose a different one." });
        }
        return Results.Problem($"Database error: {ex.Message}", statusCode: StatusCodes.Status500InternalServerError);
    }
    catch (Exception ex)
    {
        return Results.Problem($"An unexpected error occurred: {ex.Message}", statusCode: StatusCodes.Status500InternalServerError);
    }
}

// app.MapGet metodi tarkoittaa, että ao. routehandlerin koodi suoritetaan
// kun käyttäjä lähettää HTTP-pyynnön GET-metodilla osoitteeseen http://localhost:portti/api/users

// hakee kaikki käyttäjät
app.MapGet("/api/users", async () =>
{
    // luodaan tyhjä lista tietokannan rivejä varten
    var users = new List<User>();
    // luodaan yhteys ja avataan se
    using var connection = new SqliteConnection(sqliteConnString);
    try
    {
        connection.Open();


        // luodaan sql-komento ja suoritetaan se
        var command = connection.CreateCommand();
        command.CommandText = $"SELECT * FROM users";

        // Jälkeen ExecuteReadeAsync, koska odotetaan rivejä vataukseksi tietokannasta
        using var reader = await command.ExecuteReaderAsync();

        // Tässä voisi vaihtoehtoisesti palauttaa
        // tyhjän users-listan
        // on tulkinnanvaraista, onko se virhe, ettei tietokannan users-taulussa ole rivejä
        if (!reader.HasRows)
        {
            return Results.NotFound("No users found in the database.");
        }



        // luetaan luupissa kaikki rivit, jotka kyselyllä saadaan ulos
        // tietokannasta
        while (await reader.ReadAsync())
        {
            // lisätään ne ylempänä luotuun listaan
            users.Add(new User
            {
                Id = reader.GetInt64(0),
                FirstName = reader.GetString(1),
                LastName = reader.GetString(2),
                Username = reader.GetString(3)
            });
        }

        // palautetaan lista
        return Results.Ok(users);
    }
    // käsitellään mahdolliset virheet
    catch (Exception e)
    {
        return Results.Problem(e.Message, statusCode: StatusCodes.Status500InternalServerError);
    }
})
// Tässä annetaan nimi routelle
// Tämän pitäisi näkyä /swagger-pathissa dokumentaatiossa urlin vieressä
.WithName("GetAllUsers");


// app.MapPost metodi tarkoittaa, että ao. routehandlerin koodi suoritetaan
// kun käyttäjä lähettää HTTP-pyynnön POST-metodilla osoitteeseen http://localhost:portti/api/users

// [FromBody] on attribuutti ja
// ohjeistaa lukemaan AddUserRequest-luokan määrittämän
// datan HTTP-pyynnön bodysta
app.MapPost("/api/users", async ([FromBody] AddUserRequest request) =>
{
    // jos jokin pakollinen tieto puuttuu request bodysta, 
    // lähetetään vastaus HTTP-statuksella 400
    if (request == null || string.IsNullOrWhiteSpace(request.FirstName) ||
        string.IsNullOrWhiteSpace(request.LastName) || string.IsNullOrWhiteSpace(request.Username))
    {
        return Results.BadRequest(new { error = "First name, last name, and username are required." });
    }

    // luodaan tietokantayhteys ja avataan se
    using var connection = new SqliteConnection(sqliteConnString);
    try
    {
        connection.Open();


        // luodaan sql-komento lisäystä varten ja parametrit sille
        // Huomaa SELECT last_insert_rowid() INSERTIN jälkeen
        // tämä palauttaa lisätyn rivin id:n arvon
        var insertCommand = connection.CreateCommand();
        insertCommand.CommandText = @"
            INSERT INTO users(first_name, last_name, username)
            VALUES(@firstName, @lastName, @username);
            SELECT last_insert_rowid();";
        insertCommand.Parameters.AddWithValue("@firstName", request.FirstName);
        insertCommand.Parameters.AddWithValue("@lastName", request.LastName);
        insertCommand.Parameters.AddWithValue("@username", request.Username);

        // Tässä voidaan käyttää ExecuteReaderin sijasta ExeucteScalaryAsyncia,
        // koska tuloksena on ainoastaan yksi luku (SCALAR)

        // koska tässäkin on vatauksena SELECT-kyselyn tulos, 
        // teknisesti tässä voisi käyttää ExecuteReaderAsync-metodia
        // mutta ExecuteScalaria käytettäessä ei tarvitse tarkistaa, löytyykö rivejä, 
        // eikä niitä tarvitse erikseen lukea, vaan tulos on suoraan
        // käytettävissö

        // Mutta käytä ExecuteScalarAsyncia ainoastaan silloin, kun tuloksena on yhden rivin yksi sarake
        var newId = await insertCommand.ExecuteScalarAsync();
        if (newId == null)
        {
            return Results.BadRequest("Error creating a new user");
        }

        var newUser = new User
        {
            Id = (long)newId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Username = request.Username
        };

        // Created palauttaa vastauksen HTTP-statuskoodilla 201
        return Results.Created($"/api/users/{newUser.Id}", newUser);
    }
    // Käsitellään mahdolliset virheet
    catch (SqliteException ex)
    {
        if (ex.SqliteErrorCode == 19)
        {
            return Results.Conflict(new { error = "A user with this username already exists. Please choose a different one." });
        }
        return Results.Problem($"Database error: {ex.Message}", statusCode: StatusCodes.Status500InternalServerError);
    }
    catch (Exception ex)
    {

        return Results.Problem($"An unexpected error occurred: {ex.Message}", statusCode: StatusCodes.Status500InternalServerError);
    }
})
.WithName("AddUser");




// app.MapGet metodi tarkoittaa, että ao. routehandlerin koodi suoritetaan
// kun käyttäjä lähettää HTTP-pyynnön GET-metodilla osoitteeseen http://localhost:portti/api/users/1

// {id} on käyttäjän id:n arvo selaimen osoiterivillä
// muista nimetä muuttuja ja {muuttujannimi} samoin
app.MapGet("/api/users/{id}", async (long id) =>
{
    // luodaan yhteys ja avataan se
    using var connection = new SqliteConnection(sqliteConnString);
    try
    {
        connection.Open();

        // luodaan sql-kseyly käyttäjän hakua varten
        var command = connection.CreateCommand();
        command.CommandText = "SELECT id, first_name, last_name, username FROM users WHERE id = @id;";
        command.Parameters.AddWithValue("@id", id);


        // Suoritetaan kysely
        // ja tarkistetaan löytyykö kyselyllä tavaraa tietokannasta
        using var reader = await command.ExecuteReaderAsync();

        // jos käyttäjää ei annetulla id:n arvolla ole olemassa tietokannassa
        // lähetään käyttäjälle asianmukainen virhe NOT FOUND 404
        if (!reader.HasRows)
        {
            return Results.NotFound(new { error = $"User with ID {id} not found." });
        }

        // Tässä ei tarvitse silmukkaa, koska tuloksia on vain yksi rivi
        await reader.ReadAsync();
        var user = new User
        {
            Id = reader.GetInt64(0),
            FirstName = reader.GetString(1),
            LastName = reader.GetString(2),
            Username = reader.GetString(3)
        };

        // palautetaan sql-kyselyn tulos json-formaatissa takaisin HTTP-vatauksessa
        // Statuskoodilla 200
        return Results.Ok(user);
    }
    // Käsitetllään mahdolliset virheet
    catch (Exception e)
    {
        return Results.Problem(e.Message, statusCode: StatusCodes.Status500InternalServerError);
    }
})
.WithName("GetUserById");

// app.MapPut metodi tarkoittaa, että ao. routehandlerin koodi suoritetaan
// kun käyttäjä lähettää HTTP-pyynnön PUT-metodilla osoitteeseen http://localhost:portti/api/users/1 (tai jokin muu id:n arvo) 
app.MapPut("/api/users/{id}", async (long id, [FromBody] AddUserRequest request) =>
{
    // tämä lokaali funktio on luotu ylemänä
    return await UpdateUserHandler(sqliteConnString, id, request);

})
.WithName("UpdateUserPut");


// app.MapPatch metodi tarkoittaa, että ao. routehandlerin koodi suoritetaan
// kun käyttäjä lähettää HTTP-pyynnön PATCH-metodilla osoitteeseen http://localhost:portti/api/users/1 (tai jokin muu id:n arvo) 
app.MapPatch("/api/users/{id}", async (long id, [FromBody] AddUserRequest request) =>
{
    // tämä lokaali funktio on luotu ylemänä
    return await UpdateUserHandler(sqliteConnString, id, request);

})
.WithName("UpdateUserPatch");


// app.MapDelete metodi tarkoittaa, että ao. routehandlerin koodi suoritetaan
// kun käyttäjä lähettää HTTP-pyynnön DELETE-metodilla osoitteeseen http://localhost:portti/api/users/1 (tai jokin muu id:n arvo) 
app.MapDelete("/api/users/{id}", async (long id) =>
{
    // luodaan ja avataan tietokantayhteys
    using var connection = new SqliteConnection(sqliteConnString);
    try
    {
        connection.Open();

        // luodaan sql-komento poistoa varten
        var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM users WHERE id = @id;";
        command.Parameters.AddWithValue("@id", id);


        // tässä voidaan käyttää ExecuteNonQueryAsyncia,
        // koska DELETE-kysely ei palauta varsinaista tulosta tietokannasta
        int rowsAffected = await command.ExecuteNonQueryAsync(); // Execute the DELETE command

        // Jos poisto ei onnistu, palautetaan NOT FOUND 404 käyttäjälle
        if (rowsAffected == 0)
        {

            return Results.NotFound(new { error = $"User with ID {id} not found." });
        }

        // NoContent tarkoitaa, että tulos on tyhjä, mutta onnistunut
        // statuskoodilla 204
        return Results.NoContent();
    }
    // käsitellään mahdolliset virheet
    catch (Exception ex)
    {
        return Results.Problem($"An unexpected error occurred: {ex.Message}", statusCode: StatusCodes.Status500InternalServerError);
    }
})
.WithName("DeleteUser");

app.Run();

// Luokat HTTP-pyyntöjä ja vastauksia varten
public class User
{
    public long Id { get; set; }

    // Koska C#:ssa ei käytetä snake_casingia muuttjien nimeämisessä
    // [JsonPropertyName]-attribuutin avulla
    // voimme määrittää pyyntöjen ja vastausten poikkevat
    // kirjoitusasut, jotka eivät ole C#:n mukaisia
    [JsonPropertyName("first_name")]
    public string FirstName { get; set; } = string.Empty;

    [JsonPropertyName("last_name")]
    public string LastName { get; set; } = string.Empty;

    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;
}

public class AddUserRequest
{
    [JsonPropertyName("first_name")]
    public string FirstName { get; set; } = string.Empty;

    [JsonPropertyName("last_name")]
    public string LastName { get; set; } = string.Empty;

    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;
}
