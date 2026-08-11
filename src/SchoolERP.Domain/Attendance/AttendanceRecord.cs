using SchoolERP.Domain.Academic;

namespace SchoolERP.Domain.Attendance;

public sealed class AttendanceRecord
{
    private AttendanceRecord()
    {
    }

    public AttendanceRecord(
        Enrollment enrollment,
        Section section,
        DateOnly attendanceDate,
        AttendanceStatus status,
        string? note,
        string actor)
    {
        ArgumentNullException.ThrowIfNull(enrollment);
        ArgumentNullException.ThrowIfNull(section);

        DomainGuard.SameTenant(enrollment.TenantId, section.TenantId);

        if (enrollment.SectionId != section.Id)
        {
            throw new InvalidOperationException(
                "Enrollment must belong to the selected section.");
        }

        EnsureDateWithinAcademicYear(enrollment, attendanceDate);

        var timestamp = DateTimeOffset.UtcNow;

        Id = Guid.NewGuid();
        TenantId = enrollment.TenantId;
        EnrollmentId = enrollment.Id;
        SectionId = section.Id;
        AttendanceDate = attendanceDate;
        Status = status;
        Note = NormalizeNote(note);
        CreatedAt = timestamp;
        CreatedBy = DomainGuard.Required(actor, nameof(actor), 200);
        UpdatedAt = timestamp;
        UpdatedBy = CreatedBy;
        Enrollment = enrollment;
        Section = section;
    }

    public Guid Id { get; private set; }

    public Guid TenantId { get; private set; }

    public Guid EnrollmentId { get; private set; }

    public Guid SectionId { get; private set; }

    public DateOnly AttendanceDate { get; private set; }

    public AttendanceStatus Status { get; private set; }

    public string? Note { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public string CreatedBy { get; private set; } = string.Empty;

    public DateTimeOffset UpdatedAt { get; private set; }

    public string UpdatedBy { get; private set; } = string.Empty;

    public Enrollment Enrollment { get; private set; } = null!;

    public Section Section { get; private set; } = null!;

    public void Correct(AttendanceStatus status, string? note, string actor)
    {
        Status = status;
        Note = NormalizeNote(note);
        UpdatedAt = DateTimeOffset.UtcNow;
        UpdatedBy = DomainGuard.Required(actor, nameof(actor), 200);
    }

    private static void EnsureDateWithinAcademicYear(
        Enrollment enrollment,
        DateOnly attendanceDate)
    {
        if (attendanceDate < enrollment.AcademicYear.StartDate ||
            attendanceDate > enrollment.AcademicYear.EndDate)
        {
            throw new ArgumentOutOfRangeException(
                nameof(attendanceDate),
                "Attendance date must fall within the enrollment academic year.");
        }
    }

    private static string? NormalizeNote(string? note)
    {
        if (string.IsNullOrWhiteSpace(note))
        {
            return null;
        }

        var normalized = note.Trim();
        if (normalized.Length > 1000)
        {
            throw new ArgumentException(
                "Note cannot exceed 1000 characters.",
                nameof(note));
        }

        return normalized;
    }
}

public enum AttendanceStatus
{
    Absent,
    Late,
    Excused,
    EarlyDeparture
}
