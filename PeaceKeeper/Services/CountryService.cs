using Dapper;
using Discord;
using Discord.Interactions;
using Npgsql;
using PeaceKeeper.Database;
using PeaceKeeper.Database.Models;

namespace PeaceKeeper.Services;

public sealed class CountryService
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
        return await connection.QuerySingleOrDefaultAsync<Country>(
            "SELECT * FROM countries WHERE id = @id LIMIT 1",
            new {id = countryId});
    }

    public async Task<Country?> GetCountry(string countryName, NpgsqlConnection? dbConnection = null)
    {
        await using var connection = await _db.ResolveDatabase(dbConnection);
        return await connection.QuerySingleOrDefaultAsync<Country>(
            "SELECT * FROM countries WHERE name = @name LIMIT 1",
            new {name = countryName});
    }

    public async Task<Country?> GetCountryFromCode(string countryCode, NpgsqlConnection? dbConnection = null)
    {
        await using var connection = await _db.ResolveDatabase(dbConnection);
        return await connection.QuerySingleOrDefaultAsync<Country>(
            "SELECT * FROM countries WHERE shortname = @code LIMIT 1",
            new {code = countryCode});
    }
    public async Task<bool> AssignUser(IUser user, Guid countryId, bool makeOwner = true ,
        NpgsqlConnection? dbConnection = null)
    {
        await using var connection = await _db.ResolveDatabase(dbConnection);
        var countryData = await GetCountry(countryId, dbConnection);
        var userData = await _users.GetUser(user, dbConnection);
        if (countryData == null || userData == null) return false;
        if (makeOwner)
        {
            await connection.QueryAsync(
                "UPDATE users SET leader = false WHERE country = @countryid",
                new {countryid = countryId});
        }
        await connection.QueryAsync<UserRaw>(
            "UPDATE users SET country = @country, leader = @leader WHERE id = @id",
            new {id = (long) user.Id, country = countryId, leader = makeOwner});
        return true;
    }

    public async Task<bool> AssignUser(IUser user, string countryName, bool makeOwner = true,
        NpgsqlConnection? dbConnection = null)
    {
        await using var connection = await _db.ResolveDatabase(dbConnection);
        var countryData = await GetCountry(countryName, dbConnection);
        if (countryData == null) return false;
        return await AssignUser(user, countryData.Id, makeOwner, dbConnection);
    }
}
