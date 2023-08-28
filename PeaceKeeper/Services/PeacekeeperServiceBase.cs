using PeaceKeeper.Database;

namespace PeaceKeeper.Services;

public abstract class PeacekeeperServiceBase
{
    protected readonly DbService Db;
    protected readonly SettingsService Settings;
    protected readonly UserService Users;
    public PeacekeeperServiceBase(SettingsService settings, UserService users, DbService db)
    {
        Settings = settings;
        Users = users;
        Db = db;
    }

    public async virtual Task OnClientReady()
    {
    }
}
