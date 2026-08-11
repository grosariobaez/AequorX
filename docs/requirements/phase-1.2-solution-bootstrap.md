# Codex Master Prompt
# Phase 1.2 — Solution Bootstrap

## Project

**School ERP RD**

## Phase

**Phase 1.2 — Solution Bootstrap**

## Objective

Create the initial production-grade repository structure, technical foundation, development experience, automated tests, CI/CD baseline, observability baseline, persistence baseline, and architectural guardrails for School ERP RD.

This phase is infrastructure and solution bootstrap only.

Do NOT implement business features.

Do NOT implement Student, Enrollment, Attendance, Grades, Billing, Payments, DGII, or Parent Portal functionality.

The purpose of this phase is to prove that the approved technical architecture can:

- compile;
- run;
- connect to the local database;
- execute migrations;
- expose API endpoints;
- serve the Angular application;
- execute automated tests;
- emit telemetry;
- pass architecture checks;
- run successfully in CI.

---

# 1. Mandatory Mindset

The governing principle is:

> **“Make every single detail perfect, and limit the number of details.”**

Apply this continuously.

Prefer:

- fewer projects;
- fewer dependencies;
- fewer abstractions;
- fewer configuration files;
- explicit structure;
- standard framework capabilities;
- strong defaults;
- simple architecture;
- clear boundaries.

Do NOT introduce infrastructure simply because it may be useful later.

Before adding anything, ask:

> Does this solve a requirement in the current bootstrap?

If no:

do not add it.

---

# 2. Authoritative Documents

Before modifying the repository, read:

```text
/AGENTS.md
/docs/SDD.md
/docs/architecture/technical-architecture-gate-1.1.md
/docs/architecture/adr/
```

These documents are authoritative.

Do not override them based on personal preference.

If an instruction in this prompt conflicts with an approved ADR or SDD decision:

STOP the affected work and report the conflict.

---

# 3. Approved Stack

Use exactly this baseline:

## Backend

```text
.NET 10 LTS
ASP.NET Core 10
C#
```

## ORM

```text
Entity Framework Core 10
```

## Database

```text
Azure SQL Database — Production
SQL Server-compatible local environment — Development
```

## Frontend

```text
Angular 22
TypeScript
```

## Hosting

```text
Azure App Service
```

## Identity Direction

```text
Microsoft Entra platform
Microsoft Entra External ID
```

Do NOT implement production identity behavior during this phase.

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

## Source Control / CI

```text
GitHub
GitHub Actions
```

---

# 4. Explicitly Forbidden During Bootstrap

Do NOT introduce:

- microservices;
- Kubernetes;
- AKS;
- Docker orchestration;
- Azure Service Bus;
- Kafka;
- RabbitMQ;
- Redis;
- MongoDB;
- Cosmos DB;
- event sourcing;
- full CQRS architecture;
- generic workflow engine;
- generic rules engine;
- AI agents;
- MediatR unless explicitly justified;
- AutoMapper unless explicitly justified;
- FluentValidation unless explicitly justified;
- Dapper unless explicitly justified;
- GenericRepository<T>;
- UnitOfWork wrapper over EF Core;
- generic integration framework;
- generic shared kernel containing unrelated code.

Do NOT add placeholders or scaffolding for post-MVP features.

---

# 5. Repository Target

Create or normalize the repository toward:

```text
school-erp-rd/
│
├── AGENTS.md
├── README.md
├── SchoolERP.sln
├── Directory.Build.props
│
├── docs/
│   ├── SDD.md
│   ├── architecture/
│   │   ├── technical-architecture-gate-1.1.md
│   │   └── adr/
│   ├── domain/
│   ├── integrations/
│   └── requirements/
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

Do not create more projects unless absolutely necessary.

---

# 6. Solution Creation

Create:

```text
SchoolERP.sln
```

Add:

```text
src/SchoolERP.Api
src/SchoolERP.Application
src/SchoolERP.Domain
src/SchoolERP.Infrastructure

tests/SchoolERP.Domain.Tests
tests/SchoolERP.Application.Tests
tests/SchoolERP.IntegrationTests
tests/SchoolERP.ArchitectureTests
```

The Angular application should live in:

```text
src/SchoolERP.Web
```

Do not create separate Angular applications for:

```text
Admin
Teacher
Parent
```

They will later become feature areas within one application.

---

# 7. Dependency Direction

Enforce:

```text
SchoolERP.Api
    ↓
SchoolERP.Application
    ↓
SchoolERP.Domain

SchoolERP.Infrastructure
    ↓
SchoolERP.Application
    ↓
SchoolERP.Domain
```

Domain must not reference:

```text
Infrastructure
ASP.NET Core
EF Core
Azure SDKs
payment providers
DGII transports
```

Application must not depend on concrete Infrastructure implementations.

API may compose dependencies.

---

# 8. Project References

Expected relationships:

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

Avoid cycles.

Architecture tests must validate high-value dependency rules.

---

# 9. C# Project Baseline

Configure centrally where practical using:

```text
Directory.Build.props
```

Enable:

```text
Nullable
ImplicitUsings
TreatWarningsAsErrors where practical
Deterministic builds
```

Use modern C# compatible with .NET 10.

Do not introduce excessive compiler/analyzer complexity.

---

# 10. API Bootstrap

Create a minimal ASP.NET Core API host.

It must include:

- dependency injection;
- configuration;
- environment support;
- structured logging baseline;
- exception handling;
- health endpoint;
- OpenAPI;
- OpenTelemetry bootstrap;
- EF Core registration;
- infrastructure registration.

Do not add domain endpoints yet.

---

# 11. Required API Endpoints

Bootstrap only:

```text
GET /health/live
GET /health/ready
```

Optional development-only endpoint:

```text
GET /api/system/info
```

only if useful for validating environment/version configuration.

Do not create Student or other business endpoints.

---

# 12. Health Checks

## Liveness

Must verify application process is running.

## Readiness

Must verify critical internal dependencies required for serving requests.

Initial readiness should include database connectivity.

Do not make DGII, AZUL or any future provider part of liveness.

---

# 13. Error Handling

Create one consistent API error handling mechanism.

Use standard ASP.NET Core capabilities.

Differentiate at minimum:

```text
Validation
BusinessRuleViolation
Unauthorized
Forbidden
NotFound
Conflict
Unexpected
```

Bootstrap need not create every domain exception yet.

Do not return stack traces to consumers.

Development diagnostics may remain available through development tooling.

---

# 14. OpenAPI

Configure OpenAPI for development.

Requirements:

- API documentation available;
- correct environment behavior;
- no business endpoints yet;
- no unnecessary third-party Swagger libraries if framework-native capability is sufficient.

Prefer built-in platform support.

---

# 15. Application Project

The Application project should initially contain only architecture/bootstrap primitives required to demonstrate boundaries.

Do NOT create:

```text
StudentService
BillingService
EnrollmentService
```

or fake use cases.

Allowed content may include:

- common application contracts where proven necessary;
- service registration;
- minimal abstractions for current bootstrap only.

Prefer an almost-empty Application project over speculative code.

---

# 16. Domain Project

The Domain project must initially remain intentionally minimal.

Do not create business entities in this phase.

No:

```text
Student
Person
Enrollment
Invoice
Payment
```

yet.

The purpose is to establish the correct dependency boundary.

If a shared primitive is not currently required:

do not create it.

---

# 17. Infrastructure Project

Infrastructure shall contain bootstrap implementations for:

- EF Core;
- database registration;
- migrations;
- future adapter placement;
- telemetry/infrastructure configuration where appropriate.

Do not implement payment providers.

Do not implement DGII.

Do not create dummy provider implementations merely to fill folders.

---

# 18. EF Core Bootstrap

Create the initial EF Core persistence infrastructure.

Use:

```text
EF Core 10
SQL Server provider
```

Create an initial DbContext with no business tables.

A small technical schema/table may be created only if truly required for database initialization.

Avoid fake domain tables.

---

# 19. Initial Migration

Create one baseline migration representing the bootstrap schema.

The migration should prove:

- EF Core configuration works;
- SQL connection works;
- migration tooling works.

Do not create future business schema.

---

# 20. Connection Configuration

Local development configuration must not contain committed production credentials.

Support a local connection through developer configuration/environment variables/user secrets.

Example safe template may be included.

Do not commit passwords.

---

# 21. Managed Identity Preparation

Production configuration must be compatible with:

```text
Azure App Service
→ Managed Identity
→ Azure SQL
```

Do not require production SQL passwords if Managed Identity can be used.

Do not fully provision Azure resources in this phase unless infrastructure deployment is explicitly part of the repository task.

---

# 22. Key Vault Preparation

Create configuration boundaries so future secrets can be obtained securely.

Do not create dummy secrets.

Do not commit:

- AZUL keys;
- DGII certificates;
- CardNet keys;
- Entra secrets.

---

# 23. OpenTelemetry

Configure OpenTelemetry for:

- ASP.NET Core requests;
- outgoing HTTP;
- EF Core / SQL where supported and useful;
- runtime telemetry where reasonable.

The application should run locally even without Application Insights credentials.

Telemetry exporter configuration must be environment-driven.

---

# 24. Logging

Use structured logging.

Ensure logging can include:

```text
CorrelationId
Environment
Application
```

and later allow:

```text
TenantId
UserId
Module
```

Do not invent tenant context before authentication/multi-tenancy exists.

Never log secrets.

---

# 25. Correlation ID

Implement a simple correlation/request identifier mechanism if ASP.NET Core tracing does not already provide sufficient behavior.

Prefer existing platform tracing before writing custom middleware.

Do not duplicate framework capabilities.

---

# 26. Angular Bootstrap

Create Angular 22 application under:

```text
src/SchoolERP.Web
```

Requirements:

- strict TypeScript;
- standalone modern Angular architecture where appropriate;
- routing;
- environment configuration;
- base HTTP configuration;
- basic application shell;
- development API configuration;
- responsive foundation.

Do not implement school features.

---

# 27. Angular Initial Routes

Keep routes minimal.

Suggested:

```text
/
 /not-found
```

Optionally:

```text
/system
```

for development diagnostics only.

Do not create fake modules such as:

```text
/students
/billing
/attendance
```

during bootstrap.

---

# 28. Frontend Shell

Create only enough UI to prove:

- Angular starts;
- routing works;
- backend API is reachable;
- health/status may be shown in development if useful;
- basic responsive layout works.

Do not create dashboards.

Do not create a design-heavy landing page.

---

# 29. Frontend Styling

Use minimal foundational styling.

Do not select multiple UI libraries.

If a component library is not yet approved:

use standard Angular/CSS baseline only.

The goal is technical bootstrap, not visual design.

---

# 30. Frontend API Client

Create a minimal typed API integration pattern.

Do not generate dozens of clients.

If OpenAPI client generation is introduced, justify it and keep it controlled.

A simple typed health/system service is sufficient for bootstrap validation.

---

# 31. Frontend Error Handling

Provide one basic HTTP error handling strategy.

Do not create a complex global notification/toast architecture yet.

---

# 32. Frontend Authentication

Do NOT implement full Entra authentication during this bootstrap unless specifically instructed in a later task.

Create no fake login system.

Do not build local username/password authentication.

Identity integration will have a dedicated implementation gate.

---

# 33. Backend Testing

Use:

```text
xUnit
```

as the primary .NET test framework.

Set up the four approved test projects.

---

# 34. Domain Tests

At bootstrap, include a minimal architecture/sanity test only if necessary.

Do not create artificial domain tests for nonexistent domain objects.

An empty valid test project is acceptable.

---

# 35. Application Tests

Create test infrastructure but do not invent use cases.

One basic registration/configuration test may be included if useful.

---

# 36. Integration Tests

Integration tests should verify:

- API host can start;
- health endpoint responds;
- database connection works;
- migrations can be applied in test environment.

Use an isolated test database strategy.

Do not depend on production Azure SQL.

---

# 37. Test Database Strategy

Prefer an actual SQL Server-compatible database for integration testing rather than substituting SQLite for core persistence tests.

Do not use EF Core InMemory provider for relational behavior validation.

If container-based integration tests are selected, keep setup straightforward and documented.

---

# 38. Architecture Tests

Create high-value architecture tests.

At minimum verify:

```text
SchoolERP.Domain
does not depend on
SchoolERP.Infrastructure
```

and:

```text
SchoolERP.Domain
does not depend on
ASP.NET Core
```

Additional tests should only be added where useful.

Avoid a massive architecture-test framework.

---

# 39. Frontend Tests

Set up Angular unit/component test baseline using the framework-standard toolchain.

Add one meaningful shell/bootstrap test.

Do not generate meaningless boilerplate tests for every file.

---

# 40. Formatting

Configure:

Backend:

```text
dotnet format
```

or equivalent standard .NET formatting.

Frontend:

```text
ESLint
Angular compiler checks
```

Avoid overlapping formatting tools unless needed.

---

# 41. Git Ignore

Ensure `.gitignore` excludes:

- build outputs;
- local database artifacts if applicable;
- user secrets;
- Angular build outputs;
- IDE temporary files;
- local environment secret files.

Do not ignore files required for reproducible builds.

---

# 42. EditorConfig

Create or normalize:

```text
.editorconfig
```

for consistent formatting.

Keep it understandable.

Do not add hundreds of obscure rules.

---

# 43. GitHub Actions

Create PR workflow:

```text
.github/workflows/ci.yml
```

Pipeline must execute:

```text
Checkout
↓
Setup .NET
↓
Restore
↓
Build
↓
.NET Tests
↓
Setup Node
↓
Install frontend dependencies
↓
Lint
↓
Frontend tests
↓
Angular production build
```

---

# 44. CI Rules

The pipeline must fail when:

- backend build fails;
- tests fail;
- frontend build fails;
- frontend lint fails;
- architecture tests fail.

Avoid ignoring errors with `continue-on-error`.

---

# 45. Dependency Caching

Use GitHub Actions dependency caching only where standard and low-complexity.

Do not create custom cache infrastructure.

---

# 46. Deployment Workflow

Do NOT create production deployment during this bootstrap unless explicitly required.

A development deployment workflow may be prepared only if Azure resources already exist and required configuration is available.

Otherwise:

document the future deployment flow without fake credentials.

---

# 47. GitHub-to-Azure Authentication

When Azure deployment is later configured, prefer federated workload identity/OIDC.

Do not create workflows dependent on long-lived Azure credentials if avoidable.

---

# 48. README

Create/update root `README.md`.

It must include:

## Project

What School ERP RD is.

## Architecture

Short explanation:

```text
.NET 10
Angular 22
Azure SQL
Modular Monolith
```

## Prerequisites

Exact local requirements.

## Setup

Minimal commands.

## Database

How to run SQL locally.

## Migrations

How to create/apply migrations.

## Backend

How to run.

## Frontend

How to run.

## Tests

How to run all tests.

## Architecture Docs

Links to:

```text
AGENTS.md
docs/SDD.md
Technical Architecture Gate
```

Keep README practical.

---

# 49. Developer Experience Goal

Target developer flow:

```text
git clone
↓
restore
↓
configure local database
↓
apply migration
↓
run API
↓
run Angular
↓
run tests
```

Minimize manual setup.

If local setup requires more than necessary, simplify it.

---

# 50. Configuration Files

Create clear examples.

For backend:

```text
appsettings.json
appsettings.Development.json
```

without secrets.

For frontend:

use standard Angular environment/configuration patterns.

Do not create many environment files without need.

---

# 51. Secrets Documentation

README must state clearly where local secrets belong.

Do not instruct developers to place secrets in committed configuration.

---

# 52. Health Validation

After bootstrap, verify locally:

```text
GET /health/live
```

returns success.

Verify:

```text
GET /health/ready
```

returns success when database is available.

Verify readiness fails appropriately when database is unavailable.

---

# 53. Database Validation

Verify:

- database can be created/configured;
- migration can be applied;
- application can connect;
- integration test can verify connectivity.

---

# 54. OpenTelemetry Validation

Verify locally that telemetry instrumentation initializes without errors.

If no exporter is configured:

application should still run.

Do not require cloud credentials for normal local development.

---

# 55. Architecture Validation

Before completing bootstrap, verify project references manually and through tests.

No circular dependencies.

No domain dependency on infrastructure.

---

# 56. No Premature Domain Code

This phase must NOT contain placeholder classes such as:

```text
Student.cs
Person.cs
Enrollment.cs
Invoice.cs
Payment.cs
```

unless explicitly required by an approved next-phase requirement.

The purpose of bootstrap is to prove the platform, not guess the domain implementation.

---

# 57. No Fake Integrations

Do NOT create:

```text
FakeAzulService
FakeDgiiService
FakeCardNetService
```

inside production projects.

Mocks belong in tests when actual contracts exist.

Provider contracts will be created when their vertical slices begin.

---

# 58. No Speculative Interfaces

Do not create:

```text
IPaymentProvider
IFiscalProvider
INotificationProvider
```

during bootstrap unless a current bootstrap component actually uses them.

The SDD approves these boundaries conceptually.

Actual interfaces should appear when their first implementation requires them.

---

# 59. No Empty Architecture Ceremony

Avoid creating:

- hundreds of folders;
- marker interfaces;
- empty aggregate classes;
- empty managers;
- empty repositories;
- abstract base services.

A nearly empty project is better than fake architecture.

---

# 60. Architecture Fitness

At the end of bootstrap, the system must demonstrate:

```text
small solution
clear boundaries
clean dependency graph
working database
working API
working frontend
working CI
working tests
working telemetry
```

not domain functionality.

---

# 61. Documentation Updates

Create the individual ADR files approved by Technical Architecture Gate 1.1 if they do not already exist:

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

Each ADR should be concise.

Use:

```text
Title
Status
Context
Decision
Consequences
```

Do not repeat the entire SDD.

---

# 62. ADR Status

All Gate 1.1 ADRs should use:

```text
Status: Accepted
```

except any explicitly marked design follow-up.

Identity topology details may remain:

```text
Accepted Direction / Detail Deferred
```

if needed.

---

# 63. Build Commands

Ensure these commands work from repository root or are clearly documented:

```bash
dotnet restore
dotnet build
dotnet test
```

Frontend commands must be similarly clear:

```bash
npm ci
npm run lint
npm test
npm run build
```

Use actual Angular-generated command names where different.

---

# 64. Solution-Wide Build

Prefer one root-level developer command/script only if it materially simplifies workflow.

Do not add custom build orchestration if standard commands are sufficient.

---

# 65. Security Baseline

During bootstrap:

- HTTPS development support;
- no committed secrets;
- safe CORS development configuration;
- production CORS not permissive;
- security-conscious headers where reasonable;
- dependency scanning compatible with GitHub.

Do not build full security architecture yet.

---

# 66. CORS

Allow only explicitly configured development frontend origins.

Do not use permissive CORS in production configuration.

---

# 67. Dependency Vulnerability Scanning

Use GitHub-native dependency/security capabilities where available.

Do not introduce a commercial scanner during bootstrap without requirement.

---

# 68. Docker

Docker may be used only for local SQL/test dependencies if it materially improves setup.

Do not dockerize the application merely because Docker is available.

Production hosting remains Azure App Service.

---

# 69. Local Development Ports

Choose predictable development ports and document them.

Avoid unnecessary custom port configuration.

Example conceptual:

```text
API: localhost:<port>
Angular: localhost:<port>
SQL: local/container SQL port
```

Use framework defaults where practical.

---

# 70. Naming

Use:

```text
SchoolERP
```

consistently.

Avoid mixing:

```text
SchoolErp
School-ERP
SERP
ERPApp
```

in namespaces/projects unless file-system/platform conventions require otherwise.

---

# 71. Commit Hygiene

If committing changes:

- bootstrap changes should be logically grouped;
- no generated build artifacts;
- no secrets;
- no unrelated refactoring.

---

# 72. Validation Checklist

Before declaring Phase 1.2 complete, verify every item.

## Repository

- [ ] AGENTS.md exists.
- [ ] SDD exists.
- [ ] Technical Gate document exists.
- [ ] solution created.
- [ ] project structure matches approved architecture.

## Backend

- [ ] .NET 10.
- [ ] API starts.
- [ ] health/live works.
- [ ] health/ready works.
- [ ] OpenAPI works in development.
- [ ] EF Core configured.
- [ ] migration works.
- [ ] structured logging works.
- [ ] OpenTelemetry initializes.

## Frontend

- [ ] Angular 22.
- [ ] strict TypeScript.
- [ ] Angular starts.
- [ ] routing works.
- [ ] backend connectivity validated.
- [ ] responsive shell exists.
- [ ] no domain features implemented.

## Tests

- [ ] Domain test project works.
- [ ] Application test project works.
- [ ] Integration tests run.
- [ ] Architecture tests run.
- [ ] Frontend tests run.

## CI

- [ ] GitHub Actions workflow created.
- [ ] backend build/test runs.
- [ ] frontend lint/test/build runs.
- [ ] failures fail pipeline.

## Security

- [ ] no secrets committed.
- [ ] local secrets documented.
- [ ] production credential strategy documented.

## Architecture

- [ ] Domain has no Infrastructure dependency.
- [ ] no microservices.
- [ ] no broker.
- [ ] no Redis.
- [ ] no generic repositories.
- [ ] no speculative abstractions.
- [ ] no business features.

---

# 73. Required Final Report

After completing the work, provide:

## 1. Files Created

List only important files.

## 2. Architecture Created

Summarize project/reference structure.

## 3. Dependencies Added

For every non-framework dependency explain why it was necessary.

## 4. Database Bootstrap

Explain local DB and migrations.

## 5. Frontend Bootstrap

Explain Angular setup.

## 6. Test Baseline

List test projects and what is currently validated.

## 7. CI/CD

Describe workflow.

## 8. Observability

Describe OpenTelemetry setup.

## 9. Security

Describe secret/config strategy.

## 10. Deferred Decisions

List only intentionally deferred technical items.

## 11. Deviations

Explicitly report any deviation from AGENTS.md, SDD, or this prompt.

If none:

```text
No deviations.
```

## 12. Validation

Report actual results for:

```text
dotnet build
dotnet test
Angular lint
Angular tests
Angular build
```

Do not claim success if commands were not executed.

---

# 74. Stop Conditions

Stop the affected work if:

- required .NET/Angular version cannot be installed;
- an approved project dependency conflicts with current tooling;
- a required architecture decision is contradictory;
- database setup would require unsafe credentials;
- a requested action would violate AGENTS.md;
- implementation requires inventing a business concept.

Continue all independent safe work.

---

# 75. Definition of Done — Phase 1.2

Phase 1.2 is DONE when:

```text
Repository exists
+
Solution builds
+
Frontend builds
+
Tests pass
+
Database bootstrap works
+
CI is configured
+
Telemetry baseline works
+
Architecture boundaries are validated
+
Documentation is synchronized
+
No business domain implementation exists
```

---

# 76. Final Instruction

Do not optimize for impressiveness.

Do not optimize for number of files.

Do not optimize for architectural sophistication.

Optimize for:

```text
clarity
correctness
simplicity
repeatability
maintainability
```

At every decision apply:

> **“Make every single detail perfect, and limit the number of details.”**

If a file, project, dependency, abstraction, configuration option, interface, middleware, library, or infrastructure component is not required to complete this bootstrap correctly:

**do not create it.**

The expected result is a small, exceptionally clean foundation upon which the first business vertical slice can be implemented safely.

# END OF CODEX PHASE 1.2 PROMPT