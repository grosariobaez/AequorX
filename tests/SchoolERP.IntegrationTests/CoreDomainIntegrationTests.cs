using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SchoolERP.Domain.People;
using SchoolERP.Domain.Platform;
using SchoolERP.Infrastructure.Persistence;

namespace SchoolERP.IntegrationTests;

public sealed class CoreDomainIntegrationTests
{
    [Fact]
    public async Task Tenant_filter_hides_other_tenant_from_database_and_api()
    {
        await using var factory = new ApiFactory(isolateDatabase: true);

        try
        {
            Guid foreignPersonId;
            await using (var scope = factory.Services.CreateAsyncScope())
            {
                var database = scope.ServiceProvider.GetRequiredService<SchoolERPDbContext>();
                await database.Database.MigrateAsync();

                var currentTenant = new Tenant(factory.TenantId, "Current School", "CURRENT");
                var foreignTenant = new Tenant("Foreign School", "FOREIGN");
                var currentPerson = new Person(factory.TenantId, "Ana", "Pérez");
                var foreignPerson = new Person(foreignTenant.Id, "Luis", "Gómez");
                foreignPersonId = foreignPerson.Id;

                database.AddRange(currentTenant, foreignTenant, currentPerson, foreignPerson);
                await database.SaveChangesAsync();

                var visiblePeople = await database.People.AsNoTracking().ToListAsync();
                Assert.Collection(
                    visiblePeople,
                    person => Assert.Equal(currentPerson.Id, person.Id));
            }

            using var client = factory.CreateHttpsClient();
            var response = await client.GetAsync($"/api/people/{foreignPersonId}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        finally
        {
            await DeleteDatabase(factory);
        }
    }

    [Fact]
    public async Task Tenant_aware_unique_constraints_reject_duplicate_codes_and_numbers()
    {
        await using var factory = new ApiFactory(isolateDatabase: true);

        try
        {
            await using var scope = factory.Services.CreateAsyncScope();
            var database = scope.ServiceProvider.GetRequiredService<SchoolERPDbContext>();
            await database.Database.MigrateAsync();

            var tenant = new Tenant(factory.TenantId, "Current School", "CURRENT");
            database.Add(tenant);
            database.Campuses.AddRange(
                new Campus(tenant, "Main", "MAIN"),
                new Campus(tenant, "Other", "MAIN"));

            await Assert.ThrowsAsync<DbUpdateException>(() => database.SaveChangesAsync());

            database.ChangeTracker.Clear();
            database.Add(tenant);
            var firstPerson = new Person(factory.TenantId, "Ana", "Pérez");
            var secondPerson = new Person(factory.TenantId, "Luis", "Gómez");
            database.AddRange(
                firstPerson,
                secondPerson,
                new StudentProfile(firstPerson, "S-001"),
                new StudentProfile(secondPerson, "S-001"));

            await Assert.ThrowsAsync<DbUpdateException>(() => database.SaveChangesAsync());
        }
        finally
        {
            await DeleteDatabase(factory);
        }
    }

    [Fact]
    public async Task Minimal_api_creates_student_structure_and_enrollment()
    {
        await using var factory = new ApiFactory(isolateDatabase: true);

        try
        {
            await Migrate(factory);
            using var client = factory.CreateHttpsClient();

            var tenantResponse = await client.PostAsJsonAsync(
                "/api/tenant",
                new { name = "Colegio Demostración", code = "DEMO" });
            tenantResponse.EnsureSuccessStatusCode();

            var campusId = await PostAndReadId(
                client,
                "/api/campuses",
                new { name = "Campus Principal", code = "MAIN" });
            var personId = await PostAndReadId(
                client,
                "/api/people",
                new { firstName = "Ana", lastName = "Pérez" });

            var profileResponse = await client.PostAsJsonAsync(
                $"/api/students/{personId}/profile",
                new { studentNumber = "S-001" });
            profileResponse.EnsureSuccessStatusCode();

            var academicYearId = await PostAndReadId(
                client,
                "/api/academic-years",
                new
                {
                    name = "2026-2027",
                    startDate = "2026-08-01",
                    endDate = "2027-06-30",
                    status = "Active"
                });
            var gradeLevelId = await PostAndReadId(
                client,
                "/api/grade-levels",
                new { name = "Primero", code = "01", sortOrder = 1 });
            var sectionId = await PostAndReadId(
                client,
                "/api/sections",
                new
                {
                    academicYearId,
                    gradeLevelId,
                    campusId,
                    name = "A",
                    code = "A",
                    capacity = 30
                });
            var enrollmentId = await PostAndReadId(
                client,
                "/api/enrollments",
                new
                {
                    studentPersonId = personId,
                    academicYearId,
                    sectionId,
                    status = "Active",
                    enrollmentDate = "2026-08-15"
                });

            var enrollments = await client.GetFromJsonAsync<List<EnrollmentResponse>>(
                "/api/enrollments");

            Assert.NotEqual(Guid.Empty, enrollmentId);
            Assert.Collection(
                Assert.IsType<List<EnrollmentResponse>>(enrollments),
                enrollment =>
                {
                    Assert.Equal(personId, enrollment.StudentPersonId);
                    Assert.Equal(sectionId, enrollment.SectionId);
                    Assert.Equal("Active", enrollment.Status);
                });
        }
        finally
        {
            await DeleteDatabase(factory);
        }
    }

    private static async Task<Guid> PostAndReadId(
        HttpClient client,
        string path,
        object request)
    {
        var response = await client.PostAsJsonAsync(path, request);
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<IdentifierResponse>();
        return Assert.IsType<IdentifierResponse>(body).Id;
    }

    private static async Task Migrate(ApiFactory factory)
    {
        await using var scope = factory.Services.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<SchoolERPDbContext>();
        await database.Database.MigrateAsync();
    }

    private static async Task DeleteDatabase(ApiFactory factory)
    {
        await using var scope = factory.Services.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<SchoolERPDbContext>();
        await database.Database.EnsureDeletedAsync();
    }

    private sealed record IdentifierResponse(Guid Id);

    private sealed record EnrollmentResponse(
        Guid Id,
        Guid StudentPersonId,
        Guid SectionId,
        string Status);
}
