using Dapper;
using Discord.WebSocket;
using PeaceKeeper.Database;
using PeaceKeeper.Database.Models;

namespace PeaceKeeper.Services;

public sealed class WorldStateService : PeacekeeperCoreServiceBase
{
    private readonly DbService _db;
    private OnWorldTick? _onWorldTick = default;

    public int YearsSinceStart { get; private set; }
    public int CurrentQuarter{ get; private set; }

    public delegate void OnWorldTick(int year, int quarter, DateOnly date);

    public async Task<WorldState> Get()
    {
        await using var connection = await _db.Get();

        var rawState = await connection.QuerySingleAsync<WorldStateRaw>(
            "SELECT * FROM world_state WHERE lock = 0 LIMIT 1");
        return new WorldState(rawState);
    }

    public async Task<DateOnly> GetCurrentTurnDate()
    {
        await using var connection = await _db.Get();
        var worldState = await Get();
        return worldState.CurrentDate;
    }

    public async Task<(int, int)> GetCurrentTurnDateQuarters()
    {
        await using var connection = await _db.Get();
        var worldState = await Get();
        return (worldState.Year, worldState.Quarter);
    }

    public async Task Tick(int numberOfTicks = 1)
    {
        await using var connection = await _db.Get();
        var worldState = await connection.QuerySingleAsync<WorldState>(
            "SELECT * FROM world_state WHERE lock = 0 LIMIT 1");
        var currentDate = worldState.StartDate;

        for (int i = 0; i < numberOfTicks; i++)
        {
            var newQuarter = CurrentQuarter + 1;
            currentDate = worldState.StartDate.AddYears(worldState.Year);
            currentDate = currentDate.AddMonths(3*newQuarter);
            if (newQuarter == 4)
            {
                newQuarter = 0;
                YearsSinceStart++;
            }
            CurrentQuarter = newQuarter;
        }
        await connection.QuerySingleAsync<WorldState>(
            "UPDATE world_state SET year = @year, quarter = @quarter WHERE lock = 0",
            new {year = YearsSinceStart, quarter = CurrentQuarter});
        _onWorldTick?.Invoke(YearsSinceStart, CurrentQuarter, currentDate);
    }

    public void RegisterTickEvent(OnWorldTick tickDelegate)
    {
        _onWorldTick += tickDelegate;
    }

    public void RemoveTickEvent(OnWorldTick tickDelegate)
    {
        _onWorldTick -= tickDelegate;
    }

    public WorldStateService(DiscordSocketClient client, DbService db) : base(client)
    {
        _db = db;
    }
}
