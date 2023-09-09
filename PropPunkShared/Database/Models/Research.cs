namespace PropPunkShared.Database.Models;

public record CountryResearchSlot( Country Country, int SlotNumber, Technology? Tech);

public record CountryResearchSlotRaw(Guid CountryId, int SlotNumber, Guid? TechId);

public record CountryResearchProgressRaw(Guid CountryId, Guid TechId, decimal Completion);

public record CountryResearchProgress(Country Country, Technology Tech, decimal Completion);

public record CountryResearchMetaDataRaw(Guid CountryId, int PointOverflow, float ResearchBudget);

public record CountryResearchMetaData(CountryStats Country, int PointOverflow, float ResearchBudget);

public record CompanyResearchSlot(Company Company, int SlotNumber, Technology? Tech);

public record CompanyResearchSlotRaw(Guid CompanyId, int SlotNumber, Guid? Tech);

public record CompanyResearchMetaDataRaw(Guid CompanyId, int PointOverflow, float ResearchBudget);

public record CompanyResearchMetaData(Guid CompanyId, int PointOverflow, float ResearchBudget);

public record CompanyResearchProgressRaw(Guid CompanyId, Guid TechId, decimal Completion);

public record CompanyResearchProgress(Company Company, Technology Tech, decimal Completion);

