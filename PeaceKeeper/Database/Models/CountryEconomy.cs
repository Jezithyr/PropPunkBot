namespace PeaceKeeper.Database.Models;


public record CountryEconomy(
    Guid CountryId,
    float IndividualTaxRate,
    float SalesTax,
    float CorporateTaxRate,
    float InterestRate,
    float NationalDebt,
    float NationalUpkeep,
    float NationalFunds,
    float AlternativeIncome
);
