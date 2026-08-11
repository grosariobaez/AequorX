using SchoolERP.Domain.Academic;
using SchoolERP.Domain.Attendance;
using SchoolERP.Domain.People;
using SchoolERP.Domain.Platform;

namespace SchoolERP.Domain.Tests;

public sealed class AttendanceInvariantTests
{
    [Fact]
    public void Attendance_rejects_cross_tenant_section()
    {
        var setup = SetupAttendance();
        var foreignTenant = new Tenant("Foreign School", "FOREIGN");
        var foreignYear = AcademicYearFor(foreignTenant.Id);
        var foreignSection = SectionFor(foreignTenant, foreignYear, "B");

        var action = () => new AttendanceRecord(
            setup.Enrollment,
            foreignSection,
            new DateOnly(2026, 9, 1),
            AttendanceStatus.Absent,
            null,
            "teacher");

        Assert.Throws<InvalidOperationException>(action);
    }

    [Fact]
    public void Attendance_requires_enrollment_in_selected_section()
    {
        var setup = SetupAttendance();
        var otherSection = SectionFor(setup.Tenant, setup.Year, "B");

        var action = () => new AttendanceRecord(
            setup.Enrollment,
            otherSection,
            new DateOnly(2026, 9, 1),
            AttendanceStatus.Late,
            null,
            "teacher");

        Assert.Throws<InvalidOperationException>(action);
    }

    [Theory]
    [InlineData("2026-07-31")]
    [InlineData("2027-07-01")]
    public void Attendance_date_must_be_within_academic_year(string value)
    {
        var setup = SetupAttendance();

        var action = () => new AttendanceRecord(
            setup.Enrollment,
            setup.Section,
            DateOnly.Parse(value),
            AttendanceStatus.Excused,
            null,
            "teacher");

        Assert.Throws<ArgumentOutOfRangeException>(action);
    }

    [Fact]
    public void Correction_preserves_creation_audit_and_updates_change_audit()
    {
        var setup = SetupAttendance();
        var record = new AttendanceRecord(
            setup.Enrollment,
            setup.Section,
            new DateOnly(2026, 9, 1),
            AttendanceStatus.Absent,
            "Initial note",
            "creator");
        var createdAt = record.CreatedAt;

        record.Correct(AttendanceStatus.EarlyDeparture, "Corrected", "editor");

        Assert.Equal("creator", record.CreatedBy);
        Assert.Equal(createdAt, record.CreatedAt);
        Assert.Equal("editor", record.UpdatedBy);
        Assert.Equal(AttendanceStatus.EarlyDeparture, record.Status);
        Assert.Equal("Corrected", record.Note);
    }

    private static AttendanceSetup SetupAttendance()
    {
        var tenant = new Tenant("School", "SCHOOL");
        var year = AcademicYearFor(tenant.Id);
        var section = SectionFor(tenant, year, "A");
        var person = new Person(tenant.Id, "Ana", "Pérez");
        var student = new StudentProfile(person, "S-001");
        var enrollment = new Enrollment(
            student,
            year,
            section,
            EnrollmentStatus.Active,
            new DateOnly(2026, 8, 15));

        return new AttendanceSetup(tenant, year, section, enrollment);
    }

    private static AcademicYear AcademicYearFor(Guid tenantId) => new(
        tenantId,
        "2026-2027",
        new DateOnly(2026, 8, 1),
        new DateOnly(2027, 6, 30),
        AcademicYearStatus.Active);

    private static Section SectionFor(Tenant tenant, AcademicYear year, string code) => new(
        year,
        new GradeLevel(tenant.Id, "First", $"01-{code}", 1),
        new Campus(tenant, $"Campus {code}", $"CAMPUS-{code}"),
        code,
        code);

    private sealed record AttendanceSetup(
        Tenant Tenant,
        AcademicYear Year,
        Section Section,
        Enrollment Enrollment);
}
