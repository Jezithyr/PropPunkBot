using Dapper;
using Npgsql;
using PeaceKeeper.Database.Models;

namespace PeaceKeeper.Services;

public partial class ResearchService
{
    public async Task<List<Technology>> GetCompanyResearchedTechs(Guid companyId,
        NpgsqlConnection? dbConnection = null)
    {
        await using var connection = await _db.ResolveDatabase(dbConnection);
        var completedProgress = await connection.QueryAsync
            <CompanyResearchProgressRaw, Company, Technology,CompanyResearchProgress>
            (
                "SELECT * FROM company_research_progress " +
                "LEFT JOIN companies ON companies.id = company_research_progress.companyid " +
                "LEFT JOIN technologies ON technologies.id = company_research_progress.techid " +
                "WHERE companyid = @id " +
                "AND completion >= 1",
                (progressData, company, tech) => new CompanyResearchProgress(
                    company,
                    tech,
                    progressData.Completion
                ), new
                {
                    id = companyId
                }
            );

        var techlist = new List<Technology>();
        if (completedProgress == null)
            return new List<Technology>();
        foreach (var progress in completedProgress)
        {
            techlist.Add(progress.Tech);
        }
        return techlist;
    }

    public async Task<decimal> GetCompanyTechnologyProgress(Guid companyId, Guid techId, NpgsqlConnection? dbConnection = null)
    {
        await using var connection = await _db.ResolveDatabase(dbConnection);
        var completedProgress = await connection.QuerySingleOrDefaultAsync<CompanyResearchProgressRaw>(
            "SELECT * FROM company_research_progress WHERE companyid = @id " +
            "AND techid = @tech",
            new {id = companyId, tech = techId}
        );
        if (completedProgress == null)
            return 0;
        return completedProgress.Completion;
    }

     public async Task<Dictionary<int,CompanyResearchSlot>?> GetCompanyResearchSlots(Guid companyId,
        NpgsqlConnection? dbConnection = null)
    {
        await using var connection = await _db.ResolveDatabase(dbConnection);
        var researchQueues = await connection.QueryAsync
            <CompanyResearchSlotRaw, Company, Technology,CompanyResearchSlot>(
                "SELECT * FROM company_research_slots " +
                "LEFT JOIN companies ON companies.id = company_research_slots.companyid " +
                "LEFT JOIN technologies ON technologies.id = company_research_slots.techid " +
                "WHERE company_research_slots.companyid = @id",
                (researchData, company, techid) => new CompanyResearchSlot(
                    company,
                    researchData.SlotNumber,
                    techid
                ), new {id = companyId});
        if (researchQueues == null)
            return null;
        Dictionary<int,CompanyResearchSlot> slots = new();
        foreach (var slotData in researchQueues)
        {
            slots.Add(slotData.SlotNumber, slotData);
        }
        return slots;
    }

    public async Task<CompanyResearchSlot?> GetCompanyResearchSlot(Guid companyId, int slotNumber,
        NpgsqlConnection? dbConnection = null)
    {
        await using var connection = await _db.ResolveDatabase(dbConnection);
        var researchSlots = await connection.QueryAsync
            <CompanyResearchSlotRaw, Company, Technology,CompanyResearchSlot>(
            "SELECT * FROM company_research_slots " +
                "LEFT JOIN companies ON companies.id = company_research_slots.companyid " +
                "LEFT JOIN technologies ON technologies.id = company_research_slots.techid " +
                "WHERE company_research_slots.companyid = @id " +
                "AND company_research_slots.slotnumber = @slotnumber",
                (researchData, company, techid) => new CompanyResearchSlot(
                    company,
                    researchData.SlotNumber,
                    techid),
                new
                    {
                        id = companyId,
                        slotnumber = slotNumber
                    });
        return researchSlots == null ? null : researchSlots.SingleOrDefault() ?? null;
    }

    public async Task<int> UpdateCompanyTech(Guid companyId, Technology tech, int points,
        NpgsqlConnection? dbConnection = null)
    {
        await using var connection = await _db.ResolveDatabase(dbConnection);
        int overflow = points;
        var researchProgress = await connection.QuerySingleOrDefaultAsync<CompanyResearchProgress>(
            "SELECT * FROM company_research_progress WHERE companyid = @id " +
            "AND techid = @techId",
            new {id = companyId, techid = tech.Id}
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
            "UPDATE company_research_progress SET  completion = @percentage " +
            "WHERE companyid = @id AND techid = @techid",
            new { id = companyId, techid = tech.Id, percentage = completion}
        );
        return overflow;
    }

    public async Task<bool> UpdateCompanyResearch(Guid companyId, int researchPoints,
        NpgsqlConnection? dbConnection = null)
    {
        await using var connection = await _db.ResolveDatabase(dbConnection);
        var currentResearchSlots = await GetCompanyResearchSlots(companyId, dbConnection);
        if (currentResearchSlots == null)
            return false;
        //get all slots that have techs assigned
        List<CompanyResearchSlot> activeSlots = new();
        foreach (var (_, slotData) in currentResearchSlots)
        {
            if (slotData.Tech == null) continue;
            activeSlots.Add(slotData);
        }
        activeSlots.Reverse();//reverse the slots so that we can properly propagate overflows to first slot
        if (activeSlots.Count == 0)
        {
            //dump all our research points into the point overflow pool if we don't have any research
            await SetCompanyOverflow(companyId, researchPoints, connection);
            return true;
        }
        var researchPerSlot = (int) MathF.Ceiling((float) researchPoints / activeSlots.Count);
        var overflow = await GetAndClearCompanyOverflow(companyId, connection);
        foreach (var slotData in activeSlots)
        {
            overflow = await UpdateCompanyTech(companyId, slotData.Tech!, researchPerSlot + overflow, connection);
        }

        if (overflow > 0)
        {
            await SetCompanyOverflow(companyId, overflow, connection);
        }
        return true;
    }

    private async Task SetCompanyOverflow(Guid companyId,int overflow ,NpgsqlConnection connection)
    {
        await connection.QueryAsync(
            "UPDATE company_research_meta SET pointoverflow = @newoverflow " +
            "WHERE companyid = @id",
            new{id = companyId, newoverflow = overflow}
        );
    }

    private async Task<int> GetAndClearCompanyOverflow(Guid companyId, NpgsqlConnection connection)
    {
        var metadata = await connection.QuerySingleOrDefaultAsync<CompanyResearchMetaData>(
            "SELECT * FROM company_research_meta WHERE companyId = @id ",
            new{id = companyId}
            );
        var overflow = metadata.PointOverflow;
        await connection.QueryAsync(
            "UPDATE company_research_meta SET pointoverflow = 0 " +
            "WHERE companyId = @id",
            new{id = companyId}
        );
        return overflow;
    }

}
