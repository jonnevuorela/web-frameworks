
using Microsoft.Data.Sqlite;

namespace AbstractionExample;

public static class Program
{
    public static void Main()
    {

        /*
        
        Vaikka Read-metodeiden toteutukset eri luokissa poikkeavat paljon toisistaan
        (csv-tiedostosta rivien lukeminen on erilaista kuin tietokannasta rivien lukeminen)
        silti molempien luokkien read-metodeja kutsutaan täysin samalla tavalla

        Sama koskee eri luokkien Write-metodeja. 
        Koska tiedostoon rivin lisääminen on erilaista kuin tietokantaan rivin lisääminen,
        ovat toteutukset ihan erilaisia. Niiden eroista huolimatta 
        niitä kutustaan täysin samalla tavalla

        */

        var csv = new CsvFileDataSource("output.csv", "Age,Name,City");
        csv.Write("40,Juhani Kuru,Rovaniemi");
        csv.Read();

        var sqlite = new SqLiteDataSource("Data Source=database.db");
        sqlite.Write("40,Juhani Kuru,Rovaniemi");
        sqlite.Read();
    }
}

/*

Ao. koodissa on yliluokka DataSource, 
jonka perivät sekä CsvFileDataSource- että SqLiteDataSource-luokka perii. 
DataSource-luokassa on kaksi metodia read ja write. 
CsvFileDataSource ja SqLiteDataSource sisältävät omat toteutuksensa näistä metodeista. 


*/

public abstract class DataSource
{

    public abstract void Write(string data);

    public abstract void Read();


}

/*

Luokka sisältää konkreettiset toteutukset DataSourcesta 
perityille metodeille Read ja Write
Toteutukset lukevat ja kirjoittavat csv-tiedostoon.

*/



public class CsvFileDataSource(string filepath, string headers) : DataSource
{
    private string FilePath { get; } = filepath;
    private string Headers { get; } = headers;

    public override void Write(string data)
    {
        try
        {

            if (!File.Exists(FilePath))
            {

                using StreamWriter sw = new StreamWriter(FilePath);
                sw.WriteLine(Headers);
            }


            using (StreamWriter sw = new StreamWriter(FilePath, true))
            {
                sw.WriteLine(data);
            }

            Console.WriteLine($"Successfully wrote a line to '{FilePath}'.");
            Console.WriteLine("You can find the file in the application's output directory.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    public override void Read()
    {
        Console.WriteLine($"Reading data from '{FilePath}'...");

        try
        {

            if (!File.Exists(FilePath))
            {
                Console.WriteLine("File not found.");
                return;
            }


            string[] lines = File.ReadAllLines(FilePath);


            if (lines.Length == 0)
            {
                Console.WriteLine("The file is empty.");
                return;
            }


            for (int i = 0; i < lines.Length; i++)
            {

                Console.WriteLine($"Row {i + 1}: {lines[i]}");
            }

            Console.WriteLine("Finished reading file.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while reading: {ex.Message}");
        }
    }
}

/*

Luokka sisältää konkreettiset toteutukset DataSourcesta 
perityille metodeille Read ja Write
Toteutukset lukevat ja kirjoittavat SQLite-tietokantaan

*/


public class SqLiteDataSource(string connString) : DataSource
{
    private string ConnString { get; } = connString;

    private const string TableName = "People";

    public override void Write(string data)
    {
        Console.WriteLine("Writing a new row to the database...");


        string[] values = data.Split(',');
        if (values.Length != 3)
        {
            Console.WriteLine("Error: Invalid data format. Expected 'Age,Name,City'.");
            return;
        }

        string name = values[1].Trim();
        string city = values[2].Trim();
        string age = values[0].Trim();

        using var connection = new SqliteConnection(ConnString);
        try
        {
            connection.Open();


            var createTableCommand = connection.CreateCommand();
            createTableCommand.CommandText =
                $"CREATE TABLE IF NOT EXISTS {TableName} (Id INTEGER PRIMARY KEY, Age INTEGER, Name TEXT, City TEXT)";
            createTableCommand.ExecuteNonQuery();


            var insertCommand = connection.CreateCommand();
            insertCommand.CommandText = $"INSERT INTO {TableName} (Age, Name, City) VALUES (@age, @name, @city)";
            insertCommand.Parameters.AddWithValue("@name", name);
            insertCommand.Parameters.AddWithValue("@city", city);
            insertCommand.Parameters.AddWithValue("@age", int.Parse(age));

            insertCommand.ExecuteNonQuery();

            Console.WriteLine($"Successfully wrote a new row for '{name}'.");
        }
        catch (SqliteException ex)
        {
            Console.WriteLine($"A database error occurred: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        }
    }

    public override void Read()
    {
        Console.WriteLine($"Reading all rows from table '{TableName}'...");

        using var connection = new SqliteConnection(ConnString);
        try
        {
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = $"SELECT * FROM {TableName}";

            using var reader = command.ExecuteReader();

            if (!reader.HasRows)
            {
                Console.WriteLine("No data found in the table.");
                return;
            }

            Console.WriteLine("------------------------------");

            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                int age = reader.GetInt32(1);
                string name = reader.GetString(2);
                string city = reader.GetString(3);

                Console.WriteLine($"Id: {id}, Age: {age}, Name: {name}, City: {city}");
            }
            Console.WriteLine("------------------------------");

            Console.WriteLine("Finished reading data.");
        }
        catch (SqliteException ex)
        {
            Console.WriteLine($"A database error occurred: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        }
    }
}
