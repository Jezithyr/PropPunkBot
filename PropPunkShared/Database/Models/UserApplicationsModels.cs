using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace PropPunkShared.Database.Models;

[Table("applications_country"), Index(nameof(Name), IsUnique = true), Index(nameof(ShortName), IsUnique = true),
 Index(nameof(UserId), IsUnique = true)
]
public record CountryApplicationModel(
    [property: Key] Guid Id,
    [property: StringLength(128)] string Name,
    [property: StringLength(4)] string ShortName,
    bool Approved,
    bool Denied,
    bool Locked,
    string NationalLanguages,
    string CapitalCityDescription,
    string PopulationBreakdown,
    string GovernmentDescription,
    string EconomicDescription,
    string Centralization,
    string Cohesion,
    string TerrainClimate,
    string MajorCities,
    string MajorPorts,
    string Resources,
    string EconomicBoons,
    string Struggles,
    string TechLevel,
    string EducationLevel,
    string MilitaryDescription,
    string MilitaryStruggles,
    string SocialServices,
    string HistoricalCulture,
    string ModernCulture,
    string Religion,
    string CivilStrife,
    string ForeignRelations,
    string BorderStates,
    string AdditionalInfo,
    string Flag,
    string Roundel,
    string BattleFlag
)
{
    [ForeignKey(nameof(User))] public string UserId { get; set; } = default!;
    public IdentityUser User { get; set; } = default!;
}


// public record CompanyApplicationModel(
//     [property: Key] Guid Id,
//     [property: StringLength(128)] string Name,
//     [property: StringLength(4)] string ShortName
// )
// {
//
// }
//
//
