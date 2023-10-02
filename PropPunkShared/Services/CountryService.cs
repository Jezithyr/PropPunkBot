using Microsoft.EntityFrameworkCore;
using PropPunkShared.Core;
using PropPunkShared.Database;
using PropPunkShared.Database.Models;

namespace PropPunkShared.Services;

public sealed class CountryService : ScopedServiceBase
{
    private DatabaseContext _db;

    public CountryService(DatabaseContext db)
    {
        _db = db;
    }

    public async Task<CountryModel?> GetAsync(string? countryId)
    {
        if (!Guid.TryParse(countryId, out var guid))
            return null;
        return await GetAsync(guid);
    }

    public async Task<CountryModel?> GetAsync(Guid countryId)
    {
        return await _db.Countries.FirstOrDefaultAsync(c => c.Id == countryId);
    }

}
