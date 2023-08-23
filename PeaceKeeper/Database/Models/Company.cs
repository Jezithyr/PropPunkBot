namespace PeaceKeeper.Database.Models;

public record CompanyRaw(
    Guid Id,
    string Name,
    string ShortName,
    Guid? OwningCountryId);

public record Company(
    Guid Id,
    string Name,
    string ShortName,
    Country? OwningCountry);
