using Dapper;
using PeaceKeeper.Database;

namespace PeaceKeeper.Services;

public sealed class SettingsService
{
    private readonly DbService _db;

    public SettingsService(DbService db)
    {
        _db = db;
    }

    public async Task<Settings?> GetSettings(ulong guild)
    {
        await using var db = await _db.Get();
        return await db.QuerySingleOrDefaultAsync<Settings>("SELECT * FROM settings WHERE guild = @guild", new { guild = (long) guild });
    }
}
