using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SchoolERP.Domain.Academic;
using SchoolERP.Domain.Grading;
using SchoolERP.Domain.People;
using SchoolERP.Domain.Platform;
using SchoolERP.Infrastructure.Persistence;

namespace SchoolERP.IntegrationTests;

public sealed class GradingIntegrationTests
{
    [Fact]
    public async Task Minimal_api_saves_draft_publishes_and_preserves_correction_history()
    {
        await using var factory = new ApiFactory(isolateDatabase: true);
        try
        {
            var setup = await SeedAsync(factory);
            using var client = factory.CreateHttpsClient();
            var created = await client.PostAsJsonAsync("/api/assessments", new
            {
                sectionId = setup.SectionId,
                name = "Quiz 1",
                assessmentDate = "2026-09-01",
                maximumScore = 100
            });
            created.EnsureSuccessStatusCode();
            var assessment = Assert.IsType<AssessmentResponse>(await created.Content.ReadFromJsonAsync<AssessmentResponse>());

            var initial = await client.GetFromJsonAsync<GradeRosterResponse>($"/api/assessments/{assessment.Id}/grades");
            Assert.Null(Assert.Single(Assert.IsType<GradeRosterResponse>(initial).Students).Status);

            var draftResponse = await client.PutAsJsonAsync($"/api/assessments/{assessment.Id}/grades/{setup.EnrollmentId}",
                new { score = 80, createdBy = "client-spoof" });
            draftResponse.EnsureSuccessStatusCode();
            var draft = Assert.IsType<GradeStudentResponse>(await draftResponse.Content.ReadFromJsonAsync<GradeStudentResponse>());
            Assert.Equal("Draft", draft.Status);

            (await client.PostAsJsonAsync($"/api/assessments/{assessment.Id}/publish", new { })).EnsureSuccessStatusCode();
            var overwrite = await client.PutAsJsonAsync($"/api/assessments/{assessment.Id}/grades/{setup.EnrollmentId}", new { score = 90 });
            Assert.Equal(HttpStatusCode.Conflict, overwrite.StatusCode);

            var correctionResponse = await client.PostAsJsonAsync($"/api/grades/{draft.GradeId}/corrections",
                new { score = 90, reason = "Entry error", correctedBy = "client-spoof" });
            correctionResponse.EnsureSuccessStatusCode();

            await using var scope = factory.Services.CreateAsyncScope();
            var database = scope.ServiceProvider.GetRequiredService<SchoolERPDbContext>();
            var grade = await database.Grades.SingleAsync();
            var correction = await database.GradeCorrections.SingleAsync();
            Assert.Equal(GradeStatus.Corrected, grade.Status);
            Assert.Equal(90, grade.Score);
            Assert.Equal(80, correction.PreviousScore);
            Assert.Equal(90, correction.NewScore);
            Assert.Equal("Entry error", correction.Reason);
            Assert.Equal("integration-test-user", correction.CorrectedBy);
        }
        finally { await DeleteDatabase(factory); }
    }

    [Fact]
    public async Task Database_prevents_duplicate_grade()
    {
        await using var factory = new ApiFactory(isolateDatabase: true);
        try
        {
            var setup = await SeedAsync(factory);
            await using var scope = factory.Services.CreateAsyncScope();
            var database = scope.ServiceProvider.GetRequiredService<SchoolERPDbContext>();
            var enrollment = await database.Enrollments.Include(x => x.AcademicYear).Include(x => x.Section).SingleAsync(x => x.Id == setup.EnrollmentId);
            var section = await database.Sections.Include(x => x.AcademicYear).SingleAsync(x => x.Id == setup.SectionId);
            var assessment = new Assessment(section, "Quiz", new DateOnly(2026, 9, 1), 100);
            database.AddRange(new Grade(assessment, enrollment, 80, "teacher"), new Grade(assessment, enrollment, 90, "teacher"));
            await Assert.ThrowsAsync<DbUpdateException>(() => database.SaveChangesAsync());
        }
        finally { await DeleteDatabase(factory); }
    }

    [Fact]
    public async Task Assessment_api_hides_foreign_tenant_section()
    {
        await using var factory = new ApiFactory(isolateDatabase: true);
        try
        {
            await MigrateAsync(factory);
            var foreign = BuildEnrollment(Guid.NewGuid(), "FOREIGN");
            await using (var scope = factory.Services.CreateAsyncScope())
            {
                var database = scope.ServiceProvider.GetRequiredService<SchoolERPDbContext>();
                database.Add(foreign.Tenant); database.Add(foreign.Enrollment); await database.SaveChangesAsync();
            }
            using var client = factory.CreateHttpsClient();
            Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync($"/api/assessments?sectionId={foreign.Section.Id}")).StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, (await client.PostAsJsonAsync("/api/assessments", new
            { sectionId = foreign.Section.Id, name = "Quiz", assessmentDate = "2026-09-01", maximumScore = 100 })).StatusCode);
        }
        finally { await DeleteDatabase(factory); }
    }

    private static async Task<SetupIds> SeedAsync(ApiFactory factory)
    {
        await MigrateAsync(factory);
        await using var scope = factory.Services.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<SchoolERPDbContext>();
        var graph = BuildEnrollment(factory.TenantId, "CURRENT");
        database.Add(graph.Tenant); database.Add(graph.Enrollment); await database.SaveChangesAsync();
        return new SetupIds(graph.Section.Id, graph.Enrollment.Id);
    }

    private static EnrollmentGraph BuildEnrollment(Guid tenantId, string code)
    {
        var tenant = new Tenant(tenantId, $"School {code}", code);
        var year = new AcademicYear(tenantId, "2026-2027", new DateOnly(2026, 8, 1), new DateOnly(2027, 6, 30), AcademicYearStatus.Active);
        var section = new Section(year, new GradeLevel(tenantId, "First", $"01-{code}", 1), new Campus(tenant, "Main", $"MAIN-{code}"), "A", $"A-{code}");
        var student = new StudentProfile(new Person(tenantId, "Ana", "Pérez"), $"S-{code}");
        return new EnrollmentGraph(tenant, section, new Enrollment(student, year, section, EnrollmentStatus.Active, new DateOnly(2026, 8, 15)));
    }

    private static async Task MigrateAsync(ApiFactory factory) { await using var scope = factory.Services.CreateAsyncScope(); await scope.ServiceProvider.GetRequiredService<SchoolERPDbContext>().Database.MigrateAsync(); }
    private static async Task DeleteDatabase(ApiFactory factory) { await using var scope = factory.Services.CreateAsyncScope(); await scope.ServiceProvider.GetRequiredService<SchoolERPDbContext>().Database.EnsureDeletedAsync(); }
    private sealed record EnrollmentGraph(Tenant Tenant, Section Section, Enrollment Enrollment);
    private sealed record SetupIds(Guid SectionId, Guid EnrollmentId);
    private sealed record AssessmentResponse(Guid Id, Guid SectionId, string Name, DateOnly AssessmentDate, decimal MaximumScore, bool IsActive);
    private sealed record GradeRosterResponse(Guid AssessmentId, string AssessmentName, decimal MaximumScore, List<GradeStudentResponse> Students);
    private sealed record GradeStudentResponse(Guid? GradeId, Guid EnrollmentId, string StudentNumber, string StudentName, decimal? Score, string? Status);
}
