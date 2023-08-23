using Dapper;
using Npgsql;
using PeaceKeeper.Database;
using PeaceKeeper.Database.Models;

namespace PeaceKeeper.Services;

public sealed class SettingsService
{
    private readonly DbService _db;

    public SettingsService(DbService db)
    {
        _db = db;
    }

    public async Task<Settings?> GetSettings(ulong guild, NpgsqlConnection? dbConnection = null)
    {
        if (dbConnection == null)
        {
            await using var db = await _db.Get();
            return await db.QuerySingleOrDefaultAsync<Settings>("SELECT * FROM settings WHERE guild = @guild", new { guild = (long) guild });
        }
        return await dbConnection.QuerySingleOrDefaultAsync<Settings>("SELECT * FROM settings WHERE guild = @guild", new { guild = (long) guild });
    }
}
