using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Tenancy;
using SchoolERP.Domain.Academic;
using SchoolERP.Infrastructure.Persistence;

namespace SchoolERP.Api;

internal static class AcademicStructureEndpoints
{
    public static IEndpointRouteBuilder MapAcademicStructureEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var subjects = endpoints.MapGroup("/api/subjects").WithTags("Academic");
        subjects.MapGet("/", GetSubjectsAsync);
        subjects.MapPost("/", CreateSubjectAsync);

        var classes = endpoints.MapGroup("/api/classes").WithTags("Academic");
        classes.MapGet("/", GetClassesAsync);
        classes.MapPost("/", CreateClassAsync);
        return endpoints;
    }

    private static async Task<IResult> GetSubjectsAsync(SchoolERPDbContext database, CancellationToken token) =>
        Results.Ok(await database.Subjects.AsNoTracking().OrderBy(x => x.Name)
            .Select(x => new SubjectResponse(x.Id, x.Name, x.Code, x.IsActive))
            .ToListAsync(token));

    private static async Task<IResult> CreateSubjectAsync(
        CreateSubjectRequest request,
        SchoolERPDbContext database,
        ITenantContext tenantContext,
        CancellationToken token)
    {
        var subject = new Subject(tenantContext.TenantId, request.Name, request.Code);
        database.Subjects.Add(subject);
        await database.SaveChangesAsync(token);
        return Results.Created($"/api/subjects/{subject.Id}",
            new SubjectResponse(subject.Id, subject.Name, subject.Code, subject.IsActive));
    }

    private static async Task<IResult> GetClassesAsync(
        Guid sectionId,
        SchoolERPDbContext database,
        CancellationToken token)
    {
        if (!await database.Sections.AnyAsync(x => x.Id == sectionId, token)) return Results.NotFound();
        return Results.Ok(await database.Classes.AsNoTracking()
            .Where(x => x.SectionId == sectionId)
            .OrderBy(x => x.Subject.Name)
            .Select(x => new ClassResponse(
                x.Id, x.SectionId, x.SubjectId, x.Name, x.Subject.Name, x.Subject.Code, x.IsActive))
            .ToListAsync(token));
    }

    private static async Task<IResult> CreateClassAsync(
        CreateClassRequest request,
        SchoolERPDbContext database,
        CancellationToken token)
    {
        var section = await database.Sections.Include(x => x.AcademicYear)
            .SingleOrDefaultAsync(x => x.Id == request.SectionId, token);
        var subject = await database.Subjects.SingleOrDefaultAsync(x => x.Id == request.SubjectId, token);
        if (section is null || subject is null) return Results.NotFound();

        var @class = new Class(section, subject, request.Name);
        database.Classes.Add(@class);
        await database.SaveChangesAsync(token);
        return Results.Created($"/api/classes/{@class.Id}", new ClassResponse(
            @class.Id, @class.SectionId, @class.SubjectId, @class.Name,
            subject.Name, subject.Code, @class.IsActive));
    }

    private sealed record CreateSubjectRequest(string Name, string Code);
    private sealed record CreateClassRequest(Guid SectionId, Guid SubjectId, string? Name);
    private sealed record SubjectResponse(Guid Id, string Name, string Code, bool IsActive);
    private sealed record ClassResponse(
        Guid Id,
        Guid SectionId,
        Guid SubjectId,
        string Name,
        string SubjectName,
        string SubjectCode,
        bool IsActive);
}
