using Microsoft.AspNetCore.Identity;
using PropPunkShared.Database.Models;
using PropPunkShared.Services;

namespace PropPunkUniverse.Pages.Apply.Country;

public class CountryAppLandingIndex : UserApplicationPageModel
{
    public CountryApplicationModel? CountryApp;
    private IdentityUser? _user;
    public CountryAppLandingIndex(UserApplicationsService userApplications, UserManager<IdentityUser> userMgr)
        : base(userApplications, userMgr)
    {
    }
    public async Task OnGet()
    {
        ApplicationsOpen = UserApplications.CountryApplicationsOpen;
        _user = await GetUserAsync(User);
        if (_user != null) CountryApp = UserApplications.GetCountryApplicationForUser(_user);
    }
}
