namespace PeaceKeeper.Database.Models;

public record GlobalSettingsRaw(
    int Lock,
    int AotYearStart,
    float AotScaleFactor,
    int CountryResearchSlots,
    int CompanyResearchSlots
    );

public record GlobalSettings(
    int AotYearStart,
    float AotScaleFactor,
    int CountryResearchSlots,
    int CompanyResearchSlots
);
