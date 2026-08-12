namespace SchoolERP.Domain.Academic;

public sealed class Class
{
    private Class() { }

    public Class(Section section, Subject subject, string? name = null)
    {
        ArgumentNullException.ThrowIfNull(section);
        ArgumentNullException.ThrowIfNull(subject);
        DomainGuard.SameTenant(section.TenantId, subject.TenantId);

        Id = Guid.NewGuid();
        TenantId = section.TenantId;
        SectionId = section.Id;
        SubjectId = subject.Id;
        Name = DomainGuard.Required(
            string.IsNullOrWhiteSpace(name) ? $"{subject.Name} · {section.Name}" : name,
            nameof(name),
            200);
        IsActive = true;
        CreatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = CreatedAt;
        Section = section;
        Subject = subject;
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid SectionId { get; private set; }
    public Guid SubjectId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public Section Section { get; private set; } = null!;
    public Subject Subject { get; private set; } = null!;
}
