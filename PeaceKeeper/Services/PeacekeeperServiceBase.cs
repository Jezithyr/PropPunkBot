using Discord.WebSocket;
using PeaceKeeper.Database;

namespace PeaceKeeper.Services;

public abstract class PeacekeeperServiceBase
{
    protected readonly DiscordSocketClient Client;
    protected readonly DbService Db;
    protected readonly SettingsService Settings;
    protected readonly UserService Users;
    protected readonly PermissionsService Perms;
    protected readonly WorldStateService WorldState;
    public PeacekeeperServiceBase(SettingsService settings, PermissionsService perms,UserService users, DbService db, WorldStateService worldState, DiscordSocketClient client)
    {
        Settings = settings;
        Perms = perms;
        Users = users;
        Db = db;
        WorldState = worldState;
        Client = client;
    }

    public async virtual Task OnClientReady()
    {
    }
}
