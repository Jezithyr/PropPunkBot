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

    public async Task<User?> GetInfo(long userid, NpgsqlConnection? dbConnection = null)
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

    public async Task<User?> GetInfo(IUser user, NpgsqlConnection? dbConnection = null)
    {
        await using var connection = await _db.ResolveDatabase(dbConnection);
        return await GetInfo((long) user.Id, dbConnection);
    }

}
