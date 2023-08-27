using Npgsql;
using PeaceKeeper.Services;

namespace PeaceKeeper.Database;

public sealed class DbService : PeacekeeperServiceBase
{
    private readonly NpgsqlDataSource _source;

    public DbService()
    {
        var connString = new NpgsqlConnectionStringBuilder
        {
            Host = Environment.GetEnvironmentVariable("DATABASE_HOST"),
            Port = int.Parse(Environment.GetEnvironmentVariable("DATABASE_PORT")!),
            Database = Environment.GetEnvironmentVariable("DATABASE_NAME"),
            Username = Environment.GetEnvironmentVariable("DATABASE_USERNAME"),
            Password = Environment.GetEnvironmentVariable("DATABASE_PASSWORD")
        };

        _source = NpgsqlDataSource.Create(connString);
    }

    public ValueTask<NpgsqlConnection> Get()
    {
        return _source.OpenConnectionAsync();
    }

    public async ValueTask<NpgsqlConnection> ResolveDatabase(NpgsqlConnection? dbConnection)
    {
        if (dbConnection == null)
        {
            return await Get();
        }
        return dbConnection;
    }
}
