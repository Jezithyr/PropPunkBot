using Dapper;
using PeaceKeeper.Database.Models;

namespace PeaceKeeper.Services;

public partial class EconomyService : PeacekeeperServiceBase
{
    private Dictionary<Country,Dictionary<string, float>> _countryUpkeepSources = new();

    public async Task<int> GetGdpPerCapita(Country country)
    {
        var worldState = await WorldState.Get();
        var countryStats = await _countryStats.GetCountryStats(country);
        if (countryStats == null) return -1;
        return  CalculateGdpPerCapita(worldState.WorldAverageGdp, countryStats.GdpPerCapMultiplier);
    }

    public async Task<CountryEconomy?> GetEconomy(Country country)
    {
        await using var connection = await Db.Get();
        var users = await connection.QueryAsync<CountryEconomyRaw, Country, CountryEconomy>(
            "SELECT * FROM country_economy LEFT JOIN countries ON countries.id = country_economy.id " +
            "WHERE country_economy.id = @id",
            (old, foundcountry) => new CountryEconomy(
                foundcountry,
                old.IndividualTaxRate,
                old.SalesTax,
                old.CorporateTaxRate,
                old.NationalDebt,
                old.GeneralUpkeep,
                old.Funds,
                old.AlternativeIncome
            ), new {id = country.Id});
        return users?.SingleOrDefault();
    }

    public int CalculateGdpPerCapita(int worldAverageGdp, float modifier)
    {
        return (int) MathF.Ceiling(worldAverageGdp * modifier);
    }

    public async Task<int> GetMaxPopIncome(Country country)
    {
        var worldState = await WorldState.Get();
        var countryStats = await _countryStats.GetCountryStats(country);
        if (countryStats == null) return -1;
        var gdp = CalculateGdpPerCapita(worldState.WorldAverageGdp, countryStats.GdpPerCapMultiplier);
        var workingPop = (int)(countryStats.Population -
                         MathF.Ceiling(countryStats.Population * countryStats.UnEmployment));
        return gdp * workingPop;
    }

    public async Task<int> GetGrossIncome(Country country)
    {
        var worldState = await WorldState.Get();
        var countryStats = await _countryStats.GetCountryStats(country);
        var countryEcon = await GetEconomy(country);
        if (countryStats == null || countryEcon == null) return -1;
        var gdp = CalculateGdpPerCapita(worldState.WorldAverageGdp, countryStats.GdpPerCapMultiplier);
        var workingPop = (int)(countryStats.Population -
                               MathF.Ceiling(countryStats.Population * countryStats.UnEmployment));
        return (int)MathF.Ceiling((countryEcon.IndividualTaxRate * gdp * workingPop) + countryEcon.AlternativeIncome);
    }

    public async Task<int> GetNetIncome(Country country)
    {
        var gross = await GetGrossIncome(country);
        var countryEcon = await GetEconomy(country);
        if ( countryEcon == null) return -1;
        return (int)MathF.Ceiling(gross
            + countryEcon.AlternativeIncome - await GetUpkeep(country, gross, countryEcon));
    }

    public async Task<int> GetUpkeep(Country country, int? gross = null, CountryEconomy? countryEcon = null)
    {
        gross ??= await GetGrossIncome(country);
        countryEcon ??= await GetEconomy(country);
        if ( countryEcon == null) return -1;
        var fixedUpkeep = countryEcon.GeneralUpkeep + await _research.GetResearchBudget(country);
        return (int)MathF.Ceiling(gross.Value * (GetUpkeepFromSources(country) + fixedUpkeep));
    }

    public bool AddUpkeepSource(Country country, string sourceName, float upkeepPercent)
    {
        return !_countryUpkeepSources.TryGetValue(country, out var sources)
            ? _countryUpkeepSources.TryAdd(country, new Dictionary<string, float>() {{sourceName, upkeepPercent}})
            : sources.TryAdd(sourceName, upkeepPercent);
    }

    public bool RemoveUpkeepSource(Country country, string sourceName)
    {
        return _countryUpkeepSources.TryGetValue(country, out var sources) && sources.Remove(sourceName);
    }

    public bool RemoveAllUpkeepSources(Country country)
    {
        return _countryUpkeepSources.Remove(country);
    }


    private float GetUpkeepFromSources(Country country)
    {
        float upkeep = 0;
        if (!_countryUpkeepSources.TryGetValue(country, out var upkeepSources)) return upkeep;
        foreach (var (_,upkeepPercent) in upkeepSources)
        {
            upkeep += upkeepPercent;
        }
        return upkeep;
    }


    public async Task SetGeneralUpkeep(Country country, float newupkeepPercent)
    {
        await using var connection = await Db.Get();
        await connection.QueryAsync(
            "UPDATE country_economy set generalupkeep = @upkeep where id = @id",
            new {id = country.Id,  upkeep = newupkeepPercent});
    }

    public async Task AddCountryGeneralUpkeep(Country country, float newpkeepPercent)
    {
        var countryEcon = await GetEconomy(country);
        if (countryEcon == null) return;
        await SetGeneralUpkeep(country, newpkeepPercent + countryEcon.GeneralUpkeep);
    }

    public async Task UpdateFunds(Country country)
    {
        var net = await GetNetIncome(country);
        var countryEcon = await GetEconomy(country);
        if (countryEcon == null) return;
        var newFunds = countryEcon.Funds + net;
        var debt = countryEcon.NationalDebt;
        if (newFunds < 0)
        {
            debt -= newFunds;
            newFunds = 0;
        }
        await using var connection = await Db.Get();
        await connection.QueryAsync(
            "UPDATE country_economy set nationaldebt = @ndebt, funds = @nfunds where id = @id",
            new {id = country.Id, ndebt = debt, nfunds = newFunds});
    }
    private async Task UpdateCountries()
    {
        var moneyPerTech = (await Settings.GetSettings()).MoneyPerTechPoint;
        foreach (var country in await _country.GetAllCountries())
        {
            await UpdateFunds(country);
            var researchSpending = await _research.GetResearchBudget(country) * await GetGrossIncome(country);
            await _research.UpdateResearch(country,
                (int)MathF.Ceiling(researchSpending*moneyPerTech));
        }
    }

}
