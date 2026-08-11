using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SchoolERP.Domain.Academic;
using SchoolERP.Domain.Attendance;
using SchoolERP.Domain.People;
using SchoolERP.Domain.Platform;
using SchoolERP.Infrastructure.Persistence;

namespace SchoolERP.IntegrationTests;

public sealed class AttendanceIntegrationTests
{
    [Fact]
    public async Task Attendance_api_defaults_present_corrects_exception_and_restores_present()
    {
        await using var factory = new ApiFactory(isolateDatabase: true);

        try
        {
            var setup = await SeedAsync(factory);
            using var client = factory.CreateHttpsClient();
            var path = $"/api/attendance?sectionId={setup.SectionId}&date=2026-09-01";

            var initial = await client.GetFromJsonAsync<AttendanceRosterResponse>(path);
            var present = Assert.Single(Assert.IsType<AttendanceRosterResponse>(initial).Students);
            Assert.Equal("Present", present.Status);

            var createdResponse = await client.PutAsJsonAsync(
                $"/api/attendance/{setup.EnrollmentId}/2026-09-01",
                new
                {
                    status = "Absent",
                    note = "Family report",
                    createdBy = "client-spoof"
                });
            createdResponse.EnsureSuccessStatusCode();
            var created = await createdResponse.Content.ReadFromJsonAsync<AttendanceStudentResponse>();
            Assert.Equal("Absent", Assert.IsType<AttendanceStudentResponse>(created).Status);
            Assert.Equal("integration-test-user", created.CreatedBy);
            Assert.Equal("integration-test-user", created.UpdatedBy);

            Guid recordId;
            DateTimeOffset createdAt;
            await using (var scope = factory.Services.CreateAsyncScope())
            {
                var database = scope.ServiceProvider.GetRequiredService<SchoolERPDbContext>();
                var record = await database.AttendanceRecords.SingleAsync();
                recordId = record.Id;
                createdAt = record.CreatedAt;
            }

            var correctedResponse = await client.PutAsJsonAsync(
                $"/api/attendance/{setup.EnrollmentId}/2026-09-01",
                new { status = "Late", note = "Corrected" });
            correctedResponse.EnsureSuccessStatusCode();

            await using (var scope = factory.Services.CreateAsyncScope())
            {
                var database = scope.ServiceProvider.GetRequiredService<SchoolERPDbContext>();
                var record = await database.AttendanceRecords.SingleAsync();
                Assert.Equal(recordId, record.Id);
                Assert.Equal(createdAt, record.CreatedAt);
                Assert.Equal("integration-test-user", record.CreatedBy);
                Assert.Equal("integration-test-user", record.UpdatedBy);
                Assert.Equal(AttendanceStatus.Late, record.Status);
            }

            var presentResponse = await client.PutAsJsonAsync(
                $"/api/attendance/{setup.EnrollmentId}/2026-09-01",
                new { status = "Present", note = (string?)null });
            presentResponse.EnsureSuccessStatusCode();
            var restored = await presentResponse.Content.ReadFromJsonAsync<AttendanceStudentResponse>();
            Assert.Equal("Present", Assert.IsType<AttendanceStudentResponse>(restored).Status);

            await using (var scope = factory.Services.CreateAsyncScope())
            {
                var database = scope.ServiceProvider.GetRequiredService<SchoolERPDbContext>();
                Assert.Empty(await database.AttendanceRecords.ToListAsync());
            }
        }
        finally
        {
            await DeleteDatabase(factory);
        }
    }

    [Fact]
    public async Task Database_prevents_duplicate_attendance_exception()
    {
        await using var factory = new ApiFactory(isolateDatabase: true);

        try
        {
            var setup = await SeedAsync(factory);
            await using var scope = factory.Services.CreateAsyncScope();
            var database = scope.ServiceProvider.GetRequiredService<SchoolERPDbContext>();
            var enrollment = await database.Enrollments
                .Include(entity => entity.AcademicYear)
                .Include(entity => entity.Section)
                .SingleAsync(entity => entity.Id == setup.EnrollmentId);

            database.AttendanceRecords.AddRange(
                new AttendanceRecord(
                    enrollment,
                    enrollment.Section,
                    new DateOnly(2026, 9, 1),
                    AttendanceStatus.Absent,
                    null,
                    "teacher"),
                new AttendanceRecord(
                    enrollment,
                    enrollment.Section,
                    new DateOnly(2026, 9, 1),
                    AttendanceStatus.Late,
                    null,
                    "teacher"));

            await Assert.ThrowsAsync<DbUpdateException>(() => database.SaveChangesAsync());
        }
        finally
        {
            await DeleteDatabase(factory);
        }
    }

    [Fact]
    public async Task Attendance_api_hides_foreign_tenant_section_and_enrollment()
    {
        await using var factory = new ApiFactory(isolateDatabase: true);

        try
        {
            await MigrateAsync(factory);
            Guid foreignSectionId;
            Guid foreignEnrollmentId;

            await using (var scope = factory.Services.CreateAsyncScope())
            {
                var database = scope.ServiceProvider.GetRequiredService<SchoolERPDbContext>();
                var foreign = BuildEnrollment(Guid.NewGuid(), "FOREIGN");
                foreignSectionId = foreign.Section.Id;
                foreignEnrollmentId = foreign.Enrollment.Id;
                database.Add(foreign.Tenant);
                database.Add(foreign.Enrollment);
                await database.SaveChangesAsync();
            }

            using var client = factory.CreateHttpsClient();
            var getResponse = await client.GetAsync(
                $"/api/attendance?sectionId={foreignSectionId}&date=2026-09-01");
            var putResponse = await client.PutAsJsonAsync(
                $"/api/attendance/{foreignEnrollmentId}/2026-09-01",
                new { status = "Absent", note = (string?)null });

            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, putResponse.StatusCode);
        }
        finally
        {
            await DeleteDatabase(factory);
        }
    }

    private static async Task<AttendanceSetup> SeedAsync(ApiFactory factory)
    {
        await MigrateAsync(factory);
        await using var scope = factory.Services.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<SchoolERPDbContext>();
        var setup = BuildEnrollment(factory.TenantId, "CURRENT");
        database.Add(setup.Tenant);
        database.Add(setup.Enrollment);
        await database.SaveChangesAsync();
        return new AttendanceSetup(setup.Enrollment.Id, setup.Section.Id);
    }

    private static EnrollmentGraph BuildEnrollment(Guid tenantId, string code)
    {
        var tenant = new Tenant(tenantId, $"School {code}", code);
        var year = new AcademicYear(
            tenantId,
            "2026-2027",
            new DateOnly(2026, 8, 1),
            new DateOnly(2027, 6, 30),
            AcademicYearStatus.Active);
        var section = new Section(
            year,
            new GradeLevel(tenantId, "First", $"01-{code}", 1),
            new Campus(tenant, "Main", $"MAIN-{code}"),
            "A",
            $"A-{code}");
        var person = new Person(tenantId, "Ana", "Pérez");
        var student = new StudentProfile(person, $"S-{code}");
        var enrollment = new Enrollment(
            student,
            year,
            section,
            EnrollmentStatus.Active,
            new DateOnly(2026, 8, 15));

        return new EnrollmentGraph(tenant, section, enrollment);
    }

    private static async Task MigrateAsync(ApiFactory factory)
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

    private sealed record EnrollmentGraph(
        Tenant Tenant,
        Section Section,
        Enrollment Enrollment);

    private sealed record AttendanceSetup(Guid EnrollmentId, Guid SectionId);

    private sealed record AttendanceRosterResponse(
        Guid SectionId,
        string SectionName,
        DateOnly Date,
        List<AttendanceStudentResponse> Students);

    private sealed record AttendanceStudentResponse(
        Guid EnrollmentId,
        string StudentNumber,
        string StudentName,
        string Status,
        string? Note,
        DateTimeOffset? CreatedAt,
        string? CreatedBy,
        DateTimeOffset? UpdatedAt,
        string? UpdatedBy);
}
