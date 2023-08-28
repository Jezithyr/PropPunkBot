using Discord;
using PeaceKeeper.Services;
using Discord.Interactions;
using Discord.Interactions.Builders;
using Discord.WebSocket;
using PeaceKeeper.Database;
using PeaceKeeper.Database.Models;
using PeaceKeeper.Modals;

namespace PeaceKeeper.Modules;


[Group("rp", "roleplaying actions for prop-punk")]
public partial class RpModule : PeacekeeperInteractionModule
{
    private readonly WorldStateService _worldstate;
    private readonly RPService _rpService;
    private readonly CountryService _country;
    private readonly CompanyService _company;
    private readonly DbService _db;

    public RpModule(UserService user, PermissionsService perms, SettingsService settings, InteractionService interaction,
        DiscordSocketClient client, WorldStateService worldstate, RPService rpService, CountryService country, CompanyService company, DbService db) :
        base(user, perms, settings, interaction, client)
    {
        _worldstate = worldstate;
        _rpService = rpService;
        _country = country;
        _company = company;
        _db = db;
        client.ModalSubmitted += ClientOnModalSubmitted;
    }
    private async Task ClientOnModalSubmitted(SocketModal arg)
    {
        await OnNewsModalSubmitted(arg);
    }

    [SlashCommand("mode", "Switch between different RP modes and change character name (optional)")]
    public async Task SwitchRpMode(RpMode rpMode, string? characterName = null)
    {
        await DeferAsync();
        var caller = Context.User;
        if (! await CheckPermissions(GlobalPermissionLevel.CanRp))
            return;
        _rpService.SetRpMode((long)caller.Id, rpMode, characterName);
    }
    [MessageCommand("rp")]
    public async Task RpChat(IMessage msg)
    {
        var caller = msg.Author;
        if (caller == null || await Perms.UserHasPermission((long) caller.Id, GlobalPermissionLevel.CanRp))
        {
            await RespondAsync();
            return; //do nothing if the user doesn't have rp permissions
        }
        var (mode, characterName) = _rpService.GetRpMode((long) caller.Id);
        var user = await User.Get(caller);
        var embed = new EmbedBuilder();
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

        switch (mode)
        {
            case RpMode.OOC:
                embed.Author.Name = "Out Of Character";
                break;
            case RpMode.GeneralCharacter:
                if (characterName == null)
                {
                    embed.Author.Name = "Civilian";
                    break;
                }
                embed.Author.Name = $"Civilian ({characterName})";
                break;
            case RpMode.Country:
                if (characterName == null)
                {
                    embed.Author.Name = $"{user!.Country}";
                    break;
                }
                embed.Author.Name = $"{user!.Country} ({characterName})";
                break;
            case RpMode.Company:
                if (characterName == null)
                {
                    embed.Author.Name = $"{user!.Company}";
                    break;
                }
                embed.Author.Name = $"{user!.Company} ({characterName})";
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        embed.AddField("", msg.Content);
    }


}
