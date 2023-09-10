namespace PropPunkShared.Database.Models;

public record CountryStatsRaw(
    Guid CountryId,
    int Population,
    float Happiness,
    float FertilityMod,
    float UnEmployment,
    float EducationIndex,
    float GdpPerCapMultiplier,
    float Urbanization
    );

public record CountryStats(
    CountryModel CountryModel,
    int Population,
    float Happiness,
    float FertilityMod,
    float UnEmployment,
    float EducationIndex,
    float GdpPerCapMultiplier,
    float Urbanization
);
