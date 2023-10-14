using Microsoft.AspNetCore.Identity;
using PropPunkShared.Database.Models;
using PropPunkShared.Services;

namespace PropPunkUniverse.Pages.Apply.Country.App;

public class CountryAppIndex : UserApplicationPageModel
{
    public CountryApplicationModel? CountryApp;
    private IdentityUser? _user;
    public CountryAppIndex(UserApplicationsService userApplications, UserManager<IdentityUser> userMgr)
        : base(userApplications, userMgr)
    {
    }
    public async Task OnGet()
    {
        _user = await GetUserAsync(User);
        if (_user != null) CountryApp = UserApplications.GetCountryApplicationForUser(_user);
    }
}
