using PeaceKeeper.Database;

namespace PeaceKeeper.Services;

public sealed class RPService : PeacekeeperServiceBase
{
    public RPService(SettingsService settings, UserService users, DbService db) : base(settings, users, db)
    {
    }
}
