using Npgsql;

namespace PropPunkShared;

public static class Env
{
    private static bool _loaded;

    public static void EnsureLoadEnvFile()
    {
        if (_loaded)
            return;

        _loaded = true;

        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".env");
        var lines = File.ReadAllLines(path);
        foreach (var line in lines)
        {
            var str = line.Split('=');
            var key = str[0];
            var val = str[1];

            Environment.SetEnvironmentVariable(key, val);
        }
    }

    public static string CreateConnectionString()
    {
        return new NpgsqlConnectionStringBuilder
        {
            Host = Environment.GetEnvironmentVariable("DATABASE_HOST"),
            Port = int.Parse(Environment.GetEnvironmentVariable("DATABASE_PORT")!),
            Database = Environment.GetEnvironmentVariable("DATABASE_NAME"),
            Username = Environment.GetEnvironmentVariable("DATABASE_USERNAME"),
            Password = Environment.GetEnvironmentVariable("DATABASE_PASSWORD")
        }.ConnectionString;
    }
}
