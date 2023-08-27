using Dapper;
using Discord;
using Npgsql;
using PeaceKeeper.Database;
using PeaceKeeper.Database.Models;

namespace PeaceKeeper.Services;

public sealed class UserService : PeacekeeperServiceBase
{
    public async Task<bool> Exists(long userId, NpgsqlConnection? dbConnection = null)
    {
        await using var connection = await Db.ResolveDatabase(dbConnection);
        var users = await connection.QuerySingleOrDefaultAsync<UserRaw>(
            "SELECT * FROM users WHERE users.id = @id LIMIT 1",
            new {id = userId});
        return users != null;
    }

    public async Task<User?> Get(long userid, NpgsqlConnection? dbConnection = null)
    {
        await using var connection = await Db.ResolveDatabase(dbConnection);
        var users = await connection.QueryAsync<UserRaw, Country, CompanyRaw, UserRaw2>(
            "SELECT * FROM users LEFT JOIN countries ON countries.id = users.country " +
            "LEFT JOIN companies ON companies.id = users.company " +
            "WHERE users.id = @id",
            (userData, countryData, companyData) => new UserRaw2(
                userData.Id,
                countryData,
                userData.Leader,
                companyData,
                userData.Ceo
            ), new {id = userid});
        var temp = users?.SingleOrDefault();
        if (temp == null)
            return null;
        if (temp.Company == null)
            return new User(temp.Id, temp.Country, temp.Leader, null, temp.Ceo);

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
        return new User(temp.Id, temp.Country, temp.Leader, company.FirstOrDefault(), temp.Ceo);
    }

    public async Task<User?> Get(IUser user, NpgsqlConnection? dbConnection = null)
    {
        return await Get((long) user.Id, dbConnection);
    }

    public async Task<bool> Remove(long userid, NpgsqlConnection? dbConnection = null)
    {
         await using var connection = await Db.ResolveDatabase(dbConnection);
         var user = await Get(userid, dbConnection);
         if (user == null) return false;
         await connection.QueryAsync("DELETE FROM users where id = @id", new {id = userid});
         return true;
    }

    public async Task<bool> Remove(IUser user, NpgsqlConnection? dbConnection = null)
    {
        return await Remove((long)user.Id, dbConnection);
    }

    public async Task<bool> Add(long userid, NpgsqlConnection? dbConnection = null)
    {
        await using var connection = await Db.ResolveDatabase(dbConnection);
        var user = await Get(userid, dbConnection);
        if (user != null) return false;

        await connection.QuerySingleAsync<long>(
            "INSERT INTO users(id) VALUES(@id) ON CONFLICT DO NOTHING RETURNING -1",
            new {id = userid});
        return true;
    }

    public async Task<bool> Add(IUser user, NpgsqlConnection? dbConnection = null)
    {
        return await Add((long)user.Id, dbConnection);
    }

    public UserService(SettingsService settings, UserService users, DbService db) : base(settings, users, db)
    {
    }
}
