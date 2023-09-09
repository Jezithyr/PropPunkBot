using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PropPunkShared.Data.Models;

namespace PropPunkShared.Data;

public class DatabaseContext : IdentityDbContext
{
    public DbSet<Country> Countries { get; set; } = default!;

    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
    {
    }
}
