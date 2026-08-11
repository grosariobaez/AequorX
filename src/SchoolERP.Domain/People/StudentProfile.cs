namespace SchoolERP.Domain.People;

public sealed class StudentProfile
{
    private StudentProfile()
    {
    }

    public StudentProfile(Person person, string studentNumber)
    {
        ArgumentNullException.ThrowIfNull(person);

        PersonId = person.Id;
        TenantId = person.TenantId;
        StudentNumber = DomainGuard
            .Required(studentNumber, nameof(studentNumber), 50)
            .ToUpperInvariant();
        IsActive = true;
        Person = person;
    }

    public Guid PersonId { get; private set; }

    public Guid TenantId { get; private set; }

    public string StudentNumber { get; private set; } = string.Empty;

    public bool IsActive { get; private set; }

    public Person Person { get; private set; } = null!;
}
