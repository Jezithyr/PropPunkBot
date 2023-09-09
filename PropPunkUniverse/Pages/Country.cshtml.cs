using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PropPunkShared.Database;

namespace PropPunkUniverse.Pages;

public class Country : PageModel
{
    private readonly DatabaseContext _db;
    public PropPunkShared.Database.Models.Country? Search;

    public Country(DatabaseContext db)
    {
        _db = db;
    }

    public async Task OnGet(string country)
    {
        if (!Guid.TryParse(country, out var guid))
            return;

        Search = await _db.Countries.FirstOrDefaultAsync(c => c.Id == guid);
    }
}
