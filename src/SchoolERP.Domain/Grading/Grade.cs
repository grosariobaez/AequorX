using SchoolERP.Domain.Academic;

namespace SchoolERP.Domain.Grading;

public sealed class Grade
{
    private Grade() { }

    public Grade(Assessment assessment, Enrollment enrollment, decimal score, string actor)
    {
        ArgumentNullException.ThrowIfNull(assessment);
        ArgumentNullException.ThrowIfNull(enrollment);
        EnsureCompatible(assessment, enrollment);
        EnsureScore(assessment, score);

        var timestamp = DateTimeOffset.UtcNow;
        Id = Guid.NewGuid();
        TenantId = assessment.TenantId;
        AssessmentId = assessment.Id;
        EnrollmentId = enrollment.Id;
        Score = score;
        Status = GradeStatus.Draft;
        CreatedAt = timestamp;
        CreatedBy = DomainGuard.Required(actor, nameof(actor), 200);
        UpdatedAt = timestamp;
        UpdatedBy = CreatedBy;
        Assessment = assessment;
        Enrollment = enrollment;
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid AssessmentId { get; private set; }
    public Guid EnrollmentId { get; private set; }
    public decimal Score { get; private set; }
    public GradeStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public string CreatedBy { get; private set; } = string.Empty;
    public DateTimeOffset UpdatedAt { get; private set; }
    public string UpdatedBy { get; private set; } = string.Empty;
    public Assessment Assessment { get; private set; } = null!;
    public Enrollment Enrollment { get; private set; } = null!;

    public void UpdateDraft(decimal score, string actor)
    {
        if (Status != GradeStatus.Draft) throw new InvalidOperationException("Only Draft grades may be modified directly.");
        EnsureScore(Assessment, score);
        Score = score;
        Touch(actor);
    }

    public void Publish(string actor)
    {
        if (Status != GradeStatus.Draft) throw new InvalidOperationException("Only Draft grades may be published.");
        Status = GradeStatus.Published;
        Touch(actor);
    }

    public GradeCorrection Correct(decimal newScore, string reason, string actor)
    {
        if (Status is not (GradeStatus.Published or GradeStatus.Corrected))
            throw new InvalidOperationException("Only published results may be corrected.");
        EnsureScore(Assessment, newScore);
        var correction = new GradeCorrection(this, Score, newScore, reason, actor);
        Score = newScore;
        Status = GradeStatus.Corrected;
        UpdatedAt = correction.CorrectedAt;
        UpdatedBy = correction.CorrectedBy;
        return correction;
    }

    private static void EnsureCompatible(Assessment assessment, Enrollment enrollment)
    {
        DomainGuard.SameTenant(assessment.TenantId, enrollment.TenantId);
        if (assessment.SectionId != enrollment.SectionId)
            throw new InvalidOperationException("Enrollment must belong to the assessment section.");
        if (assessment.Section.AcademicYearId != enrollment.AcademicYearId)
            throw new InvalidOperationException("Enrollment must belong to the assessment academic year.");
    }

    private static void EnsureScore(Assessment assessment, decimal score)
    {
        if (score < 0 || score > assessment.MaximumScore)
            throw new ArgumentOutOfRangeException(nameof(score), "Score must be within the assessment range.");
    }

    private void Touch(string actor)
    {
        UpdatedAt = DateTimeOffset.UtcNow;
        UpdatedBy = DomainGuard.Required(actor, nameof(actor), 200);
    }
}

public enum GradeStatus { Draft, Published, Corrected }
