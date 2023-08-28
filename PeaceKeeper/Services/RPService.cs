using PeaceKeeper.Database;

namespace PeaceKeeper.Services;

public sealed class RPService : PeacekeeperServiceBase
{
    private readonly Dictionary<long, (RpMode, string?)> _rpModeData = new();

    public RPService(SettingsService settings, UserService users, DbService db) : base(settings, users, db)
    {
    }


    public void SetRpMode(long userId, RpMode mode, string? character)
    {
        _rpModeData[userId] = (mode, character);
    }

    public (RpMode, string?) GetRpMode (long userId)
    {
        return !_rpModeData.TryGetValue(userId, out var data) ? (RpMode.OOC, null) : data;
    }
}
