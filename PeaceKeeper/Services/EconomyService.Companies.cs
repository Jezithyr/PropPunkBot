using Dapper;
using PeaceKeeper.Database.Models;

namespace PeaceKeeper.Services;

public partial class EconomyService : PeacekeeperServiceBase
{
    private Dictionary<Company,Dictionary<string, float>> _companyUpkeepSources = new();

    public async Task<CompanyEconomy?> GetEconomy(Company company)
    {
        await using var connection = await Db.Get();
        var users = await connection.QueryAsync<CompanyEconomyRaw, Company, CompanyEconomy>(
            "SELECT * FROM company_economy LEFT JOIN companies ON companies.id = company_economy.id " +
            "WHERE company_economy.id = @id",
            (old, foundcompany) => new CompanyEconomy(
                foundcompany,
                old.Funds,
                old.GeneralUpkeep,
                old.Debt
            ), new {id = company.Id});
        return users?.SingleOrDefault();
    }

    public async Task<int> GetGrossIncome(Company company)
    {
        return 0;
    }

    public async Task<int> GetNetIncome(Company company)
    {
        var gross = await GetGrossIncome(company);
        return 0;
    }

    public async Task<int> GetUpkeep(Company company, int? gross = null, CountryEconomy? companyEcon = null)
    {
        return 0;
    }

    public bool AddUpkeepSource(Company company, string sourceName, float upkeepPercent)
    {
        return !_companyUpkeepSources.TryGetValue(company, out var sources)
            ? _companyUpkeepSources.TryAdd(company, new Dictionary<string, float>() {{sourceName, upkeepPercent}})
            : sources.TryAdd(sourceName, upkeepPercent);
    }

    public bool RemoveUpkeepSource(Company company, string sourceName)
    {
        return _companyUpkeepSources.TryGetValue(company, out var sources) && sources.Remove(sourceName);
    }

    public bool RemoveAllUpkeepSources(Company company)
    {
        return _companyUpkeepSources.Remove(company);
    }


    private float GetUpkeepFromSources(Company company)
    {
        float upkeep = 0;
        if (!_companyUpkeepSources.TryGetValue(company, out var upkeepSources)) return upkeep;
        foreach (var (_,upkeepPercent) in upkeepSources)
        {
            upkeep += upkeepPercent;
        }
        return upkeep;
    }


    public async Task SetGeneralUpkeep(Company company, float newupkeepPercent)
    {
        await using var connection = await Db.Get();
        await connection.QueryAsync(
            "UPDATE company_economy set generalupkeep = @upkeep where id = @id",
            new {id = company.Id,  upkeep = newupkeepPercent});
    }

    public async Task AddCountryGeneralUpkeep(Company company, float newpkeepPercent)
    {
        var countryEcon = await GetEconomy(company);
        if (countryEcon == null) return;
        await SetGeneralUpkeep(company, newpkeepPercent + countryEcon.GeneralUpkeep);
    }

    public async Task UpdateFunds(Company company)
    {
        var net = await GetNetIncome(company);
        var companyEcon = await GetEconomy(company);
        if (companyEcon == null) return;
        var newFunds = companyEcon.Funds + net;
        var debt = companyEcon.Debt;
        if (newFunds < 0)
        {
            debt -= newFunds;
            newFunds = 0;
        }
        await using var connection = await Db.Get();
        await connection.QueryAsync(
            "UPDATE company_economy set debt = @ndebt, funds = @nfunds where id = @id",
            new {id = company.Id, ndebt = debt, nfunds = newFunds});
    }
    private async Task UpdateCompanies()
    {
        var moneyPerTech = (await Settings.GetSettings()).MoneyPerTechPoint;
        foreach (var company in await _company.GetAllCompanies())
        {
            await UpdateFunds(company);
            var researchSpending = await _research.GetResearchBudget(company) * await GetGrossIncome(company);
            await _research.UpdateResearch(company,
                (int)MathF.Ceiling(researchSpending*moneyPerTech));
        }
    }

}
