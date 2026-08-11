using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Auditing;
using SchoolERP.Domain.Academic;
using SchoolERP.Domain.Grading;
using SchoolERP.Infrastructure.Persistence;

namespace SchoolERP.Api;

internal static class GradingEndpoints
{
    public static IEndpointRouteBuilder MapGradingEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var assessments = endpoints.MapGroup("/api/assessments").WithTags("Grades");
        assessments.MapGet("/", GetAssessmentsAsync);
        assessments.MapPost("/", CreateAssessmentAsync);
        assessments.MapGet("/{assessmentId:guid}/grades", GetGradesAsync);
        assessments.MapPut("/{assessmentId:guid}/grades/{enrollmentId:guid}", PutGradeAsync);
        assessments.MapPost("/{assessmentId:guid}/publish", PublishAsync);
        endpoints.MapPost("/api/grades/{gradeId:guid}/corrections", CorrectAsync).WithTags("Grades");
        return endpoints;
    }

    private static async Task<IResult> GetAssessmentsAsync(Guid sectionId, SchoolERPDbContext database, CancellationToken token)
    {
        if (!await database.Sections.AnyAsync(x => x.Id == sectionId, token)) return Results.NotFound();
        return Results.Ok(await database.Assessments.AsNoTracking()
            .Where(x => x.SectionId == sectionId).OrderByDescending(x => x.AssessmentDate)
            .Select(x => new AssessmentResponse(x.Id, x.SectionId, x.Name, x.AssessmentDate, x.MaximumScore, x.IsActive))
            .ToListAsync(token));
    }

    private static async Task<IResult> CreateAssessmentAsync(CreateAssessmentRequest request, SchoolERPDbContext database, CancellationToken token)
    {
        var section = await database.Sections.Include(x => x.AcademicYear)
            .SingleOrDefaultAsync(x => x.Id == request.SectionId, token);
        if (section is null) return Results.NotFound();
        var assessment = new Assessment(section, request.Name, request.AssessmentDate, request.MaximumScore);
        database.Assessments.Add(assessment);
        await database.SaveChangesAsync(token);
        return Results.Created($"/api/assessments/{assessment.Id}", ToResponse(assessment));
    }

    private static async Task<IResult> GetGradesAsync(Guid assessmentId, SchoolERPDbContext database, CancellationToken token)
    {
        var assessment = await database.Assessments.AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == assessmentId, token);
        if (assessment is null) return Results.NotFound();
        var roster = await database.Enrollments.AsNoTracking()
            .Where(x => x.SectionId == assessment.SectionId && x.Status == EnrollmentStatus.Active)
            .OrderBy(x => x.Student.Person.LastName).ThenBy(x => x.Student.Person.FirstName)
            .Select(x => new EnrollmentSnapshot(x.Id, x.Student.StudentNumber, x.Student.Person.FirstName + " " + x.Student.Person.LastName))
            .ToListAsync(token);
        var ids = roster.Select(x => x.Id).ToArray();
        var grades = await database.Grades.AsNoTracking()
            .Where(x => x.AssessmentId == assessmentId && ids.Contains(x.EnrollmentId))
            .ToDictionaryAsync(x => x.EnrollmentId, token);
        return Results.Ok(new GradeRosterResponse(assessment.Id, assessment.Name, assessment.MaximumScore,
            roster.Select(x => { grades.TryGetValue(x.Id, out var grade); return ToResponse(x, grade); })));
    }

    private static async Task<IResult> PutGradeAsync(Guid assessmentId, Guid enrollmentId, GradeUpdateRequest request,
        SchoolERPDbContext database, IAuditContext audit, CancellationToken token)
    {
        var assessment = await database.Assessments.Include(x => x.Section).ThenInclude(x => x.AcademicYear)
            .SingleOrDefaultAsync(x => x.Id == assessmentId, token);
        var enrollment = await database.Enrollments.Include(x => x.AcademicYear).Include(x => x.Section)
            .Include(x => x.Student).ThenInclude(x => x.Person)
            .SingleOrDefaultAsync(x => x.Id == enrollmentId && x.Status == EnrollmentStatus.Active, token);
        if (assessment is null || enrollment is null) return Results.NotFound();
        var grade = await database.Grades.SingleOrDefaultAsync(
            x => x.AssessmentId == assessmentId && x.EnrollmentId == enrollmentId, token);
        if (grade is null)
        {
            grade = new Grade(assessment, enrollment, request.Score, audit.Actor);
            database.Grades.Add(grade);
        }
        else grade.UpdateDraft(request.Score, audit.Actor);
        await database.SaveChangesAsync(token);
        return Results.Ok(ToResponse(enrollment, grade));
    }

    private static async Task<IResult> PublishAsync(Guid assessmentId, SchoolERPDbContext database, IAuditContext audit, CancellationToken token)
    {
        if (!await database.Assessments.AnyAsync(x => x.Id == assessmentId, token)) return Results.NotFound();
        var grades = await database.Grades.Where(x => x.AssessmentId == assessmentId && x.Status == GradeStatus.Draft).ToListAsync(token);
        foreach (var grade in grades) grade.Publish(audit.Actor);
        await database.SaveChangesAsync(token);
        return Results.Ok(new { published = grades.Count });
    }

    private static async Task<IResult> CorrectAsync(Guid gradeId, GradeCorrectionRequest request,
        SchoolERPDbContext database, IAuditContext audit, CancellationToken token)
    {
        var grade = await database.Grades.Include(x => x.Assessment).SingleOrDefaultAsync(x => x.Id == gradeId, token);
        if (grade is null) return Results.NotFound();
        var correction = grade.Correct(request.Score, request.Reason, audit.Actor);
        database.GradeCorrections.Add(correction);
        await database.SaveChangesAsync(token);
        return Results.Ok(new
        {
            grade.Id,
            grade.Score,
            Status = grade.Status.ToString(),
            correction.PreviousScore,
            correction.NewScore,
            correction.Reason,
            correction.CorrectedAt,
            correction.CorrectedBy
        });
    }

    private static AssessmentResponse ToResponse(Assessment x) => new(x.Id, x.SectionId, x.Name, x.AssessmentDate, x.MaximumScore, x.IsActive);
    private static GradeStudentResponse ToResponse(EnrollmentSnapshot x, Grade? grade) => new(grade?.Id, x.Id, x.StudentNumber, x.StudentName, grade?.Score, grade?.Status.ToString());
    private static GradeStudentResponse ToResponse(Enrollment x, Grade grade) => new(grade.Id, x.Id, x.Student.StudentNumber,
        x.Student.Person.FirstName + " " + x.Student.Person.LastName, grade.Score, grade.Status.ToString());

    private sealed record CreateAssessmentRequest(Guid SectionId, string Name, DateOnly AssessmentDate, decimal MaximumScore);
    private sealed record GradeUpdateRequest(decimal Score);
    private sealed record GradeCorrectionRequest(decimal Score, string Reason);
    private sealed record AssessmentResponse(Guid Id, Guid SectionId, string Name, DateOnly AssessmentDate, decimal MaximumScore, bool IsActive);
    private sealed record EnrollmentSnapshot(Guid Id, string StudentNumber, string StudentName);
    private sealed record GradeRosterResponse(Guid AssessmentId, string AssessmentName, decimal MaximumScore, IEnumerable<GradeStudentResponse> Students);
    private sealed record GradeStudentResponse(Guid? GradeId, Guid EnrollmentId, string StudentNumber, string StudentName, decimal? Score, string? Status);
}
