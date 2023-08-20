namespace PeaceKeeper.Database;

public record UserRaw(long Id, Guid? Country = null, bool Leader = false, Guid? Company = null, bool Ceo = false);
public record User(long Id, Country? Country = null, bool Leader = false, Company? Company = null, bool Ceo = false);
public record Country(Guid Id, string Name, string ShortName);
public record Company(Guid Id, string Name, string ShortName, bool StateOwned = false);
public record Technology(Guid Id, string Name, TechnologyUse Uses, int Year, TechField Field, int Cost);
public record Settings(long Guild, BotEnvironment Environment);
