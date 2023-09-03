using Dapper;
using PeaceKeeper.Database.Models;

namespace PeaceKeeper.Services;

public partial class EconomyService : PeacekeeperServiceBase
{
    private Dictionary<Country,Dictionary<string, float>> _upkeepSources = new();

    public async Task<int> GetGdpPerCapita(Country country)
    {
        var worldState = await WorldState.Get();
        var countryStats = await _countryStats.GetCountryStats(country);
        if (countryStats == null) return -1;
        return  CalculateCountryGdpPerCapita(worldState.WorldAverageGdp, countryStats.GdpPerCapMultiplier);
    }

    public async Task<CountryEconomy?> GetEconomy(Country country)
    {
        await using var connection = await Db.Get();
        var users = await connection.QueryAsync<CountryEconomyRaw, Country, CountryEconomy>(
            "SELECT * FROM country_economies LEFT JOIN countries ON countries.id = country_economies.id " +
            "WHERE country_economies.id = @id",
            (old, foundcountry) => new CountryEconomy(
                foundcountry,
                old.IndividualTaxRate,
                old.SalesTax,
                old.CorporateTaxRate,
                old.NationalDebt,
                old.GeneralUpkeep,
                old.NationalFunds,
                old.AlternativeIncome
            ), new {id = country.Id});
        return users?.SingleOrDefault();
    }

    public int CalculateCountryGdpPerCapita(int worldAverageGdp, float modifier)
    {
        return (int) MathF.Ceiling(worldAverageGdp * modifier);
    }

    public async Task<int> GetCountryMaxPopIncome(Country country)
    {
        var worldState = await WorldState.Get();
        var countryStats = await _countryStats.GetCountryStats(country);
        if (countryStats == null) return -1;
        var gdp = CalculateCountryGdpPerCapita(worldState.WorldAverageGdp, countryStats.GdpPerCapMultiplier);
        var workingPop = (int)(countryStats.Population -
                         MathF.Ceiling(countryStats.Population * countryStats.UnEmployment));
        return gdp * workingPop;
    }

    public async Task<int> GetCountryGrossIncome(Country country)
    {
        var worldState = await WorldState.Get();
        var countryStats = await _countryStats.GetCountryStats(country);
        var countryEcon = await GetEconomy(country);
        if (countryStats == null || countryEcon == null) return -1;
        var gdp = CalculateCountryGdpPerCapita(worldState.WorldAverageGdp, countryStats.GdpPerCapMultiplier);
        var workingPop = (int)(countryStats.Population -
                               MathF.Ceiling(countryStats.Population * countryStats.UnEmployment));
        return (int)MathF.Ceiling((countryEcon.IndividualTaxRate * gdp * workingPop) + countryEcon.AlternativeIncome);
    }

    public async Task<int> GetCountryNetIncome(Country country)
    {
        var gross = await GetCountryGrossIncome(country);
        var countryEcon = await GetEconomy(country);
        if ( countryEcon == null) return -1;
        return (int)MathF.Ceiling(gross
            + countryEcon.AlternativeIncome - gross*(countryEcon.GeneralUpkeep+GetUpkeepFromSources(country)));
    }

    public bool AddUpkeepSource(Country country, string sourceName, float upkeepPercent)
    {
        return !_upkeepSources.TryGetValue(country, out var sources)
            ? _upkeepSources.TryAdd(country, new Dictionary<string, float>() {{sourceName, upkeepPercent}})
            : sources.TryAdd(sourceName, upkeepPercent);
    }

    public bool RemoveUpkeepSource(Country country, string sourceName)
    {
        return _upkeepSources.TryGetValue(country, out var sources) && sources.Remove(sourceName);
    }

    public bool RemoveAllUpkeepSources(Country country)
    {
        return _upkeepSources.Remove(country);
    }


    private float GetUpkeepFromSources(Country country)
    {
        float upkeep = 0;
        if (!_upkeepSources.TryGetValue(country, out var upkeepSources)) return upkeep;
        foreach (var (_,upkeepPercent) in upkeepSources)
        {
            upkeep += upkeepPercent;
        }
        return upkeep;
    }


    public async Task SetCountryGeneralUpkeep(Country country, float newpkeepPercent)
    {
        await using var connection = await Db.Get();
        await connection.QueryAsync(
            "UPDATE country_economies set generalupkeep = @upkeep where id = @id",
            new {id = country.Id,  upkeep = newpkeepPercent});
    }

    public async Task AddCountryGeneralUpkeep(Country country, float newpkeepPercent)
    {
        var countryEcon = await GetEconomy(country);
        if (countryEcon == null) return;
        await SetCountryGeneralUpkeep(country, newpkeepPercent + countryEcon.GeneralUpkeep);
    }

    public async Task UpdateCountryFunds(Country country)
    {
        var net = await GetCountryNetIncome(country);
        var countryEcon = await GetEconomy(country);
        if (countryEcon == null) return;
        var newFunds = countryEcon.NationalFunds + net;
        var debt = countryEcon.NationalDebt;
        if (newFunds < 0)
        {
            debt -= newFunds;
            newFunds = 0;
        }
        await using var connection = await Db.Get();
        await connection.QueryAsync(
            "UPDATE country_economies set nationaldebt = @ndebt, nationalfunds = @nfunds where id = @id",
            new {id = country.Id, ndebt = debt, nfunds = newFunds});
    }
    private async Task UpdateCountries()
    {
        foreach (var country in await _country.GetAllCountries())
        {
            await UpdateCountryFunds(country);
        }
    }

}
