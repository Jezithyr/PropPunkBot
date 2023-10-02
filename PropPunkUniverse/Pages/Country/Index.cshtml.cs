using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PropPunkShared.Database;
using PropPunkShared.Services;

namespace PropPunkUniverse.Pages.Country;

public abstract class CountryPageModel : PageModel
{
    public PropPunkShared.Database.Models.CountryModel? Search;
}

public class CountryIndex : CountryPageModel
{
    private readonly CountryService _countries;

    public CountryIndex(CountryService countries)
    {
        _countries = countries;
    }
    public async Task OnGet(string? country)
    {
        Search = await _countries.GetAsync(country);
    }
}
