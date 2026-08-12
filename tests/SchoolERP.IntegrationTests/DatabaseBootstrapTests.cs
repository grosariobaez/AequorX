using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using SchoolERP.Infrastructure.Persistence;

namespace SchoolERP.IntegrationTests;

public sealed class DatabaseBootstrapTests
{
    [Fact]
    public async Task Phase23_migration_refuses_to_invent_class_mapping_for_existing_assessments()
    {
        await using var factory = new ApiFactory(isolateDatabase: true);
        await using var scope = factory.Services.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<SchoolERPDbContext>();
        var script = database.GetService<IMigrator>().GenerateScript(
            "20260811205022_Phase22AssessmentGradesFoundation",
            "20260811235234_Phase23SubjectsClassesFoundation");

        Assert.Contains("requires an explicit Subject/Class mapping", script, StringComparison.Ordinal);
        Assert.DoesNotContain("DROP TABLE [Grades]", script, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("DROP TABLE [GradeCorrections]", script, StringComparison.OrdinalIgnoreCase);
    }

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
                Assert.Contains(
                    appliedMigrations,
                    migration => migration.EndsWith(
                        "_Phase21AttendanceFoundation",
                        StringComparison.Ordinal));
                Assert.Contains(appliedMigrations, migration => migration.EndsWith(
                    "_Phase22AssessmentGradesFoundation", StringComparison.Ordinal));
                Assert.Contains(appliedMigrations, migration => migration.EndsWith(
                    "_Phase23SubjectsClassesFoundation", StringComparison.Ordinal));
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
