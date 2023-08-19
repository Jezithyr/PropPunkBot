using Dapper;
using Discord;
using Discord.Interactions;
using PeaceKeeper.Database;

namespace PeaceKeeper.Modules;

public partial class AdminModule
{
    
    [SlashCommand("createcountry", "create a new country")]
    public async Task CreateCountry(string countryName, string shortName)
    {
        await DeferAsync();
        if (countryName.Length > 128)
        {
            await FollowupAsync("Country name is too long, must be less than 128 characters!");
            return;
        }
        if (shortName.Length > 4)
        {
            await FollowupAsync("Country Tag must be 4 characters or less!");
            return;
        }
        
        
        await using var connection = DatabaseConnection.Get();
        //TODO: check if user is on the mod-list
        var country = await connection.QuerySingleOrDefaultAsync<Country>("SELECT * FROM countries WHERE name = @name LIMIT 1",
            new {name = countryName});
        if (country == null)
        {
            await connection.QuerySingleAsync<long>(
                "INSERT INTO countries (name, shortname) VALUES (@name,@shortname) ON CONFLICT DO NOTHING RETURNING -1",
                new {name = countryName, shortname = shortName});
            await FollowupAsync($"Registered new country: {countryName}");
        }
        else
        {
            await FollowupAsync($"Country with name: {countryName} already exists!");
        }
    }
    [SlashCommand("assigncountry", "assign a country to a user")]
    public async Task AssignCountry(IUser user)
    {
        await DeferAsync();
        
    }
}