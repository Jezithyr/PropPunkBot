using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using PeaceKeeper.Services;

namespace PeaceKeeper.Modules;

[Group("user", "user related commands")]
public sealed class UserModule : PeacekeeperInteractionModule
{

    [UserCommand("Get Info")]
    public async Task GetInfo(IUser user)
    {
        await DeferAsync();

        var userData = await User.Get(user);
        if (userData == null)
        {
            await RespondAsync($"User {user.Username} is not registered!");
            return;
        }

        var embed = new EmbedBuilder();
        embed.WithAuthor(user.GlobalName, user.GetAvatarUrl());
        var country = $"Country ({(userData.Leader ? "Leader" : "Member")})";
        var company = $"Company ({(userData.Ceo ? "CEO" : "Member")})";
        embed.AddField(country, userData.Country?.Name ?? "None", true);
        embed.AddField(company, userData.Company?.Name ?? "None", true);

        await FollowupAsync(null, new[] {embed.Build()}, ephemeral: true);
    }
    
    [SlashCommand("register", "register for the prop-punk universe")]
    public async Task Register()
    {
        await DeferAsync();
        if (await User.Add((long)Context.User.Id))
        {
            await FollowupAsync("Registered you for the prop-punk universe!");
        }
        else
        {
            await FollowupAsync("You are already registered!");
        }
    }
    [SlashCommand("unregister", "unregister from the prop-punk universe")]
    public async Task UnRegister()
    {
        await DeferAsync();
        if (await User.Remove((long)Context.User.Id))
        {
            await FollowupAsync("Removed you from the prop-punk universe!");
        }
        else
        {
            await FollowupAsync("You are not registered already!");
        }
    }

    public UserModule(UserService user, PermissionsService perms, SettingsService settings, InteractionService interaction, DiscordSocketClient client) : base(user, perms, settings, interaction, client)
    {
    }
}
