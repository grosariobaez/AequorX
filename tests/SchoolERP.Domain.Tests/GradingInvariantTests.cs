using SchoolERP.Domain.Academic;
using SchoolERP.Domain.Grading;
using SchoolERP.Domain.People;
using SchoolERP.Domain.Platform;

namespace SchoolERP.Domain.Tests;

public sealed class GradingInvariantTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Assessment_requires_positive_maximum_score(decimal maximum) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new Assessment(Setup().Section, "Quiz", new DateOnly(2026, 9, 1), maximum));

    [Fact]
    public void Assessment_date_must_be_inside_academic_year() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new Assessment(Setup().Section, "Quiz", new DateOnly(2027, 7, 1), 100));

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void Grade_score_must_be_inside_assessment_range(decimal score)
    {
        var setup = Setup();
        Assert.Throws<ArgumentOutOfRangeException>(() => new Grade(setup.Assessment, setup.Enrollment, score, "teacher"));
    }

    [Fact]
    public void Grade_rejects_enrollment_from_another_section()
    {
        var setup = Setup();
        var other = Setup("OTHER");
        Assert.Throws<InvalidOperationException>(() => new Grade(setup.Assessment, other.Enrollment, 80, "teacher"));
    }

    [Fact]
    public void Draft_can_change_then_publish_but_published_cannot_be_overwritten()
    {
        var setup = Setup();
        var grade = new Grade(setup.Assessment, setup.Enrollment, 80, "creator");
        grade.UpdateDraft(85, "editor");
        Assert.Equal(85, grade.Score);
        grade.Publish("publisher");
        Assert.Equal(GradeStatus.Published, grade.Status);
        Assert.Throws<InvalidOperationException>(() => grade.UpdateDraft(90, "editor"));
    }

    [Fact]
    public void Correction_preserves_previous_score_reason_and_server_actor()
    {
        var setup = Setup();
        var grade = new Grade(setup.Assessment, setup.Enrollment, 80, "creator");
        grade.Publish("publisher");
        var correction = grade.Correct(90, "Entry error", "corrector");
        Assert.Equal(80, correction.PreviousScore);
        Assert.Equal(90, correction.NewScore);
        Assert.Equal("Entry error", correction.Reason);
        Assert.Equal("corrector", correction.CorrectedBy);
        Assert.Equal(GradeStatus.Corrected, grade.Status);
        Assert.Equal(90, grade.Score);
    }

    private static SetupGraph Setup(string code = "CURRENT")
    {
        var tenant = new Tenant($"School {code}", code);
        var year = new AcademicYear(tenant.Id, "2026-2027", new DateOnly(2026, 8, 1), new DateOnly(2027, 6, 30), AcademicYearStatus.Active);
        var section = new Section(year, new GradeLevel(tenant.Id, "First", $"01-{code}", 1), new Campus(tenant, "Main", $"MAIN-{code}"), "A", $"A-{code}");
        var student = new StudentProfile(new Person(tenant.Id, "Ana", "Pérez"), $"S-{code}");
        var enrollment = new Enrollment(student, year, section, EnrollmentStatus.Active, new DateOnly(2026, 8, 15));
        var assessment = new Assessment(section, "Quiz", new DateOnly(2026, 9, 1), 100);
        return new SetupGraph(section, enrollment, assessment);
    }

    private sealed record SetupGraph(Section Section, Enrollment Enrollment, Assessment Assessment);
}
