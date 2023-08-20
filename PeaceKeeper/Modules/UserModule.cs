using Dapper;
using Discord;
using Discord.Interactions;
using PeaceKeeper.Database;

namespace PeaceKeeper.Modules;

[Group("user", "user related commands")]
public sealed class UserModule : InteractionModuleBase
{
    private readonly DbService _db;

    public UserModule(DbService db)
    {
        _db = db;
    }

    [UserCommand("getinfo")]
    public async Task GetInfo(IUser user)
    {
        await DeferAsync();
        await using var db = await _db.Get();
        var users = await db.QueryAsync<UserRaw, Country, Company, User>(
            "SELECT * FROM users LEFT JOIN countries ON countries.id = users.country LEFT JOIN companies ON companies.id = users.company WHERE users.id = @id",
            (userData, countryData, companyData) => new User(
                userData.Id,
                countryData,
                userData.Leader,
                companyData,
                userData.Ceo
            ), new {id = (long) user.Id});
        if (users.SingleOrDefault() is not { } userData)
        {
            await FollowupAsync("User not found");
            return;
        }

        var embed = new EmbedBuilder();
        embed.WithAuthor(user.GlobalName, user.GetAvatarUrl());

        var country = $"Country ({(userData.Leader ? "Leader" : "Member")})";
        var company = $"Company ({(userData.Ceo ? "CEO" : "Member")})";
        embed.AddField(country, userData.Country?.Name ?? "None", true);
        embed.AddField(company, userData.Company?.Name ?? "None", true);

        await FollowupAsync(null, new[] {embed.Build()}, ephemeral: true);
    }
    
    [SlashCommand("register", "register for the prop-punk universe")]
    public async Task Register()
    {
        await DeferAsync();
        await using var connection = await _db.Get();

        var user = await connection.QuerySingleOrDefaultAsync<UserRaw>("SELECT * FROM users WHERE id = @id LIMIT 1",
            new {id = (long) Context.User.Id});
        if (user == null)
        {
            await connection.QuerySingleAsync<long>(
                "INSERT INTO users(id) VALUES(@id) ON CONFLICT DO NOTHING RETURNING -1",
                new {id = (long) Context.User.Id});
            await FollowupAsync("Registered new user");
        }
        else
        {
            await FollowupAsync("User already registered");
        }
    }
    [SlashCommand("unregister", "unregister from the prop-punk universe")]
    public async Task UnRegister()
    {
        await DeferAsync();
        await using var connection = await _db.Get();

        var userdata = await connection.QuerySingleOrDefaultAsync<UserRaw>("SELECT * FROM users WHERE id = @id LIMIT 1",
            new {id = (long) Context.User.Id});
        if (userdata == null)
        {
            await FollowupAsync($"You are not registered!");
        }
        else
        {
            await connection.QueryAsync("DELETE FROM users where id = @id", new {id = (long) Context.User.Id});
            await FollowupAsync($"Removed you from the prop-punk universe.");
        }
    }
}
