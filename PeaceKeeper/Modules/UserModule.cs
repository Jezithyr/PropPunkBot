using Dapper;
using Discord.Interactions;
using PeaceKeeper.Database;

namespace PeaceKeeper.Modules;

[Group("user", "user related commands")]
public sealed class UserModule : InteractionModuleBase
{
    [SlashCommand("getinfo", "get a user's info")]
    public async Task GetInfo()
    {
        await RespondAsync("test");
    }
    
    [SlashCommand("register", "register for the prop-punk universe")]
    public async Task Register()
    {
        await DeferAsync();
        await using var connection = DatabaseConnection.Get();

        var user = await connection.QuerySingleOrDefaultAsync<User>("SELECT * FROM users WHERE id = @id LIMIT 1",
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
        await using var connection = DatabaseConnection.Get();

        var userdata = await connection.QuerySingleOrDefaultAsync<User>("SELECT * FROM users WHERE id = @id LIMIT 1",
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