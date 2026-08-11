namespace SchoolERP.Domain.Academic;

public sealed class AcademicYear
{
    private AcademicYear()
    {
    }

    public AcademicYear(
        Guid tenantId,
        string name,
        DateOnly startDate,
        DateOnly endDate,
        AcademicYearStatus status)
    {
        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException("Tenant is required.", nameof(tenantId));
        }

        if (startDate >= endDate)
        {
            throw new ArgumentException(
                "Academic year start date must precede end date.",
                nameof(startDate));
        }

        Id = Guid.NewGuid();
        TenantId = tenantId;
        Name = DomainGuard.Required(name, nameof(name), 100);
        StartDate = startDate;
        EndDate = endDate;
        Status = status;
    }

    public Guid Id { get; private set; }

    public Guid TenantId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public DateOnly StartDate { get; private set; }

    public DateOnly EndDate { get; private set; }

    public AcademicYearStatus Status { get; private set; }
}

public enum AcademicYearStatus
{
    Planned,
    Active,
    Closed
}
