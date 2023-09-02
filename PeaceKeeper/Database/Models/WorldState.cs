namespace PeaceKeeper.Database.Models;

public record WorldStateRaw(
    int Lock,
    string StartDate,
    int Year,
    int Quarter,
    string CurrentDate
    );

public record WorldState(
    DateOnly StartDate,
    int Year,
    int Quarter,
    DateOnly CurrentDate
)
{
    public WorldState(WorldStateRaw raw) : this(DateOnly.Parse(raw.StartDate), raw.Year, raw.Quarter,
        DateOnly.Parse(raw.CurrentDate))
    {
    }
}
