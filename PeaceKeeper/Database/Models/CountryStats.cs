namespace PeaceKeeper.Database.Models;

public record CountryStats(
    Guid CountryId,
    long Gdp,
    long NonCityPopulation,
    float AverageIncomePq,
    float PopulationDensity,
    float Fertility,
    float EducationLevel);

