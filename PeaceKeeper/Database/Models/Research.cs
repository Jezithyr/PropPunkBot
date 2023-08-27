namespace PeaceKeeper.Database.Models;

public record CountryResearch(
    Country Country,
    int SlotNumber,
    Technology? Tech,
    int Points
    );

public record CountryResearchRaw(
    Guid CountryId,
    int SlotNumber,
    Guid? TechId,
    int Points
);

public record CompanyResearch(
    Company Company,
    int SlotNumber,
    Technology? TechId,
    int Points
);

public record CompanyResearchRaw(
    Guid CompanyId,
    int SlotNumber,
    Guid? Tech,
    int Points
);
