using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using PeaceKeeper.Services;

namespace PeaceKeeper.Modules;

[Group("company", "company actions for prop-punk")]
public sealed class CompanyModule : PeacekeeperInteractionModule
{

    [UserCommand("Get Company")]
    public async Task GetCompany(IUser user)
    {
        await DeferAsync();
    }

    public CompanyModule(UserService user, PermissionsService perms, SettingsService settings, InteractionService interaction, DiscordSocketClient client) : base(user, perms, settings, interaction, client)
    {
    }
}
