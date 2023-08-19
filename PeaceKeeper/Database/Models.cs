namespace PeaceKeeper.Database;

public record User(long Id);
public record Country(Guid Id, string Name, string ShortName, long Owner, long[] Members, GovernmentStatus Status);
public record Company(Guid Id, string Name, string ShortName, bool IsStateOwned ,long Owner, long[] Members, 
    Guid HeadquartersCountry, Guid[] Branches);