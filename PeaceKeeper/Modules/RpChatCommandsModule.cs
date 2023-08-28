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

    [Command("rp")]
    public async Task RpChat(string msg)
    {
        await Context.Message.DeleteAsync();
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
            mode = RpMode.GeneralCharacter;
        }
        if (mode == RpMode.Company && user is {Company: null})
        {
            mode = RpMode.GeneralCharacter;
        }

        string author = "Out Of Character";
        switch (mode)
        {
            case RpMode.OOC:
                break;
            case RpMode.GeneralCharacter:
                if (characterName == null)
                {
                    author = "Civilian";
                    break;
                }
                author = $"Civilian ({characterName})";
                break;
            case RpMode.Country:
                if (characterName == null)
                {
                    author = $"{user!.Country}";
                    break;
                }
                author = $"{user!.Country} ({characterName})";
                break;
            case RpMode.Company:
                if (characterName == null)
                {
                    author = $"{user!.Company}";
                    break;
                }
                author = $"{user!.Company} ({characterName})";
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        await ReplyAsync($"## {author}:");
        await ReplyAsync($"> {msg}");
    }

    public RpChatCommandsModule(UserService user, PermissionsService perms, SettingsService settings, InteractionService interaction, DiscordSocketClient client, RPService rpService) : base(user, perms, settings, interaction, client)
    {
        _rpService = rpService;
    }
}
