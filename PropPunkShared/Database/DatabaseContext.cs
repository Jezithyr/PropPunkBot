using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PropPunkShared.Database.Models;

namespace PropPunkShared.Database;

public class DatabaseContext : IdentityDbContext
{
    public DbSet<CountryModel> Countries { get; set; } = default!;

    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<CountryModel>()
            .Property(c => c.Id)
            .HasDefaultValueSql("gen_random_uuid()");
    }
}
