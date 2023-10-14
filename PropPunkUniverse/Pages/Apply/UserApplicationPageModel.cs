using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PropPunkShared.Database;
using PropPunkShared.Services;

namespace PropPunkUniverse.Pages.Apply;

public abstract class UserApplicationPageModel : PageModel
{
    public bool ApplicationsOpen;
    public readonly UserApplicationsService UserApplications;
    protected UserManager<IdentityUser> UserManager;

    protected UserApplicationPageModel(UserApplicationsService userApplications, UserManager<IdentityUser> userManager)
    {
        UserApplications = userApplications;
        UserManager = userManager;
    }

    protected async Task<IdentityUser?> GetUserAsync(ClaimsPrincipal? claimsPrincipal)
    {
        if (claimsPrincipal == null)
            return null;
        return await UserManager.GetUserAsync(claimsPrincipal);
    }
}
