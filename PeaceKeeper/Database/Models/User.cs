namespace PeaceKeeper.Database.Models;

public record UserRaw(long Id,
    Guid? Country = null,
    bool Leader = false,
    Guid? Company = null,
    bool Ceo = false,
    int RpMode = 0,
    string? RpCharacter = null
    );
public record UserRaw2(long Id,
    Country? Country = null,
    bool Leader = false,
    CompanyRaw? Company = null,
    bool Ceo = false,
    int RpMode = 0,
    string? RpCharacter = null
    );


public record User(long Id,
    Country? Country = null,
    bool Leader = false,
    Company? Company = null,
    bool Ceo = false,
    RpMode RpMode = RpMode.OOC,
    string? RpCharacter = null
    );
