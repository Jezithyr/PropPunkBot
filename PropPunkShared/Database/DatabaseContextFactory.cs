using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PropPunkShared.Database;

public class DatabaseContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
{
    public DatabaseContext CreateDbContext(string[] args)
    {
        Env.EnsureLoadEnvFile();
        var options = new DbContextOptionsBuilder<DatabaseContext>().UseNpgsql(Env.CreateConnectionString()).Options;
        return new DatabaseContext(options);
    }
}
