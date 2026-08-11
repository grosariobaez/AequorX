using SchoolERP.Domain.Academic;
using SchoolERP.Domain.People;
using SchoolERP.Domain.Platform;

namespace SchoolERP.Domain.Tests;

public sealed class CoreDomainInvariantTests
{
    [Theory]
    [InlineData("2026-08-01", "2026-08-01")]
    [InlineData("2026-08-02", "2026-08-01")]
    public void Academic_year_requires_start_before_end(string start, string end)
    {
        var action = () => new AcademicYear(
            Guid.NewGuid(),
            "2026-2027",
            DateOnly.Parse(start),
            DateOnly.Parse(end),
            AcademicYearStatus.Planned);

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void Student_relationship_rejects_cross_tenant_people()
    {
        var student = PersonFor(Guid.NewGuid(), "Ana");
        var relatedPerson = PersonFor(Guid.NewGuid(), "Maria");

        var action = () => new StudentRelationship(
            student,
            relatedPerson,
            StudentRelationshipType.Mother,
            isPrimaryContact: true,
            isAuthorizedPickup: true);

        Assert.Throws<InvalidOperationException>(action);
    }

    [Fact]
    public void Section_rejects_cross_tenant_academic_references()
    {
        var tenant = new Tenant("School A", "A");
        var otherTenant = new Tenant("School B", "B");
        var year = AcademicYearFor(tenant.Id);
        var grade = new GradeLevel(tenant.Id, "First", "01", 1);
        var foreignCampus = new Campus(otherTenant, "Main", "MAIN");

        var action = () => new Section(year, grade, foreignCampus, "A", "A");

        Assert.Throws<InvalidOperationException>(action);
    }

    [Fact]
    public void Enrollment_rejects_cross_tenant_student()
    {
        var tenant = new Tenant("School A", "A");
        var otherTenant = new Tenant("School B", "B");
        var year = AcademicYearFor(tenant.Id);
        var section = SectionFor(tenant, year);
        var foreignStudent = new StudentProfile(
            PersonFor(otherTenant.Id, "Ana"),
            "S-001");

        var action = () => new Enrollment(
            foreignStudent,
            year,
            section,
            EnrollmentStatus.Active,
            new DateOnly(2026, 8, 15));

        Assert.Throws<InvalidOperationException>(action);
    }

    [Fact]
    public void Enrollment_requires_section_from_selected_academic_year()
    {
        var tenant = new Tenant("School A", "A");
        var selectedYear = AcademicYearFor(tenant.Id);
        var otherYear = new AcademicYear(
            tenant.Id,
            "2027-2028",
            new DateOnly(2027, 8, 1),
            new DateOnly(2028, 6, 30),
            AcademicYearStatus.Planned);
        var section = SectionFor(tenant, otherYear);
        var student = new StudentProfile(PersonFor(tenant.Id, "Ana"), "S-001");

        var action = () => new Enrollment(
            student,
            selectedYear,
            section,
            EnrollmentStatus.Pending,
            new DateOnly(2026, 8, 15));

        Assert.Throws<InvalidOperationException>(action);
    }

    [Fact]
    public void Student_profile_does_not_store_current_placement()
    {
        var propertyNames = typeof(StudentProfile)
            .GetProperties()
            .Select(property => property.Name)
            .ToArray();

        Assert.DoesNotContain("AcademicYearId", propertyNames);
        Assert.DoesNotContain("GradeLevelId", propertyNames);
        Assert.DoesNotContain("SectionId", propertyNames);
    }

    private static Person PersonFor(Guid tenantId, string firstName) =>
        new(tenantId, firstName, "Pérez");

    private static AcademicYear AcademicYearFor(Guid tenantId) =>
        new(
            tenantId,
            "2026-2027",
            new DateOnly(2026, 8, 1),
            new DateOnly(2027, 6, 30),
            AcademicYearStatus.Planned);

    private static Section SectionFor(Tenant tenant, AcademicYear year)
    {
        var campus = new Campus(tenant, "Main", "MAIN");
        var grade = new GradeLevel(tenant.Id, "First", "01", 1);
        return new Section(year, grade, campus, "A", "A");
    }
}
