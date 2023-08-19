namespace PeaceKeeper.Database;

public record User(long Id, Guid? Country = null, bool Leader = false, Guid? Company = null, bool Ceo = false);
public record Country(Guid Id, string Name, string ShortName);
public record Company(Guid Id, string Name, string ShortName, bool StateOwned = false);
public record Technology(Guid Id, string Name, TechnologyUse Uses, int Year, TechField Field, int Cost)
{
    public Technology(Guid Id, string Name, int Uses, int Year, int Field, int Cost) : this(Id, Name,
        (TechnologyUse) Uses, Year, (TechField) Field, Cost)
    {
    }
}