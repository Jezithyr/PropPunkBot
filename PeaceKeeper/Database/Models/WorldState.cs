namespace PeaceKeeper.Database.Models;

public record WorldStateRaw(
    int Lock,
    string StartDate,
    int Year,
    int Quarter,
    string CurrentDate
    );

public record WorldState(
    DateTime StartDate,
    int Year,
    int Quarter,
    DateTime CurrentDate
)
{
    public WorldState(WorldStateRaw raw) : this(DateTime.Parse(raw.StartDate), raw.Year, raw.Quarter,
        DateTime.Parse(raw.CurrentDate))
    {
    }
}
