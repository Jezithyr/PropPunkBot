using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using PeaceKeeper.Database;
using PeaceKeeper.Database.Models;
using PeaceKeeper.Services;

namespace PeaceKeeper.Modules;

public abstract class PeacekeeperInteractionModule : InteractionModuleBase
{
    protected readonly DiscordSocketClient Client;
    protected readonly UserService User;
    protected readonly PermissionsService Perms;
    protected readonly SettingsService Settings;
    protected readonly InteractionService Interaction;

    public PeacekeeperInteractionModule(UserService user, PermissionsService perms, SettingsService settings,
        InteractionService interaction, DiscordSocketClient client)
    {
        User = user;
        Perms = perms;
        Settings = settings;
        Interaction = interaction;
        Client = client;
    }

    protected async Task<bool> CheckPermissions(GlobalPermissionLevel permission)
    {
        var caller = Context.User;
        if (caller == null)
        {
            await FollowupAsync($"Cannot run this command without a user", ephemeral:true);
            return false;
        }
        if (!await Perms.UserHasPermission((long) caller.Id, permission)) return true;
        await FollowupAsync($"You do not have the permissions to run this command", ephemeral:true);
        return false;
    }
}
