using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropPunkShared.Database.Models;


public enum AppEnvironment
{
    Development,
    Production,
}

[Table("configs")]
public record ConfigModel(
    [property: Key] AppEnvironment Environment,
    bool CountryAppsAllowed,
    bool CompanyAppsAllowed
);
