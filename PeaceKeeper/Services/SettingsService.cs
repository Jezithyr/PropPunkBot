using Dapper;
using Discord;
using Npgsql;
using PeaceKeeper.Database;
using PeaceKeeper.Database.Models;

namespace PeaceKeeper.Services;

public sealed class SettingsService : PeacekeeperServiceBase
{

    public SettingsService(SettingsService settings, UserService users, DbService db) : base(settings, users, db)
    {
    }

    public async Task<ServerSettings?> GetServerSettings(ulong guild, NpgsqlConnection? dbConnection = null)
    {
        await using var connection = await Db.ResolveDatabase(dbConnection);
        return await connection.QuerySingleOrDefaultAsync<ServerSettings>("SELECT * FROM server_settings WHERE guild = @guild", new { guild = (long) guild });
    }

    //get the global settings for the bot, NOTE: this shifts research slots to array notation for easier programming
    public async Task<GlobalSettings> GetSettings(NpgsqlConnection? dbConnection = null)
    {
        await using var connection = await Db.ResolveDatabase(dbConnection);
        var temp = await connection.QuerySingleOrDefaultAsync<GlobalSettingsRaw>(
            "SELECT * FROM global_settings WHERE lock = 0");
        var globalSettings = new GlobalSettings(temp);
        return globalSettings;
    }

    public async Task<long> GetOfficialServerId(NpgsqlConnection? dbConnection = null)
    {
        await using var connection = await Db.ResolveDatabase(dbConnection);
        return (await GetSettings(connection)).OfficialServerId;
    }
}
