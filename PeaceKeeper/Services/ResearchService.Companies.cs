using Dapper;
using Npgsql;
using PeaceKeeper.Database.Models;

namespace PeaceKeeper.Services;

public partial class ResearchService
{
    public async Task<List<CompanyResearch>?> GetCompanyResearchQueue(Guid countryId, NpgsqlConnection? dbConnection = null)
    {
        await using var connection = await _db.ResolveDatabase(dbConnection);
        var researchQueues = await connection.QueryAsync
            <CompanyResearchRaw, Company, Technology,CompanyResearch>(
                "SELECT * FROM company_research_queues " +
                "LEFT JOIN countries ON countries.id = company_research_queues.companyid " +
                "LEFT JOIN technologies ON technologies.id = company_research_queues.techid " +
                "WHERE company_research_queues.companyid = @id",
                (researchData, company, tech) => new CompanyResearch(
                    company,
                    researchData.SlotNumber,
                    tech,
                    researchData.Points
                ), new {id = countryId});
        return researchQueues?.ToList();
    }
}
