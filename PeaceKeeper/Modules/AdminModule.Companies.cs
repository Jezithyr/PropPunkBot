using Dapper;
using Discord;
using Discord.Interactions;
using PeaceKeeper.Database;

namespace PeaceKeeper.Modules;

public partial class AdminModule
{
    [SlashCommand("createcompany", "create a new un-owned company")]
    public async Task CreateCompany(string companyName, string shortName)
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
        await using var connection = await _db.Get();
        //TODO: check if user is on the mod-list
        var country = await connection.QuerySingleOrDefaultAsync<Company>("SELECT * FROM companies WHERE name = @name LIMIT 1",
            new {name = companyName});
        if (country == null)
        {
            await connection.QuerySingleAsync<long>(
                "INSERT INTO companies (name, shortname) VALUES (@name,@shortname) ON CONFLICT DO NOTHING RETURNING -1",
                new {name = companyName, shortname = shortName});
            await FollowupAsync($"Registered new company: {companyName}");
        }
        else
        {
            await FollowupAsync($"Company with name: {companyName} already exists!");
        }
        
    }

    [SlashCommand("assigntocompany", "assign a user to a company")]
    public async Task AssignToCompany(IUser user,string companyName, bool makeOwner, bool reassign = true)
    {
        await DeferAsync();
        if (companyName.Length > 128)
        {
            await FollowupAsync("Company name is too long, must be less than 128 characters!");
            return;
        }

        await using var connection = await _db.Get();
        //TODO: check if user is on the mod-list
        var userData = await connection.QuerySingleOrDefaultAsync<UserRaw>(
            "SELECT * FROM users WHERE id = @id LIMIT 1",
            new {id = (long)user.Id});
        
        if (userData == null)
        {
            await FollowupAsync($"User with name: {user.Username} is not registered!");
            return;
        }
        
        var companyData = await connection.QuerySingleOrDefaultAsync<Company>(
            "SELECT * FROM companies WHERE name = @name LIMIT 1",
            new {name = companyName});
        
        if (!reassign && userData.Company.HasValue)
        {
            await FollowupAsync($"User with name: {user.Username} is already a member in a company!");
            return;
        }
        
        if (companyData == null)
        {
            await FollowupAsync($"Company with name: {companyName} does not exist!");
            return;
        }
        
        if (makeOwner)
        {
            await connection.QueryAsync(
                "UPDATE users SET ceo = false WHERE company = @id",
                new {id = companyData.Id});
        }
            
        await connection.QueryAsync(
            "UPDATE users SET company = @company, ceo = @ceo WHERE id = @id",
            new {id = (long)user.Id, company = companyData.Id, ceo = makeOwner});
        if (makeOwner)
        {
            await FollowupAsync($"User: {user.Username} is now the owner of {companyName}!");
        }
        else
        {
            await FollowupAsync($"User: {user.Username} is now a member of {companyName}!");
        }
        
    }
}
