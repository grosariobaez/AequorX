using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SchoolERP.Domain.Academic;
using SchoolERP.Domain.Platform;
using SchoolERP.Infrastructure.Persistence;

namespace SchoolERP.IntegrationTests;

public sealed class AcademicStructureIntegrationTests
{
    [Fact]
    public async Task Subject_and_class_minimal_api_enforces_tenant_scoped_uniqueness()
    {
        await using var factory = new ApiFactory(isolateDatabase: true);
        try
        {
            Guid sectionId;
            await using (var scope = factory.Services.CreateAsyncScope())
            {
                var database = scope.ServiceProvider.GetRequiredService<SchoolERPDbContext>();
                await database.Database.MigrateAsync();
                var tenant = new Tenant(factory.TenantId, "Current School", "CURRENT");
                var year = new AcademicYear(factory.TenantId, "2026-2027", new DateOnly(2026, 8, 1), new DateOnly(2027, 6, 30), AcademicYearStatus.Active);
                var section = new Section(year, new GradeLevel(factory.TenantId, "First", "01", 1), new Campus(tenant, "Main", "MAIN"), "A", "A");
                sectionId = section.Id;
                database.Add(tenant);
                database.Add(section);
                await database.SaveChangesAsync();

                var foreignTenant = new Tenant(Guid.NewGuid(), "Foreign School", "FOREIGN");
                database.Add(foreignTenant);
                database.Add(new Subject(foreignTenant.Id, "Mathematics", "MAT"));
                await database.SaveChangesAsync();
            }

            using var client = factory.CreateHttpsClient();
            var subjectResponse = await client.PostAsJsonAsync("/api/subjects", new { name = "Mathematics", code = "mat" });
            subjectResponse.EnsureSuccessStatusCode();
            var subject = Assert.IsType<SubjectResponse>(await subjectResponse.Content.ReadFromJsonAsync<SubjectResponse>());
            Assert.Equal("MAT", subject.Code);

            Assert.Equal(HttpStatusCode.Conflict,
                (await client.PostAsJsonAsync("/api/subjects", new { name = "Other", code = "MAT" })).StatusCode);

            var classResponse = await client.PostAsJsonAsync("/api/classes", new { sectionId, subjectId = subject.Id });
            classResponse.EnsureSuccessStatusCode();
            var @class = Assert.IsType<ClassResponse>(await classResponse.Content.ReadFromJsonAsync<ClassResponse>());
            Assert.Equal(sectionId, @class.SectionId);
            Assert.Equal(subject.Id, @class.SubjectId);

            Assert.Equal(HttpStatusCode.Conflict,
                (await client.PostAsJsonAsync("/api/classes", new { sectionId, subjectId = subject.Id })).StatusCode);
            var classes = await client.GetFromJsonAsync<List<ClassResponse>>($"/api/classes?sectionId={sectionId}");
            Assert.Equal(@class.Id, Assert.Single(Assert.IsType<List<ClassResponse>>(classes)).Id);
        }
        finally
        {
            await using var scope = factory.Services.CreateAsyncScope();
            await scope.ServiceProvider.GetRequiredService<SchoolERPDbContext>().Database.EnsureDeletedAsync();
        }
    }

    private sealed record SubjectResponse(Guid Id, string Name, string Code, bool IsActive);
    private sealed record ClassResponse(Guid Id, Guid SectionId, Guid SubjectId, string Name, string SubjectName, string SubjectCode, bool IsActive);
}
