using Dapper;
using Discord.WebSocket;
using PeaceKeeper.Database;
using PeaceKeeper.Database.Models;

namespace PeaceKeeper.Services;

public sealed class CountryStatsService : PeacekeeperServiceBase
{
    public async Task<CountryStats?> GetCountryStats(Guid countryId)
    {
        await using var connection = await Db.Get();
        var users = await connection.QueryAsync<CountryStatsRaw, Country, CountryStats>(
            "SELECT * FROM country_stats LEFT JOIN countries ON countries.id = country_stats.id " +
            "WHERE country_stats.id = @id",
            (old, country) => new CountryStats(
                country,
                old.Population,
                old.Happiness,
                old.FertilityMod,
                old.UnEmployment,
                old.EducationIndex,
                old.GdpPerCapMultiplier,
                old.Urbanization
            ), new {id = countryId});
       return users?.SingleOrDefault();
    }

    public async Task<bool> UpdateCountryStats(Guid countryId, int? population, float? happiness, float? fertility,
        float? unemployment, float? educationIndex, float? gdpPerCapMultiplier, float? urbanization)
    {
        await using var connection = await Db.Get();
        var stats = await GetCountryStats(countryId);
        if (stats == null) return false;
        var newStats = new StatData(population, happiness, fertility, unemployment, educationIndex, gdpPerCapMultiplier, urbanization);
        newStats.Population ??= stats.Population;
        newStats.Happiness ??= stats.Happiness;
        newStats.FertilityMod ??= stats.FertilityMod;
        newStats.UnEmployment ??= stats.UnEmployment;
        newStats.EducationIndex ??= stats.EducationIndex;
        newStats.GdpPerCapMultiplier ??= stats.GdpPerCapMultiplier;
        newStats.Urbanization ??= stats.Urbanization;
        await connection.QueryAsync("UPDATE country_stats set population = @pop, happiness = @hap, fertilitymod = @fert," +
                                    "unemployment = @unemp, educationindex = @edi, gdppercapmultiplier = @gdp, " +
                                    "urbanization = @urb where id = @id",
            new
            {
            id = countryId, pop = newStats.Population, hap = newStats.Happiness,
            fert = newStats.FertilityMod, unemp = newStats.UnEmployment, edi = newStats.EducationIndex,
            gdp = newStats.GdpPerCapMultiplier, urb = urbanization
        });
        return true;
    }

    private async Task UpdatePopulation()
    {
        await using var connection = await Db.Get();
        var worldState = await WorldState.Get();
        await connection.QueryAsync("UPDATE country_stats set population = country_stats.population =" +
                                    "country_stats.population + country_stats.population* country_stats.fertilitymod *" +
                                    " @globalfert", new {globalfert = worldState});
    }
    
    public CountryStatsService(SettingsService settings, PermissionsService perms, UserService users, DbService db, WorldStateService worldState, DiscordSocketClient client) : base(settings, perms, users, db, worldState, client)
    {
        WorldState.RegisterTickEvent(OnWorldTick);
    }

    private void OnWorldTick(int year, int quarter, DateOnly date)
    {
        UpdatePopulation().RunSynchronously();
    }

    private record struct StatData(int? Population, float? Happiness, float? FertilityMod,
        float? UnEmployment, float? EducationIndex, float? GdpPerCapMultiplier, float? Urbanization);
    
}
