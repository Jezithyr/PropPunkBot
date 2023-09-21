using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PropPunkShared.Database;

namespace PropPunkUniverse.Pages.Country.Panel;

public class PanelModel : CountryPageModel
{
    private readonly DatabaseContext _db;
    public PanelModel(DatabaseContext db)
    {
        _db = db;
    }
    public virtual async Task OnGet(string? country)
    {
        if (RouteData.Values.TryGetValue("id", out var idOverride))
        {
            country = idOverride?.ToString();
        }
        if (!Guid.TryParse(country, out var guid))
            return;

        Search = await _db.Countries.FirstOrDefaultAsync(c => c.Id == guid);
    }
}
