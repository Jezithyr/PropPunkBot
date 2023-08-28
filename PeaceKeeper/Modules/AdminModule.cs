using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using PeaceKeeper.Database.Models;
using PeaceKeeper.Services;

namespace PeaceKeeper.Modules;

[Group("admin", "moderation actions for prop-punk")]
public partial class AdminModule : PeacekeeperInteractionModule
{
    private readonly CountryService _country;
    private readonly CompanyService _company;
    private readonly TechService _tech;

    [SlashCommand("removeuser", "remove a user from the prop-punk universe")]
    public async Task Remove(IUser user)
    {
        await DeferAsync();
        var caller = Context.User;
        if (caller == null)
        {
            await FollowupAsync($"Cannot run this command without a user");
            return;
        }
        if (await Perms.UserHasPermission((long) caller.Id, GlobalPermissionLevel.UserManagement))
        {
            await FollowupAsync($"You do not have the permissions to run this command");
            return;
        }

        if (await User.Remove((long) user.Id))
        {
            await FollowupAsync($"Removed user: {user.Username}");
            return;
        }
        await FollowupAsync($"User {user.Username} was not found!");
    }

    [SlashCommand("adduser", "remove a user from the prop-punk universe")]
    public async Task Add(IUser user)
    {
        await DeferAsync();
        var caller = Context.User;
        if (caller == null)
        {
            await FollowupAsync($"Cannot run this command without a user");
            return;
        }

        if (await Perms.UserHasPermission((long) user.Id, GlobalPermissionLevel.UserManagement))
        {
            await FollowupAsync($"You do not have the permissions to run this command");
            return;
        }

        if (await User.Add((long) user.Id))
        {
            await FollowupAsync($"Added user: {user.Username}");
            return;
        }
        await FollowupAsync($"User {user.Username} could not be added! They may be already present!");
    }

    [SlashCommand("trustuser", "make a user trusted, this allows them to use additional commands")]
    public async Task TrustUser(IUser user)
    {
        await DeferAsync();
        var caller = Context.User;
        if (caller == null)
        {
            await FollowupAsync($"Cannot run this command without a user");
            return;
        }
        if (await Perms.UserHasPermission((long) caller.Id, GlobalPermissionLevel.MakeTrusted))
        {
            await FollowupAsync($"You do not have the permissions to run this command");
            return;
        }
        await Perms.SetPermissionsForUser((long) user.Id, GlobalPermissionLevel.TrustedUser);
        await FollowupAsync($"{user.Username} is now trusted!");
    }

    [SlashCommand("untrustuser", "remove a user from the trusted list, sets them as a regular user")]
    public async Task UnTrustUser(IUser user)
    {
        await DeferAsync();
        var caller = Context.User;
        if (caller == null)
        {
            await FollowupAsync($"Cannot run this command without a user");
            return;
        }
        if (await Perms.UserHasPermission((long) caller.Id, GlobalPermissionLevel.RemoveTrusted))
        {
            await FollowupAsync($"You do not have the permissions to run this command");
            return;
        }
        await Perms.SetPermissionsForUser((long) user.Id, GlobalPermissionLevel.User);
        await FollowupAsync($"{user.Username} is now trusted!");
    }

    public AdminModule(UserService user, PermissionsService perms, SettingsService settings, InteractionService interaction, DiscordSocketClient client, CountryService country, CompanyService company, TechService tech) : base(user, perms, settings, interaction, client)
    {
        _country = country;
        _company = company;
        _tech = tech;
    }
}
