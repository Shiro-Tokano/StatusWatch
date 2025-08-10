using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace StatusWatch.Infrastructure;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var cs =
            Environment.GetEnvironmentVariable("POSTGRES_CONNECTION")
            ?? "Host=localhost;Port=5432;Database=statuswatch;Username=;Password=";

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(cs, o => o.MigrationsAssembly("StatusWatch.Infrastructure"))
            .Options;

        return new AppDbContext(options);
    }
}
