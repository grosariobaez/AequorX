# Assessment and Grades Foundation

Phase 2.2 implements the smallest grading vertical slice. Canonical vocabulary is
English; the grading screen defaults to Spanish and also supports English.

## Model and invariants

- Assessment represents one grading activity for one tenant-scoped Class.
- MaximumScore is positive and AssessmentDate falls within the Class Section's AcademicYear.
- Grade links one active Enrollment to one Assessment. Enrollment belongs to the
  Assessment Class's Section and AcademicYear, and Score is between zero and MaximumScore.
- TenantId, AssessmentId, and EnrollmentId uniquely identify a Grade.
- Tenant filters and composite foreign keys protect reads and writes.

## Lifecycle and history

- New or edited grades are Draft.
- Publishing is an explicit operation that changes eligible Draft grades to Published.
- Published and Corrected grades cannot be modified through draft entry.
- Correcting an official grade requires a new score and reason, changes the current
  state to Corrected, and appends GradeCorrection with previous/new scores, reason,
  timestamp, and server-derived actor.
- Grades and correction history have no DELETE endpoints.

## API and UI

The API contains only class assessment listing/creation, assessment roster grade
entry, explicit publishing, and grade correction. The single grading screen follows:
select section, select class, select/create assessment, enter Draft scores, publish, and explicitly
correct an official score with a reason.

## Deferred

Teacher assignments, schedules, weighting formulas, GPA, promotion, report cards,
term averages, configurable scales, competencies/rubrics, notifications,
portals, AI, billing, and fiscal functionality are not implemented.
