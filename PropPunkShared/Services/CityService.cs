using Microsoft.EntityFrameworkCore;
using PropPunkShared.Core;
using PropPunkShared.Database;
using PropPunkShared.Database.Models;

namespace PropPunkShared.Services;

public sealed class CityService : ScopedServiceBase
{
    private DatabaseContext _db;

    public CityService(DatabaseContext db)
    {
        _db = db;
    }

    public async Task<CityModel?> GetAsync(string? cityId)
    {
        if (!Guid.TryParse(cityId, out var guid))
            return null;
        return await GetAsync(guid);
    }

    public async Task<CityModel?> GetAsync(Guid cityId)
    {
        return await _db.Cities.FirstOrDefaultAsync(c => c.Id == cityId);
    }
}
