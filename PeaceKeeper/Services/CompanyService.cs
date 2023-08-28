using Dapper;
using Discord;
using Npgsql;
using PeaceKeeper.Database;
using PeaceKeeper.Database.Models;

namespace PeaceKeeper.Services;

public class CompanyService : PeacekeeperServiceBase
{

    public async Task<Company?> GetCompany(Guid companyId, NpgsqlConnection? dbConnection = null)
    {
        await using var connection = await Db.ResolveDatabase(dbConnection);
        return await connection.QuerySingleOrDefaultAsync<Company>(
            "SELECT * FROM companies WHERE id = @id LIMIT 1",
            new {id = companyId});
    }

    public async Task<Company?> GetCompany(string companyName, NpgsqlConnection? dbConnection = null)
    {
        await using var connection = await Db.ResolveDatabase(dbConnection);
        return await connection.QuerySingleOrDefaultAsync<Company>(
            "SELECT * FROM companies WHERE name = @name LIMIT 1",
            new {name = companyName});
    }

    public async Task<Company?> GetCompanyFromCode(string companyTicker, NpgsqlConnection? dbConnection = null)
    {
        await using var connection = await Db.ResolveDatabase(dbConnection);
        return await connection.QuerySingleOrDefaultAsync<Company>(
            "SELECT * FROM companies WHERE shortname = @code LIMIT 1",
            new {code = companyTicker});
    }

    public async Task<bool> AssignUser(long userId, Guid companyId, bool makeOwner = true,
        NpgsqlConnection? dbConnection = null)
    {
        await using var connection = await Db.ResolveDatabase(dbConnection);
        var companyData = await GetCompany(companyId, dbConnection);
        var userData = await Users.Get(userId, dbConnection);
        if (companyData == null || userData == null) return false;
        if (makeOwner)
        {
            await connection.QueryAsync(
                "UPDATE users SET ceo = false WHERE company = @companyid",
                new {companyid = companyId});
        }
        await connection.QueryAsync<UserRaw>(
            "UPDATE users SET company = @company, ceo = @ceo WHERE id = @id",
            new {id = userId, company = companyId, ceo = makeOwner});
        return true;
    }

    public async Task<bool> CreateCompany(string companyName, string companyTicker, long? ownerId = null,
        NpgsqlConnection? dbConnection = null)
    {
        await using var connection = await Db.ResolveDatabase(dbConnection);
        if (companyName.Length > 128 || companyTicker.Length > 4)
             return false;
        var company = await connection.QuerySingleOrDefaultAsync<Company>(
            "SELECT * FROM companies WHERE name = @name LIMIT 1",
            new {name = companyName});

        if (company != null) return false;
        var companyId = await connection.QuerySingleAsync<Guid>(
            "INSERT INTO companies (name, shortname) VALUES (@name,@shortname) ON CONFLICT DO NOTHING RETURNING id",
            new {name = companyName, shortname = companyTicker});
        if (ownerId != null)
        {
            return await AssignUser(ownerId.Value, companyId, true, connection);
        }
        return true;
    }

    public async Task<bool> RemoveCompany(Guid companyId, NpgsqlConnection? dbConnection = null)
    {
        await using var connection = await Db.ResolveDatabase(dbConnection);
        var companyData = await dbConnection.QuerySingleOrDefaultAsync<Company>(
            "SELECT * FROM companies WHERE id = @id LIMIT 1",
            new {id = companyId});
        if (companyData == null)
            return false;
        await connection.QueryAsync("DELETE FROM companies where id = @id", new {id = companyId});
        return true;
    }

    public async Task<bool> RemoveCompany(string companyName, NpgsqlConnection? dbConnection = null)
    {
        await using var connection = await Db.ResolveDatabase(dbConnection);
        var companyData = await dbConnection.QuerySingleOrDefaultAsync<Company>(
            "SELECT * FROM companies WHERE name = @name LIMIT 1",
            new {name = companyName});
        if (companyData == null)
            return false;
        await connection.QueryAsync("DELETE FROM companies where name = @name", new {name = companyName});
        return true;
    }

    public CompanyService(SettingsService settings, PermissionsService perms, UserService users, DbService db, WorldStateService worldState) : base(settings, perms, users, db, worldState)
    {
    }
}
