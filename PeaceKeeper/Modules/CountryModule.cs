using Dapper;
using Discord;
using Discord.Interactions;
using PeaceKeeper.Database;
using PeaceKeeper.Database.Models;
using PeaceKeeper.Services;

namespace PeaceKeeper.Modules;

[Group("country", "country actions for prop-punk")]
public sealed class CountryModule: PeacekeeperInteractionModule
{


    public CountryModule(UserService user, PermissionsService perms, SettingsService settings)
        :base(user, perms, settings)
    {
    }

    [UserCommand("Get Country")]
    public async Task GetCountry(IUser user)
    {
        await DeferAsync();
    }
}
