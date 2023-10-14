using PropPunkShared.Core;
using PropPunkShared.Database;

namespace PropPunkShared.Services;

public sealed class ResearchService : ScopedServiceBase
{
    private DatabaseContext _db;

    public ResearchService(DatabaseContext db)
    {
        _db = db;
    }
}
