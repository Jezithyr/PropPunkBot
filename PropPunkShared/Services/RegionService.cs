using Microsoft.EntityFrameworkCore;
using PropPunkShared.Core;
using PropPunkShared.Database;
using PropPunkShared.Database.Models;

namespace PropPunkShared.Services;

public sealed class RegionService : ScopedServiceBase
{
    private DatabaseContext _db;

    public RegionService(DatabaseContext db)
    {
        _db = db;
    }

    public async Task<RegionModel?> GetAsync(string? regionId)
    {
        if (!Guid.TryParse(regionId, out var guid))
            return null;
        return await GetAsync(guid);
    }

    public async Task<RegionModel?> GetAsync(Guid regionId)
    {
        return await _db.Regions.FirstOrDefaultAsync(c => c.Id == regionId);
    }

}
