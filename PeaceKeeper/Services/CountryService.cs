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

}
