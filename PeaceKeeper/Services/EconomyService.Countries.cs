using Dapper;
using PeaceKeeper.Database.Models;

namespace PeaceKeeper.Services;

public partial class EconomyService : PeacekeeperServiceBase
{
    public async Task<int> GetCountryGdpPerCapita(Guid countryId)
    {
        var worldState = await WorldState.Get();
        var countryStats = await _countryStats.GetCountryStats(countryId);
        if (countryStats == null) return -1;
        return  CalculateCountryGdpPerCapita(worldState.WorldAverageGdp, countryStats.GdpPerCapMultiplier);
    }

    public async Task<CountryEconomy?> GetCountryEconomy(Guid countryId)
    {
        await using var connection = await Db.Get();
        var users = await connection.QueryAsync<CountryEconomyRaw, Country, CountryEconomy>(
            "SELECT * FROM country_economies LEFT JOIN countries ON countries.id = country_economies.id " +
            "WHERE country_economies.id = @id",
            (old, country) => new CountryEconomy(
                country,
                old.IndividualTaxRate,
                old.SalesTax,
                old.CorporateTaxRate,
                old.NationalDebt,
                old.NationalUpkeep,
                old.NationalFunds,
                old.AlternativeIncome
            ), new {id = countryId});
        return users?.SingleOrDefault();
    }

    public int CalculateCountryGdpPerCapita(int worldAverageGdp, float modifier)
    {
        return (int) MathF.Ceiling(worldAverageGdp * modifier);
    }

    public async Task<int> GetCountryMaxPopIncome(Guid countryId)
    {
        var worldState = await WorldState.Get();
        var countryStats = await _countryStats.GetCountryStats(countryId);
        if (countryStats == null) return -1;
        var gdp = CalculateCountryGdpPerCapita(worldState.WorldAverageGdp, countryStats.GdpPerCapMultiplier);
        var workingPop = (int)(countryStats.Population -
                         MathF.Ceiling(countryStats.Population * countryStats.UnEmployment));
        return gdp * workingPop;
    }

    public async Task<int> GetCountryGrossIncome(Guid countryId)
    {
        var worldState = await WorldState.Get();
        var countryStats = await _countryStats.GetCountryStats(countryId);
        var countryEcon = await GetCountryEconomy(countryId);
        if (countryStats == null || countryEcon == null) return -1;
        var gdp = CalculateCountryGdpPerCapita(worldState.WorldAverageGdp, countryStats.GdpPerCapMultiplier);
        var workingPop = (int)(countryStats.Population -
                               MathF.Ceiling(countryStats.Population * countryStats.UnEmployment));
        return (int)MathF.Ceiling((countryEcon.IndividualTaxRate * gdp * workingPop) + countryEcon.AlternativeIncome);
    }

    public async Task<int> GetCountryNetIncome(Guid countryId)
    {
        var gross = await GetCountryGrossIncome(countryId);
        var countryEcon = await GetCountryEconomy(countryId);
        if ( countryEcon == null) return -1;
        return (int)MathF.Ceiling(gross
            + countryEcon.AlternativeIncome - countryEcon.NationalUpkeep);
    }

    public async Task UpdateCountryFunds(Guid countryId)
    {
        var net = await GetCountryNetIncome(countryId);
        var countryEcon = await GetCountryEconomy(countryId);
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
            new {id = countryId, ndebt = debt, nfunds = newFunds});
    }
    private async Task UpdateCountries()
    {
        foreach (var country in await _country.GetAllCountries())
        {
            await UpdateCountryFunds(country.Id);
        }
    }

}
