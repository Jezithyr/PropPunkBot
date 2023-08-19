namespace PeaceKeeper.Database;

public record User(long Id, Guid? Country = null, bool IsLeader = false, Guid? Company = null, bool IsCeo = false);
public record Country(Guid Id, string Name, string ShortName);
public record Company(Guid Id, string Name, string ShortName, bool IsStateOwned = false);