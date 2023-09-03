using Dapper;
using Discord;
using Discord.WebSocket;
using Npgsql;
using PeaceKeeper.Database;
using PeaceKeeper.Database.Models;

namespace PeaceKeeper.Services;

public class CompanyService : PeacekeeperServiceBase
{

    public async Task<Company?> GetCompany(Company company)
    {
        await using var connection = await Db.Get();
        return await connection.QuerySingleOrDefaultAsync<Company>(
            "SELECT * FROM companies WHERE id = @id LIMIT 1",
            new {id = company.Id});
    }

    public async Task<Company?> GetCompany(string companyName)
    {
        await using var connection = await Db.Get();
        return await connection.QuerySingleOrDefaultAsync<Company>(
            "SELECT * FROM companies WHERE name = @name LIMIT 1",
            new {name = companyName});
    }

    public async Task<List<Company>> GetAllCompanies()
    {
        await using var connection = await Db.Get();
        List<Company> output = new();
        foreach (var company in  await connection.QueryAsync<Company>(
                     "SELECT * FROM companies "))
        {
            if (company == null) continue;
            output.Add(company);
        }
        return output;
    }

    public async Task<Company?> GetCompanyFromCode(string companyTicker)
    {
        await using var connection = await Db.Get();
        return await connection.QuerySingleOrDefaultAsync<Company>(
            "SELECT * FROM companies WHERE shortname = @code LIMIT 1",
            new {code = companyTicker});
    }

    public async Task<bool> AssignUser(long userId, Company company, bool makeOwner = true)
    {
        await using var connection = await Db.Get();
        var userData = await Users.Get(userId);
        if (userData == null) return false;
        if (makeOwner)
        {
            await connection.QueryAsync(
                "UPDATE users SET ceo = false WHERE company = @companyid",
                new {companyid = company.Id});
        }
        await connection.QueryAsync<UserRaw>(
            "UPDATE users SET company = @company, ceo = @ceo WHERE id = @id",
            new {id = userId, company = company.Id, ceo = makeOwner});
        return true;
    }

    public async Task<bool> CreateCompany(string companyName, string companyTicker, long? ownerId = null)
    {
        await using var connection = await Db.Get();
        if (companyName.Length > 128 || companyTicker.Length > 4)
             return false;
        var company = await connection.QuerySingleOrDefaultAsync<Company>(
            "SELECT * FROM companies WHERE name = @name LIMIT 1",
            new {name = companyName});

        if (company != null) return false;
        var companyId = await connection.QuerySingleAsync<Guid>(
            "INSERT INTO companies (name, shortname) VALUES (@name,@shortname) ON CONFLICT DO NOTHING RETURNING id",
            new {name = companyName, shortname = companyTicker});
        var newcomp = await connection.QuerySingleOrDefaultAsync<Company>(
            "SELECT * FROM companies WHERE name = @name LIMIT 1",
            new {name = companyName});
        var newCompany = await GetCompany(newcomp);
        if (ownerId != null && newCompany != null)
        {
            return await AssignUser(ownerId.Value, newCompany, true);
        }
        return true;
    }

    public async Task<bool> RemoveCompany(Company company)
    {
        await using var connection = await Db.Get();
        var companyData = await connection.QuerySingleOrDefaultAsync<Company>(
            "SELECT * FROM companies WHERE id = @id LIMIT 1",
            new {id = company.Id});
        if (companyData == null)
            return false;
        await connection.QueryAsync("DELETE FROM companies where id = @id", new {id = company.Id});
        return true;
    }

    public async Task<bool> RemoveCompany(string companyName)
    {
        await using var connection = await Db.Get();
        var companyData = await connection.QuerySingleOrDefaultAsync<Company>(
            "SELECT * FROM companies WHERE name = @name LIMIT 1",
            new {name = companyName});
        if (companyData == null)
            return false;
        await connection.QueryAsync("DELETE FROM companies where name = @name", new {name = companyName});
        return true;
    }

    public CompanyService(SettingsService settings, PermissionsService perms, UserService users, DbService db, WorldStateService worldState, DiscordSocketClient client) : base(settings, perms, users, db, worldState, client)
    {
    }
}
