
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
    public static readonly char CommandPrefix = '$';

    public const ulong TestGuildId = 895070870870040618;
    public CommandHandlerService Commands { get; init; }
    public DiscordSocketClient Client { get; init; }
    public InteractionService InteractionService { get; init; }
    private readonly IServiceProvider _services;
    private PeaceKeeperBot()
    {
        var services = new ServiceCollection();
        var socketConfig = new DiscordSocketConfig()
        {
            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
                                                            | GatewayIntents.GuildMembers
        };
        var discordClient = new DiscordSocketClient(socketConfig);
        InteractionService = new InteractionService(discordClient.Rest);
        services.AddSingleton(discordClient);
        services.AddSingleton<CommandService>();
        services.AddSingleton<CommandHandlerService>();
        services.AddSingleton(InteractionService);
        AutoRegisterServices(ref services, typeof(PeacekeeperCoreServiceBase));
        AutoRegisterServices(ref services, typeof(PeacekeeperServiceBase));
        _services = services.BuildServiceProvider();
        Client = _services.GetRequiredService<DiscordSocketClient>();
        Commands = _services.GetRequiredService<CommandHandlerService>();
        Commands.Initialize(_services);
    }

    public static async Task<PeaceKeeperBot> Create()
    {
        var inst = new PeaceKeeperBot();
        await inst.Initialize();
        return inst;
    }

    private void AutoRegisterServices(ref ServiceCollection services, Type serviceBaseClass)
    {
        foreach (var type in typeof(PeaceKeeperBot).Assembly.GetTypes())
        {
            if (!type.IsAssignableTo(serviceBaseClass) || type.IsAbstract) continue;
            services.AddSingleton(type);
        }
    }

    private async Task Initialize()
    {
        Client.Ready += OnClientReady;
        Client.InteractionCreated += OnInteractionCreated;
        Client.UserCommandExecuted += command =>
        {
            return Task.CompletedTask;
        };
        await InteractionService.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        await _services.GetRequiredService<CommandService>().AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        await Commands.InstallCommandsAsync();
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
