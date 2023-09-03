using Discord;
using Discord.Interactions;
namespace PeaceKeeper.Modules;

public partial class AdminModule
{
    [SlashCommand("createcountry", "create a new country")]
    public async Task CreateCountry(string countryName, string shortName)
    {
        await DeferAsync();
        if (countryName.Length > 128)
        {
            await FollowupAsync("Company name is too long, must be less than 128 characters!");
            return;
        }

        if (shortName.Length > 4)
        {
            await FollowupAsync("Company Tag must be 4 characters or less!");
            return;
        }

        if (await _country.CreateCountry(countryName, shortName))
        {
            await FollowupAsync($"Registered new country: {countryName}");
            return;
        }
        await FollowupAsync($"Could not create country! {countryName} may already exist!");
    }

    [SlashCommand("removecountry", "create a new country")]
    public async Task RemoveCountry(string countryName)
    {
        await DeferAsync();
        if (countryName.Length > 128)
        {
            await FollowupAsync("Company name is too long, must be less than 128 characters!");
            return;
        }
        if (await _country.RemoveCountry(countryName))
        {
            await FollowupAsync($"Removed country: {countryName}");
            return;
        }
        await FollowupAsync($"Could not remove country! {countryName} does not exist!");
    }


    [SlashCommand("assigntocountry", "assign a country to a user")]
    public async Task AssignToCountry(IUser user,string countryName, bool makeOwner, bool reassign = true)
    {
        await DeferAsync();
        if (countryName.Length > 128)
        {
            await FollowupAsync("Company name is too long, must be less than 128 characters!");
            return;
        }

        if (!await User.Exists((long)user.Id))
        {
            await FollowupAsync($"User with name: {user.Username} is not registered!");
            return;
        }


        var country = await _country.GetCountry(countryName);
        if (country == null)
        {
            await FollowupAsync($"Company with name: {countryName} does not exist!");
            return;
        }
        await _country.AssignUser((long) user.Id, country);
        if (makeOwner)
        {
            await FollowupAsync($"User: {user.Username} is now the leader of {countryName}!");
        }
        else
        {
            await FollowupAsync($"User: {user.Username} is now a member of {countryName}!");
        }

    }
}
