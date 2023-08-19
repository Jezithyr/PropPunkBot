using Dapper;
using Discord;
using Discord.Interactions;
using PeaceKeeper.Database;

namespace PeaceKeeper.Modules;

[Group("company", "company actions for prop-punk")]
public sealed class CompanyModule: InteractionModuleBase
{
    [UserCommand("Get Company")]
    public async Task GetCompany(IUser user)
    {
        await DeferAsync();
        await using var connection = DatabaseConnection.Get();

        var userdata = await connection.QuerySingleOrDefaultAsync<User>("SELECT * FROM users WHERE id = @id LIMIT 1",
            new {id = (long) user.Id});
        if (userdata != null)
        {
            if (userdata.Company.HasValue)
            {
                var companyData = await connection.QuerySingleOrDefaultAsync<Company>("SELECT * FROM companies WHERE name = @id LIMIT 1",
                    new {id = userdata.Company});
                if (userdata.Ceo)
                {
                    await FollowupAsync($"{user.Username} is the CEO of {companyData.Name} [{companyData.ShortName}]");
                }
                else
                {
                    await FollowupAsync($"{user.Username} is a member of {companyData.Name} [{companyData.ShortName}]");
                    
                }
                return;
            }
            await FollowupAsync($"{user.Username} is not a member of any company!");
            return;
        }
        await FollowupAsync($"User {user.Username} was not found!");
    }
}