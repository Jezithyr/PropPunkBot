namespace PropPunkShared.Database.Models;

public record WorldStateRaw(
    int Lock,
    string StartDate,
    int Year,
    int Quarter,
    string CurrentDate,
    int WorldGdp,
    float WorldGdpGrowth,
    float WorldFertility
);

public record WorldState(
    DateOnly StartDate,
    int Year,
    int Quarter,
    DateOnly CurrentDate,
    int WorldAverageGdp,
    float WorldGdpGrowth,
    float WorldFertility
)
{
    public WorldState(WorldStateRaw raw) : this(DateOnly.Parse(raw.StartDate), raw.Year, raw.Quarter,
        DateOnly.Parse(raw.CurrentDate), raw.WorldGdp, raw.WorldGdpGrowth, raw.WorldFertility)
    {
    }
}
