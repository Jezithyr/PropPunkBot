using Dapper;
using Discord;
using Discord.WebSocket;
using Npgsql;
using PeaceKeeper.Database;
using PeaceKeeper.Database.Models;

namespace PeaceKeeper.Services;

public sealed class SettingsService : PeacekeeperCoreServiceBase
{
    private readonly UserService _users;
    private readonly DbService _db;

    public async Task<ServerSettings?> GetServerSettings(ulong guild)
    {
        await using var connection = await _db.Get();
        return await connection.QuerySingleOrDefaultAsync<ServerSettings>(
            "SELECT * FROM server_settings WHERE guild = @guild", new { guild = (long) guild });
    }

    //get the global settings for the bot, NOTE: this shifts research slots to array notation for easier programming
    public async Task<GlobalSettings> GetSettings()
    {
        await using var connection = await _db.Get();
        var temp = await connection.QuerySingleOrDefaultAsync<GlobalSettingsRaw>(
            "SELECT * FROM global_settings WHERE lock = 0");
        var globalSettings = new GlobalSettings(temp);
        return globalSettings;
    }

    public async Task<long> GetOfficialServerId()
    {
        await using var connection = await _db.Get();
        return (await GetSettings()).OfficialServerId;
    }

    public SettingsService(DiscordSocketClient client, UserService users, DbService db)
        : base(client)
    {
        _users = users;
        _db = db;
    }
}
