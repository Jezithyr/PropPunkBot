namespace PeaceKeeper.Database.Models;


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
    Country Country,
    float IndividualTaxRate,
    float SalesTax,
    float CorporateTaxRate,
    long NationalDebt,
    float GeneralUpkeep,
    long Funds,
    long AlternativeIncome
);
