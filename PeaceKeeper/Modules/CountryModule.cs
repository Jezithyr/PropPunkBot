using Dapper;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using PeaceKeeper.Database;
using PeaceKeeper.Database.Models;
using PeaceKeeper.Services;

namespace PeaceKeeper.Modules;

[Group("country", "country actions for prop-punk")]
public sealed class CountryModule: PeacekeeperInteractionModule
{



    [UserCommand("Get Country")]
    public async Task GetCountry(IUser user)
    {
        await DeferAsync();
    }


    public CountryModule(UserService user, PermissionsService perms, SettingsService settings, InteractionService interaction, DiscordSocketClient client) : base(user, perms, settings, interaction, client)
    {
    }
}
