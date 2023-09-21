using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PropPunkShared.Database;

namespace PropPunkUniverse.Pages.Country;

public abstract class CountryPageModel : PageModel
{
    public PropPunkShared.Database.Models.CountryModel? Search;
}

public class CountryIndex : CountryPageModel
{
    private readonly DatabaseContext _db;


    public CountryIndex(DatabaseContext db)
    {
        _db = db;
    }
    public async Task OnGet(string? country)
    {
        if (!Guid.TryParse(country, out var guid))
            return;

        Search = await _db.Countries.FirstOrDefaultAsync(c => c.Id == guid);
    }
}
