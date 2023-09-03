using Dapper;
using Npgsql;
using PeaceKeeper.Database;
using PeaceKeeper.Database.Models;

namespace PeaceKeeper.Services;

public partial class ResearchService
{
    public async Task<HashSet<Technology>> GetValidTechsFor(Company company)
    {
        HashSet<Technology> researchedTechs = await GetResearchedTechs(company);
        return await GetValidTechs(researchedTechs);

    }

    public async Task<HashSet<Technology>> GetValidTechsFor(Company company, TechField techField)
    {
        HashSet<Technology> researchedTechs = await GetResearchedTechsInField(company, techField);
        return await GetValidTechs(researchedTechs);
    }

    public async Task<HashSet<Technology>> GetResearchedTechs(Company company)
    {
        await using var connection = await Db.Get();
        var completedProgress = await connection.QueryAsync
            <CompanyResearchProgressRaw, Company, Technology,CompanyResearchProgress>
            (
                "SELECT * FROM company_research_progress " +
                "LEFT JOIN companies ON companies.id = company_research_progress.id " +
                "LEFT JOIN technologies ON technologies.id = company_research_progress.techid " +
                "WHERE id = @id " +
                "AND completion >= 1",
                (progressData, newcompany, tech) => new CompanyResearchProgress(
                    newcompany,
                    tech,
                    progressData.Completion
                ), new
                {
                    id = company.Id
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

    public async Task<HashSet<Technology>> GetResearchedTechsInField(Company company, TechField field)
    {
        await using var connection = await Db.Get();
        var completedProgress = await connection.QueryAsync
            <CompanyResearchProgressRaw, Company, Technology,CompanyResearchProgress>
            (
                "SELECT * FROM company_research_progress " +
                "LEFT JOIN companies ON companies.id = company_research_progress.id " +
                "LEFT JOIN technologies ON technologies.id = company_research_progress.techid " +
                "WHERE id = @id AND completion >= 1 AND field = @techfield",
                (progressData, newcompany, tech) => new CompanyResearchProgress(
                    newcompany,
                    tech,
                    progressData.Completion
                ), new
                {
                    id = company.Id,
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

    public async Task<decimal> GetTechnologyProgress(Company company, Technology tech)
    {
        await using var connection = await Db.Get();
        var completedProgress = await connection.QuerySingleOrDefaultAsync<CompanyResearchProgressRaw>(
            "SELECT * FROM company_research_progress WHERE id = @id " +
            "AND techid = @tech",
            new {id = company.Id, tech = tech.Id}
        );
        if (completedProgress == null)
            return 0;
        return completedProgress.Completion;
    }

     public async Task<Dictionary<int,CompanyResearchSlot>?> GetResearchSlots(Company company)
    {
        await using var connection = await Db.Get();
        var researchQueues = await connection.QueryAsync
            <CompanyResearchSlotRaw, Company, Technology,CompanyResearchSlot>(
                "SELECT * FROM company_research_slots " +
                "LEFT JOIN companies ON companies.id = company_research_slots.id " +
                "LEFT JOIN technologies ON technologies.id = company_research_slots.techid " +
                "WHERE company_research_slots.id = @id",
                (researchData, newcompany, techid) => new CompanyResearchSlot(
                    newcompany,
                    researchData.SlotNumber,
                    techid
                ), new {id = company.Id});
        if (researchQueues == null)
            return null;
        Dictionary<int,CompanyResearchSlot> slots = new();
        foreach (var slotData in researchQueues)
        {
            slots.Add(slotData.SlotNumber, slotData);
        }
        return slots;
    }

    public async Task<CompanyResearchSlot?> GetResearchSlot(Guid companyId, int slotNumber)
    {
        await using var connection = await Db.Get();
        var researchSlots = await connection.QueryAsync
            <CompanyResearchSlotRaw, Company, Technology,CompanyResearchSlot>(
            "SELECT * FROM company_research_slots " +
                "LEFT JOIN companies ON companies.id = company_research_slots.id " +
                "LEFT JOIN technologies ON technologies.id = company_research_slots.techid " +
                "WHERE company_research_slots.id = @id " +
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

    public async Task<int> UpdateTech(Company company, Technology tech, int points)
    {
        await using var connection = await Db.Get();
        int overflow = points;
        var researchProgress = await connection.QuerySingleOrDefaultAsync<CompanyResearchProgress>(
            "SELECT * FROM company_research_progress WHERE id = @id " +
            "AND techid = @techId",
            new {id = company.Id, techid = tech.Id}
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
            "WHERE id = @id AND techid = @techid",
            new { id = company.Id, techid = tech.Id, percentage = completion}
        );
        return overflow;
    }

    public async Task<float> GetResearchBudget(Company company)
    {
        await using var connection = await Db.Get();
        var metadata = await connection.QuerySingleOrDefaultAsync<CompanyResearchMetaDataRaw>(
            "SELECT * FROM company_research_data " +
            "WHERE id = @id",
            new{id = company.Id}
        );
        return metadata.ResearchBudget;
    }

    public async Task<bool> UpdateResearch(Company company, int researchPoints)
    {
        await using var connection = await Db.Get();
        var currentResearchSlots = await GetResearchSlots(company);
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
            await SetOverflow(company, researchPoints);
            return true;
        }
        var researchPerSlot = (int) MathF.Ceiling((float) researchPoints / activeSlots.Count);
        var overflow = await GetAndClearOverflow(company);
        foreach (var slotData in activeSlots)
        {
            overflow = await UpdateTech(company, slotData.Tech!, researchPerSlot + overflow);
        }

        if (overflow > 0)
        {
            await SetOverflow(company, overflow);
        }
        return true;
    }

    private async Task SetOverflow(Company company,int overflow)
    {
        await using var connection = await Db.Get();
        await connection.QueryAsync(
            "UPDATE company_research_data SET pointoverflow = @newoverflow " +
            "WHERE id = @id",
            new{id = company.Id, newoverflow = overflow}
        );
    }

    private async Task<int> GetAndClearOverflow(Company company)
    {
        await using var connection = await Db.Get();
        var metadata = await connection.QuerySingleOrDefaultAsync<CompanyResearchMetaDataRaw>(
            "SELECT * FROM company_research_data WHERE id = @id ",
            new{id = company.Id}
            );
        var overflow = metadata.PointOverflow;
        await connection.QueryAsync(
            "UPDATE company_research_data SET pointoverflow = 0 " +
            "WHERE id = @id",
            new{id = company.Id}
        );
        return overflow;
    }

}
