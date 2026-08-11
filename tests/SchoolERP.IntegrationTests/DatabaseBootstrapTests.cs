using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SchoolERP.Infrastructure.Persistence;

namespace SchoolERP.IntegrationTests;

public sealed class DatabaseBootstrapTests
{
    [Fact]
    public async Task Baseline_migration_applies_and_readiness_is_healthy()
    {
        await using var factory = new ApiFactory(isolateDatabase: true);

        try
        {
            await using (var scope = factory.Services.CreateAsyncScope())
            {
                var database = scope.ServiceProvider.GetRequiredService<SchoolERPDbContext>();
                Assert.False(await database.Database.CanConnectAsync());
                await database.Database.MigrateAsync();

                var appliedMigrations = await database.Database.GetAppliedMigrationsAsync();
                Assert.Contains(
                    appliedMigrations,
                    migration => migration.EndsWith("_InitialBootstrap", StringComparison.Ordinal));
                Assert.Contains(
                    appliedMigrations,
                    migration => migration.EndsWith(
                        "_Phase20CoreDomainFoundation",
                        StringComparison.Ordinal));
            }

            using var client = factory.CreateHttpsClient();
            var response = await client.GetAsync("/health/ready");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        finally
        {
            await using var scope = factory.Services.CreateAsyncScope();
            var database = scope.ServiceProvider.GetRequiredService<SchoolERPDbContext>();
            await database.Database.EnsureDeletedAsync();
        }
    }
}
