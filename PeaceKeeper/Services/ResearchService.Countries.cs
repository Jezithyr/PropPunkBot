using Dapper;
using Npgsql;
using PeaceKeeper.Database.Models;

namespace PeaceKeeper.Services;

public partial class ResearchService
{
    public async Task<List<CountryResearch>?> GetCountryResearchQueue(Guid countryId, NpgsqlConnection? dbConnection = null)
    {
        await using var connection = await _db.ResolveDatabase(dbConnection);
        var researchQueues = await connection.QueryAsync
            <CountryResearchRaw, Country, Technology,CountryResearch>(
                "SELECT * FROM country_research_queues " +
                "LEFT JOIN countries ON countries.id = country_research_queues.countryid " +
                "LEFT JOIN technologies ON technologies.id = country_research_queues.techid " +
                "WHERE country_research_queues.countryid = @id",
                (researchData, country, tech1) => new CountryResearch(
                    country,
                    researchData.SlotNumber,
                    tech1,
                    researchData.Points
                ), new {id = countryId});
        return researchQueues?.ToList();
    }

}
