namespace PropPunkShared.Data.Models;

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
    Country Country,
    int Population,
    float Happiness,
    float FertilityMod,
    float UnEmployment,
    float EducationIndex,
    float GdpPerCapMultiplier,
    float Urbanization
);
