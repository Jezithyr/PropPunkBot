using Dapper;
using Npgsql;
using PropPunkBot;

Env.EnsureLoadEnvFile();

var connString = new NpgsqlConnectionStringBuilder()
{
    Host = Environment.GetEnvironmentVariable("DATABASE_HOST"),
    Port = int.Parse(Environment.GetEnvironmentVariable("DATABASE_PORT")!),
    Database = Environment.GetEnvironmentVariable("DATABASE_NAME"),
    Username = Environment.GetEnvironmentVariable("DATABASE_USERNAME"),
    Password = Environment.GetEnvironmentVariable("DATABASE_PASSWORD")
}.ConnectionString;

await using var connection = new NpgsqlConnection(connString);
var techs = await connection.QueryAsync($"SELECT * FROM technologies");
Console.WriteLine(techs.ToList());
