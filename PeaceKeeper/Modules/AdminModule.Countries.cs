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
    [SlashCommand("assigntocountry", "assign a country to a user")]
    public async Task AssignToCountry(IUser user,string countryName, bool makeOwner, bool reassign = true)
    {
        await DeferAsync();
        if (countryName.Length > 128)
        {
            await FollowupAsync("Country name is too long, must be less than 128 characters!");
            return;
        }

        await using var connection = DatabaseConnection.Get();
        //TODO: check if user is on the mod-list
        var userData = await connection.QuerySingleOrDefaultAsync<User>(
            "SELECT * FROM users WHERE id = @id LIMIT 1",
            new {id =(long) user.Id});
        
        if (userData == null)
        {
            await FollowupAsync($"User with name: {user.Username} is not registered!");
            return;
        }
        
        var countryData = await connection.QuerySingleOrDefaultAsync<Country>(
            "SELECT * FROM countries WHERE name = @name LIMIT 1",
            new {name = countryName});
        
        if (!reassign && userData.Country.HasValue)
        {
            await FollowupAsync($"User with name: {user.Username} is already a member in a country!");
            return;
        }
        
        if (countryData == null)
        {
            await FollowupAsync($"Country with name: {countryName} does not exist!");
            return;
        }
        if (makeOwner)
        {
            await connection.QueryAsync(
                "UPDATE users SET leader = false WHERE country = @countryid",
                new {countryid = countryData.Id});
        }
            
        await connection.QueryAsync<User>(
            "UPDATE users SET country = @country, leader = @leader WHERE id = @id",
            new {id = (long) user.Id, country = countryData.Id, leader = makeOwner});
        if (makeOwner)
        {
            await FollowupAsync($"User: {user.Username} is now the owner of {countryName}!");
        }
        else
        {
            await FollowupAsync($"User: {user.Username} is now a member of {countryName}!");
        }
    }
}