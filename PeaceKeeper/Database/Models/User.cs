namespace PeaceKeeper.Database.Models;

public record UserRaw(long Id,
    Guid? Country = null,
    bool Leader = false,
    Guid? Company = null,
    bool Ceo = false);
public record UserRaw2(long Id,
    Country? Country = null,
    bool Leader = false,
    CompanyRaw? Company = null,
    bool Ceo = false);


public record User(long Id,
    Country? Country = null,
    bool Leader = false,
    Company? Company = null,
    bool Ceo = false);
