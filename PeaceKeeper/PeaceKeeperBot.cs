
using System.ComponentModel.Design;
using System.Reflection;
using Dapper;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using PeaceKeeper.Database;
using PeaceKeeper.Services;

namespace PeaceKeeper;

public sealed class PeaceKeeperBot
{
    public const ulong TestGuildId = 895070870870040618;
    public CommandHandler Commands { get; init; }
    public DiscordSocketClient Client { get; init; }
    public InteractionService InteractionService { get; init; }
    private readonly IServiceProvider _services;

    private PeaceKeeperBot()
    {
        var services = new ServiceCollection();
        services.AddSingleton<SettingsService>();
        services.AddSingleton<DiscordSocketClient>();
        services.AddSingleton<DbService>();
        _services = services.BuildServiceProvider();

        Client = _services.GetRequiredService<DiscordSocketClient>();

        Commands = 
            new CommandHandler(Client, new CommandService(), _services);
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
        Client.UserCommandExecuted += command =>
        {
            return Task.CompletedTask;
        };
        await Commands.InstallCommandsAsync();
        await InteractionService.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
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
        try
        {
            var ctx = new SocketInteractionContext(Client, interaction);
            await InteractionService.ExecuteCommandAsync(ctx, _services);
        }
        catch (Exception e)
        {
            Console.WriteLine($"[Exception] {e}");
            throw;
        }
    }
}