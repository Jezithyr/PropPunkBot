using Dapper;
using Npgsql;
using PeaceKeeper.Database;
using PeaceKeeper.Database.Models;

namespace PeaceKeeper.Services;

public partial class ResearchService : PeacekeeperServiceBase
{
    private readonly DbService _db;
    private readonly SettingsService _settings;
    private readonly UserService _users;
    private readonly TechService _tech;
    private readonly WorldStateService _worldState;
    public ResearchService(DbService db, SettingsService settings, UserService users, TechService tech,
        WorldStateService worldState)
    {
        _db = db;
        _settings = settings;
        _users = users;
        _tech = tech;
        _worldState = worldState;
    }
    public async Task<float> GetAotMultiplier(Technology tech)
    {
        var globalSettings = await _settings.GetSettings();
        var worldState = await _worldState.GetWorldState();
        var percentAot = tech.Year - worldState.Year/globalSettings.AotYearStart;
        var aotModifier = MathF.Pow(globalSettings.AotScaleFactor, MathF.Max(percentAot, 1) + 1)
                          - globalSettings.AotScaleFactor;
        return aotModifier;
    }

    public async Task<int> GetAdjustedTechCost(Technology tech)
    {
        var aotCost = await GetAotMultiplier(tech) ;
        return (int)MathF.Ceiling(aotCost* tech.Cost);
    }

    private async Task<HashSet<Technology>> GetValidTechs(HashSet<Technology> researchedTechs,
        NpgsqlConnection connection)
    {
        HashSet<Technology> validTechs = new();
        foreach (var validTech in researchedTechs)
        {
            foreach (var possibleTech in await _tech.GetWithRequirement(validTech.Id, connection))
            {
                if (researchedTechs.Contains(possibleTech))
                    continue;
                var valid = true;
                foreach (var possiblePre in await _tech.GetRequirements(possibleTech.Id, connection))
                {
                    if (researchedTechs.Contains(possiblePre))
                        continue;
                    valid = false;
                }
                if (!valid)
                    validTechs.Add(possibleTech);
            }
        }
        return validTechs;
    }

}
