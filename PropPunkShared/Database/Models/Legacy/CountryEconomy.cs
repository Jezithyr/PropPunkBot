namespace PropPunkShared.Database.Models;

public record CountryEconomyRaw(
    Guid CountryId,
    float IndividualTaxRate,
    float SalesTax,
    float CorporateTaxRate,
    long NationalDebt,
    float GeneralUpkeep,
    long Funds,
    long AlternativeIncome
);

public record CountryEconomy(
    CountryModel CountryModel,
    float IndividualTaxRate,
    float SalesTax,
    float CorporateTaxRate,
    long NationalDebt,
    float GeneralUpkeep,
    long Funds,
    long AlternativeIncome
);