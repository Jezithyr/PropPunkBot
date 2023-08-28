using Discord.WebSocket;

namespace PeaceKeeper.Services;

public abstract class PeacekeeperCoreServiceBase
{
    protected DiscordSocketClient Client;

    public PeacekeeperCoreServiceBase(DiscordSocketClient client)
    {
        Client = client;
        client.Ready += OnClientReady;
    }
    public async virtual Task OnClientReady()
    {
    }
}
