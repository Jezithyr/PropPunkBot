using PropPunkShared.Core;
using PropPunkShared.Database;
using PropPunkShared.Database.Models;

namespace PropPunkShared.Services;

public sealed class TechnologyService : ScopedServiceBase
{
    private DatabaseContext _db;

    public TechnologyService(DatabaseContext db)
    {
        _db = db;
    }

    //Tries to create a new tech, if one already exists return that instead!
    public TechnologyModel CreateTech(string name, string description, TechUse use,
        TechField field, int cost)
    {
        var tech = _db.Techs.Find(name);
        if (tech != null)
            return tech;

        var techEnt =_db.Techs.Add(
            new TechnologyModel(Guid.NewGuid(), name, description, use, field, cost));
        _db.SaveChanges();
        return techEnt.Entity;
    }

    //TODO: implement remove tech in a reversible way(for now just delete it from the database directly)

    //Tries to create a new tech, if one already exists return that instead!
    public async Task<TechnologyModel> CreateTechAsync(string name, string description, TechUse use,
        TechField field, int cost)
    {
        var tech = await _db.Techs.FindAsync(name);
        if (tech != null)
            return tech;

        var techEnt =_db.Techs.Add(
            new TechnologyModel(Guid.NewGuid(), name, description, use, field, cost));
        await _db.SaveChangesAsync();
        return techEnt.Entity;
    }

    public void SetTechRequirements(TechnologyModel tech, List<TechnologyModel> requiredTechs)
    {
        tech.Requirements.Clear();
        tech.Requirements.AddRange(requiredTechs);
        _db.SaveChanges();
    }

    public async Task SetTechRequirementsAsync(TechnologyModel tech, List<TechnologyModel> requiredTechs)
    {
        tech.Requirements.Clear();
        tech.Requirements.AddRange(requiredTechs);
        await _db.SaveChangesAsync();
    }

    public async Task<TechnologyModel?> GetAsync(string? techId)
    {
        if (!Guid.TryParse(techId, out var guid))
            return null;
        return await GetAsync(guid);
    }

    public async Task<TechnologyModel?> GetAsync(Guid techId)
    {
        return  await _db.Techs.FindAsync(techId);
    }

    public HashSet<TechnologyModel> GetValidPostReqs(List<TechnologyModel> techs)
    {
        HashSet<TechnologyModel> currentTechs = new(techs);
        HashSet<TechnologyModel> uniquePostReqs = new();
        foreach (var tech in techs)
        {
            bool valid = true;
            foreach (var techReqs in tech.Requirements)
            {
                if (currentTechs.Contains(techReqs)) continue;
                valid = false;
                break;
            }

            if (valid)
                uniquePostReqs.Add(tech);
        }

        return uniquePostReqs;
    }
}
