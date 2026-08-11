namespace SchoolERP.Domain.Academic;

public sealed class GradeLevel
{
    private GradeLevel()
    {
    }

    public GradeLevel(Guid tenantId, string name, string code, int sortOrder)
    {
        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException("Tenant is required.", nameof(tenantId));
        }

        Id = Guid.NewGuid();
        TenantId = tenantId;
        Name = DomainGuard.Required(name, nameof(name), 100);
        Code = DomainGuard.Required(code, nameof(code), 50).ToUpperInvariant();
        SortOrder = sortOrder;
        IsActive = true;
    }

    public Guid Id { get; private set; }

    public Guid TenantId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Code { get; private set; } = string.Empty;

    public int SortOrder { get; private set; }

    public bool IsActive { get; private set; }
}
