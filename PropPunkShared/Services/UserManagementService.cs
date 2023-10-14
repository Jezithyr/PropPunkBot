using Microsoft.AspNetCore.Identity;
using PropPunkShared.Core;
using PropPunkShared.Database;

namespace PropPunkShared.Services;

public sealed class UserManagementService : ScopedServiceBase
{
    public UserManager<IdentityUser> UserManager;
    private DatabaseContext _db;

    public UserManagementService(UserManager<IdentityUser> userManager, DatabaseContext db)
    {
        UserManager = userManager;
        _db = db;
    }



}
