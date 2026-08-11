# Attendance Foundation

Phase 2.1 implements the smallest attendance-by-exception slice. Canonical code
vocabulary remains English; the attendance screen defaults to Spanish and also
supports English.

## Effective attendance

- No AttendanceRecord means Present.
- Persisted exception statuses are Absent, Late, Excused, and EarlyDeparture.
- Present is never persisted by this phase.
- Only active enrollments in the selected section appear in the attendance roster.

## Invariants

- AttendanceRecord, Enrollment, and Section share the current tenant.
- Enrollment belongs to the recorded Section.
- AttendanceDate falls within the Enrollment AcademicYear dates.
- TenantId, EnrollmentId, and AttendanceDate uniquely identify an exception.
- Tenant-aware query filters and composite foreign keys protect reads and writes.

## Corrections and audit

Changing one exception to another updates the existing AttendanceRecord. CreatedAt
and CreatedBy remain unchanged; UpdatedAt and UpdatedBy identify the latest
correction. Returning a student to Present removes the exception only through the
attendance PUT operation. There is no attendance DELETE endpoint.

Audit identity is supplied by the server-side IAuditContext. During development it
comes from Audit:Actor configuration; clients cannot provide or override it.

## API and UI

- GET `/api/attendance?sectionId={id}&date={yyyy-MM-dd}` returns the active roster
  with each student's effective status.
- PUT `/api/attendance/{enrollmentId}/{date}` creates, corrects, or removes an
  exception through the defined operation.
- The single attendance screen follows: select date, select section, load roster,
  mark exceptions, save.

## Deferred

Schedules, periods, subjects/classes, teacher assignment, notifications, absence
justification workflows, medical notes, reports/dashboards, grades, assessments,
billing, and AI are not implemented.
