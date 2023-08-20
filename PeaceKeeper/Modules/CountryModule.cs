using Dapper;
using Discord;
using Discord.Interactions;
using PeaceKeeper.Database;

namespace PeaceKeeper.Modules;

[Group("country", "country actions for prop-punk")]
public sealed class CountryModule : InteractionModuleBase
{
    private readonly DbService _db;

    public CountryModule(DbService db)
    {
        _db = db;
    }

    [UserCommand("Get Country")]
    public async Task GetCountry(IUser user)
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
            if (userdata.Country.HasValue)
            {
                var countryData = await connection.QuerySingleOrDefaultAsync<Country>("SELECT * FROM countries WHERE name = @id LIMIT 1",
                    new {id = userdata.Country});
                if (userdata.Leader)
                {
                    await FollowupAsync($"{user.Username} is the Leader of {countryData.Name} [{countryData.ShortName}]");
                }
                else
                {
                    await FollowupAsync($"{user.Username} is a member of {countryData.Name} [{countryData.ShortName}]");
                }
            }
            else
            {
                await FollowupAsync($"{user.Username} is not a member of any country!");
            }
        }
    }
}
