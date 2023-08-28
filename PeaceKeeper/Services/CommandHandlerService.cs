using System.Reflection;
using Discord.Commands;
using Discord.WebSocket;
using PeaceKeeper.Database;

namespace PeaceKeeper.Services;

public class CommandHandlerService : PeacekeeperServiceBase
{
    private readonly CommandService _commands;
    private IServiceProvider? _services = null;

    public async Task InstallCommandsAsync()
    {
        // Hook the MessageReceived event into our command handler
        Client.MessageReceived += HandleCommandAsync;
    }

    private async Task HandleCommandAsync(SocketMessage messageParam)
    {
        // Don't process the command if it was a system message
        var message = messageParam as SocketUserMessage;
        if (message == null) return;

        // Create a number to track where the prefix ends and the command begins
        int argPos = 0;

        // Determine if the message is a command based on the prefix and make sure no bots trigger commands
        if (!(message.HasCharPrefix(PeaceKeeperBot.CommandPrefix, ref argPos) ||
              message.HasMentionPrefix(Client.CurrentUser, ref argPos)) ||
            message.Author.IsBot)
        {
            return;
        }

        // Create a WebSocket-based command context based on the message
        var context = new SocketCommandContext(Client, message);

        // Execute the command with the command context we just
        // created, along with the service provider for precondition checks.
        var test = await _commands.ExecuteAsync(
            context: context,
            argPos: argPos,
            services: _services);
        Console.WriteLine($"[Exception] {test}");
    }

    public void Initialize(IServiceProvider services)
    {
        _services = services;
    }

    public CommandHandlerService(SettingsService settings, PermissionsService perms, UserService users, DbService db,
        WorldStateService worldState, DiscordSocketClient client, CommandService commands) :
        base(settings, perms, users, db, worldState, client)
    {
        _commands = commands;
    }
}
