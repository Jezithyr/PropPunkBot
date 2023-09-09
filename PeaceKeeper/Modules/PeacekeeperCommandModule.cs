using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using PeaceKeeper.Services;
using PropPunkShared.Data.Models;

namespace PeaceKeeper.Modules;

public abstract class PeacekeeperCommandModule : ModuleBase<SocketCommandContext>
{
    protected readonly DiscordSocketClient Client;
    protected readonly UserService User;
    protected readonly PermissionsService Perms;
    protected readonly SettingsService Settings;
    protected readonly InteractionService Interaction;

    public PeacekeeperCommandModule(UserService user, PermissionsService perms, SettingsService settings,
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
            return false;
        }
        return await Perms.UserHasPermission((long) caller.Id, permission);
    }
}
