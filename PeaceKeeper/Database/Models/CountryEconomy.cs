namespace PeaceKeeper.Database.Models;


public record CountryEconomyRaw(
    Guid CountryId,
    float IndividualTaxRate,
    float SalesTax,
    float CorporateTaxRate,
    float NationalDebt,
    float NationalUpkeep,
    float NationalFunds,
    float AlternativeIncome
);

public record CountryEconomy(
    Country Country,
    float IndividualTaxRate,
    float SalesTax,
    float CorporateTaxRate,
    float NationalDebt,
    float NationalUpkeep,
    float NationalFunds,
    float AlternativeIncome
);
