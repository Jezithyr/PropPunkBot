namespace PropPunkShared.Database.Models;

public record CompanyEconomyRaw(
    Guid CompanyId,
    long Funds,
    float GeneralUpkeep,
    long Debt
);

public record CompanyEconomy(
    Company Company,
    long Funds,
    float GeneralUpkeep,
    long Debt
);
