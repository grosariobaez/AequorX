namespace SchoolERP.Domain.Grading;

public sealed class GradeCorrection
{
    private GradeCorrection() { }

    internal GradeCorrection(Grade grade, decimal previousScore, decimal newScore, string reason, string actor)
    {
        Id = Guid.NewGuid();
        TenantId = grade.TenantId;
        GradeId = grade.Id;
        PreviousScore = previousScore;
        NewScore = newScore;
        Reason = DomainGuard.Required(reason, nameof(reason), 1000);
        CorrectedAt = DateTimeOffset.UtcNow;
        CorrectedBy = DomainGuard.Required(actor, nameof(actor), 200);
        Grade = grade;
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid GradeId { get; private set; }
    public decimal PreviousScore { get; private set; }
    public decimal NewScore { get; private set; }
    public string Reason { get; private set; } = string.Empty;
    public DateTimeOffset CorrectedAt { get; private set; }
    public string CorrectedBy { get; private set; } = string.Empty;
    public Grade Grade { get; private set; } = null!;
}
