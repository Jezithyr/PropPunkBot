using Dapper;
using Discord.Interactions;
using PeaceKeeper.Database;

namespace PeaceKeeper.Modules;

public partial class AdminModule
{
    [SlashCommand("createcompany", "create a new un-owned company")]
    public async Task CreateCompany(string companyName, string shortName, bool isStateOwned = false)
    {
        await DeferAsync();
        if (companyName.Length > 128)
        {
            await FollowupAsync("Company name is too long, must be less than 128 characters!");
            return;
        }
        if (shortName.Length > 4)
        {
            await FollowupAsync("Company Tag must be 4 characters or less!");
            return;
        }
        await using var connection = DatabaseConnection.Get();
        //TODO: check if user is on the mod-list
        var country = await connection.QuerySingleOrDefaultAsync<Company>("SELECT * FROM companies WHERE name = @name LIMIT 1",
            new {name = companyName});
        if (country == null)
        {
            await connection.QuerySingleAsync<long>(
                "INSERT INTO companies (name, shortname, stateowned) VALUES (@name,@shortname, @stateowned) ON CONFLICT DO NOTHING RETURNING -1",
                new {name = companyName, shortname = shortName, stateowned = isStateOwned});
            await FollowupAsync($"Registered new company: {companyName}");
        }
        else
        {
            await FollowupAsync($"Country with name: {companyName} already exists!");
        }
        
    }

}