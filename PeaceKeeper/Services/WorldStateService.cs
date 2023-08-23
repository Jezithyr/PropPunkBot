using Dapper;
using Npgsql;
using PeaceKeeper.Database;
using PeaceKeeper.Database.Models;

namespace PeaceKeeper.Services;

public sealed class WorldStateService
{
    private readonly DbService _db;

    public DateTime CurrentDate{ get; private set; }

    public int YearsSinceStart { get; private set; }
    public int CurrentQuarter{ get; private set; }

    public delegate void OnWorldTick(int year, int quarter, DateOnly date);


    public WorldStateService(DbService db)
    {
        _db = db;
    }

    public async Task<WorldState?> GetWorldState(NpgsqlConnection? dbConnection = null)
    {
        if (dbConnection == null)
        {
            await using var db = await _db.Get();
            dbConnection = db;
        }
        return await dbConnection.QuerySingleAsync<WorldState>(
            "SELECT * FROM world_state WHERE lock = 0 LIMIT 1");
    }


    public async Task TickWorld(int numberOfTicks = 1, NpgsqlConnection? dbConnection = null)
    {
        if (dbConnection == null)
        {
            await using var db = await _db.Get();
            dbConnection = db;
        }
        var worldState = await dbConnection.QuerySingleAsync<WorldState>(
            "SELECT * FROM world_state WHERE lock = 0 LIMIT 1");
        for (int i = 0; i < numberOfTicks; i++)
        {
            var newQuarter = CurrentQuarter + 1;
            var newYear = YearsSinceStart;
            if (newQuarter == 4)
            {
                newQuarter = 0;
                newYear++;
            }
            DateTime newCurrentDate = CurrentDate.AddYears(newYear);
            newCurrentDate = newCurrentDate.AddMonths(3);

            CurrentDate = newCurrentDate;
            YearsSinceStart = newYear;
            CurrentQuarter = newQuarter;
        }
        await dbConnection.QuerySingleAsync<WorldState>(
            "UPDATE world_state SET year = @year, quarter = @quarter WHERE lock = 0",
            new {year = YearsSinceStart, quarter = CurrentQuarter});
    }

}
