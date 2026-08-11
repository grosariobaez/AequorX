using SchoolERP.Domain.Platform;

namespace SchoolERP.Domain.Academic;

public sealed class Section
{
    private Section()
    {
    }

    public Section(
        AcademicYear academicYear,
        GradeLevel gradeLevel,
        Campus campus,
        string name,
        string code,
        int? capacity = null)
    {
        ArgumentNullException.ThrowIfNull(academicYear);
        ArgumentNullException.ThrowIfNull(gradeLevel);
        ArgumentNullException.ThrowIfNull(campus);

        DomainGuard.SameTenant(academicYear.TenantId, gradeLevel.TenantId);
        DomainGuard.SameTenant(academicYear.TenantId, campus.TenantId);

        Id = Guid.NewGuid();
        TenantId = academicYear.TenantId;
        AcademicYearId = academicYear.Id;
        GradeLevelId = gradeLevel.Id;
        CampusId = campus.Id;
        Name = DomainGuard.Required(name, nameof(name), 100);
        Code = DomainGuard.Required(code, nameof(code), 50).ToUpperInvariant();
        Capacity = capacity;
        IsActive = true;
        AcademicYear = academicYear;
        GradeLevel = gradeLevel;
        Campus = campus;
    }

    public Guid Id { get; private set; }

    public Guid TenantId { get; private set; }

    public Guid AcademicYearId { get; private set; }

    public Guid GradeLevelId { get; private set; }

    public Guid CampusId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Code { get; private set; } = string.Empty;

    public int? Capacity { get; private set; }

    public bool IsActive { get; private set; }

    public AcademicYear AcademicYear { get; private set; } = null!;

    public GradeLevel GradeLevel { get; private set; } = null!;

    public Campus Campus { get; private set; } = null!;
}
