
using System.Reflection;
using Dapper;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using PeaceKeeper.Database;

namespace PeaceKeeper;

public sealed class PeaceKeeperBot
{
    public const ulong TestGuildId = 895070870870040618;
    public CommandHandler Commands { get; init; }
    public DiscordSocketClient Client { get; init; }
    public InteractionService InteractionService { get; init; }

    private PeaceKeeperBot()
    {
        Client = new DiscordSocketClient();
        Commands = 
            new CommandHandler(Client, new CommandService());
        InteractionService = new InteractionService(Client.Rest);
    }

    public static async Task<PeaceKeeperBot> Create()
    {
        var inst = new PeaceKeeperBot();
        await inst.Initialize();
        return inst;
    }

    private async Task Initialize()
    {
        Client.Ready += OnClientReady;
        Client.InteractionCreated += OnInteractionCreated;
        await Commands.InstallCommandsAsync();
        await InteractionService.AddModulesAsync(Assembly.GetEntryAssembly(), null);
    }

    private async Task OnClientReady()
    {
#if DEBUG
        await InteractionService.RegisterCommandsToGuildAsync(TestGuildId);
#else
        await InteractionService.RegisterCommandsGloballyAsync();
#endif
    }

    private async Task OnInteractionCreated(SocketInteraction interaction)
    {
        var ctx = new SocketInteractionContext(Client, interaction);
        await InteractionService.ExecuteCommandAsync(ctx, null);
    }
}