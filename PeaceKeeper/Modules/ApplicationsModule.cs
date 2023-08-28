using Discord.Interactions;
using Discord.WebSocket;
using PeaceKeeper.Services;

namespace PeaceKeeper.Modules;


[Group("apply", "create an application for the prop-punk universe")]
public sealed class ApplicationsModule : PeacekeeperInteractionModule
{
    public ApplicationsModule(UserService user, PermissionsService perms, SettingsService settings, InteractionService interaction, DiscordSocketClient client) : base(user, perms, settings, interaction, client)
    {
    }
}
