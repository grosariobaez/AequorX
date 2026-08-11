namespace SchoolERP.Domain.People;

public sealed class StudentRelationship
{
    private StudentRelationship()
    {
    }

    public StudentRelationship(
        Person student,
        Person relatedPerson,
        StudentRelationshipType relationshipType,
        bool isPrimaryContact,
        bool isAuthorizedPickup)
    {
        ArgumentNullException.ThrowIfNull(student);
        ArgumentNullException.ThrowIfNull(relatedPerson);
        DomainGuard.SameTenant(student.TenantId, relatedPerson.TenantId);

        Id = Guid.NewGuid();
        TenantId = student.TenantId;
        StudentPersonId = student.Id;
        RelatedPersonId = relatedPerson.Id;
        RelationshipType = relationshipType;
        IsPrimaryContact = isPrimaryContact;
        IsAuthorizedPickup = isAuthorizedPickup;
        Student = student;
        RelatedPerson = relatedPerson;
    }

    public Guid Id { get; private set; }

    public Guid TenantId { get; private set; }

    public Guid StudentPersonId { get; private set; }

    public Guid RelatedPersonId { get; private set; }

    public StudentRelationshipType RelationshipType { get; private set; }

    public bool IsPrimaryContact { get; private set; }

    public bool IsAuthorizedPickup { get; private set; }

    public Person Student { get; private set; } = null!;

    public Person RelatedPerson { get; private set; } = null!;
}

public enum StudentRelationshipType
{
    Mother,
    Father,
    Guardian,
    Tutor,
    Other
}
