using Discord;
using Discord.Interactions;

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
        if (await _company.CreateCompany(companyName, shortName))
        {
            await FollowupAsync($"Registered new company: {companyName}");
            return;
        }
        await FollowupAsync($"Could not create company! {companyName} may already exist!");
    }

    [SlashCommand("removecompany", "create a new un-owned company")]
    public async Task RemoveCompany(string companyName)
    {
        await DeferAsync();
        if (companyName.Length > 128)
        {
            await FollowupAsync("Company name is too long, must be less than 128 characters!");
            return;
        }
        if (await _company.RemoveCompany(companyName))
        {
            await FollowupAsync($"Registered new company: {companyName}");
            return;
        }
        await FollowupAsync($"Could not create company! {companyName} may already exist!");
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

        if (!await User.Exists((long)user.Id))
        {
            await FollowupAsync($"User with name: {user.Username} is not registered!");
            return;
        }


        var company = await _company.GetCompany(companyName);
        if (company == null)
        {
            await FollowupAsync($"Company with name: {companyName} does not exist!");
            return;
        }
        await _company.AssignUser((long) user.Id, company);
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
