using Dapper;
using Discord.WebSocket;
using Npgsql;
using PeaceKeeper.Database;
using PeaceKeeper.Database.Models;

namespace PeaceKeeper.Services;

public partial class ResearchService : PeacekeeperServiceBase
{
    private readonly TechService _techs;

    public async Task<float> GetAotMultiplier(Technology tech)
    {
        var globalSettings = await Settings.GetSettings();
        var worldState = await WorldState.Get();
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

    private async Task<HashSet<Technology>> GetValidTechs(HashSet<Technology> researchedTechs)
    {
        HashSet<Technology> validTechs = new();
        foreach (var validTech in researchedTechs)
        {
            foreach (var possibleTech in await _techs.GetWithRequirement(validTech.Id))
            {
                if (researchedTechs.Contains(possibleTech))
                    continue;
                var valid = true;
                foreach (var possiblePre in await _techs.GetRequirements(possibleTech.Id))
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

    
    
    public ResearchService(SettingsService settings, PermissionsService perms, UserService users, DbService db, WorldStateService worldState, DiscordSocketClient client, TechService techs) : base(settings, perms, users, db, worldState, client)
    {
        _techs = techs;
        WorldState.RegisterTickEvent(OnWorldTick);
    }

    private void OnWorldTick(int year, int quarter, DateOnly date)
    {

    }
}
