using Dapper;
using Discord;
using Npgsql;
using PeaceKeeper.Database;
using PeaceKeeper.Database.Models;

namespace PeaceKeeper.Services;

public class CountryService : PeacekeeperServiceBase
 {
    private readonly DbService _db;
    private readonly SettingsService _settings;
    private readonly UserService _users;
    public CountryService(DbService db, SettingsService settings, UserService users)
    {
        _db = db;
        _settings = settings;
        _users = users;
    }

    public async Task<Country?> GetCountry(Guid countryId, NpgsqlConnection? dbConnection = null)
    {
        await using var connection = await _db.ResolveDatabase(dbConnection);
        return await connection.QuerySingleOrDefaultAsync<Database.Models.Country>(
            "SELECT * FROM countries WHERE id = @id LIMIT 1",
            new {id = countryId});
    }

    public async Task<Country?> GetCountry(string countryName, NpgsqlConnection? dbConnection = null)
    {
        await using var connection = await _db.ResolveDatabase(dbConnection);
        return await connection.QuerySingleOrDefaultAsync<Database.Models.Country>(
            "SELECT * FROM countries WHERE name = @name LIMIT 1",
            new {name = countryName});
    }

    public async Task<Country?> GetCountryFromCode(string countryCode, NpgsqlConnection? dbConnection = null)
    {
        await using var connection = await _db.ResolveDatabase(dbConnection);
        return await connection.QuerySingleOrDefaultAsync<Database.Models.Country>(
            "SELECT * FROM countries WHERE shortname = @code LIMIT 1",
            new {code = countryCode});
    }

    public async Task<bool> AssignUser(long userId, Guid countryId, bool makeOwner = true,
        NpgsqlConnection? dbConnection = null)
    {
        await using var connection = await _db.ResolveDatabase(dbConnection);
        var countryData = await GetCountry(countryId, dbConnection);
        var userData = await _users.Get(userId, dbConnection);
        if (countryData == null || userData == null) return false;
        if (makeOwner)
        {
            await connection.QueryAsync(
                "UPDATE users SET leader = false WHERE country = @countryid",
                new {countryid = countryId});
        }
        await connection.QueryAsync<UserRaw>(
            "UPDATE users SET country = @country, leader = @leader WHERE id = @id",
            new {id = userId, country = countryId, leader = makeOwner});
        return true;
    }

    public async Task<bool> AssignUser(IUser user, Guid countryId, bool makeOwner = true ,
        NpgsqlConnection? dbConnection = null)
    {
        return await AssignUser((long)user.Id, countryId, makeOwner, dbConnection);
    }

    public async Task<bool> AssignUser(IUser user, string countryName, bool makeOwner = true,
        NpgsqlConnection? dbConnection = null)
    {
        await using var connection = await _db.ResolveDatabase(dbConnection);
        var countryData = await GetCountry(countryName, dbConnection);
        if (countryData == null) return false;
        return await AssignUser(user, countryData.Id, makeOwner, dbConnection);
    }

    public async Task<bool> CreateCountry(string countryName, string countryCode, long? ownerId = null,
        NpgsqlConnection? dbConnection = null)
    {
        await using var connection = await _db.ResolveDatabase(dbConnection);
        if (countryName.Length > 128 || countryCode.Length > 4)
             return false;
        var country = await connection.QuerySingleOrDefaultAsync<Database.Models.Country>("SELECT * FROM countries WHERE name = @name LIMIT 1",
            new {name = countryName});

        if (country != null) return false;
        var countryId = await connection.QuerySingleAsync<Guid>(
            "INSERT INTO countries (name, shortname) VALUES (@name,@shortname) ON CONFLICT DO NOTHING RETURNING id",
            new {name = countryName, shortname = countryCode});
        if (ownerId != null)
        {
            return await AssignUser(ownerId.Value, countryId, true, connection);
        }
        return true;
    }
    public async Task<bool> RemoveCountry(Guid countryId, NpgsqlConnection? dbConnection = null)
    {
        await using var connection = await _db.ResolveDatabase(dbConnection);
        var countryData = await dbConnection.QuerySingleOrDefaultAsync<Database.Models.Country>(
            "SELECT * FROM countries WHERE id = @id LIMIT 1",
            new {id = countryId});
        if (countryData == null)
            return false;
        await connection.QueryAsync("DELETE FROM countries where id = @id", new {id = countryId});
        return true;
    }

    public async Task<bool> RemoveCountry(string countryName, NpgsqlConnection? dbConnection = null)
    {
        await using var connection = await _db.ResolveDatabase(dbConnection);
        var countryData = await dbConnection.QuerySingleOrDefaultAsync<Database.Models.Country>(
            "SELECT * FROM countries WHERE name = @name LIMIT 1",
            new {name = countryName});
        if (countryData == null)
            return false;
        await connection.QueryAsync("DELETE FROM countries where name = @name", new {name = countryName});
        return true;
    }

}
