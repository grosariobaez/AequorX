using SchoolERP.Domain.Academic;
using SchoolERP.Domain.Platform;

namespace SchoolERP.Domain.Tests;

public sealed class AcademicStructureInvariantTests
{
    [Fact]
    public void Subject_normalizes_its_tenant_scoped_code()
    {
        var tenantId = Guid.NewGuid();
        var subject = new Subject(tenantId, "Mathematics", " mat ");
        Assert.Equal(tenantId, subject.TenantId);
        Assert.Equal("MAT", subject.Code);
    }

    [Fact]
    public void Class_rejects_subject_from_another_tenant()
    {
        var section = BuildSection(Guid.NewGuid());
        var subject = new Subject(Guid.NewGuid(), "Mathematics", "MAT");
        Assert.Throws<InvalidOperationException>(() => new Class(section, subject));
    }

    private static Section BuildSection(Guid tenantId)
    {
        var tenant = new Tenant(tenantId, "School", tenantId.ToString("N"));
        var year = new AcademicYear(tenantId, "2026-2027", new DateOnly(2026, 8, 1), new DateOnly(2027, 6, 30), AcademicYearStatus.Active);
        return new Section(year, new GradeLevel(tenantId, "First", "01", 1), new Campus(tenant, "Main", "MAIN"), "A", "A");
    }
}
