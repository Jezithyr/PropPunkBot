using Discord.WebSocket;

namespace PeaceKeeper.Services;

public abstract class PeacekeeperServiceBase
{
    protected readonly DiscordSocketClient Client;

    public PeacekeeperServiceBase(DiscordSocketClient client)
    {
        Client = client;
    }

    public async virtual Task OnClientReady()
    {
    }
}
