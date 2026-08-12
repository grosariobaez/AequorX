namespace SchoolERP.Domain.Academic;

public sealed class Subject
{
    private Subject() { }

    public Subject(Guid tenantId, string name, string code)
    {
        Id = Guid.NewGuid();
        TenantId = tenantId;
        Name = DomainGuard.Required(name, nameof(name), 200);
        Code = DomainGuard.Required(code, nameof(code), 50).ToUpperInvariant();
        IsActive = true;
        CreatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
}
