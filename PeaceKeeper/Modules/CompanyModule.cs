using Discord;
using Discord.Interactions;
using PeaceKeeper.Services;

namespace PeaceKeeper.Modules;

[Group("company", "company actions for prop-punk")]
public sealed class CompanyModule : PeacekeeperInteractionModule
{
    public CompanyModule(UserService user, PermissionsService perms, SettingsService settings) :
        base(user, perms, settings)
    {
    }

    [UserCommand("Get Company")]
    public async Task GetCompany(IUser user)
    {
        await DeferAsync();
    }
}
