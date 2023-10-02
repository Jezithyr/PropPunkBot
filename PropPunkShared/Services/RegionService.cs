using PropPunkShared.Core;
using PropPunkShared.Database;

namespace PropPunkShared.Services;

public sealed class RegionService : ScopedServiceBase
{
    private DatabaseContext _db;

    public RegionService(DatabaseContext db)
    {
        _db = db;
    }
}
