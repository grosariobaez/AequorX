# Codex Execution Prompt
# School ERP RD — Phase 1.2 Solution Bootstrap

You are now executing:

# Phase 1.2 — Solution Bootstrap

for the project:

# School ERP RD

Repository location:

```text
C:\Projects\AequorX
```

Your job is to create and validate the approved technical foundation of the system.

This is an implementation task.

Do not redesign the product.

Do not expand scope.

Do not implement business features.

---

# 1. Mandatory First Step

Before changing any file, read completely:

```text
C:\Projects\AequorX\AGENTS.md
C:\Projects\AequorX\PROJECT_MEMORY.md
C:\Projects\AequorX\docs\SDD.md
C:\Projects\AequorX\docs\architecture\technical-architecture-gate-1.1.md
C:\Projects\AequorX\docs\architecture\bootstrap-architecture-review-gate-1.3.md
```

Also inspect:

```text
C:\Projects\AequorX\docs\architecture\adr\
```

If the repository already contains implementation files, inspect them before changing them.

Do not recreate or replace correct existing work unnecessarily.

---

# 2. Governing Mindset

The primary engineering principle is:

> **“Make every single detail perfect, and limit the number of details.”**

Apply this continuously.

Prefer:

- fewer files;
- fewer projects;
- fewer dependencies;
- fewer abstractions;
- explicit configuration;
- framework-native capabilities;
- simple boundaries;
- reproducible builds;
- testable infrastructure.

Do not add anything simply because it may be useful later.

Every new dependency, file, abstraction, middleware, project, configuration option, or infrastructure element must solve a current Phase 1.2 requirement.

---

# 3. Current Phase

The project is currently in:

```text
Phase 1.2 — Solution Bootstrap
```

The following has NOT yet occurred:

```text
Phase 1.3 — Bootstrap Architecture Review
```

and therefore:

```text
Phase 2.0 — Core Domain Foundation
```

is NOT authorized.

Do not begin business-domain implementation.

---

# 4. Approved Technical Stack

Use exactly the approved baseline.

## Backend

```text
.NET 10 LTS
ASP.NET Core 10
C#
```

## Persistence

```text
Entity Framework Core 10
Azure SQL Database in production
SQL Server-compatible database locally
```

## Frontend

```text
Angular 22
TypeScript
```

## Hosting Direction

```text
Azure App Service
```

## Identity Direction

```text
Microsoft Entra
Microsoft Entra External ID
```

Do NOT implement production authentication yet.

## Secrets

```text
Azure Key Vault
Managed Identity
```

## Observability

```text
OpenTelemetry
Azure Monitor
Application Insights
```

## CI/CD

```text
GitHub
GitHub Actions
```

---

# 5. Explicitly Forbidden

Do not introduce:

- microservices;
- AKS;
- Kubernetes;
- service mesh;
- Azure Service Bus;
- Kafka;
- RabbitMQ;
- Redis;
- MongoDB;
- Cosmos DB;
- Event Sourcing;
- CQRS architecture;
- MediatR without approved need;
- AutoMapper without approved need;
- Dapper without approved need;
- GenericRepository<T>;
- generic UnitOfWork wrappers;
- generic workflow engines;
- generic rules engines;
- AI runtime;
- native mobile applications;
- background processing framework unless required by this phase;
- speculative provider implementations.

Do not create placeholders for future modules.

---

# 6. Business Features Explicitly Forbidden in Phase 1.2

Do NOT implement:

```text
Person
StudentProfile
StudentRelationship
Household
AcademicYear
AcademicTerm
Level
Cycle
GradeLevel
Section
Subject
Class
Enrollment
Attendance
Assessment
Grade
BillingAccount
Charge
Invoice
Receivable
Payment
PaymentAllocation
FiscalDocument
DGII
AZUL
CardNet
Parent Portal functionality
```

Do not create empty domain entities for them either.

Phase 1.2 is technical foundation only.

---

# 7. Repository Target

Normalize the repository toward:

```text
C:\Projects\AequorX
│
├── AGENTS.md
├── PROJECT_MEMORY.md
├── README.md
├── SchoolERP.sln
├── Directory.Build.props
├── .editorconfig
│
├── docs/
│   ├── SDD.md
│   └── architecture/
│       ├── technical-architecture-gate-1.1.md
│       ├── bootstrap-architecture-review-gate-1.3.md
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
        └── ci.yml
```

Do not add more projects unless required to satisfy an explicit bootstrap need.

---

# 8. Solution Creation

If the solution does not exist, create:

```text
SchoolERP.sln
```

Create or validate these backend projects:

```text
src/SchoolERP.Api
src/SchoolERP.Application
src/SchoolERP.Domain
src/SchoolERP.Infrastructure
```

Create or validate these test projects:

```text
tests/SchoolERP.Domain.Tests
tests/SchoolERP.Application.Tests
tests/SchoolERP.IntegrationTests
tests/SchoolERP.ArchitectureTests
```

Create or validate Angular application:

```text
src/SchoolERP.Web
```

One Angular application only.

Do not create separate applications for Admin, Teacher, and Parent.

---

# 9. Required Project References

Implement and verify:

```text
SchoolERP.Api
    → SchoolERP.Application
    → SchoolERP.Infrastructure

SchoolERP.Application
    → SchoolERP.Domain

SchoolERP.Infrastructure
    → SchoolERP.Application
    → SchoolERP.Domain
```

There must be no circular references.

The Domain project must not reference:

- Infrastructure;
- EF Core;
- ASP.NET Core;
- Azure SDK;
- external provider libraries.

---

# 10. Directory.Build.props

Create or normalize:

```text
Directory.Build.props
```

Use it for high-value common .NET settings only.

Enable appropriate settings such as:

```text
Nullable
ImplicitUsings
Deterministic
```

Use warnings-as-errors only if it does not create unnecessary bootstrap friction from framework-generated warnings.

Do not overconfigure compiler/analyzer rules.

---

# 11. API Bootstrap

Create a minimal ASP.NET Core host.

It must support:

- dependency injection;
- environment configuration;
- structured logging;
- exception handling;
- OpenAPI in development;
- health checks;
- EF Core registration;
- OpenTelemetry bootstrap.

Do not create business controllers/endpoints.

---

# 12. Required Health Endpoints

Implement:

```text
GET /health/live
GET /health/ready
```

Expected behavior:

## `/health/live`

Reports whether the application process is alive.

It must not fail merely because SQL is unavailable.

## `/health/ready`

Checks whether the application is ready to serve normal requests.

Initially it should include database connectivity.

Do not expose secrets or internal connection details.

---

# 13. Optional Diagnostic Endpoint

You may create:

```text
GET /api/system/info
```

only if it materially helps validate:

- runtime;
- application version;
- environment.

It must expose no secrets.

If not needed, do not create it.

---

# 14. Error Handling

Configure one consistent global API error strategy using ASP.NET Core-native capabilities where practical.

Do not create a large exception hierarchy yet.

Ensure unexpected exceptions:

- are logged;
- do not expose stack traces in production responses;
- include useful correlation/trace context.

---

# 15. OpenAPI

Enable OpenAPI for development.

Prefer .NET platform-native support when sufficient.

Do not add unnecessary Swagger libraries.

No fake business endpoints.

---

# 16. Domain Project

Keep:

```text
SchoolERP.Domain
```

almost empty.

It exists to establish dependency boundaries.

Do not create speculative:

```text
Entity
AggregateRoot
DomainEvent
Money
Result
AuditableEntity
BaseEntity
```

unless actually required by a current bootstrap component.

No architecture ceremony without use.

---

# 17. Application Project

Keep the Application project minimal.

Do not create fake:

```text
StudentService
EnrollmentService
BillingService
```

Allowed content:

- dependency registration;
- only application-level primitives required by actual bootstrap behavior.

Prefer empty over speculative.

---

# 18. Infrastructure Project

Infrastructure may contain only current technical concerns such as:

- EF Core setup;
- SQL provider;
- database migrations;
- configuration integration;
- future provider namespace locations only if needed.

Do not implement:

- AZUL;
- CardNet;
- DGII;
- notifications;
- Entra adapters.

No dummy provider classes.

---

# 19. EF Core Bootstrap

Use:

```text
Entity Framework Core 10
Microsoft SQL Server provider
```

Create a minimal DbContext.

Do not create business DbSets.

If EF requires no tables for baseline operation, do not invent a table.

If a technical migration marker is truly required, explain why.

---

# 20. Database Migration

Create a clean baseline migration if necessary to prove migrations work.

Do not create domain schema.

Validate that migrations can be:

- generated;
- applied;
- listed.

Document commands.

---

# 21. Local SQL Strategy

Use a SQL Server-compatible local development database.

Preferred options:

- existing local SQL Server;
- SQL Server Developer;
- SQL Server container if practical.

Do not substitute SQLite or EF InMemory as proof of SQL Server relational behavior.

Local setup must be documented.

---

# 22. Connection Strings and Secrets

Never commit real credentials.

Use:

- environment variables;
- .NET user secrets;
- safe development configuration;
- Managed Identity-compatible production configuration.

Do not commit:

```text
Password=
ApiKey=
ClientSecret=
PrivateKey=
```

with real values.

---

# 23. Azure SQL Compatibility

Production architecture must remain compatible with:

```text
Azure App Service
→ Managed Identity
→ Azure SQL
```

Do not make username/password SQL auth a mandatory architectural requirement.

---

# 24. Key Vault Compatibility

Prepare configuration so future secrets can come from Key Vault.

Do not require Key Vault access for local startup.

Do not create placeholder secrets.

---

# 25. Observability

Configure OpenTelemetry minimally.

Instrument useful technical signals:

- ASP.NET Core requests;
- outgoing HTTP;
- database calls where appropriate;
- exceptions.

The application must run without Azure Monitor credentials locally.

Application Insights/Azure Monitor exporter must be configuration-driven.

---

# 26. Logging

Use structured logging.

Prefer built-in .NET logging unless another already-approved implementation exists.

Useful technical context:

```text
TraceId
CorrelationId where needed
Application
Environment
```

Do not invent TenantId/UserId context yet.

No sensitive payloads.

---

# 27. Angular Bootstrap

Create or validate:

```text
src/SchoolERP.Web
```

using:

```text
Angular 22
TypeScript strict mode
```

Requirements:

- one application;
- routing;
- basic application shell;
- API configuration;
- responsive baseline;
- HTTP integration;
- lint;
- test setup.

Do not implement business features.

---

# 28. Angular State

Do not introduce:

- NgRx;
- Redux;
- other state-management libraries

during bootstrap.

Use Angular-native state patterns.

---

# 29. Angular UI Dependencies

Do not add a large UI library unless already approved.

If no component library is approved, use minimal CSS/HTML.

Do not spend this phase designing dashboards.

---

# 30. Angular Initial Experience

Create only enough UI to prove:

```text
Angular starts
Routing works
API can be reached
Responsive shell works
```

A minimal technical status page is acceptable if useful.

Do not create:

```text
Students
Attendance
Billing
Payments
Dashboard
```

pages.

---

# 31. Frontend Backend Configuration

Do not hard-code a production backend URL.

Use environment/configuration appropriate to Angular.

Development CORS must support the selected local Angular origin.

Production CORS must not be permissive.

---

# 32. Authentication

Do not implement real Entra authentication yet.

Do not build:

- username/password login;
- local JWT issuer;
- refresh token system;
- custom MFA;
- fake authentication screens pretending to be production auth.

Identity implementation will have its own gate.

---

# 33. Tests

Use:

```text
xUnit
```

for .NET tests.

The four approved test projects must build and execute.

Do not create meaningless tests just to increase count.

---

# 34. Architecture Tests

Implement only high-value rules.

At minimum test:

```text
SchoolERP.Domain
must not depend on
SchoolERP.Infrastructure
```

and:

```text
SchoolERP.Domain
must not depend on
ASP.NET Core
```

and:

```text
SchoolERP.Domain
must not depend on
EF Core
```

Use the simplest maintainable approach.

Do not add a large architecture testing framework unless justified.

---

# 35. Integration Tests

Implement tests proving:

- API host starts;
- liveness works;
- readiness behavior works;
- SQL connection works;
- migrations apply successfully.

Use an isolated SQL Server-compatible test database.

Do not rely on EF InMemory.

---

# 36. Angular Tests

Create only meaningful bootstrap tests.

At minimum validate:

- application shell can render;
- one basic routing or HTTP integration behavior where useful.

Remove autogenerated meaningless tests if they provide no value.

---

# 37. Editor and Formatting

Create or validate:

```text
.editorconfig
```

Configure reasonable formatting.

Backend should support:

```text
dotnet format
```

Frontend should support:

```text
npm run lint
```

Do not add multiple competing formatters.

---

# 38. GitHub Actions CI

Create:

```text
.github/workflows/ci.yml
```

On pull request, run:

```text
Checkout
→ Setup .NET 10
→ Restore
→ Build
→ .NET tests
→ Setup compatible Node
→ npm ci
→ lint
→ frontend tests
→ Angular production build
```

All required failures must fail CI.

Do not use:

```text
continue-on-error: true
```

for mandatory checks.

---

# 39. GitHub Actions Simplicity

Use one primary CI workflow during bootstrap.

Do not create separate workflows for every project.

Do not create production deployment yet unless explicitly authorized separately.

---

# 40. README

Create or update:

```text
README.md
```

It must clearly explain:

## Project

School ERP RD.

## Approved Stack

```text
.NET 10
ASP.NET Core 10
Angular 22
EF Core 10
Azure SQL
```

## Prerequisites

Actual software required.

## Setup

Exact commands.

## Database

Local SQL configuration.

## Migration

How to apply EF migrations.

## Backend

How to run.

## Frontend

How to run.

## Tests

How to run.

## Documentation

References to:

```text
AGENTS.md
PROJECT_MEMORY.md
docs/SDD.md
Technical Gate 1.1
Gate 1.3
```

Keep README operational, not promotional.

---

# 41. ADR Synchronization

Verify these ADRs exist or create them if missing:

```text
ADR-016-dotnet-10.md
ADR-017-angular-22.md
ADR-018-azure-sql.md
ADR-019-ef-core-10.md
ADR-020-azure-app-service.md
ADR-021-entra-identity.md
ADR-022-key-vault-managed-identity.md
ADR-023-observability.md
ADR-024-github-actions.md
ADR-025-shared-database-multitenancy.md
```

Keep each ADR concise.

Use:

```text
Title
Status
Context
Decision
Consequences
```

Do not duplicate the entire SDD.

---

# 42. No Infrastructure as Code Yet Unless Already Approved

Do not introduce:

- Bicep;
- Terraform;
- Pulumi

unless the repository already contains an approved IaC decision.

If none exists, leave IaC for a later gate.

---

# 43. No Background Job Framework

Do not install:

- Hangfire;
- Quartz;
- Functions framework;
- job scheduler infrastructure.

Background processing remains deferred.

---

# 44. No Redis

Do not add Redis.

No cache infrastructure.

---

# 45. No Message Broker

Do not add:

- Service Bus;
- RabbitMQ;
- Kafka.

Internal events do not need infrastructure during Phase 1.2.

---

# 46. No Provider SDKs

Do not add AZUL, CardNet, DGII, WhatsApp, payment, or fiscal provider SDKs during bootstrap.

---

# 47. No Generic Repository

Do not create:

```text
IRepository<T>
GenericRepository<T>
UnitOfWork
RepositoryFactory
```

as generic architecture.

Use EF Core directly from Infrastructure/Application when future requirements justify it.

---

# 48. No MediatR by Default

Do not install MediatR.

Commands and queries can later be implemented explicitly.

Only add MediatR if a later gate demonstrates real value.

---

# 49. No AutoMapper by Default

Do not install AutoMapper.

Use explicit mapping later until repetitive mapping becomes a demonstrated problem.

---

# 50. No FluentValidation by Default

Do not install FluentValidation during bootstrap.

Framework-native validation is sufficient at this phase.

---

# 51. No Fake Shared Kernel

Do not create a SharedKernel unless a real shared primitive exists.

No empty:

```text
BaseEntity
AggregateRoot
DomainEvent
EntityId
Money
Result
```

just because they are common architecture patterns.

---

# 52. Build Validation

Execute from repository root:

```powershell
dotnet --version
dotnet restore
dotnet build
dotnet test
```

Report actual results.

Do not claim PASS if not executed.

---

# 53. Frontend Validation

Execute inside:

```text
src\SchoolERP.Web
```

or from appropriate root command:

```powershell
node --version
npm --version
npm ci
npm run lint
npm test
npm run build
```

Use actual scripts generated by Angular.

If a standard Angular command differs, report it.

---

# 54. Runtime Validation

Run backend and frontend.

Verify:

```text
API Startup
Frontend Startup
Database Connection
Migration Apply
GET /health/live
GET /health/ready
Frontend → Backend connectivity
```

Report each:

```text
PASS
FAIL
NOT APPLICABLE
```

with a brief reason for non-PASS results.

---

# 55. Failure Validation

With SQL unavailable, verify:

```text
/health/live
```

remains healthy.

Verify:

```text
/health/ready
```

reports unhealthy/not ready.

Do not expose DB credentials or connection strings in response.

---

# 56. Secret Scan

Inspect tracked repository files for accidentally committed secrets.

Search relevant patterns, but do not print any secret values if found.

Report only:

- file;
- category;
- required remediation.

Any real committed secret is a blocker.

---

# 57. Dependency Inventory

Before completing the phase, list every significant non-framework dependency added.

For each:

```text
Package
Purpose
Why framework-native capability was insufficient
```

If the reason is weak:

remove it.

---

# 58. Cleanup Pass

Before declaring completion perform a deletion-oriented review.

Ask for every new:

- file;
- project;
- package;
- middleware;
- helper;
- interface;
- configuration setting

whether Phase 1.2 still works without it.

If yes and it provides no clear current value:

remove it.

---

# 59. Do Not Execute Gate 1.3 Automatically

Important:

The file:

```text
docs\architecture\bootstrap-architecture-review-gate-1.3.md
```

defines the NEXT review gate.

Do not execute Phase 1.3 as part of this task.

Do not declare Phase 1.3 passed.

Do not begin Phase 2.0.

Your job ends when Phase 1.2 bootstrap is complete and accurately reported.

---

# 60. PROJECT_MEMORY.md

Update:

```text
PROJECT_MEMORY.md
```

only with concise durable project state changes resulting from the bootstrap.

Record:

- Phase 1.2 completion status;
- actual stack initialized;
- important technical decisions implemented;
- intentionally deferred items;
- known bootstrap issues if any.

Do not turn PROJECT_MEMORY.md into a verbose work log.

---

# 61. Required Final Report

When finished return exactly these sections.

## 1. Phase Result

One of:

```text
PHASE 1.2 COMPLETE
PHASE 1.2 COMPLETE WITH KNOWN ISSUES
PHASE 1.2 BLOCKED
```

## 2. Repository Structure

List only important projects/files created or materially changed.

## 3. Architecture

Explain actual project dependency structure.

## 4. Dependencies Added

List each non-trivial package and justification.

## 5. Database Bootstrap

State:

- SQL strategy;
- DbContext;
- migration;
- connectivity validation.

## 6. Backend Bootstrap

State:

- API;
- health endpoints;
- OpenAPI;
- error handling;
- configuration.

## 7. Frontend Bootstrap

State:

- Angular version;
- routing;
- API connectivity;
- tests;
- build.

## 8. Observability

State:

- OpenTelemetry;
- logging;
- Application Insights readiness.

## 9. CI

State:

- workflow;
- exact steps;
- current validation.

## 10. Security

State:

- secrets strategy;
- CORS;
- HTTPS;
- whether any secret issue was found.

## 11. Test Results

Report actual:

```text
dotnet restore
dotnet build
dotnet test
npm ci
npm run lint
npm test
npm run build
```

using PASS/FAIL.

## 12. Runtime Validation

Report:

```text
API Startup
Frontend Startup
Database Connection
Migration Apply
Health Live
Health Ready
Frontend → Backend
```

## 13. Deviations

List all deviations from:

```text
AGENTS.md
SDD.md
Technical Gate 1.1
this prompt
```

If none:

```text
No deviations.
```

## 14. Deferred Items

Only intentionally deferred bootstrap decisions.

## 15. Known Issues

Only real known issues.

## 16. Files Changed

Concise list.

## 17. Final Statement

End with:

```text
Phase 1.2 has been completed.

Phase 1.3 — Bootstrap Architecture Review
has NOT been executed.

Phase 2.0 remains unauthorized.
```

unless Phase 1.2 is blocked.

---

# 62. Definition of Done

Phase 1.2 is complete only when:

```text
Solution builds
AND
Backend tests execute
AND
Angular builds
AND
Angular tests execute
AND
CI exists
AND
Database bootstrap is validated
AND
Health checks work
AND
Telemetry baseline initializes
AND
Architecture boundaries hold
AND
No business domain code was introduced
AND
Documentation is synchronized
```

---

# 63. Stop Conditions

Stop only the affected task and report clearly if:

- .NET 10 is unavailable;
- Angular 22 cannot be installed safely;
- an approved document conflicts with another approved document;
- repository state would be destroyed by proceeding;
- database credentials would need to be exposed;
- architecture cannot be implemented without violating AGENTS.md;
- business behavior would need to be invented.

Continue all independent safe bootstrap work.

---

# 64. Final Engineering Rule

Do not optimize for the amount of work completed.

Optimize for the quality of the foundation.

The ideal result should feel intentionally small.

At every step ask:

> **Does this detail need to exist now?**

If not:

remove it.

If yes:

make it correct, explicit, tested, and easy to understand.

Always follow:

> **“Make every single detail perfect, and limit the number of details.”**

# END — CODEX PHASE 1.2 EXECUTION PROMPT