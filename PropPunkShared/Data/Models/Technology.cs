namespace PropPunkShared.Data.Models;
public record Technology(
    Guid Id,
    string Name,
    TechnologyUse Uses,
    int Year,
    TechField Field,
    int Cost);

public record TechRequirementRaw(
    Guid TechId,
    Guid RequirementId
    );

public record TechRequirement(
    Technology Tech,
    Technology Requirement
);

public enum TechnologyUse
{
    Civilian = 1 << 0,
    Military = 1 << 1,
    Both = Civilian | Military
}

public enum TechField
{
    Generic,
    Infrastructure,
    Industry,
    Materials,
    Economy,
    Strategy,
    Sociology,
    Biology,
    Electronics,
    Physics,
    Mechanics,
    Chemistry,
    Aerospace,
    Nautical,
    Munitions,
    Weaponry
}
