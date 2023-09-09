namespace PropPunkShared.Data.Models;

public record User(long Id,
    Country? Country = null,
    bool Leader = false,
    Company? Company = null,
    bool Ceo = false,
    RpMode RpMode = RpMode.OOC,
    string? RpCharacter = null
    );
