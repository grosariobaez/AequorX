using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Auditing;
using SchoolERP.Domain.Academic;
using SchoolERP.Domain.Attendance;
using SchoolERP.Infrastructure.Persistence;

namespace SchoolERP.Api;

internal static class AttendanceEndpoints
{
    public static IEndpointRouteBuilder MapAttendanceEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/attendance").WithTags("Attendance");

        group.MapGet("/", GetAttendanceAsync);
        group.MapPut("/{enrollmentId:guid}/{date}", PutAttendanceAsync);

        return endpoints;
    }

    private static async Task<IResult> GetAttendanceAsync(
        Guid sectionId,
        DateOnly date,
        SchoolERPDbContext database,
        CancellationToken cancellationToken)
    {
        var section = await database.Sections
            .AsNoTracking()
            .Where(entity => entity.Id == sectionId)
            .Select(entity => new SectionSnapshot(
                entity.Id,
                entity.Name,
                entity.AcademicYear.StartDate,
                entity.AcademicYear.EndDate))
            .SingleOrDefaultAsync(cancellationToken);

        if (section is null)
        {
            return Results.NotFound();
        }

        if (!IsWithinAcademicYear(date, section.StartDate, section.EndDate))
        {
            return Results.BadRequest(new { code = "AttendanceDateOutsideAcademicYear" });
        }

        var roster = await database.Enrollments
            .AsNoTracking()
            .Where(entity =>
                entity.SectionId == sectionId &&
                entity.Status == EnrollmentStatus.Active)
            .OrderBy(entity => entity.Student.Person.LastName)
            .ThenBy(entity => entity.Student.Person.FirstName)
            .Select(entity => new EnrollmentSnapshot(
                entity.Id,
                entity.Student.StudentNumber,
                entity.Student.Person.FirstName + " " + entity.Student.Person.LastName))
            .ToListAsync(cancellationToken);

        var enrollmentIds = roster.Select(entity => entity.Id).ToArray();
        var exceptions = await database.AttendanceRecords
            .AsNoTracking()
            .Where(entity =>
                entity.AttendanceDate == date &&
                enrollmentIds.Contains(entity.EnrollmentId))
            .ToDictionaryAsync(entity => entity.EnrollmentId, cancellationToken);

        var students = roster.Select(enrollment =>
        {
            exceptions.TryGetValue(enrollment.Id, out var exception);
            return ToResponse(enrollment, exception);
        });

        return Results.Ok(new AttendanceRosterResponse(
            section.Id,
            section.Name,
            date,
            students));
    }

    private static async Task<IResult> PutAttendanceAsync(
        Guid enrollmentId,
        DateOnly date,
        AttendanceUpdateRequest request,
        SchoolERPDbContext database,
        IAuditContext auditContext,
        CancellationToken cancellationToken)
    {
        var enrollment = await database.Enrollments
            .Include(entity => entity.AcademicYear)
            .Include(entity => entity.Section)
            .Include(entity => entity.Student)
                .ThenInclude(entity => entity.Person)
            .SingleOrDefaultAsync(
                entity => entity.Id == enrollmentId &&
                          entity.Status == EnrollmentStatus.Active,
                cancellationToken);

        if (enrollment is null)
        {
            return Results.NotFound();
        }

        if (!IsWithinAcademicYear(
                date,
                enrollment.AcademicYear.StartDate,
                enrollment.AcademicYear.EndDate))
        {
            return Results.BadRequest(new { code = "AttendanceDateOutsideAcademicYear" });
        }

        var existing = await database.AttendanceRecords
            .SingleOrDefaultAsync(
                entity => entity.EnrollmentId == enrollmentId &&
                          entity.AttendanceDate == date,
                cancellationToken);

        if (string.Equals(request.Status, "Present", StringComparison.OrdinalIgnoreCase))
        {
            if (existing is not null)
            {
                database.AttendanceRecords.Remove(existing);
                await database.SaveChangesAsync(cancellationToken);
            }

            return Results.Ok(ToResponse(enrollment, null));
        }

        var status = ParseStatus(request.Status);
        if (existing is null)
        {
            existing = new AttendanceRecord(
                enrollment,
                enrollment.Section,
                date,
                status,
                request.Note,
                auditContext.Actor);
            database.AttendanceRecords.Add(existing);
        }
        else
        {
            existing.Correct(status, request.Note, auditContext.Actor);
        }

        await database.SaveChangesAsync(cancellationToken);

        return Results.Ok(ToResponse(enrollment, existing));
    }

    private static bool IsWithinAcademicYear(
        DateOnly date,
        DateOnly startDate,
        DateOnly endDate) => date >= startDate && date <= endDate;

    private static AttendanceStatus ParseStatus(string value)
    {
        if (!Enum.TryParse<AttendanceStatus>(value, ignoreCase: true, out var status) ||
            !Enum.IsDefined(status))
        {
            throw new ArgumentException("Unsupported attendance status.", nameof(value));
        }

        return status;
    }

    private static AttendanceStudentResponse ToResponse(
        EnrollmentSnapshot enrollment,
        AttendanceRecord? exception) => new(
            enrollment.Id,
            enrollment.StudentNumber,
            enrollment.StudentName,
            exception?.Status.ToString() ?? "Present",
            exception?.Note,
            exception?.CreatedAt,
            exception?.CreatedBy,
            exception?.UpdatedAt,
            exception?.UpdatedBy);

    private static AttendanceStudentResponse ToResponse(
        Enrollment enrollment,
        AttendanceRecord? exception) => new(
            enrollment.Id,
            enrollment.Student.StudentNumber,
            enrollment.Student.Person.FirstName + " " + enrollment.Student.Person.LastName,
            exception?.Status.ToString() ?? "Present",
            exception?.Note,
            exception?.CreatedAt,
            exception?.CreatedBy,
            exception?.UpdatedAt,
            exception?.UpdatedBy);

    private sealed record AttendanceUpdateRequest(string Status, string? Note);

    private sealed record SectionSnapshot(
        Guid Id,
        string Name,
        DateOnly StartDate,
        DateOnly EndDate);

    private sealed record EnrollmentSnapshot(
        Guid Id,
        string StudentNumber,
        string StudentName);

    private sealed record AttendanceRosterResponse(
        Guid SectionId,
        string SectionName,
        DateOnly Date,
        IEnumerable<AttendanceStudentResponse> Students);

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
