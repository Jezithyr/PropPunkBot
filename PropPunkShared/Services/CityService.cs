using PropPunkShared.Core;
using PropPunkShared.Database;

namespace PropPunkShared.Services;

public sealed class CityService : ScopedServiceBase
{
    private DatabaseContext _db;

    public CityService(DatabaseContext db)
    {
        _db = db;
    }
}
