using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SchoolERP.Infrastructure.Persistence;

namespace SchoolERP.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<SchoolERPDbContext>((serviceProvider, options) =>
        {
            var currentConfiguration = serviceProvider.GetRequiredService<IConfiguration>();
            var connectionString = currentConfiguration.GetConnectionString("SchoolERP")
                ?? throw new InvalidOperationException(
                    "Connection string 'SchoolERP' must be configured outside source control.");

            options.UseSqlServer(
                connectionString,
                sql => sql.MigrationsAssembly(typeof(SchoolERPDbContext).Assembly.FullName));
        });

        services
            .AddHealthChecks()
            .AddDbContextCheck<SchoolERPDbContext>("database", tags: ["ready"]);

        return services;
    }
}
