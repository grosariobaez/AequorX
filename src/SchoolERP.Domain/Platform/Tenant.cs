namespace SchoolERP.Domain.Platform;

public sealed class Tenant
{
    private Tenant()
    {
    }

    public Tenant(string name, string code)
        : this(Guid.NewGuid(), name, code)
    {
    }

    public Tenant(Guid id, string name, string code)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Tenant identifier is required.", nameof(id));
        }

        Id = id;
        Name = DomainGuard.Required(name, nameof(name), 200);
        Code = DomainGuard.Required(code, nameof(code), 50).ToUpperInvariant();
        IsActive = true;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Code { get; private set; } = string.Empty;

    public bool IsActive { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
}
