using Dapper;
using Discord;
using Discord.Interactions;
using PeaceKeeper.Database;

namespace PeaceKeeper.Modules;

[Group("admin", "moderation actions for prop-punk")]
public partial class AdminModule : InteractionModuleBase
{
    private readonly DbService _db;

    public AdminModule(DbService db)
    {
        _db = db;
    }

    [SlashCommand("removeuser", "remove a user from the prop-punk universe")]
    public async Task Remove(IUser user)
    {
        await DeferAsync();

        await using var connection = await _db.Get();

        var userdata = await connection.QuerySingleOrDefaultAsync<UserRaw>("SELECT * FROM users WHERE id = @id LIMIT 1",
            new {id = (long) user.Id});
        if (userdata == null)
        {
            await FollowupAsync($"User {user.Username} was not found!");
        }
        else
        {
            await connection.QueryAsync("DELETE FROM users where id = @id", new {id = (long) user.Id});
            await FollowupAsync($"Removed user: {user.Username}");
        }
    }
}
