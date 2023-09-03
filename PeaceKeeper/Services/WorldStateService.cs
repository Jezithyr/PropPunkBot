using Dapper;
using Discord.WebSocket;
using PeaceKeeper.Database;
using PeaceKeeper.Database.Models;

namespace PeaceKeeper.Services;

public sealed class WorldStateService : PeacekeeperCoreServiceBase
{
    private readonly DbService _db;
    private readonly List<(OnWorldTick,int)> _onWorldTickEvents = new ();
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
        await UpdateWorldGdp();
        foreach (var tickEvent in _onWorldTickEvents)
        {
            tickEvent.Item1.Invoke(YearsSinceStart, CurrentQuarter, currentDate);
        }
    }

    public async Task SetWorldGdp(int worldGdp)
    {
        await using var connection = await _db.Get();
        await connection.QuerySingleAsync<WorldState>(
            "UPDATE world_state SET worldgdp = @gdp WHERE lock = 0",
            new {gdp = worldGdp});
    }

    private async Task UpdateWorldGdp()
    {
        await using var connection = await _db.Get();
        await connection.QuerySingleAsync<WorldState>(
            "UPDATE world_state SET worldgdp = worldgdp + worldgdp * worldgdpgrowth WHERE lock = 0");
    }


    public async Task SetWorldGdpGrowth(float newGrowth)
    {
        await using var connection = await _db.Get();
        await connection.QuerySingleAsync<WorldState>(
            "UPDATE world_state SET worldgdpgrowth = @growth WHERE lock = 0",
            new {growth = newGrowth});
    }


    public void RegisterTickEvent(OnWorldTick tickDelegate, int priority = 10)
    {
        _onWorldTickEvents.Add((tickDelegate,priority));
    }
    public WorldStateService(DiscordSocketClient client, DbService db) : base(client)
    {
        _db = db;
    }
}
