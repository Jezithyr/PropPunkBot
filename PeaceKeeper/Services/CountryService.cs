using Dapper;
using Discord;
using Discord.WebSocket;
using Npgsql;
using PeaceKeeper.Database;
using PeaceKeeper.Database.Models;

namespace PeaceKeeper.Services;

public class CountryService : PeacekeeperServiceBase
 {
    public async Task<Country?> GetCountry(Guid countryId)
    {
        await using var connection = await Db.Get();
        return await connection.QuerySingleOrDefaultAsync<Database.Models.Country>(
            "SELECT * FROM countries WHERE id = @id LIMIT 1",
            new {id = countryId});
    }

    public async Task<Country?> GetCountry(string countryName)
    {
        await using var connection = await Db.Get();
        return await connection.QuerySingleOrDefaultAsync<Database.Models.Country>(
            "SELECT * FROM countries WHERE name = @name LIMIT 1",
            new {name = countryName});
    }


    public async Task<List<Country>> GetAllCountries()
    {
        await using var connection = await Db.Get();
        List<Country> output = new();
        foreach (var country in  await connection.QueryAsync<Country>(
                     "SELECT * FROM countries "))
        {
            if (country == null) continue;
            output.Add(country);
        }
        return output;
    }

    public async Task<Country?> GetCountryFromCode(string countryCode)
    {
        await using var connection = await Db.Get();
        return await connection.QuerySingleOrDefaultAsync<Database.Models.Country>(
            "SELECT * FROM countries WHERE shortname = @code LIMIT 1",
            new {code = countryCode});
    }

    public async Task<bool> AssignUser(long userId, Country country, bool makeOwner = true)
    {
        await using var connection = await Db.Get();
        var userData = await Users.Get(userId);
        if ( userData == null) return false;
        if (makeOwner)
        {
            await connection.QueryAsync(
                "UPDATE users SET leader = false WHERE country = @countryid",
                new {countryid = country.Id});
        }
        await connection.QueryAsync<UserRaw>(
            "UPDATE users SET country = @countryid, leader = @leader WHERE id = @id",
            new {id = userId, countryid = country.Id, leader = makeOwner});
        return true;
    }

    public async Task<bool> AssignUser(IUser user, Country country, bool makeOwner = true)
    {
        return await AssignUser((long)user.Id, country, makeOwner);
    }


    public async Task<bool> CreateCountry(string countryName, string countryCode, long? ownerId = null)
    {
        await using var connection = await Db.Get();
        if (countryName.Length > 128 || countryCode.Length > 4)
             return false;
        var country = await connection.QuerySingleOrDefaultAsync<Database.Models.Country>("SELECT * FROM countries WHERE name = @name LIMIT 1",
            new {name = countryName});

        if (country != null) return false;
        var countryId = await connection.QuerySingleAsync<Guid>(
            "INSERT INTO countries (name, shortname) VALUES (@name,@shortname) ON CONFLICT DO NOTHING RETURNING id",
            new {name = countryName, shortname = countryCode});
        var newCountry = await GetCountry(countryId);
        if (ownerId != null && newCountry != null)
        {
            return await AssignUser(ownerId.Value, newCountry, true);
        }
        return true;
    }
    public async Task<bool> RemoveCountry(Guid countryId)
    {
        await using var connection = await Db.Get();
        var countryData = await connection.QuerySingleOrDefaultAsync<Database.Models.Country>(
            "SELECT * FROM countries WHERE id = @id LIMIT 1",
            new {id = countryId});
        if (countryData == null)
            return false;
        await connection.QueryAsync("DELETE FROM countries where id = @id", new {id = countryId});
        return true;
    }

    public async Task<bool> RemoveCountry(string countryName)
    {
        await using var connection = await Db.Get();
        var countryData = await connection.QuerySingleOrDefaultAsync<Database.Models.Country>(
            "SELECT * FROM countries WHERE name = @name LIMIT 1",
            new {name = countryName});
        if (countryData == null)
            return false;
        await connection.QueryAsync("DELETE FROM countries where name = @name", new {name = countryName});
        return true;
    }

    public CountryService(SettingsService settings, PermissionsService perms, UserService users, DbService db, WorldStateService worldState, DiscordSocketClient client) : base(settings, perms, users, db, worldState, client)
    {
    }
 }
