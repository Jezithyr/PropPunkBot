using Microsoft.AspNetCore.Mvc.RazorPages;
using PropPunkShared.Data;
using PropPunkUniverse.Data;

namespace PropPunkUniverse.Pages;

public class IndexModel : PageModel
{
    private readonly DatabaseContext _db;

    public IndexModel(DatabaseContext db)
    {
        _db = db;
    }

    public void OnGet()
    {
    }
}
