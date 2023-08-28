using Dapper;
using Npgsql;
using PeaceKeeper.Database;
using PeaceKeeper.Database.Models;

namespace PeaceKeeper.Services;

public sealed class WorldStateService : PeacekeeperServiceBase
{
    public int YearsSinceStart { get; private set; }
    public int CurrentQuarter{ get; private set; }

    public delegate void OnWorldTick(int year, int quarter, DateOnly date);

    public async Task<WorldState> Get(NpgsqlConnection? dbConnection = null)
    {
        await using var connection = await Db.ResolveDatabase(dbConnection);

        var rawState = await dbConnection.QuerySingleAsync<WorldStateRaw>(
            "SELECT * FROM world_state WHERE lock = 0 LIMIT 1");
        return new WorldState(rawState);
    }

    public async Task<DateTime> GetCurrentTurnDate(NpgsqlConnection? dbConnection = null)
    {
        await using var connection = await Db.ResolveDatabase(dbConnection);
        var worldState = await Get(connection);
        return worldState.CurrentDate;
    }

    public async Task<(int, int)> GetCurrentTurnDateQuarters(NpgsqlConnection? dbConnection = null)
    {
        await using var connection = await Db.ResolveDatabase(dbConnection);
        var worldState = await Get(connection);
        return (worldState.Year, worldState.Quarter);
    }

    public async Task Tick(int numberOfTicks = 1, NpgsqlConnection? dbConnection = null)
    {
        await using var connection = await Db.ResolveDatabase(dbConnection);
        var worldState = await dbConnection.QuerySingleAsync<WorldState>(
            "SELECT * FROM world_state WHERE lock = 0 LIMIT 1");

        for (int i = 0; i < numberOfTicks; i++)
        {
            var newQuarter = CurrentQuarter + 1;

            DateTime currentDate = worldState.StartDate;
            currentDate = worldState.StartDate.AddYears(worldState.Year);
            currentDate = currentDate.AddMonths(3*newQuarter);
            if (newQuarter == 4)
            {
                newQuarter = 0;
                YearsSinceStart++;
            }
            CurrentQuarter = newQuarter;
        }
        await dbConnection.QuerySingleAsync<WorldState>(
            "UPDATE world_state SET year = @year, quarter = @quarter WHERE lock = 0",
            new {year = YearsSinceStart, quarter = CurrentQuarter});
    }

    public WorldStateService(SettingsService settings, UserService users, DbService db) : base(settings, users, db)
    {
    }
}
