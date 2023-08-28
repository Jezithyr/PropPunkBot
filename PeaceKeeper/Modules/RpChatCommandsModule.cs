using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using PeaceKeeper.Database;
using PeaceKeeper.Database.Models;
using PeaceKeeper.Services;

namespace PeaceKeeper.Modules;

public sealed class RpChatCommandsModule : PeacekeeperCommandModule
{
    private readonly RPService _rpService;

    [Command("ooc")]
    public async Task OOCChat([Remainder] string msg)
    {
        var caller = Context.User;
        if (caller == null || ! await Perms.UserHasPermission((long) caller.Id, GlobalPermissionLevel.CanRp))
        {
            await ReplyAsync($"You are not registered for prop-punk!", flags: MessageFlags.Ephemeral);
            return; //do nothing if the user doesn't have rp permissions
        }
        await Context.Message.DeleteAsync();
        var user = await User.Get(caller);
        string author = $"Out Of Character   ({caller.Username.Capitalize()})";
        EmbedBuilder embed = new();
        embed.WithAuthor(author.Capitalize(), user!.Country!.FlagUrl);
        embed.WithDescription(msg);
        await ReplyAsync(embed:embed.Build());
    }


    [Command("rp")]
    public async Task RpChat([Remainder]string msg)
    {

        var caller = Context.User;
        if (caller == null || ! await Perms.UserHasPermission((long) caller.Id, GlobalPermissionLevel.CanRp))
        {
            await ReplyAsync($"You are not registered for prop-punk!", flags: MessageFlags.Ephemeral);
            return; //do nothing if the user doesn't have rp permissions
        }
        var (mode, characterName) = await _rpService.GetRpMode((long) caller.Id);
        var user = await User.Get(caller);
        if (user == null && mode != RpMode.OOC)
            mode = RpMode.GeneralCharacter;
        if (mode == RpMode.Country && user is {Country: null})
        {
            mode = RpMode.OOC;
        }
        if (mode == RpMode.Company && user is {Company: null})
        {
            mode = RpMode.OOC;
        }

        string author = $"Out Of Character   ({caller.Username.Capitalize()})";
        switch (mode)
        {
            case RpMode.OOC:
                break;
            case RpMode.GeneralCharacter:
                if (characterName == null)
                {
                    author = $"Civilian   ({caller.Username.Capitalize()})";
                    break;
                }
                author = $"Civilian:   {characterName}   ({caller.Username.Capitalize()})";
                break;
            case RpMode.Country:
                if (characterName == null)
                {
                    author = $"{user!.Country!.Name} [{user.Country!.ShortName}]" +
                             $"   ({caller.Username.Capitalize()})";
                    break;
                }
                author = $"{user!.Country!.Name} [{user.Country!.ShortName}]:   {characterName}" +
                         $"   ({caller.Username.Capitalize()})";
                break;
            case RpMode.Company:
                if (characterName == null)
                {
                    author = $"{user!.Company} [{user.Company!.ShortName}]" +
                             $"   ({caller.Username.Capitalize()})";
                    break;
                }
                author = $"{user!.Company!.Name} [{user.Company!.ShortName}]:   {characterName}" +
                         $"   ({caller.Username.Capitalize()})";
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        List<Embed> messageEmbeds = new(Context.Message.Embeds);
        EmbedBuilder embed = new();
        embed.WithAuthor(author.Capitalize(), user!.Country!.FlagUrl);
        embed.WithDescription(msg.Capitalize());
        await ReplyAsync(embed: embed.Build(),
            embeds: messageEmbeds.ToArray()
            );
        await Context.Message.DeleteAsync();
    }

    public RpChatCommandsModule(UserService user, PermissionsService perms, SettingsService settings, InteractionService interaction, DiscordSocketClient client, RPService rpService) : base(user, perms, settings, interaction, client)
    {
        _rpService = rpService;
    }
}
