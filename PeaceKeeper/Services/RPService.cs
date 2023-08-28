using Dapper;
using Discord.WebSocket;
using Npgsql;
using PeaceKeeper.Database;

namespace PeaceKeeper.Services;

public sealed class RPService : PeacekeeperServiceBase
{
    public async Task<bool> SetRpMode(long userId, RpMode mode, string? character)
    {
        await using var connection = await Db.Get();
        var userdata = await Users.Get(userId);
        if (userdata == null)
            return false;
        await connection.QueryAsync(
            "UPDATE users SET rpmode = @rpmode, rpcharacter = @rpchar WHERE id = @id",
            new {id = userId, rpmode = (int)mode, rpchar = character});
        return true;
    }

    public async Task<(RpMode, string?)> GetRpMode (long userId)
    {
        await using var connection = await Db.Get();
        var userdata = await Users.Get(userId);
        if (userdata == null)
            throw new ArgumentException("Invalid user");
        return (userdata.RpMode,userdata.RpCharacter);
    }

    public RPService(SettingsService settings, PermissionsService perms, UserService users, DbService db, WorldStateService worldState, DiscordSocketClient client) : base(settings, perms, users, db, worldState, client)
    {
    }
}
