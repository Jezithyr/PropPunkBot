using Microsoft.AspNetCore.Mvc.RazorPages;
using PropPunkShared.Database;
using PropPunkShared.Database.Models;
using PropPunkShared.Services;

namespace PropPunkUniverse.Pages.Country;
public class List : PageModel
{
    public readonly DatabaseContext Db;
    public readonly ConfigService Config;

    public List(DatabaseContext db, ConfigService config)
    {
        Db = db;
        Config = config;
    }
    public void OnGet()
    {
    }
}
