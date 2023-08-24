using Dapper;
using Discord;
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
        await using var connection = await _db.ResolveDatabase(dbConnection);
        return await connection.QuerySingleOrDefaultAsync<Settings>("SELECT * FROM settings WHERE guild = @guild", new { guild = (long) guild });
    }
}
