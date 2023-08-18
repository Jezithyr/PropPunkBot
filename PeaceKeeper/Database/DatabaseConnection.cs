using Npgsql;

namespace PeaceKeeper.Database;

public sealed class DatabaseConnection
{
    public static NpgsqlConnection Get()
    {
        var connString = new NpgsqlConnectionStringBuilder
        {
            Host = Environment.GetEnvironmentVariable("DATABASE_HOST"),
            Port = int.Parse(Environment.GetEnvironmentVariable("DATABASE_PORT")!),
            Database = Environment.GetEnvironmentVariable("DATABASE_NAME"),
            Username = Environment.GetEnvironmentVariable("DATABASE_USERNAME"),
            Password = Environment.GetEnvironmentVariable("DATABASE_PASSWORD")
        }.ConnectionString;

        return new NpgsqlConnection(connString);
    }
}