# School ERP RD
# Technical Architecture Gate 1.1

**Document ID:** TA-GATE-1.1  
**Version:** 1.0  
**Status:** APPROVED BASELINE  
**Project:** School ERP RD  
**Parent Document:** `/docs/SDD.md`  
**Related Contract:** `/AGENTS.md`  
**Phase:** 1 — System Design  
**Gate:** 1.1 — Technical Stack Selection  
**Market:** Dominican Republic  
**Architecture Style:** Modular Monolith  
**Guiding Principle:**

> **“Make every single detail perfect, and limit the number of details.”**

---

# 1. Purpose

Este documento define el stack tecnológico aprobado para la primera implementación de **School ERP RD**.

Su objetivo es cerrar únicamente las decisiones técnicas necesarias para iniciar el **Solution Bootstrap** sin introducir infraestructura, frameworks, dependencias o abstracciones que todavía no estén justificadas por requisitos reales.

Este documento define:

- backend runtime;
- backend framework;
- frontend framework;
- database;
- ORM;
- hosting;
- identity direction;
- secrets management;
- observability;
- CI/CD;
- testing baseline;
- deployment model;
- local development expectations;
- multi-tenancy technical direction;
- dependency restrictions;
- deferred technical decisions.

Este documento no redefine el modelo de dominio.

El modelo de dominio definido en `/docs/SDD.md` continúa siendo la autoridad funcional y arquitectónica principal.

---

# 2. Technical Design Principle

Todas las decisiones de este documento deben respetar:

> **Make every single detail perfect, and limit the number of details.**

Aplicado al stack significa:

- one backend runtime;
- one primary relational database;
- one frontend framework;
- one deployment unit for the MVP;
- one primary ORM;
- one CI/CD platform;
- no unnecessary broker;
- no unnecessary cache;
- no unnecessary container orchestration;
- no microservices;
- no duplicate frameworks solving the same problem;
- no infrastructure without a proven requirement.

La tecnología debe servir al dominio.

El dominio no debe adaptarse artificialmente a una tecnología.

---

# 3. Architecture Context

La arquitectura funcional aprobada sigue siendo:

```text
Administration Web
Teacher Web
Parent Web
      │
      ▼
ASP.NET Core API / Application
      │
      ▼
Modular Monolith
      │
      ├── Identity & Access
      ├── People
      ├── Academic
      ├── Enrollment
      ├── Attendance
      ├── Assessment
      ├── Billing
      ├── Payments
      ├── Fiscal
      ├── Notifications
      └── Audit
      │
      ▼
Azure SQL Database
      │
      ├── DGII
      ├── AZUL
      ├── CardNet
      └── Notification Providers
```

La solución permanecerá inicialmente como un único deployable backend.

---

# 4. Approved Technology Stack

El stack aprobado para el MVP es:

| Layer | Technology | Decision |
|---|---|---|
| Runtime | .NET 10 LTS | APPROVED |
| Backend | ASP.NET Core 10 | APPROVED |
| ORM | Entity Framework Core 10 | APPROVED |
| Database | Azure SQL Database | APPROVED |
| Frontend | Angular 22 + TypeScript | APPROVED |
| Backend Hosting | Azure App Service | APPROVED |
| Identity | Microsoft Entra platform / External ID | APPROVED WITH DESIGN BOUNDARY |
| Secrets | Azure Key Vault | APPROVED |
| Azure Authentication | Managed Identity | APPROVED |
| Observability | OpenTelemetry | APPROVED |
| Monitoring Backend | Azure Monitor / Application Insights | APPROVED |
| CI/CD | GitHub Actions | APPROVED |
| Source Control | GitHub | APPROVED |
| Background Processing | TBD when required | DEFERRED |
| Distributed Cache | None | NOT REQUIRED |
| Message Broker | None | NOT REQUIRED |
| Kubernetes | None | REJECTED FOR MVP |
| Microservices | None | REJECTED FOR MVP |
| Native Mobile | None | POST-MVP |

---

# 5. ADR-016 — Backend Runtime

## Decision

School ERP RD backend SHALL use:

```text
.NET 10 LTS
ASP.NET Core 10
C#
```

## Rationale

.NET 10 provides:

- long-term support;
- mature enterprise ecosystem;
- strong ASP.NET Core capabilities;
- first-class SQL Server/Azure SQL support;
- excellent performance;
- mature authentication/authorization libraries;
- mature testing ecosystem;
- strong observability support;
- dependency injection;
- configuration;
- background service primitives;
- strong typing;
- excellent development support in Visual Studio and VS Code.

## Constraint

No additional backend runtime shall be introduced into the MVP without an approved ADR.

This includes:

- Node.js backend services;
- Python backend services;
- Java services;
- Go services.

Such runtimes may be introduced later only when a real requirement cannot be reasonably satisfied within the primary stack.

---

# 6. Backend Application Style

ASP.NET Core will expose the application through HTTP APIs.

The backend shall remain:

```text
Modular Monolith
```

not:

```text
Microservices
```

The logical modules defined by the SDD remain internal architectural boundaries.

---

# 7. Backend Layers

The initial logical layering is:

```text
Presentation / API
        ↓
Application
        ↓
Domain
        ↑
Infrastructure
```

Responsibilities:

## API

- HTTP transport;
- authentication boundary;
- request/response handling;
- API-level validation;
- error mapping.

## Application

- use cases;
- orchestration;
- commands;
- queries;
- authorization coordination;
- transaction coordination;
- domain interaction.

## Domain

- entities;
- value objects;
- domain rules;
- invariants;
- state transitions;
- domain events;
- canonical business behavior.

## Infrastructure

- EF Core;
- Azure SQL;
- payment adapters;
- DGII adapter;
- notification adapters;
- identity integration;
- telemetry implementation.

---

# 8. Project Structure

The initial .NET solution SHOULD remain small.

Recommended structure:

```text
SchoolERP.sln

src/
├── SchoolERP.Api/
├── SchoolERP.Application/
├── SchoolERP.Domain/
└── SchoolERP.Infrastructure/

tests/
├── SchoolERP.Domain.Tests/
├── SchoolERP.Application.Tests/
├── SchoolERP.IntegrationTests/
└── SchoolERP.ArchitectureTests/
```

Do not create a separate .NET project for every bounded context during bootstrap.

Modules shall initially be represented as internal folders/namespaces within the appropriate layers.

Example:

```text
SchoolERP.Domain/
├── People/
├── Academic/
├── Enrollment/
├── Attendance/
├── Assessment/
├── Billing/
├── Payments/
└── Fiscal/
```

This decision minimizes project sprawl while preserving domain boundaries.

---

# 9. Namespace Convention

Namespaces SHOULD reflect:

```text
SchoolERP.[Layer].[Module]
```

Examples:

```text
SchoolERP.Domain.People
SchoolERP.Domain.Enrollment
SchoolERP.Application.Attendance
SchoolERP.Infrastructure.Payments.Azul
```

Avoid generic namespaces such as:

```text
SchoolERP.Common
SchoolERP.Helpers
SchoolERP.Managers
SchoolERP.Utils
```

unless a narrowly defined technical purpose exists.

---

# 10. ADR-017 — Frontend

## Decision

The web frontend SHALL use:

```text
Angular 22
TypeScript
```

## Initial frontend model

The MVP SHALL begin with a single Angular workspace/application:

```text
SchoolERP.Web
```

The following experiences SHALL initially exist within the same application:

```text
Administration
Teacher
Parent
```

using:

- routing;
- authorization;
- layout shells;
- feature areas.

Do not create three independent frontend applications unless future operational requirements justify separation.

---

# 11. Frontend Architecture Principle

Angular features should follow domain-oriented organization.

Recommended conceptual structure:

```text
src/app/

core/
shared/

features/
├── people/
├── academic/
├── enrollment/
├── attendance/
├── assessment/
├── billing/
├── payments/
├── parent/
└── administration/
```

Avoid global `components`, `services`, or `models` folders containing unrelated concepts.

---

# 12. Frontend UX Standard

All frontend implementation must follow:

> One screen should have one dominant job.

Frequent tasks must minimize interaction.

Examples:

Attendance:

```text
Open class
→ mark exceptions
→ save
```

Grade entry:

```text
Open assessment
→ enter grades
→ save draft/publish
```

Parent payment:

```text
Open account
→ select balance
→ pay
```

---

# 13. Frontend State

Do not introduce a global state management library during bootstrap unless a concrete requirement demonstrates its necessity.

Start with:

- Angular services;
- signals;
- feature-scoped state;
- standard Angular patterns.

Do not introduce NgRx or equivalent merely as an architectural preference.

A state-management dependency requires demonstrated complexity.

---

# 14. Frontend Styling

The UI design system must remain consistent.

The final component library/design system selection may occur during UI bootstrap.

Requirements:

- accessible;
- responsive;
- enterprise-friendly;
- maintainable;
- compatible with Angular 22.

Do not mix multiple major component libraries.

---

# 15. ADR-018 — Database

## Decision

Production persistence SHALL use:

```text
Azure SQL Database
```

## Rationale

School ERP RD is highly relational and transactional.

Key requirements include:

- referential integrity;
- financial consistency;
- enrollment history;
- payment allocation;
- audit;
- complex queries;
- transactions;
- uniqueness constraints;
- tenant isolation.

A relational database is the correct primary persistence model.

---

# 16. Database Anti-Patterns

The MVP SHALL NOT use:

- MongoDB as primary database;
- Cosmos DB as primary database;
- generic document storage for domain entities;
- polyglot persistence without requirement;
- EAV as primary domain modeling strategy.

---

# 17. Local Database

Local development SHALL use a SQL Server-compatible environment.

Acceptable choices include:

- SQL Server Developer;
- SQL Server container;
- approved local SQL environment.

Production behavior must remain compatible with Azure SQL Database.

Development code shall not rely on SQL Server features unavailable in Azure SQL Database unless explicitly approved.

---

# 18. ADR-019 — ORM

## Decision

Primary data access SHALL use:

```text
Entity Framework Core 10
```

## Rules

EF Core SHALL be used for:

- persistence;
- transactions;
- schema migrations;
- standard read/write operations;
- query projections.

Do not add Dapper during bootstrap.

Dapper or raw SQL may be introduced later only for a demonstrated query/performance requirement.

---

# 19. Repository Pattern

Do NOT introduce a generic repository abstraction such as:

```text
IRepository<T>
GenericRepository<T>
RepositoryFactory
```

by default.

EF Core DbContext already provides useful unit-of-work/repository behavior.

Use repositories only when they represent meaningful domain persistence boundaries or aggregates.

---

# 20. DbContext Strategy

The initial implementation may use one application DbContext while retaining logical ownership boundaries between modules.

Do not prematurely create:

```text
PeopleDbContext
EnrollmentDbContext
BillingDbContext
PaymentsDbContext
...
```

unless modular persistence requirements justify it.

A future ADR may split contexts while still sharing a physical database.

---

# 21. Database Migrations

EF Core migrations SHALL be the approved schema migration mechanism.

Rules:

- migrations committed to source control;
- migration names meaningful;
- no automatic destructive migration on production startup;
- migrations reviewed;
- backwards compatibility preferred where practical.

---

# 22. Multi-Tenancy Physical Model

## Initial Decision

The MVP SHALL use:

```text
Shared Azure SQL Database
+
Shared Schema
+
TenantId
```

The design SHALL NOT use database-per-tenant initially.

---

# 23. Tenant Data Rules

All tenant-owned entities SHALL include an explicit tenant boundary.

Conceptually:

```text
TenantId
```

Tenant ownership must be enforced:

- at application boundary;
- during data access;
- through constraints where appropriate;
- through automated tests.

---

# 24. Tenant-Aware Uniqueness

Business uniqueness must usually include TenantId.

Example:

```text
UNIQUE (
    TenantId,
    StudentNumber
)
```

rather than:

```text
UNIQUE (
    StudentNumber
)
```

unless uniqueness is intentionally global.

---

# 25. Tenant Resolution

TenantId SHALL NOT be trusted simply because it appears in:

- route;
- query string;
- request body;
- frontend state.

Tenant context must be derived or verified against authenticated identity and authorized membership.

---

# 26. Row-Level Security

Azure SQL Row-Level Security is NOT mandatory for the initial MVP.

Application-level tenant enforcement remains mandatory.

Database-level RLS may later be evaluated as defense-in-depth through an ADR.

Do not introduce it simply because it exists.

---

# 27. ADR-020 — Hosting

## Decision

The ASP.NET Core backend SHALL initially run on:

```text
Azure App Service
```

## Rationale

App Service provides:

- managed hosting;
- native .NET support;
- TLS;
- deployment slots;
- autoscaling options;
- Managed Identity;
- Application Insights integration;
- reduced operational burden.

---

# 28. Deployment Model

Initial production topology:

```text
Internet
   ↓
Azure App Service
   ↓
ASP.NET Core Modular Monolith
   ↓
Azure SQL Database
```

Angular deployment may initially use an appropriate Azure-hosted static/web deployment model selected during bootstrap.

Avoid unnecessary distributed components.

---

# 29. Containers

Containers are NOT required for production MVP.

Containerization may be used for local development if helpful.

Do not introduce:

- AKS;
- Kubernetes;
- service mesh;
- container orchestration

without an approved ADR.

---

# 30. ADR-021 — Identity Direction

## Decision

Identity SHALL use Microsoft's Entra identity platform rather than a custom credential system.

School ERP RD SHALL NOT build its own:

- password authentication protocol;
- MFA engine;
- OAuth server;
- OpenID provider;
- password reset system.

---

# 31. User vs Person

This distinction is mandatory:

```text
User
≠
Person
```

`Person` represents a business identity.

`User` represents an authenticated system identity.

Examples:

```text
Student Person
User account = optional
```

```text
Parent Person
User account = yes
```

```text
Employee Person
User account = yes
```

A User may reference a Person where appropriate.

---

# 32. Internal and External Users

The product has two primary identity audiences:

## Workforce / Internal

- administrators;
- teachers;
- finance staff;
- registrars.

## External / Customer

- parents;
- guardians;
- potentially students.

Microsoft Entra / Entra External ID SHALL provide the identity foundation.

The exact tenant/authority topology remains an implementation design decision to be finalized before production authentication implementation.

---

# 33. Identity Architectural Boundary

Application roles and permissions remain owned by School ERP RD.

External identity provider owns authentication.

Conceptually:

```text
Microsoft Entra
      ↓
Authenticated Identity
      ↓
School ERP User
      ↓
Tenant Membership
      ↓
Role / Permission
```

Do not place core business authorization exclusively in Entra groups or tokens.

---

# 34. Authorization

Application authorization SHALL use:

```text
RBAC
+
resource/context authorization
```

Initially.

Do not build a generalized ABAC engine.

Authorization rules must be enforced server-side.

---

# 35. ADR-022 — Secrets

## Decision

Production secrets SHALL use:

```text
Azure Key Vault
```

where secrets are required.

The application SHALL use:

```text
Managed Identity
```

for supported Azure resource authentication.

---

# 36. Managed Identity

Preferred Azure authentication:

```text
App Service
     ↓
Managed Identity
     ↓
Azure SQL
```

and:

```text
App Service
     ↓
Managed Identity
     ↓
Key Vault
```

Avoid persisted Azure passwords/connection secrets when Managed Identity is available.

---

# 37. Secrets That May Require Key Vault

Potential examples:

- DGII credentials/certificates;
- AZUL credentials;
- CardNet credentials;
- email provider secrets;
- WhatsApp provider secrets;
- signing material where required.

Never commit secrets.

---

# 38. Configuration

Non-sensitive configuration SHALL use standard application configuration.

Sensitive configuration SHALL use Key Vault/secret references.

Environment-specific configuration must remain explicit.

---

# 39. ADR-023 — Observability

## Decision

Instrumentation SHALL use:

```text
OpenTelemetry
```

The initial Azure telemetry backend SHALL be:

```text
Azure Monitor
+
Application Insights
```

---

# 40. Observability Goals

The application must provide visibility into:

- HTTP requests;
- exceptions;
- SQL performance;
- external HTTP calls;
- payment operations;
- fiscal operations;
- background operations when added.

---

# 41. Observability Principle

Instrument meaningful operations.

Do not create excessive telemetry merely because it is available.

Focus on signals that help answer:

- Is the system healthy?
- What failed?
- Which tenant/workflow was affected?
- Which external provider caused delay/failure?
- Can the operation be safely retried?

---

# 42. Correlation

All meaningful requests SHOULD have:

```text
CorrelationId
```

Logs and external workflow telemetry should propagate correlation context where appropriate.

---

# 43. Logging

Use structured logging.

Important context may include:

```text
CorrelationId
TenantId
UserId
Module
Operation
EntityId
Provider
```

Never include sensitive payloads unnecessarily.

---

# 44. ADR-024 — CI/CD

## Decision

Source control and CI/CD SHALL use:

```text
GitHub
GitHub Actions
```

---

# 45. Pull Request Pipeline

Every pull request SHALL initially execute:

```text
Checkout
   ↓
Restore backend
   ↓
Build backend
   ↓
Backend tests
   ↓
Restore frontend
   ↓
Frontend build
   ↓
Frontend tests
   ↓
Lint/static checks
```

Architecture tests SHOULD run where configured.

---

# 46. Main Branch Pipeline

On approved merge to main:

```text
Build
   ↓
Test
   ↓
Create deployable artifacts
   ↓
Deploy development environment
```

Production deployment SHALL NOT initially occur automatically from every merge.

---

# 47. Branch Protection

`main` SHOULD be protected.

Expected controls:

- PR required;
- CI required;
- no direct uncontrolled pushes;
- review required according to team process.

---

# 48. Environment Strategy

Initial environments:

```text
Local
Development
Production
```

A separate staging/test environment may be added before production if deployment validation requires it.

Do not create many environments without operational need.

---

# 49. Environment Configuration

Each environment SHALL have separate:

- configuration;
- secrets;
- database;
- provider credentials;
- telemetry context.

Production secrets shall never be reused in development.

---

# 50. Testing Stack

Backend testing SHALL use the standard .NET test ecosystem selected during bootstrap.

Preferred baseline:

```text
xUnit
```

unless the repository already establishes a different approved framework.

Use only one primary unit test framework.

---

# 51. Backend Test Layers

## Domain Tests

Test:

- invariants;
- policies;
- transitions;
- value objects;
- financial calculations.

## Application Tests

Test:

- use cases;
- orchestration;
- authorization behavior;
- commands/queries.

## Integration Tests

Test:

- database behavior;
- EF Core mappings;
- external provider adapters;
- infrastructure.

## Architecture Tests

Test:

- forbidden dependencies;
- module boundaries where practical.

---

# 52. Frontend Testing

Angular testing SHALL cover high-value behavior.

Do not seek arbitrary coverage percentages.

Test especially:

- critical forms;
- authorization-driven navigation;
- attendance flow;
- grade entry;
- payment flow;
- error states.

---

# 53. End-to-End Testing

End-to-end automation SHALL be added first for critical workflows rather than broad superficial coverage.

Candidates:

1. login;
2. enrollment;
3. attendance;
4. payment;
5. fiscal workflow in test/sandbox environment.

Exact E2E framework selection may occur during bootstrap.

---

# 54. Formatting and Static Analysis

The bootstrap SHALL configure:

Backend:

- standard .NET formatting;
- nullable reference types;
- analyzers as appropriate.

Frontend:

- TypeScript strictness;
- ESLint;
- Angular build checks.

Do not adopt a large collection of overlapping analyzers.

---

# 55. C# Language Rules

Use modern C# supported by .NET 10.

Enable:

```text
Nullable
```

and appropriate compiler warnings.

Avoid excessive language cleverness.

Prefer readable, explicit domain code.

---

# 56. TypeScript Rules

Use strict TypeScript configuration.

Avoid:

```text
any
```

except at unavoidable external/interop boundaries and with explicit justification.

External API responses must be mapped into internal typed contracts.

---

# 57. API Documentation

The backend SHALL expose machine-readable API documentation during development.

OpenAPI SHALL be supported.

Do not treat OpenAPI as the business specification.

The SDD and requirements remain authoritative.

---

# 58. API Versioning

Do not introduce complex versioning during bootstrap.

The API starts as one version.

Formal API versioning shall be introduced when a real compatibility requirement exists.

---

# 59. Validation Strategy

Backend SHALL enforce validation.

Frontend validation exists for UX only.

Domain invariants cannot depend solely on frontend validation.

---

# 60. Error Contract

The API SHALL use a consistent error model.

Minimum error categories:

```text
Validation
BusinessRuleViolation
Unauthorized
Forbidden
NotFound
Conflict
ConcurrencyConflict
ExternalProviderFailure
UnexpectedFailure
```

Use standard HTTP semantics where appropriate.

---

# 61. Background Processing

## Status

```text
DEFERRED
```

No background processing library is selected during Gate 1.1.

A future ADR will select one when the first durable workflow requires it.

Likely triggers:

- monthly billing;
- fiscal retry;
- notification delivery;
- reconciliation.

---

# 62. Background Processing Selection Criteria

When required, evaluate:

- durability;
- retries;
- idempotency;
- scheduling;
- Azure compatibility;
- SQL-backed option;
- operational simplicity.

Do not select based solely on popularity.

---

# 63. Message Broker

## Decision

No message broker for MVP bootstrap.

Do not introduce:

- Azure Service Bus;
- Kafka;
- RabbitMQ;
- Event Hubs

until there is a demonstrated durable cross-process messaging requirement.

Internal domain events remain in-process initially.

---

# 64. Distributed Cache

## Decision

No distributed cache during bootstrap.

Do not add Redis preemptively.

Introduce caching only after:

- performance issue exists;
- query/index optimization has been evaluated;
- cache semantics are clearly defined.

---

# 65. Local Cache

Normal in-memory caching may be used for small stable technical data only when useful.

Do not cache financial or tenant-sensitive state without explicit invalidation design.

---

# 66. File Storage

File/document storage technology is not frozen by this gate.

When required for identity documents or future admission documents, storage shall be placed behind a document/file boundary.

Do not store large binary documents directly in relational entities without an approved design.

---

# 67. Notification Provider

Email/WhatsApp/push vendor selection is deferred.

The application SHALL use a provider abstraction.

Conceptually:

```text
NotificationService
        ↓
NotificationProvider
```

No domain module should depend directly on vendor SDKs.

---

# 68. DGII Provider

The Fiscal module SHALL expose a provider boundary.

Conceptually:

```text
Fiscal Application
       ↓
FiscalProvider
       ↓
DgiiFiscalProvider
```

DGII transport details belong in Infrastructure.

---

# 69. Payment Providers

AZUL is an approved payment provider candidate for implementation.

CardNet may be implemented once its concrete integration contract is approved.

Provider implementation SHALL remain isolated.

---

# 70. External HTTP

Use standard .NET HTTP client infrastructure.

External calls SHALL define:

- timeout;
- logging;
- error mapping;
- authentication;
- retry decision;
- idempotency behavior.

Avoid scattered raw `HttpClient` construction.

---

# 71. Resilience Library

No resilience library is mandated during Gate 1.1.

ASP.NET/.NET resilience capabilities or an approved focused dependency may be introduced when implementing external providers.

Avoid global retry policies that could duplicate financial/fiscal operations.

---

# 72. Transactions

Local database operations requiring atomicity SHALL use explicit transaction boundaries where necessary.

Do not keep database transactions open during slow external provider calls.

---

# 73. Concurrency

EF Core concurrency mechanisms SHALL be evaluated for entities requiring protection.

Candidates:

- Invoice;
- Payment;
- PaymentAllocation;
- Enrollment;
- Grade;
- FiscalDocument.

The exact concurrency token strategy may vary by aggregate.

---

# 74. Financial Precision

C#:

```text
decimal
```

for monetary values.

SQL:

appropriate fixed decimal precision.

Do not use:

```text
float
double
```

for money.

---

# 75. Time Strategy

System timestamps SHOULD use UTC internally.

Business dates remain distinct.

User-facing school dates SHALL use tenant/local timezone.

Initial market timezone:

```text
America/Santo_Domingo
```

---

# 76. IDs

Identifier technology may use GUID/UUID or another approved consistent strategy.

Do not expose database identity assumptions unnecessarily into domain behavior.

The final strategy shall be selected during bootstrap and used consistently.

---

# 77. Database Naming

Database naming conventions SHALL be consistent.

Prefer explicit domain terminology.

Avoid cryptic table/column abbreviations.

The physical naming convention will be selected during bootstrap but must remain stable once migrations begin.

---

# 78. Soft Delete

No universal `IsDeleted` implementation shall be introduced across all entities.

Deletion/lifecycle behavior is domain-specific.

Use:

- Active/Inactive;
- Cancelled;
- Reversed;
- Archived;
- historical state

according to approved domain behavior.

---

# 79. Audit Persistence

AuditEvent SHALL use durable relational persistence initially unless a future requirement justifies another mechanism.

Audit data should be queryable while protected from normal modification.

---

# 80. Security Headers

Web hosting SHOULD enforce appropriate standard web security headers.

Exact policy will be configured during application bootstrap.

---

# 81. CORS

CORS SHALL be restrictive.

Do not configure:

```text
AllowAnyOrigin
AllowAnyHeader
AllowAnyMethod
```

for production without an explicit reason.

Allowed origins SHALL be environment-specific.

---

# 82. HTTPS

Production SHALL enforce HTTPS.

No sensitive application endpoint shall be exposed over plain HTTP.

---

# 83. Rate Limiting

Rate limiting SHOULD be considered for public/external endpoints such as:

- authentication-related application endpoints;
- payment callbacks where appropriate;
- public portal APIs.

Do not apply arbitrary limits to internal flows without usage analysis.

---

# 84. Provider Callback Security

Payment/fiscal callbacks MUST validate provider authenticity using the official provider mechanism.

Never trust callback data solely because it reaches the endpoint.

---

# 85. Secrets in Logs

The logging pipeline SHALL redact or exclude secrets.

This includes:

- authorization headers;
- payment tokens where sensitive;
- DGII signing material;
- full identity tokens.

---

# 86. Database Backups

Azure SQL managed backup capabilities SHALL be used as the production baseline.

Additional recovery requirements shall align with the SDD RPO/RTO targets.

---

# 87. Production Deployment

The initial production deployment should remain:

```text
Angular Web
      │
      ▼
Azure-hosted Web Delivery
      │
      ▼
Azure App Service
      │
      ▼
Azure SQL
```

with:

```text
Key Vault
Application Insights
Managed Identity
```

as supporting services.

---

# 88. Network Architecture

Do not introduce complex private networking during bootstrap unless production security requirements justify it.

Before production, evaluate:

- Azure SQL network exposure;
- Key Vault network policy;
- App Service access restrictions;
- private endpoints where required.

This requires a production security ADR.

---

# 89. Infrastructure as Code

Infrastructure as Code is recommended for repeatable Azure environments.

Exact IaC technology is deferred to Solution Bootstrap.

Candidates may include native Azure tooling or Terraform.

Only one primary IaC approach should be selected.

---

# 90. Deployment Slots

Azure App Service deployment slots may be used for safer production deployment.

Do not create elaborate blue/green infrastructure until needed.

---

# 91. CI/CD Security

GitHub Actions SHALL use secure authentication to Azure.

Prefer federated identity / workload identity over long-lived Azure secrets where possible.

Long-lived cloud credentials should not be stored in GitHub secrets if a safer supported mechanism exists.

---

# 92. Dependency Management

Backend dependencies SHALL use NuGet.

Frontend dependencies SHALL use npm tooling associated with the Angular workspace.

Lock files must be committed where applicable.

Avoid unnecessary package proliferation.

---

# 93. Dependency Update Strategy

Security updates shall be prioritized.

Major framework upgrades shall be intentional and tested.

Do not auto-merge breaking upgrades into production.

---

# 94. Package Approval Rule

A package may be added only if:

1. it solves a current problem;
2. platform capabilities are insufficient;
3. maintenance health is acceptable;
4. license is acceptable;
5. security posture is acceptable;
6. complexity introduced is justified.

---

# 95. Static Architecture Enforcement

Architecture tests SHOULD enforce high-value constraints such as:

```text
Domain must not reference Infrastructure.
Domain must not reference ASP.NET Core.
Domain must not reference payment provider SDKs.
Billing must not depend on AZUL-specific classes.
```

Do not attempt to encode every architectural preference as a test.

---

# 96. Code Generation

Code generation may be used for:

- OpenAPI clients;
- external provider contracts;
- generated migrations;
- tooling outputs

only when generated artifacts are controlled and maintainable.

Do not generate the core domain model from database tables.

---

# 97. Database-First

Database-first domain generation is rejected.

The database schema shall follow the approved domain and application requirements.

---

# 98. API-First Meaning

API-first means external/application contracts are deliberately designed.

It does NOT mean implementation must start by writing every possible API before domain behavior exists.

Vertical slices remain the preferred implementation approach.

---

# 99. First Bootstrap Goal

Solution Bootstrap must produce a system that:

- compiles;
- tests;
- starts;
- connects to local SQL;
- exposes health endpoint;
- emits telemetry;
- has base error handling;
- supports configuration;
- contains no business feature implementation yet.

---

# 100. Bootstrap Deliverables

Codex shall initially create:

```text
SchoolERP.sln

Backend projects
Angular workspace
Test projects
GitHub workflows
Configuration baseline
Logging
OpenTelemetry baseline
EF Core baseline
Migration baseline
Health checks
Architecture tests
README
ADR files
```

Do NOT implement Student, Enrollment, Billing or DGII during bootstrap.

---

# 101. Health Checks

The backend SHALL expose appropriate health checks.

Initial checks may include:

- application liveness;
- database connectivity.

External provider health should not necessarily determine application liveness.

---

# 102. Development Experience

Local setup should require as few manual steps as practical.

Preferred developer flow:

```text
Clone
→ configure local environment
→ start SQL
→ run migrations
→ run backend
→ run Angular
→ test
```

Document this in README.

---

# 103. Developer Secrets

Local development SHALL use an approved local secret strategy.

Do not commit developer secrets into:

```text
appsettings.json
environment.ts
source code
```

---

# 104. API Base Path

The API SHOULD use a consistent root such as:

```text
/api
```

No complex public version prefix is required initially unless bootstrap decides otherwise.

---

# 105. Modular Boundaries in Code

Each feature/module SHOULD contain only code owned by that domain.

Example:

```text
Billing/
├── Domain
├── Application
└── Contracts
```

depending on final project organization.

Avoid direct cross-module database mutation.

---

# 106. Shared Kernel

Do not create a large `SharedKernel` at project bootstrap.

Only truly universal technical/domain primitives may be shared.

Examples may eventually include:

- Money;
- EntityId primitives;
- Result/error abstraction

if proven useful.

Do not move unrelated concepts into Shared simply to avoid references.

---

# 107. Result Pattern

No specific Result library is mandated.

Application/domain error handling may use an explicit Result pattern if it improves clarity.

Avoid excessive abstraction around exceptions/errors.

---

# 108. Mediator Pattern

MediatR is NOT mandated.

Command/query organization may be implemented without an external mediator dependency.

Introduce MediatR only if the codebase demonstrates enough orchestration complexity to justify it.

---

# 109. CQRS

Full CQRS architecture is rejected for MVP.

Using separate command/query classes is acceptable.

Separate databases/read stores are not approved.

---

# 110. Mapping Libraries

AutoMapper or similar mapping libraries are NOT required during bootstrap.

Prefer explicit mappings initially.

Introduce mapping libraries only if repetitive mapping becomes materially harmful.

---

# 111. Validation Libraries

FluentValidation or similar library is NOT mandated.

Start with framework capabilities or explicit validation.

Adopt an external validation library only if it meaningfully improves consistency.

---

# 112. API Endpoint Style

Controller vs minimal API style shall be selected consistently during bootstrap.

Only one dominant API style should be used.

Selection criteria:

- clarity;
- testability;
- maintainability;
- domain expression.

Do not mix styles randomly.

---

# 113. Recommended Backend API Style

For this enterprise modular application, conventional ASP.NET Core controllers or carefully organized endpoint modules are both acceptable.

The selected approach must support domain-oriented operations and consistent authorization/error handling.

---

# 114. Frontend Backend Contract

Angular SHALL consume typed API contracts.

Generated API clients may be considered if OpenAPI generation remains controlled.

Do not manually duplicate large backend contract structures throughout the frontend without reason.

---

# 115. PWA

The Parent Portal may be PWA-capable later.

PWA/offline behavior is NOT required by this gate.

Do not implement service-worker complexity unless approved.

---

# 116. Browser Support

Support modern actively supported browsers.

Exact browser matrix shall be defined before production.

No support for obsolete browsers without explicit customer requirement.

---

# 117. Accessibility Target

Target modern WCAG-aligned accessibility practices.

Exact conformance level shall be formalized before production.

---

# 118. Localization

Initial UI language:

```text
Spanish
```

Architecture should allow future localization.

Do not build a complex localization platform.

---

# 119. Currency

Initial business currency:

```text
DOP
```

Domain monetary types should avoid assumptions that make future additional currencies impossible.

No multi-currency accounting engine in MVP.

---

# 120. External Provider Abstractions

The following interfaces/boundaries are permitted:

```text
IPaymentProvider
IFiscalProvider
INotificationProvider
IIdentityProviderIntegration
```

or equivalent domain-appropriate names.

Do not create interfaces for every implementation class.

---

# 121. No Generic Integration Framework

Do not create a universal:

```text
IExternalProvider<TRequest,TResponse>
```

for all external systems.

Payments, fiscal systems and notifications have different semantics.

Use explicit contracts.

---

# 122. Failure Handling

External-provider errors shall map to internal stable error categories.

Provider-specific errors may be preserved internally for support diagnostics but shall not leak uncontrolled into domain/UI behavior.

---

# 123. Retrying External Calls

No universal retry policy.

Each operation must answer:

- is it idempotent?
- is failure transient?
- can duplicate execution cause harm?
- does provider support idempotency?
- should status query occur instead?

---

# 124. Production Readiness Gate

Before production, the following must exist:

- security review;
- backup/restore validation;
- environment isolation;
- secrets validation;
- telemetry validation;
- load/performance baseline;
- tenant isolation tests;
- external provider sandbox tests;
- DGII certification/integration requirements where applicable;
- deployment rollback procedure.

---

# 125. Deferred Technology Decisions

The following remain intentionally unresolved:

```text
Background job framework
Notification vendor
Infrastructure-as-Code tool
Frontend component library
E2E testing framework
Exact Entra topology
Document storage
Production network/private endpoint design
CardNet adapter final contract
```

These decisions SHALL be made when their first real implementation requirement appears.

---

# 126. Explicitly Rejected Technologies for MVP

Unless an ADR reverses this decision:

```text
Kubernetes
AKS
Kafka
RabbitMQ
Azure Service Bus
Redis
Cosmos DB
MongoDB
Event Sourcing
Microservices
Service Mesh
Generic BPM Engine
Native Mobile Framework
AI Agent Runtime
```

They are not considered “bad technologies.”

They are unnecessary details for the current problem.

---

# 127. Technical Stack Diagram

```text
┌──────────────────────────────────────────────┐
│                 USER EXPERIENCE              │
│                                              │
│       Angular 22 + TypeScript                │
│ Admin │ Teacher │ Parent Responsive Portal  │
└──────────────────────┬───────────────────────┘
                       │ HTTPS / REST
                       ▼
┌──────────────────────────────────────────────┐
│             ASP.NET CORE 10 API              │
│                                              │
│                .NET 10 LTS                   │
├──────────────────────────────────────────────┤
│              APPLICATION LAYER               │
├──────────────────────────────────────────────┤
│                  DOMAIN                      │
│                                              │
│ Modular Monolith / Domain-Oriented Modules   │
├──────────────────────────────────────────────┤
│              INFRASTRUCTURE                  │
│                                              │
│ EF Core 10                                   │
│ OpenTelemetry                                │
│ Provider Adapters                            │
└──────────┬────────────┬─────────────┬────────┘
           │            │             │
           ▼            ▼             ▼
    Azure SQL         DGII       AZUL/CardNet
    Database

Supporting Azure Services:

Azure App Service
Azure Key Vault
Managed Identity
Azure Monitor
Application Insights

Delivery:

GitHub
GitHub Actions
```

---

# 128. Bootstrap Repository Target

```text
school-erp-rd/
│
├── AGENTS.md
├── README.md
├── SchoolERP.sln
│
├── docs/
│   ├── SDD.md
│   └── architecture/
│       ├── technical-architecture-gate-1.1.md
│       └── adr/
│
├── src/
│   ├── SchoolERP.Api/
│   ├── SchoolERP.Application/
│   ├── SchoolERP.Domain/
│   ├── SchoolERP.Infrastructure/
│   └── SchoolERP.Web/
│
├── tests/
│   ├── SchoolERP.Domain.Tests/
│   ├── SchoolERP.Application.Tests/
│   ├── SchoolERP.IntegrationTests/
│   └── SchoolERP.ArchitectureTests/
│
└── .github/
    └── workflows/
```

---

# 129. Approved Technical ADR Register

The following decisions SHALL be represented in individual ADR files:

```text
ADR-016 — .NET 10 / ASP.NET Core 10
ADR-017 — Angular 22
ADR-018 — Azure SQL Database
ADR-019 — EF Core 10
ADR-020 — Azure App Service
ADR-021 — Microsoft Entra Identity Direction
ADR-022 — Azure Key Vault + Managed Identity
ADR-023 — OpenTelemetry + Application Insights
ADR-024 — GitHub Actions
ADR-025 — Shared Database Multi-Tenancy
```

---

# 130. Gate 1.1 Exit Criteria

Technical Architecture Gate 1.1 is considered complete when:

- backend runtime is selected;
- frontend framework is selected;
- database is selected;
- ORM is selected;
- hosting model is selected;
- identity direction is selected;
- secrets strategy is selected;
- observability strategy is selected;
- CI/CD strategy is selected;
- multi-tenancy persistence direction is selected;
- deferred infrastructure is explicitly identified;
- rejected premature complexity is documented.

All criteria are satisfied by this document.

---

# 131. Gate Decision

```text
TECHNICAL ARCHITECTURE GATE 1.1

STATUS:

APPROVED
```

The project may advance to:

# Phase 1.2 — Solution Bootstrap

Codex may create:

- solution;
- projects;
- Angular workspace;
- tests;
- CI/CD;
- EF Core infrastructure baseline;
- observability baseline;
- configuration;
- health checks;
- architecture checks.

Codex SHALL NOT yet implement:

- Student domain;
- Enrollment workflows;
- Billing;
- Payments;
- DGII;
- production authentication behavior.

Those features begin only after the bootstrap architecture passes validation.

---

# 132. Final Technical Principle

Every technical addition must answer:

> Does this technology solve a real requirement we have now?

If not:

do not add it.

Prefer:

```text
one application
one database
one frontend
one backend stack
one CI/CD system
explicit boundaries
```

over distributed sophistication.

The system should become more complex only when the business proves that complexity is necessary.

---

# END

**Document:** Technical Architecture Gate 1.1  
**Version:** 1.0  
**Status:** APPROVED BASELINE  
**Next Phase:** Phase 1.2 — Solution Bootstrap  
**Guiding Principle:**

> **“Make every single detail perfect, and limit the number of details.”**