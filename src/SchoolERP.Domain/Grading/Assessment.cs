using SchoolERP.Domain.Academic;

namespace SchoolERP.Domain.Grading;

public sealed class Assessment
{
    private Assessment() { }

    public Assessment(Class @class, string name, DateOnly assessmentDate, decimal maximumScore)
    {
        ArgumentNullException.ThrowIfNull(@class);
        if (maximumScore <= 0) throw new ArgumentOutOfRangeException(nameof(maximumScore), "Maximum score must be greater than zero.");
        if (assessmentDate < @class.Section.AcademicYear.StartDate || assessmentDate > @class.Section.AcademicYear.EndDate)
            throw new ArgumentOutOfRangeException(nameof(assessmentDate), "Assessment date must fall within the class academic year.");

        Id = Guid.NewGuid();
        TenantId = @class.TenantId;
        ClassId = @class.Id;
        Name = DomainGuard.Required(name, nameof(name), 200);
        AssessmentDate = assessmentDate;
        MaximumScore = maximumScore;
        IsActive = true;
        CreatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = CreatedAt;
        Class = @class;
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid ClassId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public DateOnly AssessmentDate { get; private set; }
    public decimal MaximumScore { get; private set; }
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public Class Class { get; private set; } = null!;
}
