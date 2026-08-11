namespace SchoolERP.Domain.People;

public sealed class Person
{
    private Person()
    {
    }

    public Person(
        Guid tenantId,
        string firstName,
        string lastName,
        string? middleName = null,
        string? secondLastName = null,
        string? preferredName = null,
        DateOnly? dateOfBirth = null,
        string? email = null,
        string? phone = null)
    {
        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException("Tenant is required.", nameof(tenantId));
        }

        Id = Guid.NewGuid();
        TenantId = tenantId;
        FirstName = DomainGuard.Required(firstName, nameof(firstName), 100);
        MiddleName = NormalizeOptional(middleName, 100, nameof(middleName));
        LastName = DomainGuard.Required(lastName, nameof(lastName), 100);
        SecondLastName = NormalizeOptional(secondLastName, 100, nameof(secondLastName));
        PreferredName = NormalizeOptional(preferredName, 100, nameof(preferredName));
        DateOfBirth = dateOfBirth;
        Email = NormalizeOptional(email, 254, nameof(email));
        Phone = NormalizeOptional(phone, 50, nameof(phone));
        IsActive = true;
        CreatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public Guid Id { get; private set; }

    public Guid TenantId { get; private set; }

    public string FirstName { get; private set; } = string.Empty;

    public string? MiddleName { get; private set; }

    public string LastName { get; private set; } = string.Empty;

    public string? SecondLastName { get; private set; }

    public string? PreferredName { get; private set; }

    public DateOnly? DateOfBirth { get; private set; }

    public string? Email { get; private set; }

    public string? Phone { get; private set; }

    public bool IsActive { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    private static string? NormalizeOptional(string? value, int maximumLength, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return DomainGuard.Required(value, parameterName, maximumLength);
    }
}
