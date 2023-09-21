using PropPunkShared.Database;
using PropPunkShared.Services;

namespace PropPunkUniverse.Services;

public sealed class CountryService : ScopedServiceBase
{
    private DatabaseContext _db;

    public CountryService(DatabaseContext db)
    {
        _db = db;
    }


}
