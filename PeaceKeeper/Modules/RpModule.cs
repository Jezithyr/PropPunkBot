using Discord;
using Discord.Commands;
using PeaceKeeper.Services;
using Discord.Interactions;
using Discord.Interactions.Builders;
using Discord.WebSocket;
using PeaceKeeper.Database;
using PeaceKeeper.Database.Models;
using PeaceKeeper.Modals;

namespace PeaceKeeper.Modules;


[Discord.Interactions.Group("rp", "roleplaying actions for prop-punk")]
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
        await DeferAsync(ephemeral:true);
        var caller = Context.User;
        if (! await CheckPermissions(GlobalPermissionLevel.CanRp))
            return;
        if (characterName is {Length: > 128})
        {
            await FollowupAsync("Character name must be less than 128 characters!");
            return;
        }
        if (!await _rpService.SetRpMode((long) caller.Id, rpMode, characterName))
        {
            await FollowupAsync("You are not registered in the prop-punk universe!");
            return;
        }
        await FollowupAsync("Updated Rp Mode");
    }
}
