using Dapper;
using Npgsql;
using PeaceKeeper.Database;
using PeaceKeeper.Database.Models;

namespace PeaceKeeper.Services;

public sealed class TechService
{
    private readonly DbService _db;
    private readonly SettingsService _settings;
    private readonly UserService _users;
    public TechService(DbService db, SettingsService settings, UserService users)
    {
        _db = db;
        _settings = settings;
        _users = users;
    }
    public async Task<bool> CreateTechnology(string techName, TechnologyUse uses, int yearDeveloped, TechField field,
        int cost, NpgsqlConnection? dbConnection = null)
    {
        if (techName.Length > 128)
        {
            return false;
        }
        await using var connection = await _db.ResolveDatabase(dbConnection);
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

    public async Task<bool> RemoveTech(string techName, NpgsqlConnection? dbConnection = null)
    {
        if (techName.Length > 128)
        {
            return false;
        }
        await using var connection = await _db.ResolveDatabase(dbConnection);
        var tech = await connection.QuerySingleOrDefaultAsync<Technology>(
            "SELECT * FROM technologies WHERE name = @name LIMIT 1",
            new {name = techName});
        if (tech == null)
            return false;
        await connection.QueryAsync("DELETE FROM technologies where name = @name", new {name =techName});
        return true;

    }

    public async Task<bool> TechnologyExists(string techName, NpgsqlConnection? dbConnection = null)
    {
        if (techName.Length > 128)
        {
            return false;
        }
        await using var connection = await _db.ResolveDatabase(dbConnection);
        var tech = await connection.QuerySingleOrDefaultAsync<Technology>(
            "SELECT * FROM technologies WHERE name = @name LIMIT 1",
            new {name = techName});
        return tech != null;
    }

    public async Task<Guid?> GetTechId(string techName, NpgsqlConnection? dbConnection = null)
    {
        if (techName.Length > 128)
        {
            return null;
        }
        await using var connection = await _db.ResolveDatabase(dbConnection);
        var tech = await connection.QuerySingleOrDefaultAsync<Technology>(
            "SELECT * FROM technologies WHERE name = @name LIMIT 1",
            new {name = techName});
        return tech?.Id;
    }

    public async Task<bool> UpdateTech(string techName, TechnologyUse? uses, int? yearDeveloped, TechField? field,
        int? cost, NpgsqlConnection? dbConnection = null)
    {
        if (techName.Length > 128)
        {
            return false;
        }
        await using var connection = await _db.ResolveDatabase(dbConnection);
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

    public async Task<bool> UpdateTech(Guid techId, TechnologyUse? uses, int? yearDeveloped, TechField? field,
        int? cost, NpgsqlConnection? dbConnection = null)
    {
        await using var connection = await _db.ResolveDatabase(dbConnection);
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

    public async Task<List<Technology>> GetTechRequirements(Guid techId, NpgsqlConnection? dbConnection = null)
    {
        await using var connection = await _db.ResolveDatabase(dbConnection);
        var temp = await connection.QueryAsync<Technology>(
            "SELECT * FROM technology_requirements " +
            "LEFT JOIN technologies ON technologies.id = technology_requirements.techid " +
            "LEFT JOIN technologies ON technologies.id = technology_requirements.requirementid " +
            "WHERE techid = @id LIMIT 1",
            new {id = techId}
            );
        return temp == null ? new List<Technology>() : temp.ToList();
    }

    public async Task<List<Technology>> GetTechsWithRequirement(Guid techId, NpgsqlConnection? dbConnection = null)
    {
        await using var connection = await _db.ResolveDatabase(dbConnection);
        var technologies = await connection.QueryAsync<Technology>(
            "SELECT * FROM technology_requirements " +
            "LEFT JOIN technologies ON technologies.id = technology_requirements.techid " +
            "LEFT JOIN technologies ON technologies.id = technology_requirements.requirementid " +
            "WHERE requirementid = @id LIMIT 1",
            new {id = techId}
        );
        return technologies == null ? new List<Technology>() : technologies.ToList();
    }

    public async Task<HashSet<Technology>> GetAllTechnologies( NpgsqlConnection? dbConnection = null)
    {
        await using var connection = await _db.ResolveDatabase(dbConnection);
        var techs = await connection.QueryAsync<Technology>(
            "SELECT * FROM technologies");
        return techs == null ? new HashSet<Technology>() : techs.ToHashSet();
    }

    public async Task<HashSet<Technology>> GetAllTechnologiesFromField(TechField field,
        NpgsqlConnection? dbConnection = null)
    {
        await using var connection = await _db.ResolveDatabase(dbConnection);
        var techs = await connection.QueryAsync<Technology>(
            "SELECT * FROM technologies WHERE field = @techfield",
            new {techfield = field}
            );
        return techs == null ? new HashSet<Technology>() : techs.ToHashSet();
    }

    public async Task<HashSet<Technology>> GetAllTechnologiesFromUsage(TechnologyUse use,
        NpgsqlConnection? dbConnection = null)
    {
        await using var connection = await _db.ResolveDatabase(dbConnection);
        var techs = await connection.QueryAsync<Technology>(
            "SELECT * FROM technologies WHERE uses = @techUse",
            new {techUse = use}
        );
        return techs == null ? new HashSet<Technology>() : techs.ToHashSet();
    }
}
