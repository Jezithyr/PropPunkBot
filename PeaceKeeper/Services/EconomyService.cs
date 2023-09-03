using Discord.WebSocket;
using PeaceKeeper.Database;

namespace PeaceKeeper.Services;

public partial class EconomyService : PeacekeeperServiceBase
{
    private readonly CountryStatsService _countryStats;
    private readonly CountryService _country;
    private readonly ResearchService _research;
    private readonly CompanyService _company;

    public EconomyService(SettingsService settings, PermissionsService perms, UserService users, DbService db, WorldStateService worldState, DiscordSocketClient client, CountryStatsService countryStats, CountryService country, ResearchService research, CompanyService company) : base(settings, perms, users, db, worldState, client)
    {
        _countryStats = countryStats;
        _country = country;
        _research = research;
        _company = company;
        WorldState.RegisterTickEvent(OnWorldTick);
    }

    private void OnWorldTick(int year, int quarter, DateOnly date)
    {
        UpdateCountries().RunSynchronously();
        UpdateCompanies().RunSynchronously();
    }

}
