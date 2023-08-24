using Dapper;
using Discord;
using Npgsql;
using PeaceKeeper.Database;
using PeaceKeeper.Database.Models;

namespace PeaceKeeper.Services;

public sealed class UserService
{
    private readonly DbService _db;

    public UserService(DbService db)
    {
        _db = db;
    }

    public async Task<User?> GetUser(long userid, NpgsqlConnection? dbConnection = null)
    {
        await using var connection = await _db.ResolveDatabase(dbConnection);
        var users = await connection.QueryAsync<UserRaw, Country, Company, User>(
            "SELECT * FROM users LEFT JOIN countries ON countries.id = users.country LEFT JOIN companies ON companies.id = users.company WHERE users.id = @id",
            (userData, countryData, companyData) => new User(
                userData.Id,
                countryData,
                userData.Leader,
                companyData,
                userData.Ceo
            ), new {id = userid});
        return users?.SingleOrDefault();
    }

    public async Task<User?> GetUser(IUser user, NpgsqlConnection? dbConnection = null)
    {
        return await GetUser((long) user.Id, dbConnection);
    }

    public async Task<bool> Remove(long userid, NpgsqlConnection? dbConnection = null)
    {
         await using var connection = await _db.ResolveDatabase(dbConnection);
         var user = await GetUser(userid, dbConnection);
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
        await using var connection = await _db.ResolveDatabase(dbConnection);
        var user = await GetUser(userid, dbConnection);
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

}
