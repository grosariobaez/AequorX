namespace SchoolERP.Domain.Platform;

public sealed class Campus
{
    private Campus()
    {
    }

    public Campus(Tenant tenant, string name, string code)
    {
        ArgumentNullException.ThrowIfNull(tenant);

        Id = Guid.NewGuid();
        TenantId = tenant.Id;
        Name = DomainGuard.Required(name, nameof(name), 200);
        Code = DomainGuard.Required(code, nameof(code), 50).ToUpperInvariant();
        IsActive = true;
        Tenant = tenant;
    }

    public Guid Id { get; private set; }

    public Guid TenantId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Code { get; private set; } = string.Empty;

    public bool IsActive { get; private set; }

    public Tenant Tenant { get; private set; } = null!;
}
