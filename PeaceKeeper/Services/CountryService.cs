using Discord;
using PeaceKeeper.Database;

namespace PeaceKeeper.Services;

public sealed class CountryService
{
    private readonly DbService _db;
    private readonly SettingsService _settings;

    public CountryService(DbService db, SettingsService settings)
    {
        _db = db;
        _settings = settings;
    }


}
