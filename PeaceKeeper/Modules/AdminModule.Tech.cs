using Dapper;
using Discord.Interactions;
using PeaceKeeper.Database;

namespace PeaceKeeper.Modules;

public partial class AdminModule
{
    [SlashCommand("createtech", "create a new technology")]
    public async Task CreateTechnology(string techName, TechnologyUse uses, int yearDeveloped, TechField field, int cost)
    {
        await DeferAsync();
        if (techName.Length > 128)
        {
            await FollowupAsync("Technology name is too long, must be less than 128 characters!");
            return;
        }
        await using var connection = DatabaseConnection.Get();
        var tech = await connection.QuerySingleOrDefaultAsync<Technology>(
            "SELECT * FROM technologies WHERE name = @name LIMIT 1",
            new {name = techName});
        if (tech == null)
        {
            await connection.QuerySingleAsync<long>(
                "INSERT INTO technologies (name, uses, year, field, cost) VALUES (@name, @uses, @year, @field, @cost) ON CONFLICT DO NOTHING RETURNING -1",
                new {name = techName, uses = uses, year = yearDeveloped, field = field, cost = cost});
            await FollowupAsync($"Registered new tech: {techName} | Uses: {uses} | Field: {field} | Year: {yearDeveloped} raw cost: {cost}");
        }
        else
        {
            await FollowupAsync($"Tech with name: {techName} already exists!");
        }
        
    }
    [SlashCommand("removetech", "remove a technology")]
    public async Task RemoveTech(string techName)
    {
        await DeferAsync();
        if (techName.Length > 128)
        {
            await FollowupAsync("Technology name is too long, must be less than 128 characters!");
            return;
        }
        
        await using var connection = DatabaseConnection.Get();
        var tech = await connection.QuerySingleOrDefaultAsync<Technology>(
            "SELECT * FROM technologies WHERE name = @name LIMIT 1",
            new {name = techName});
        if (tech == null)
        {
            await FollowupAsync($"Tech {techName} was not found!");
        }
        else
        {
            await connection.QueryAsync("DELETE FROM technologies where name = @name", new {name =techName});
            await FollowupAsync($"Removed Tech: {techName}");
        }
    }
}