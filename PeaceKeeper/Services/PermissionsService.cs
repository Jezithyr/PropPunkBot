using Dapper;
using Discord.WebSocket;
using Npgsql;
using PeaceKeeper.Database;
using PeaceKeeper.Database.Models;

namespace PeaceKeeper.Services;

public sealed class PermissionsService : PeacekeeperCoreServiceBase
{
    private readonly DbService _db;

    public async Task<bool> UserHasPermission(long userId, GlobalPermissionLevel permission,
        NpgsqlConnection? dbConnection = null)
    {
        await using var connection = await _db.ResolveDatabase(dbConnection);
        var userPerms = await connection.QuerySingleOrDefaultAsync<GlobalPermissions>(
            "SELECT * FROM global_permissions where id = @id LIMIT 1",
            new {id = userId}
        );
        if (userPerms == null)
            return GlobalPermissionLevel.Default.HasFlag(permission);
        return userPerms.Permissions.HasFlag(permission);
    }


    public async Task<GlobalPermissionLevel> GetPermissionsForUser(long userId, NpgsqlConnection? dbConnection = null)
    {
        await using var connection = await _db.ResolveDatabase(dbConnection);
        var userPerms = await connection.QuerySingleOrDefaultAsync<GlobalPermissions>(
            "SELECT * FROM global_permissions where id = @id LIMIT 1",
            new {id = userId}
        );
        return userPerms?.Permissions ?? GlobalPermissionLevel.Default;
    }

    public async Task SetPermissionsForUser(long userId, GlobalPermissionLevel permissions,
        NpgsqlConnection? dbConnection = null)
    {
        await using var connection = await _db.ResolveDatabase(dbConnection);
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
