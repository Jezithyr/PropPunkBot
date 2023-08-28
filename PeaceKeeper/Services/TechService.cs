using Dapper;
using Discord.WebSocket;
using Npgsql;
using PeaceKeeper.Database;
using PeaceKeeper.Database.Models;

namespace PeaceKeeper.Services;

public sealed class TechService : PeacekeeperServiceBase
{
    public async Task<bool> Create(string techName, TechnologyUse uses, int yearDeveloped, TechField field,
        int cost)
    {
        if (techName.Length > 128)
        {
            return false;
        }
        await using var connection = await Db.Get();
        var tech = await connection.QuerySingleOrDefaultAsync<Technology>(
            "SELECT * FROM technologies WHERE name = @name LIMIT 1",
            new {name = techName});
        if (tech != null)
            return false;
        await connection.QuerySingleAsync<long>(
            "INSERT INTO technologies (name, uses, year, field, cost) VALUES (@name, @uses, @year, @field, @cost) ON CONFLICT DO NOTHING RETURNING -1",
            new {name = techName, uses = uses, year = yearDeveloped, field = field, cost = cost});
        return true;
    }

    public async Task<bool> Remove(string techName)
    {
        if (techName.Length > 128)
        {
            return false;
        }
        await using var connection = await Db.Get();
        var tech = await connection.QuerySingleOrDefaultAsync<Technology>(
            "SELECT * FROM technologies WHERE name = @name LIMIT 1",
            new {name = techName});
        if (tech == null)
            return false;
        await connection.QueryAsync("DELETE FROM technologies where name = @name", new {name =techName});
        return true;

    }

    public async Task<bool> Exists(string techName)
    {
        if (techName.Length > 128)
        {
            return false;
        }
        await using var connection = await Db.Get();
        var tech = await connection.QuerySingleOrDefaultAsync<Technology>(
            "SELECT * FROM technologies WHERE name = @name LIMIT 1",
            new {name = techName});
        return tech != null;
    }

    public async Task<Guid?> GetId(string techName)
    {
        if (techName.Length > 128)
        {
            return null;
        }
        await using var connection = await Db.Get();
        var tech = await connection.QuerySingleOrDefaultAsync<Technology>(
            "SELECT * FROM technologies WHERE name = @name LIMIT 1",
            new {name = techName});
        return tech?.Id;
    }

    public async Task<bool> Update(string techName, TechnologyUse? uses, int? yearDeveloped, TechField? field,
        int? cost)
    {
        if (techName.Length > 128)
        {
            return false;
        }
        await using var connection = await Db.Get();
        var tech = await connection.QuerySingleOrDefaultAsync<Technology>(
            "SELECT * FROM technologies WHERE name = @name LIMIT 1",
            new {name = techName});
        if (tech == null)
            return false;
        uses ??= tech.Uses;
        yearDeveloped ??= tech.Year;
        field ??= field;
        cost ??= cost;
        await connection.QueryAsync(
            "UPDATE technologies SET uses = @uses,year = @year, field = @field, cost = @cost WHERE name = @name",
            new {name = techName, uses=uses, year = yearDeveloped, field = field, cost = cost});
        return true;
    }

    public async Task<bool> Update(Guid techId, TechnologyUse? uses, int? yearDeveloped, TechField? field,
        int? cost)
    {
        await using var connection = await Db.Get();
        var tech = await connection.QuerySingleOrDefaultAsync<Technology>(
            "SELECT * FROM technologies WHERE id = @id LIMIT 1",
            new {id = techId});
        if (tech == null)
            return false;
        uses ??= tech.Uses;
        yearDeveloped ??= tech.Year;
        field ??= field;
        cost ??= cost;
        await connection.QueryAsync(
            "UPDATE technologies SET uses = @uses,year = @year, field = @field, cost = @cost WHERE id = @id",
            new {id = techId, uses=uses, year = yearDeveloped, field = field, cost = cost});
        return true;
    }

    public async Task<List<Technology>> GetRequirements(Guid techId)
    {
        await using var connection = await Db.Get();
        var temp = await connection.QueryAsync<Technology>(
            "SELECT * FROM technology_requirements " +
            "LEFT JOIN technologies ON technologies.id = technology_requirements.id " +
            "LEFT JOIN technologies ON technologies.id = technology_requirements.requirementid " +
            "WHERE id = @id LIMIT 1",
            new {id = techId}
            );
        return temp == null ? new List<Technology>() : temp.ToList();
    }

    public async Task<List<Technology>> GetWithRequirement(Guid techId)
    {
        await using var connection = await Db.Get();
        var technologies = await connection.QueryAsync<Technology>(
            "SELECT * FROM technology_requirements " +
            "LEFT JOIN technologies ON technologies.id = technology_requirements.id " +
            "LEFT JOIN technologies ON technologies.id = technology_requirements.requirementid " +
            "WHERE requirementid = @id LIMIT 1",
            new {id = techId}
        );
        return technologies == null ? new List<Technology>() : technologies.ToList();
    }

    public async Task<HashSet<Technology>> GetAll( )
    {
        await using var connection = await Db.Get();
        var techs = await connection.QueryAsync<Technology>(
            "SELECT * FROM technologies");
        return techs == null ? new HashSet<Technology>() : techs.ToHashSet();
    }

    public async Task<HashSet<Technology>> GetAllFromField(TechField field)
    {
        await using var connection = await Db.Get();
        var techs = await connection.QueryAsync<Technology>(
            "SELECT * FROM technologies WHERE field = @techfield",
            new {techfield = field}
            );
        return techs == null ? new HashSet<Technology>() : techs.ToHashSet();
    }

    public async Task<HashSet<Technology>> GetAllFromUsage(TechnologyUse use)
    {
        await using var connection = await Db.Get();
        var techs = await connection.QueryAsync<Technology>(
            "SELECT * FROM technologies WHERE uses = @techUse",
            new {techUse = use}
        );
        return techs == null ? new HashSet<Technology>() : techs.ToHashSet();
    }

    public TechService(SettingsService settings, PermissionsService perms, UserService users, DbService db, WorldStateService worldState, DiscordSocketClient client) : base(settings, perms, users, db, worldState, client)
    {
    }
}
