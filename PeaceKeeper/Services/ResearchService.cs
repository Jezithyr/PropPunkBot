using Dapper;
using Npgsql;
using PeaceKeeper.Database;
using PeaceKeeper.Database.Models;

namespace PeaceKeeper.Services;

public partial class ResearchService : PeacekeeperServiceBase
{
    private readonly TechService _techs;
    private readonly WorldStateService _worldState;

    public async Task<float> GetAotMultiplier(Technology tech)
    {
        var globalSettings = await Settings.GetSettings();
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
            foreach (var possibleTech in await _techs.GetWithRequirement(validTech.Id, connection))
            {
                if (researchedTechs.Contains(possibleTech))
                    continue;
                var valid = true;
                foreach (var possiblePre in await _techs.GetRequirements(possibleTech.Id, connection))
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

    public ResearchService(SettingsService settings, UserService users, DbService db, TechService techs, WorldStateService worldState) : base(settings, users, db)
    {
        _techs = techs;
        _worldState = worldState;
    }
}
