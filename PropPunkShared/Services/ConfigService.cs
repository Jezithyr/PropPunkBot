using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PropPunkShared.Core;
using PropPunkShared.Database;
using PropPunkShared.Database.Models;

namespace PropPunkShared.Services;

public sealed class ConfigService : ServiceBase
{
    private ConfigModel? _config;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ConfigService(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }


    public ConfigModel Config
    {
        get
        {
            if (_config != null) return _config;
            throw new ArgumentException($"No Config found for {GetAppEnvironment()}!");
        }
    }

    public AppEnvironment GetAppEnvironment()
    {
        Env.EnsureLoadEnvFile();
        var currentEnv = Env.Get("ENVIRONMENT");
        switch (currentEnv)
        {
            case "DEVELOPMENT":
            {
                return AppEnvironment.Development;
            }
            case "PRODUCTION":
            {
                return AppEnvironment.Production;
            }
            default:
            {
                throw new ArgumentException("Environment variable for ENVIRONMENT was not set properly, " +
                                            "this must be set to DEVELOPMENT or PRODUCTION!");
            }
        }
    }
    public async Task UpdateCachedSettings()
    {
        var appEnv = GetAppEnvironment();
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var db = scope.ServiceProvider.GetService<DatabaseContext>();
            _config = await db!.Configs.FirstOrDefaultAsync(c => c.Environment == appEnv)
                      ?? throw new ArgumentException($"No Config found for {appEnv}!");
        }


    }

}
public sealed class ConfigSyncService : BackgroundService
{
    private readonly ConfigService _config;

    public ConfigSyncService(ConfigService config)
    {
        _config = config;
    }

    protected override async Task ExecuteAsync(CancellationToken stopToken)
    {
        while (!stopToken.IsCancellationRequested)
        {
            await _config.UpdateCachedSettings();
            await Task.Delay(TimeSpan.FromMinutes(10), stopToken);
        }
    }
}
