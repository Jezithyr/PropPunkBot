using Dapper;
using Npgsql;
using PeaceKeeper.Database.Models;

namespace PeaceKeeper.Services;

public partial class ResearchService
{
    public async Task<Dictionary<int,CountryResearchSlot>?> GetCountryResearchSlots(Guid countryId,
        NpgsqlConnection? dbConnection = null)
    {
        await using var connection = await _db.ResolveDatabase(dbConnection);
        var researchQueues = await connection.QueryAsync
            <CountryResearchSlotRaw, Country, Technology,CountryResearchSlot>(
                "SELECT * FROM country_research_slots " +
                "LEFT JOIN countries ON countries.id = country_research_slots.countryid " +
                "LEFT JOIN technologies ON technologies.id = country_research_slots.techid " +
                "WHERE country_research_slots.countryid = @id",
                (researchData, countryid, techid) => new CountryResearchSlot(
                    countryid,
                    researchData.SlotNumber,
                    techid
                ), new {id = countryId});
        if (researchQueues == null)
            return null;
        Dictionary<int,CountryResearchSlot> slots = new();
        foreach (var slotData in researchQueues)
        {
            slots.Add(slotData.SlotNumber, slotData);
        }
        return slots;
    }

    public async Task<CountryResearchSlot?> GetCountryResearchSlot(Guid countryId, int slotNumber,
        NpgsqlConnection? dbConnection = null)
    {
        await using var connection = await _db.ResolveDatabase(dbConnection);
        var researchSlots = await connection.QueryAsync
            <CountryResearchSlotRaw, Country, Technology,CountryResearchSlot>(
            "SELECT * FROM country_research_slots " +
                "LEFT JOIN countries ON countries.id = country_research_slots.countryid " +
                "LEFT JOIN technologies ON technologies.id = country_research_slots.techid " +
                "WHERE country_research_slots.countryid = @id " +
                "AND country_research_slots.slotnumber = @slotnumber",
                (researchData, country, techid) => new CountryResearchSlot(
                    country,
                    researchData.SlotNumber,
                    techid),
                new
                    {
                        id = countryId,
                        slotnumber = slotNumber
                    });
        return researchSlots == null ? null : researchSlots.SingleOrDefault() ?? null;
    }

    public async Task<int> UpdateCountryTech(Guid countryId, Technology tech, int points,
        NpgsqlConnection? dbConnection = null)
    {
        await using var connection = await _db.ResolveDatabase(dbConnection);
        int overflow = points;
        var researchProgress = await connection.QuerySingleOrDefaultAsync<CountryResearchProgress>(
            "SELECT * FROM country_research_progress WHERE countryid = @id " +
            "AND techid = @techId",
            new {id = countryId, techid = tech.Id}
        );
        if (researchProgress == null || researchProgress.Completion >= 1)
            return overflow;
        var techCost = await GetAdjustedTechCost(tech);
        decimal completion =  (decimal)techCost/points;
        if (points >= techCost)
        {
            overflow = points - techCost;
            completion = 1m;
        }
        await connection.QueryAsync(
            "UPDATE country_research_progress SET  completion = @percentage " +
            "WHERE countryid = @id AND techid = @techid",
            new { id = countryId, techid = tech.Id, percentage = completion}
        );
        return overflow;
    }

    public async Task<bool> UpdateCountryResearch(Guid countryId, int researchPoints,
        NpgsqlConnection? dbConnection = null)
    {
        await using var connection = await _db.ResolveDatabase(dbConnection);
        var currentResearchSlots = await GetCountryResearchSlots(countryId, dbConnection);
        if (currentResearchSlots == null)
            return false;
        //get all slots that have techs assigned
        List<CountryResearchSlot> activeSlots = new();
        foreach (var (_, slotData) in currentResearchSlots)
        {
            if (slotData.Tech == null) continue;
            activeSlots.Add(slotData);
        }
        activeSlots.Reverse();//reverse the slots so that we can properly propagate overflows to first slot
        if (activeSlots.Count == 0)
        {
            //dump all our research points into the point overflow pool if we don't have any research
            await SetCountryOverflow(countryId, researchPoints, connection);
            return true;
        }
        var researchPerSlot = (int) MathF.Ceiling((float) researchPoints / activeSlots.Count);
        var overflow = await GetAndClearCountryOverflow(countryId, connection);
        foreach (var slotData in activeSlots)
        {
            overflow = await UpdateCountryTech(countryId, slotData.Tech!, researchPerSlot + overflow, connection);
        }

        if (overflow > 0)
        {
            await SetCountryOverflow(countryId, overflow, connection);
        }
        return true;
    }

    private async Task SetCountryOverflow(Guid countryId,int overflow ,NpgsqlConnection connection)
    {
        await connection.QueryAsync(
            "UPDATE country_research_meta SET pointoverflow = @newoverflow " +
            "WHERE countryid = @id",
            new{id = countryId, newoverflow = overflow}
        );
    }

    private async Task<int> GetAndClearCountryOverflow(Guid countryId, NpgsqlConnection connection)
    {
        var metadata = await connection.QuerySingleOrDefaultAsync<CountryResearchMetaData>(
            "SELECT * FROM country_research_meta WHERE countryid = @id ",
            new{id = countryId}
            );
        var overflow = metadata.PointOverflow;
        await connection.QueryAsync(
            "UPDATE country_research_meta SET pointoverflow = 0 " +
            "WHERE countryid = @id",
            new{id = countryId}
        );
        return overflow;
    }

}
