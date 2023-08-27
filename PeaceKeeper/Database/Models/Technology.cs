namespace PeaceKeeper.Database.Models;

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
