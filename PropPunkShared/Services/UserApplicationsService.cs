using Microsoft.AspNetCore.Identity;
using PropPunkShared.Core;
using PropPunkShared.Database;
using PropPunkShared.Database.Models;

namespace PropPunkShared.Services;

public sealed class UserApplicationsService : ScopedServiceBase
{
    private DatabaseContext _db;
    private ConfigService _config;

    public bool CountryApplicationsOpen => _config.Config.CountryAppsAllowed;

    public UserApplicationsService(DatabaseContext db, ConfigService config)
    {
        _db = db;
        _config = config;
    }

    public void SetCountryApplicationsState(bool open)
    {
        EFCHelpers.UpdateData(_db, ref _config.Config, model => model with{CountryAppsAllowed = open});
    }

    public CountryApplicationModel? GetCountryApplicationForUser(IdentityUser user)
    {
        return _db.CountryApplications.FirstOrDefault(a => a.User == user);
    }

    public void DeleteCountryApplicationForUser(IdentityUser user)
    {
        var countryApp = _db.CountryApplications.FirstOrDefault(a=>a.User == user);
        if (countryApp == null)
            return;
        _db.Remove(countryApp);
        _db.SaveChanges();
    }

    public async Task DeleteCountryApplicationForUserAsync(IdentityUser user)
    {
        var countryApp = _db.CountryApplications.FirstOrDefault(a=>a.User == user);
        if (countryApp == null)
            return;
        _db.Remove(countryApp);
        await _db.SaveChangesAsync();
    }

    public CountryApplicationModel? CreateOrModifyCountryApplicationForUser(
        IdentityUser user,
        string name,
        string shortName,
        bool approved,
        bool denied,
        bool locked,
        string nationalLanguages,
        string capitalCityDescription,
        string populationBreakdown,
        string governmentDescription,
        string economicDescription,
        string centralization,
        string cohesion,
        string terrainClimate,
        string majorCities,
        string majorPorts,
        string resources,
        string economicBoons,
        string struggles,
        string techLevel,
        string educationLevel,
        string militaryDescription,
        string militaryStruggles,
        string socialServices,
        string historicalCulture,
        string modernCulture,
        string religion,
        string civilStrife,
        string foreignRelations,
        string borderStates,
        string additionalInfo,
        string flag,
        string roundel,
        string battleFlag
    )
    {
        var countryApp = new CountryApplicationModel( Guid.NewGuid(), name, shortName, approved, denied, locked,nationalLanguages,
            capitalCityDescription,populationBreakdown, governmentDescription, economicDescription, centralization,
            cohesion, terrainClimate, majorCities, majorPorts, resources, economicBoons, struggles, techLevel,
            educationLevel, militaryDescription, militaryStruggles, socialServices, historicalCulture, modernCulture,
            religion, civilStrife, foreignRelations, borderStates, additionalInfo, flag, roundel, battleFlag);
        var oldApp = GetCountryApplicationForUser(user);
        if (oldApp != null)
        {
            EFCHelpers.UpdateData(_db, ref oldApp, c => countryApp with {Id = c.Id});
            return oldApp;
        }

        _db.CountryApplications.Add(countryApp);
        _db.SaveChanges();
        return countryApp;
    }

    public async Task<CountryApplicationModel?> CreateOrModifyCountryApplicationForUserAsync(
        IdentityUser user,
        string name,
        string shortName,
        bool approved,
        bool denied,
        bool locked,
        string nationalLanguages,
        string capitalCityDescription,
        string populationBreakdown,
        string governmentDescription,
        string economicDescription,
        string centralization,
        string cohesion,
        string terrainClimate,
        string majorCities,
        string majorPorts,
        string resources,
        string economicBoons,
        string struggles,
        string techLevel,
        string educationLevel,
        string militaryDescription,
        string militaryStruggles,
        string socialServices,
        string historicalCulture,
        string modernCulture,
        string religion,
        string civilStrife,
        string foreignRelations,
        string borderStates,
        string additionalInfo,
        string flag,
        string roundel,
        string battleFlag
    )
    {
        var countryApp = new CountryApplicationModel( Guid.NewGuid(), name, shortName, approved, denied, locked, nationalLanguages,
            capitalCityDescription,populationBreakdown, governmentDescription, economicDescription, centralization,
            cohesion, terrainClimate, majorCities, majorPorts, resources, economicBoons, struggles, techLevel,
            educationLevel, militaryDescription, militaryStruggles, socialServices, historicalCulture, modernCulture,
            religion, civilStrife, foreignRelations, borderStates, additionalInfo, flag, roundel, battleFlag);
        var oldApp = GetCountryApplicationForUser(user);
        if (oldApp != null)
        {
            EFCHelpers.UpdateData(_db, ref oldApp, c => countryApp with {Id = c.Id});
            return oldApp;
        }
        _db.CountryApplications.Add(countryApp);
        await _db.SaveChangesAsync();
        return countryApp;
    }

}
