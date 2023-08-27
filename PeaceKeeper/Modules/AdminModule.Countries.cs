using Dapper;
using Discord;
using Discord.Interactions;
using PeaceKeeper.Database;
using PeaceKeeper.Database.Models;

namespace PeaceKeeper.Modules;

public partial class AdminModule
{
    [SlashCommand("createcountry", "create a new country")]
    public async Task CreateCountry(string countryName, string shortName)
    {
        await DeferAsync();
    }

    [SlashCommand("assigntocountry", "assign a country to a user")]
    public async Task AssignToCountry(IUser user,string countryName, bool makeOwner, bool reassign = true)
    {
        await DeferAsync();
    }
}
