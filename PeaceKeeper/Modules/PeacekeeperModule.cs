using Discord.Interactions;
using PeaceKeeper.Database;
using PeaceKeeper.Database.Models;
using PeaceKeeper.Services;

namespace PeaceKeeper.Modules;

public abstract class PeacekeeperInteractionModule : InteractionModuleBase
{
    protected readonly UserService User;
    protected readonly PermissionsService Perms;
    protected readonly SettingsService Settings;

    public PeacekeeperInteractionModule(UserService user, PermissionsService perms, SettingsService settings)
    {
        User = user;
        Perms = perms;
        Settings = settings;
    }

    protected async Task<bool> CheckPermissions(GlobalPermissionLevel permission)
    {
        var caller = Context.User;
        if (caller == null)
        {
            await FollowupAsync($"Cannot run this command without a user");
            return false;
        }
        if (!await Perms.UserHasPermission((long) caller.Id, permission)) return true;
        await FollowupAsync($"You do not have the permissions to run this command");
        return false;
    }
}
