using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace StatusWatch.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration cfg)
    {
        var cs = cfg.GetConnectionString("DefaultConnection")
                 ?? "Host=;Port=;Database=;Username=;Password=";

        services.AddDbContext<AppDbContext>(opt =>
            opt.UseNpgsql(cs, o => o.MigrationsAssembly("StatusWatch.Infrastructure")));

        return services;
    }
}
