using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Tenancy;
using SchoolERP.Domain.Academic;
using SchoolERP.Domain.People;
using SchoolERP.Domain.Platform;
using SchoolERP.Infrastructure.Persistence;

namespace SchoolERP.Api;

internal static class CoreDomainEndpoints
{
    public static IEndpointRouteBuilder MapCoreDomainEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        MapTenant(endpoints);
        MapPeople(endpoints);
        MapStudents(endpoints);
        MapAcademicYears(endpoints);
        MapGradeLevels(endpoints);
        MapSections(endpoints);
        MapEnrollments(endpoints);

        return endpoints;
    }

    private static void MapTenant(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api").WithTags("Tenant");

        group.MapGet("/tenant", async (
            SchoolERPDbContext database,
            ITenantContext tenantContext,
            CancellationToken cancellationToken) =>
        {
            var tenant = await database.Tenants
                .AsNoTracking()
                .Where(entity => entity.Id == tenantContext.TenantId)
                .Select(entity => new
                {
                    entity.Id,
                    entity.Name,
                    entity.Code,
                    entity.IsActive,
                    entity.CreatedAt
                })
                .SingleOrDefaultAsync(cancellationToken);

            return tenant is null ? Results.NotFound() : Results.Ok(tenant);
        });

        if (endpoints.ServiceProvider.GetRequiredService<IHostEnvironment>().IsDevelopment())
        {
            group.MapPost("/tenant", async (
                CreateTenantRequest request,
                SchoolERPDbContext database,
                ITenantContext tenantContext,
                CancellationToken cancellationToken) =>
            {
                var exists = await database.Tenants
                    .IgnoreQueryFilters()
                    .AnyAsync(
                        entity => entity.Id == tenantContext.TenantId,
                        cancellationToken);
                if (exists)
                {
                    return Results.Conflict();
                }

                var tenant = new Tenant(tenantContext.TenantId, request.Name, request.Code);
                database.Tenants.Add(tenant);
                await database.SaveChangesAsync(cancellationToken);

                return Results.Created($"/api/tenant", new
                {
                    tenant.Id,
                    tenant.Name,
                    tenant.Code,
                    tenant.IsActive,
                    tenant.CreatedAt
                });
            });
        }

        group.MapGet("/campuses", async (
            SchoolERPDbContext database,
            CancellationToken cancellationToken) =>
            Results.Ok(await database.Campuses
                .AsNoTracking()
                .OrderBy(entity => entity.Name)
                .Select(entity => new
                {
                    entity.Id,
                    entity.Name,
                    entity.Code,
                    entity.IsActive
                })
                .ToListAsync(cancellationToken)));

        group.MapPost("/campuses", async (
            CreateCampusRequest request,
            SchoolERPDbContext database,
            ITenantContext tenantContext,
            CancellationToken cancellationToken) =>
        {
            var tenant = await database.Tenants
                .SingleOrDefaultAsync(
                    entity => entity.Id == tenantContext.TenantId,
                    cancellationToken);
            if (tenant is null)
            {
                return Results.Conflict(new { code = "TenantNotProvisioned" });
            }

            var campus = new Campus(tenant, request.Name, request.Code);
            database.Campuses.Add(campus);
            await database.SaveChangesAsync(cancellationToken);

            return Results.Created($"/api/campuses/{campus.Id}", new
            {
                campus.Id,
                campus.Name,
                campus.Code,
                campus.IsActive
            });
        });
    }

    private static void MapPeople(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/people").WithTags("People");

        group.MapGet("/", async (
            string? search,
            SchoolERPDbContext database,
            CancellationToken cancellationToken) =>
        {
            var query = database.People.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(entity =>
                    entity.FirstName.Contains(term) ||
                    entity.LastName.Contains(term) ||
                    (entity.Email != null && entity.Email.Contains(term)));
            }

            return Results.Ok(await query
                .OrderBy(entity => entity.LastName)
                .ThenBy(entity => entity.FirstName)
                .Select(PersonResponse.Expression)
                .ToListAsync(cancellationToken));
        });

        group.MapGet("/{id:guid}", async (
            Guid id,
            SchoolERPDbContext database,
            CancellationToken cancellationToken) =>
        {
            var person = await database.People
                .AsNoTracking()
                .Where(entity => entity.Id == id)
                .Select(PersonResponse.Expression)
                .SingleOrDefaultAsync(cancellationToken);

            return person is null ? Results.NotFound() : Results.Ok(person);
        });

        group.MapPost("/", async (
            CreatePersonRequest request,
            SchoolERPDbContext database,
            ITenantContext tenantContext,
            CancellationToken cancellationToken) =>
        {
            var person = new Person(
                tenantContext.TenantId,
                request.FirstName,
                request.LastName,
                request.MiddleName,
                request.SecondLastName,
                request.PreferredName,
                request.DateOfBirth,
                request.Email,
                request.Phone);

            database.People.Add(person);
            await database.SaveChangesAsync(cancellationToken);

            return Results.Created($"/api/people/{person.Id}", PersonResponse.From(person));
        });
    }

    private static void MapStudents(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/students").WithTags("Students");

        group.MapGet("/", async (
            SchoolERPDbContext database,
            CancellationToken cancellationToken) =>
            Results.Ok(await database.StudentProfiles
                .AsNoTracking()
                .OrderBy(entity => entity.Person.LastName)
                .ThenBy(entity => entity.Person.FirstName)
                .Select(entity => new StudentResponse(
                    entity.PersonId,
                    entity.StudentNumber,
                    entity.Person.FirstName,
                    entity.Person.LastName,
                    entity.IsActive))
                .ToListAsync(cancellationToken)));

        group.MapGet("/{id:guid}", async (
            Guid id,
            SchoolERPDbContext database,
            CancellationToken cancellationToken) =>
        {
            var student = await database.StudentProfiles
                .AsNoTracking()
                .Where(entity => entity.PersonId == id)
                .Select(entity => new StudentResponse(
                    entity.PersonId,
                    entity.StudentNumber,
                    entity.Person.FirstName,
                    entity.Person.LastName,
                    entity.IsActive))
                .SingleOrDefaultAsync(cancellationToken);

            return student is null ? Results.NotFound() : Results.Ok(student);
        });

        group.MapPost("/{personId:guid}/profile", async (
            Guid personId,
            CreateStudentProfileRequest request,
            SchoolERPDbContext database,
            CancellationToken cancellationToken) =>
        {
            var person = await database.People
                .SingleOrDefaultAsync(entity => entity.Id == personId, cancellationToken);
            if (person is null)
            {
                return Results.NotFound();
            }

            var profile = new StudentProfile(person, request.StudentNumber);
            database.StudentProfiles.Add(profile);
            await database.SaveChangesAsync(cancellationToken);

            return Results.Created($"/api/students/{person.Id}", new StudentResponse(
                person.Id,
                profile.StudentNumber,
                person.FirstName,
                person.LastName,
                profile.IsActive));
        });
    }

    private static void MapAcademicYears(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/academic-years").WithTags("Academic");

        group.MapGet("/", async (
            SchoolERPDbContext database,
            CancellationToken cancellationToken) =>
            Results.Ok(await database.AcademicYears
                .AsNoTracking()
                .OrderByDescending(entity => entity.StartDate)
                .Select(entity => new
                {
                    entity.Id,
                    entity.Name,
                    entity.StartDate,
                    entity.EndDate,
                    Status = entity.Status.ToString()
                })
                .ToListAsync(cancellationToken)));

        group.MapPost("/", async (
            CreateAcademicYearRequest request,
            SchoolERPDbContext database,
            ITenantContext tenantContext,
            CancellationToken cancellationToken) =>
        {
            var academicYear = new AcademicYear(
                tenantContext.TenantId,
                request.Name,
                request.StartDate,
                request.EndDate,
                ParseEnum<AcademicYearStatus>(request.Status));

            database.AcademicYears.Add(academicYear);
            await database.SaveChangesAsync(cancellationToken);

            return Results.Created($"/api/academic-years/{academicYear.Id}", new
            {
                academicYear.Id,
                academicYear.Name,
                academicYear.StartDate,
                academicYear.EndDate,
                Status = academicYear.Status.ToString()
            });
        });
    }

    private static void MapGradeLevels(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/grade-levels").WithTags("Academic");

        group.MapGet("/", async (
            SchoolERPDbContext database,
            CancellationToken cancellationToken) =>
            Results.Ok(await database.GradeLevels
                .AsNoTracking()
                .OrderBy(entity => entity.SortOrder)
                .ThenBy(entity => entity.Name)
                .Select(entity => new
                {
                    entity.Id,
                    entity.Name,
                    entity.Code,
                    entity.SortOrder,
                    entity.IsActive
                })
                .ToListAsync(cancellationToken)));

        group.MapPost("/", async (
            CreateGradeLevelRequest request,
            SchoolERPDbContext database,
            ITenantContext tenantContext,
            CancellationToken cancellationToken) =>
        {
            var gradeLevel = new GradeLevel(
                tenantContext.TenantId,
                request.Name,
                request.Code,
                request.SortOrder);

            database.GradeLevels.Add(gradeLevel);
            await database.SaveChangesAsync(cancellationToken);

            return Results.Created($"/api/grade-levels/{gradeLevel.Id}", new
            {
                gradeLevel.Id,
                gradeLevel.Name,
                gradeLevel.Code,
                gradeLevel.SortOrder,
                gradeLevel.IsActive
            });
        });
    }

    private static void MapSections(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/sections").WithTags("Academic");

        group.MapGet("/", async (
            SchoolERPDbContext database,
            CancellationToken cancellationToken) =>
            Results.Ok(await database.Sections
                .AsNoTracking()
                .OrderBy(entity => entity.AcademicYear.StartDate)
                .ThenBy(entity => entity.GradeLevel.SortOrder)
                .ThenBy(entity => entity.Name)
                .Select(entity => new
                {
                    entity.Id,
                    entity.Name,
                    entity.Code,
                    entity.Capacity,
                    entity.IsActive,
                    entity.AcademicYearId,
                    AcademicYearName = entity.AcademicYear.Name,
                    entity.GradeLevelId,
                    GradeLevelName = entity.GradeLevel.Name,
                    entity.CampusId,
                    CampusName = entity.Campus.Name
                })
                .ToListAsync(cancellationToken)));

        group.MapPost("/", async (
            CreateSectionRequest request,
            SchoolERPDbContext database,
            CancellationToken cancellationToken) =>
        {
            var academicYear = await database.AcademicYears
                .SingleOrDefaultAsync(entity => entity.Id == request.AcademicYearId, cancellationToken);
            var gradeLevel = await database.GradeLevels
                .SingleOrDefaultAsync(entity => entity.Id == request.GradeLevelId, cancellationToken);
            var campus = await database.Campuses
                .SingleOrDefaultAsync(entity => entity.Id == request.CampusId, cancellationToken);
            if (academicYear is null || gradeLevel is null || campus is null)
            {
                return Results.NotFound();
            }

            var section = new Section(
                academicYear,
                gradeLevel,
                campus,
                request.Name,
                request.Code,
                request.Capacity);

            database.Sections.Add(section);
            await database.SaveChangesAsync(cancellationToken);

            return Results.Created($"/api/sections/{section.Id}", new { section.Id });
        });
    }

    private static void MapEnrollments(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/enrollments").WithTags("Enrollments");

        group.MapGet("/", async (
            SchoolERPDbContext database,
            CancellationToken cancellationToken) =>
            Results.Ok(await database.Enrollments
                .AsNoTracking()
                .OrderByDescending(entity => entity.EnrollmentDate)
                .Select(entity => new
                {
                    entity.Id,
                    entity.StudentPersonId,
                    StudentNumber = entity.Student.StudentNumber,
                    StudentName = entity.Student.Person.FirstName + " " + entity.Student.Person.LastName,
                    entity.AcademicYearId,
                    AcademicYearName = entity.AcademicYear.Name,
                    entity.SectionId,
                    SectionName = entity.Section.Name,
                    Status = entity.Status.ToString(),
                    entity.EnrollmentDate,
                    entity.EndDate
                })
                .ToListAsync(cancellationToken)));

        group.MapPost("/", async (
            CreateEnrollmentRequest request,
            SchoolERPDbContext database,
            CancellationToken cancellationToken) =>
        {
            var student = await database.StudentProfiles
                .Include(entity => entity.Person)
                .SingleOrDefaultAsync(
                    entity => entity.PersonId == request.StudentPersonId,
                    cancellationToken);
            var academicYear = await database.AcademicYears
                .SingleOrDefaultAsync(
                    entity => entity.Id == request.AcademicYearId,
                    cancellationToken);
            var section = await database.Sections
                .SingleOrDefaultAsync(
                    entity => entity.Id == request.SectionId,
                    cancellationToken);
            if (student is null || academicYear is null || section is null)
            {
                return Results.NotFound();
            }

            var enrollment = new Enrollment(
                student,
                academicYear,
                section,
                ParseEnum<EnrollmentStatus>(request.Status),
                request.EnrollmentDate,
                request.EndDate);

            database.Enrollments.Add(enrollment);
            await database.SaveChangesAsync(cancellationToken);

            return Results.Created($"/api/enrollments/{enrollment.Id}", new { enrollment.Id });
        });
    }

    private static TEnum ParseEnum<TEnum>(string value)
        where TEnum : struct, Enum
    {
        if (!Enum.TryParse<TEnum>(value, ignoreCase: true, out var parsed))
        {
            throw new ArgumentException("Unsupported status value.", nameof(value));
        }

        return parsed;
    }

    private sealed record CreateTenantRequest(string Name, string Code);

    private sealed record CreateCampusRequest(string Name, string Code);

    private sealed record CreatePersonRequest(
        string FirstName,
        string LastName,
        string? MiddleName,
        string? SecondLastName,
        string? PreferredName,
        DateOnly? DateOfBirth,
        string? Email,
        string? Phone);

    private sealed record CreateStudentProfileRequest(string StudentNumber);

    private sealed record CreateAcademicYearRequest(
        string Name,
        DateOnly StartDate,
        DateOnly EndDate,
        string Status);

    private sealed record CreateGradeLevelRequest(
        string Name,
        string Code,
        int SortOrder);

    private sealed record CreateSectionRequest(
        Guid AcademicYearId,
        Guid GradeLevelId,
        Guid CampusId,
        string Name,
        string Code,
        int? Capacity);

    private sealed record CreateEnrollmentRequest(
        Guid StudentPersonId,
        Guid AcademicYearId,
        Guid SectionId,
        string Status,
        DateOnly EnrollmentDate,
        DateOnly? EndDate);

    private sealed record StudentResponse(
        Guid PersonId,
        string StudentNumber,
        string FirstName,
        string LastName,
        bool IsActive);

    private sealed record PersonResponse(
        Guid Id,
        string FirstName,
        string? MiddleName,
        string LastName,
        string? SecondLastName,
        string? PreferredName,
        DateOnly? DateOfBirth,
        string? Email,
        string? Phone,
        bool IsActive)
    {
        public static System.Linq.Expressions.Expression<Func<Person, PersonResponse>> Expression =>
            entity => new PersonResponse(
                entity.Id,
                entity.FirstName,
                entity.MiddleName,
                entity.LastName,
                entity.SecondLastName,
                entity.PreferredName,
                entity.DateOfBirth,
                entity.Email,
                entity.Phone,
                entity.IsActive);

        public static PersonResponse From(Person entity) => new(
            entity.Id,
            entity.FirstName,
            entity.MiddleName,
            entity.LastName,
            entity.SecondLastName,
            entity.PreferredName,
            entity.DateOfBirth,
            entity.Email,
            entity.Phone,
            entity.IsActive);
    }
}
