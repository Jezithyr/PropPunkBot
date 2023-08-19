using Dapper;
using Discord.Interactions;
using PeaceKeeper.Database;

namespace PeaceKeeper.Modules;

[Group("admin", "moderation actions for prop-punk")]
public sealed class AdminModule : InteractionModuleBase
{
    [SlashCommand("createcountry", "create a new country")]
    public async Task CreateCountry()
    {
        await DeferAsync();
        await using var connection = DatabaseConnection.Get();

        var country = await connection.QuerySingleOrDefaultAsync<Country>("SELECT * FROM countries WHERE id = @id LIMIT 1",
            new {id = (long) Context.User.Id});
        if (country == null)
        {
            await connection.QuerySingleAsync<long>(
                "INSERT INTO users(id) VALUES(@id) ON CONFLICT DO NOTHING RETURNING -1",
                new {id = (long) Context.User.Id});
            await FollowupAsync("Created New Country");
        }
        else
        {
            await FollowupAsync("Country Already Registered");
        }
    }
}