# Subjects and Classes Foundation

Phase 2.3 adds the smallest tenant-scoped instructional structure required by
grading. Canonical vocabulary remains English; the administrative UI defaults to
Spanish and also supports English.

## Model

    AcademicYear
        └── Section
              └── Class
                    ├── Subject
                    └── Assessment
                          └── Grade

- Subject is reusable across sections and academic years. Its code is unique per
  tenant.
- Class represents one Subject taught to one Section. TenantId, SectionId, and
  SubjectId uniquely identify it in this phase.
- Class, Section, and Subject must share a tenant. Class derives its academic year
  from Section and contains no teacher, schedule, room, or period data.
- Assessment belongs to Class and derives Section, AcademicYear, and Tenant through
  that relationship.

## Grading invariants

- AssessmentDate remains inside the Class Section's AcademicYear.
- A Grade Enrollment belongs to the Assessment Class's Section and AcademicYear.
- The Draft, Published, and Corrected lifecycle and append-only GradeCorrection
  history are unchanged.

## Migration safety

The Phase 2.3 migration creates Subjects and Classes and replaces the Assessment
Section foreign key with Class. An existing Assessment does not contain enough
information to infer its Subject. The migration therefore stops before schema
changes when Assessment rows exist and requests an explicit Subject/Class mapping;
it never fabricates academic data or drops grading history. Empty development,
test, and new databases migrate automatically.

## API and UI

The API exposes list/create operations for Subjects and section-filtered Classes.
Assessment list/create operations use ClassId. The grading UI follows Section →
Class → Assessment → Grades, with focused list/create screens for Subjects and
Classes.

## Deferred

Schedules, periods, teacher assignments, classrooms, curriculum plans,
competencies, prerequisites, grading weights, averages, GPA, report cards,
promotion, notifications, portals, AI, billing, and fiscal functionality remain
deferred.
