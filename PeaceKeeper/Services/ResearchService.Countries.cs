using Dapper;
using Npgsql;
using PeaceKeeper.Database;
using PeaceKeeper.Database.Models;

namespace PeaceKeeper.Services;

public partial class ResearchService
{
    public async Task<HashSet<Technology>> GetResearchedTechs(Country country)
    {
        await using var connection = await Db.Get();
        var completedProgress = await connection.QueryAsync
            <CountryResearchProgressRaw, Country, Technology,CountryResearchProgress>
        (
            "SELECT * FROM country_research_progress " +
            "LEFT JOIN countries ON countries.id = country_research_progress.id " +
            "LEFT JOIN technologies ON technologies.id = country_research_progress.techid " +
            "WHERE id = @id " +
            "AND completion >= 1",
            (progressData, newcountry, tech) => new CountryResearchProgress(
                newcountry,
                tech,
                progressData.Completion
            ),
            new
            {
                id = country.Id
            }
        );

        var techlist = new HashSet<Technology>();
        if (completedProgress == null)
            return new HashSet<Technology>();
        foreach (var progress in completedProgress)
        {
            techlist.Add(progress.Tech);
        }
        return techlist;
    }

    public async Task<HashSet<Technology>> GetResearchedTechsInField(Country country, TechField field)
    {
        await using var connection = await Db.Get();
        var completedProgress = await connection.QueryAsync
            <CountryResearchProgressRaw, Country, Technology,CountryResearchProgress>
            (
                "SELECT * FROM country_research_progress " +
                "LEFT JOIN countries ON countries.id = country_research_progress.id " +
                "LEFT JOIN technologies ON technologies.id = country_research_progress.techid " +
                "WHERE id = @id AND completion >= 1 AND field = @techfield",
                (progressData, newcountry, tech) => new CountryResearchProgress(
                    newcountry,
                    tech,
                    progressData.Completion
                ),
                new
                {
                    id = country.Id,
                    techfield = field
                }
            );

        var techlist = new HashSet<Technology>();
        if (completedProgress == null)
            return new HashSet<Technology>();
        foreach (var progress in completedProgress)
        {
            techlist.Add(progress.Tech);
        }
        return techlist;
    }

    public async Task<decimal> GetTechnologyProgress(Country country, Technology tech)
    {
        await using var connection = await Db.Get();
        var completedProgress = await connection.QuerySingleOrDefaultAsync<CountryResearchProgressRaw>(
            "SELECT * FROM country_research_progress WHERE id = @id " +
            "AND techid = @tech",
            new {id = country.Id, tech = tech.Id}
        );
        if (completedProgress == null)
            return 0;
        return completedProgress.Completion;
    }

    public async Task<Dictionary<int,CountryResearchSlot>?> GetResearchSlots(Country country)
    {
        await using var connection = await Db.Get();
        var researchQueues = await connection.QueryAsync
            <CountryResearchSlotRaw, Country, Technology,CountryResearchSlot>(
                "SELECT * FROM country_research_slots " +
                "LEFT JOIN countries ON countries.id = country_research_slots.id " +
                "LEFT JOIN technologies ON technologies.id = country_research_slots.techid " +
                "WHERE country_research_slots.id = @id",
                (researchData, countryid, techid) => new CountryResearchSlot(
                    countryid,
                    researchData.SlotNumber,
                    techid
                ), new {id = country});
        if (researchQueues == null)
            return null;
        Dictionary<int,CountryResearchSlot> slots = new();
        foreach (var slotData in researchQueues)
        {
            slots.Add(slotData.SlotNumber, slotData);
        }
        return slots;
    }

    public async Task<CountryResearchSlot?> GetCountryResearchSlot(Country country, int slotNumber)
    {
        await using var connection = await Db.Get();
        var researchSlots = await connection.QueryAsync
            <CountryResearchSlotRaw, Country, Technology,CountryResearchSlot>(
            "SELECT * FROM country_research_slots " +
                "LEFT JOIN countries ON countries.id = country_research_slots.id " +
                "LEFT JOIN technologies ON technologies.id = country_research_slots.techid " +
                "WHERE country_research_slots.id = @id " +
                "AND country_research_slots.slotnumber = @slotnumber",
                (researchData, newcountry, techid) => new CountryResearchSlot(
                    newcountry,
                    researchData.SlotNumber,
                    techid),
                new
                    {
                        id = country.Id,
                        slotnumber = slotNumber
                    });
        return researchSlots == null ? null : researchSlots.SingleOrDefault() ?? null;
    }

    public async Task<int> UpdateTech(Country country, Technology tech, int points)
    {
        await using var connection = await Db.Get();
        int overflow = points;
        var researchProgress = await connection.QuerySingleOrDefaultAsync<CountryResearchProgress>(
            "SELECT * FROM country_research_progress WHERE id = @id " +
            "AND techid = @techId",
            new {id = country.Id, techid = tech.Id}
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
            "WHERE id = @id AND techid = @techid",
            new { id = country.Id, techid = tech.Id, percentage = completion}
        );
        return overflow;
    }

    public async Task<bool> UpdateResearch(Country country, int researchPoints)
    {
        await using var connection = await Db.Get();
        var currentResearchSlots = await GetResearchSlots(country);
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
            await SetOverflow(country, researchPoints);
            return true;
        }
        var researchPerSlot = (int) MathF.Ceiling((float) researchPoints / activeSlots.Count);
        var overflow = await GetAndClearTechOverflow(country);
        foreach (var slotData in activeSlots)
        {
            overflow = await UpdateTech(country, slotData.Tech!, researchPerSlot + overflow);
        }

        if (overflow > 0)
        {
            await SetOverflow(country, overflow);
        }
        return true;
    }

    public async Task<float> GetResearchBudget(Country country)
    {
        await using var connection = await Db.Get();
        var metadata = await connection.QuerySingleOrDefaultAsync<CountryResearchMetaData>(
            "SELECT * FROM country_research_data " +
            "WHERE id = @id",
            new{id = country.Id}
        );
        return metadata.ResearchBudget;
    }

    private async Task SetOverflow(Country country,int overflow)
    {
        await using var connection = await Db.Get();
        await connection.QueryAsync(
            "UPDATE country_research_data SET pointoverflow = @newoverflow " +
            "WHERE id = @id",
            new{id = country.Id, newoverflow = overflow}
        );
    }

    private async Task<int> GetAndClearTechOverflow(Country country)
    {
        await using var connection = await Db.Get();
        var metadata = await connection.QuerySingleOrDefaultAsync<CountryResearchMetaDataRaw>(
            "SELECT * FROM country_research_data WHERE id = @id ",
            new{id = country.Id}
            );
        var overflow = metadata.PointOverflow;
        await connection.QueryAsync(
            "UPDATE country_research_data SET pointoverflow = 0 " +
            "WHERE id = @id",
            new{id = country.Id}
        );
        return overflow;
    }

    public async Task<HashSet<Technology>> GetValidTechs(Country country)
    {
        HashSet<Technology> researchedTechs = await GetResearchedTechs(country);
        return await GetValidTechs(researchedTechs);

    }

    public async Task<HashSet<Technology>> GetValidTechs(Country country, TechField techField)
    {
        HashSet<Technology> researchedTechs = await GetResearchedTechsInField(country, techField);
        return await GetValidTechs(researchedTechs);
    }
}
