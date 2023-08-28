using Dapper;
using Discord.WebSocket;
using Npgsql;
using PeaceKeeper.Database;
using PeaceKeeper.Database.Models;

namespace PeaceKeeper.Services;

public sealed class PermissionsService : PeacekeeperCoreServiceBase
{
    private readonly DbService _db;

    public async Task<bool> UserHasPermission(long userId, GlobalPermissionLevel permission)
    {
        await using var connection = await _db.Get();
        var userPerms = await connection.QuerySingleOrDefaultAsync<GlobalPermissionsRaw>(
            "SELECT * FROM global_permissions where id = @id LIMIT 1",
            new {id = userId}
        );
        return userPerms == null
            ? GlobalPermissionLevel.Default.HasFlag(permission)
            : ((GlobalPermissionLevel)userPerms.Permissions).HasFlag(permission);
    }


    public async Task<GlobalPermissionLevel> GetPermissionsForUser(long userId)
    {
        await using var connection = await _db.Get();
        var userPerms = await connection.QuerySingleOrDefaultAsync<GlobalPermissions>(
            "SELECT * FROM global_permissions where id = @id LIMIT 1",
            new {id = userId}
        );
        return userPerms?.Permissions ?? GlobalPermissionLevel.Default;
    }

    public async Task SetPermissionsForUser(long userId, GlobalPermissionLevel permissions)
    {
        await using var connection = await _db.Get();
        await connection.QueryAsync(
            "UPDATE global_permissions SET permissions = @perms WHERE id = @id",
            new {id = userId}
        );
    }

    public PermissionsService(DiscordSocketClient client, DbService db) : base(client)
    {
        _db = db;
    }
}
