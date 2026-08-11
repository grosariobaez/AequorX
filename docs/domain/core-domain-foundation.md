# Core Domain Foundation

Phase 2.0 establishes the first tenant-scoped vertical slice. Canonical code
vocabulary remains English; the administrative UI defaults to Spanish and also
supports English.

## Ownership model

    AequorX Platform
    └── Tenant (school)
        ├── Campus
        ├── Person
        │   └── StudentProfile
        ├── AcademicYear
        ├── GradeLevel
        ├── Section
        └── Enrollment

StudentRelationship connects a student Person to another Person; Guardian is a
relationship type, not a separate identity entity.

## Implemented invariants

- Cross-tenant relationships are rejected by domain constructors and composite
  database foreign keys.
- AcademicYear.StartDate precedes EndDate.
- Student number, campus code, and grade-level code are unique within a tenant.
- Section references one campus, grade level, and academic year from its tenant.
- Enrollment requires a StudentProfile; its student, section, and academic year
  share the tenant, and the section belongs to the selected academic year.
- Student placement is historical in Enrollment, never mutable placement fields
  on StudentProfile.
- Enrollment has no hard-delete API.

## Tenant isolation

ITenantContext obtains the current tenant from server configuration in this
development phase. EF Core global query filters use the context tenant, and
tenant-aware alternate keys and foreign keys protect writes. API requests never
accept TenantId.

Production identity resolution and tenant switching remain deferred. The
configured development tenant can be provisioned only through the
development-only POST /api/tenant endpoint.

## Phase boundary

Organization, AcademicTerm, Level, Cycle, Class, Attendance, Assessment, Grades,
Admissions, Billing, Payments, DGII, Parent Portal, notifications, and advanced
enrollment workflows are not implemented in Phase 2.0.
