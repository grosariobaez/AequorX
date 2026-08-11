using SchoolERP.Domain.People;

namespace SchoolERP.Domain.Academic;

public sealed class Enrollment
{
    private Enrollment()
    {
    }

    public Enrollment(
        StudentProfile student,
        AcademicYear academicYear,
        Section section,
        EnrollmentStatus status,
        DateOnly enrollmentDate,
        DateOnly? endDate = null)
    {
        ArgumentNullException.ThrowIfNull(student);
        ArgumentNullException.ThrowIfNull(academicYear);
        ArgumentNullException.ThrowIfNull(section);

        DomainGuard.SameTenant(student.TenantId, academicYear.TenantId);
        DomainGuard.SameTenant(student.TenantId, section.TenantId);

        if (section.AcademicYearId != academicYear.Id)
        {
            throw new InvalidOperationException(
                "Section must belong to the selected academic year.");
        }

        Id = Guid.NewGuid();
        TenantId = student.TenantId;
        StudentPersonId = student.PersonId;
        AcademicYearId = academicYear.Id;
        SectionId = section.Id;
        Status = status;
        EnrollmentDate = enrollmentDate;
        EndDate = endDate;
        CreatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = CreatedAt;
        Student = student;
        AcademicYear = academicYear;
        Section = section;
    }

    public Guid Id { get; private set; }

    public Guid TenantId { get; private set; }

    public Guid StudentPersonId { get; private set; }

    public Guid AcademicYearId { get; private set; }

    public Guid SectionId { get; private set; }

    public EnrollmentStatus Status { get; private set; }

    public DateOnly EnrollmentDate { get; private set; }

    public DateOnly? EndDate { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public StudentProfile Student { get; private set; } = null!;

    public AcademicYear AcademicYear { get; private set; } = null!;

    public Section Section { get; private set; } = null!;
}

public enum EnrollmentStatus
{
    Pending,
    Active,
    Withdrawn,
    Transferred,
    Completed
}
