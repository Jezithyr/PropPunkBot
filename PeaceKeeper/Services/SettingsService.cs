using Dapper;
using Discord;
using Npgsql;
using PeaceKeeper.Database;
using PeaceKeeper.Database.Models;

namespace PeaceKeeper.Services;

public sealed class SettingsService : PeacekeeperServiceBase
{
    private readonly DbService _db;

    public SettingsService(DbService db)
    {
        _db = db;
    }

    public async Task<ServerSettings?> GetServerSettings(ulong guild, NpgsqlConnection? dbConnection = null)
    {
        await using var connection = await _db.ResolveDatabase(dbConnection);
        return await connection.QuerySingleOrDefaultAsync<ServerSettings>("SELECT * FROM server_settings WHERE guild = @guild", new { guild = (long) guild });
    }

    //get the global settings for the bot, NOTE: this shifts research slots to array notation for easier programming
    public async Task<GlobalSettings> GetSettings()
    {
        await using var connection = await _db.Get();
        var temp = await connection.QuerySingleOrDefaultAsync<GlobalSettingsRaw>(
            "SELECT * FROM global_settings WHERE lock = 0");
        var globalSettings = new GlobalSettings(temp.AotYearStart, temp.AotScaleFactor, temp.CountryResearchSlots-1,
            temp.CompanyResearchSlots-1);
        return globalSettings;
    }
}
