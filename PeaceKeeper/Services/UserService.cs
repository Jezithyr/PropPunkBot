using Dapper;
using Discord;
using Discord.WebSocket;
using Npgsql;
using PeaceKeeper.Database;
using PeaceKeeper.Database.Models;

namespace PeaceKeeper.Services;

public sealed class UserService : PeacekeeperCoreServiceBase
{
    private readonly DbService _db;


    public async Task<bool> Exists(long userId)
    {
        await using var connection = await _db.Get();
        var users = await connection.QuerySingleOrDefaultAsync<UserRaw>(
            "SELECT * FROM users WHERE users.id = @id LIMIT 1",
            new {id = userId});
        return users != null;
    }

    public async Task<User?> Get(long userid)
    {
        await using var connection = await _db.Get();
        var users = await connection.QueryAsync<UserRaw, Country, CompanyRaw, UserRaw2>(
            "SELECT * FROM users LEFT JOIN countries ON countries.id = users.country " +
            "LEFT JOIN companies ON companies.id = users.company " +
            "WHERE users.id = @id",
            (userData, countryData, companyData) => new UserRaw2(
                userData.Id,
                countryData,
                userData.Leader,
                companyData,
                userData.Ceo,
                userData.RpMode,
                userData.RpCharacter
            ), new {id = userid});
        var temp = users?.SingleOrDefault();
        if (temp == null)
            return null;
        if (temp.Company == null)
        {
            return new User(temp.Id, temp.Country, temp.Leader, null, temp.Ceo, (RpMode)temp.RpMode,
                temp.RpCharacter);
        }

        var company = await connection.QueryAsync<CompanyRaw, Country, Company>(
            "SELECT * FROM companies " +
            "LEFT JOIN countries ON countries.id = companies.owningcountryid " +
            "where companies.id = @companyId",
            (companyData, countryData) => new Company(
                companyData.Id,
                companyData.Name,
                companyData.ShortName,
                countryData
            ),
            new
            {
                companyId = temp.Company.Id
            });
        return new User(temp.Id, temp.Country, temp.Leader, company.FirstOrDefault(), temp.Ceo,
            (RpMode)temp.RpMode,
            temp.RpCharacter);
    }

    public async Task<User?> Get(IUser user)
    {
        return await Get((long) user.Id);
    }

    public async Task<bool> Remove(long userid)
    {
         await using var connection = await _db.Get();
         var user = await Get(userid);
         if (user == null) return false;
         await connection.QueryAsync("DELETE FROM users where id = @id", new {id = userid});
         return true;
    }

    public async Task<bool> Remove(IUser user)
    {
        return await Remove((long)user.Id);
    }

    public async Task<bool> Add(long userid)
    {
        await using var connection = await _db.Get();
        var user = await Get(userid);
        if (user != null) return false;

        await connection.QuerySingleAsync<long>(
            "INSERT INTO users(id) VALUES(@id) ON CONFLICT DO NOTHING RETURNING -1",
            new {id = userid});
        return true;
    }

    public async Task<bool> Add(IUser user)
    {
        return await Add((long)user.Id);
    }

    public UserService(DiscordSocketClient client, DbService db) : base(client)
    {
        _db = db;
    }
}
