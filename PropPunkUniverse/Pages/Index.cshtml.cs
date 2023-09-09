using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PropPunkShared.Database;

namespace PropPunkUniverse.Pages;

public class IndexModel : PageModel
{
    private readonly DatabaseContext _db;
    public PropPunkShared.Database.Models.Country? Country;

    public IndexModel(DatabaseContext db)
    {
        _db = db;
    }

    public async Task OnGet()
    {
        Country = await _db.Countries.FirstOrDefaultAsync();
    }
}
