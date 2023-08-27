using Dapper;
using Npgsql;
using PeaceKeeper.Database;
using PeaceKeeper.Database.Models;

namespace PeaceKeeper.Services;

public sealed class PermissionsService
{
    private readonly DbService _db;
    private readonly SettingsService _settings;
    private readonly UserService _users;
    public PermissionsService(DbService db, SettingsService settings, UserService users)
    {
        _db = db;
        _settings = settings;
        _users = users;
    }

    public async Task<GlobalPermissionLevel> GetPermissionsForUser(long userId, NpgsqlConnection? dbConnection = null)
    {
        await using var connection = await _db.ResolveDatabase(dbConnection);
        var userPerms = await connection.QuerySingleOrDefaultAsync<GlobalPermissions>(
            "SELECT * FROM global_permissions where userid = @id LIMIT 1",
            new {id = userId}
        );
        return userPerms?.Permissions ?? GlobalPermissionLevel.Default;
    }

    public async Task SetPermissionsForUser(long userId, GlobalPermissionLevel permissions,
        NpgsqlConnection? dbConnection = null)
    {
        await using var connection = await _db.ResolveDatabase(dbConnection);
        await connection.QueryAsync(
            "UPDATE global_permissions SET permissions = @perms WHERE userid = @id",
            new {id = userId}
        );
    }


}
