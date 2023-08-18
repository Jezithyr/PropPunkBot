using Discord.WebSocket;

namespace PeaceKeeper;

public static class Globals
{
    private static DiscordSocketClient? _client;

    public static DiscordSocketClient CreateClient()
    {
        return _client ??= new DiscordSocketClient();
    }
    
    public static DiscordSocketClient Client
    {
        get
        {
            if (_client is null)
            {
                throw new ApplicationException("Client has not initialized yet!");
            }
            return _client;
        }
    }
}