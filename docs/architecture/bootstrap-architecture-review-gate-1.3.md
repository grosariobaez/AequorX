# School ERP RD
# Phase 1.3 — Bootstrap Architecture Review

**Document ID:** ARCH-GATE-1.3  
**Version:** 1.0  
**Status:** REVIEW GATE  
**Project:** School ERP RD  
**Previous Phase:** Phase 1.2 — Solution Bootstrap  
**Next Phase Candidate:** Phase 2.0 — First Business Vertical Slice  
**Authoritative Documents:**

- `/AGENTS.md`
- `/docs/SDD.md`
- `/docs/architecture/technical-architecture-gate-1.1.md`
- `/docs/architecture/adr/`

**Guiding Principle:**

> **“Make every single detail perfect, and limit the number of details.”**

---

# 1. Purpose

This gate performs a strict architecture review of the repository produced during Phase 1.2.

The objective is NOT to add features.

The objective is to verify that the technical foundation is:

- correct;
- minimal;
- buildable;
- testable;
- secure;
- observable;
- maintainable;
- aligned with approved architecture;
- free from speculative complexity.

No business vertical slice may begin until this gate is passed.

---

# 2. Review Mindset

Do not ask:

> “Does the project work?”

Ask:

> “Is this the smallest correct foundation on which we can safely build the product?”

Look actively for:

- unnecessary projects;
- unnecessary dependencies;
- incorrect references;
- accidental coupling;
- speculative abstractions;
- fake domain code;
- hidden infrastructure complexity;
- duplicated configuration;
- unsafe secrets;
- CI gaps;
- testing weaknesses;
- provider-specific leakage;
- tenant assumptions;
- undocumented deviations.

Prefer deletion over justification when something does not yet serve a real requirement.

---

# 3. Required Inputs

Review the repository as it currently exists.

Inspect at minimum:

```text
/
├── AGENTS.md
├── README.md
├── SchoolERP.sln
├── Directory.Build.props
├── .editorconfig
├── docs/
├── src/
├── tests/
├── .github/
└── package / project configuration
```

Also inspect:

- project references;
- NuGet packages;
- npm dependencies;
- EF Core configuration;
- migrations;
- API setup;
- Angular setup;
- telemetry;
- health checks;
- error handling;
- CI workflow.

---

# 4. Gate Outcomes

This review may result in only one of:

## PASS

Architecture is safe for the first business vertical slice.

## PASS WITH REQUIRED FIXES

Only small, non-structural corrections remain.

Codex must apply them before Phase 2.0.

## FAIL

A structural, security, dependency, architectural, or build issue exists.

Phase 2.0 is blocked.

---

# 5. Review Area A — Repository Simplicity

Evaluate whether the repository follows the approved minimal structure.

Expected baseline:

```text
src/
├── SchoolERP.Api/
├── SchoolERP.Application/
├── SchoolERP.Domain/
├── SchoolERP.Infrastructure/
└── SchoolERP.Web/

tests/
├── SchoolERP.Domain.Tests/
├── SchoolERP.Application.Tests/
├── SchoolERP.IntegrationTests/
└── SchoolERP.ArchitectureTests/
```

Flag:

- unnecessary projects;
- duplicate hosts;
- fake shared libraries;
- unused packages;
- empty abstraction projects;
- premature module projects.

For every additional project answer:

> Why must this exist now?

If no strong answer exists:

recommend removal.

---

# 6. Review Area B — Dependency Direction

Verify actual project references.

Required direction:

```text
Api
 ├── Application
 └── Infrastructure

Application
 └── Domain

Infrastructure
 ├── Application
 └── Domain

Domain
 └── no inward infrastructure/framework dependency
```

Reject:

```text
Domain → Infrastructure
Domain → EF Core
Domain → ASP.NET Core
Application → concrete provider implementation
```

Check for circular references.

---

# 7. Architecture Tests

Verify that architecture tests actually enforce high-value rules.

Minimum required:

```text
Domain must not reference Infrastructure.
Domain must not reference ASP.NET Core.
Domain must not reference EF Core.
```

If architecture tests use a dependency/package, ensure the dependency is justified and small.

Do not create dozens of trivial architecture rules.

---

# 8. Review Area C — Domain Purity

The Domain project should still contain no speculative business implementation.

During Phase 1.2 there should be no premature entities such as:

```text
Person
Student
Enrollment
Invoice
Payment
FiscalDocument
```

unless specifically approved after the bootstrap prompt.

Flag:

- placeholder aggregates;
- fake value objects;
- speculative interfaces;
- empty base entities;
- generic domain services.

The correct Domain project may still be almost empty.

---

# 9. Shared Kernel Review

Check whether Codex created any:

```text
Common
Shared
SharedKernel
Core
Utilities
Helpers
```

Review every item inside.

Ask:

> Is this truly shared domain/technical infrastructure, or simply code with no clear owner?

Move domain concepts back to their owning modules.

Remove generic helpers where standard framework functionality already exists.

---

# 10. Review Area D — Backend Dependencies

List every non-framework NuGet dependency.

For each answer:

1. What problem does it solve?
2. Is that problem present in Phase 1.2?
3. Could .NET solve it natively?
4. Is the dependency actually used?
5. Is it introducing architecture we rejected?

Explicitly review for accidental introduction of:

- MediatR;
- AutoMapper;
- FluentValidation;
- Dapper;
- Serilog;
- Polly;
- Hangfire;
- Quartz;
- MassTransit;
- Azure Service Bus SDK;
- Redis libraries;
- generic Result libraries;
- repository libraries.

None are automatically prohibited, but every addition requires evidence.

Default outcome for unjustified dependencies:

REMOVE.

---

# 11. Review Area E — ASP.NET Core Host

Validate the API host is small.

Expected responsibilities:

- DI;
- configuration;
- middleware;
- OpenAPI;
- health checks;
- telemetry;
- infrastructure composition;
- error handling.

Flag if `Program.cs` contains:

- business rules;
- manual SQL;
- future provider code;
- fake domain services;
- excessive custom middleware.

The composition root may be explicit but should remain easy to read.

---

# 12. Health Check Review

Validate:

```text
GET /health/live
GET /health/ready
```

Expected:

### Liveness

Does not fail simply because an external service is unavailable.

### Readiness

Validates database connectivity or other essential local dependency.

Confirm:

- correct response status;
- no secret leakage;
- no excessive internal detail exposed publicly.

---

# 13. OpenAPI Review

Verify OpenAPI is configured appropriately.

Check:

- available in development;
- no fake business routes;
- no unnecessary Swagger dependency if built-in support is sufficient;
- endpoint descriptions remain clean.

Do not expand API docs during this gate.

---

# 14. Error Handling Review

Check that API errors are consistent.

Verify no:

- raw exception object;
- stack trace;
- connection string;
- SQL statement containing sensitive data

is sent to clients.

Ensure unexpected errors are logged with correlation context.

---

# 15. Review Area F — EF Core

Validate:

```text
EF Core 10
SQL Server provider
```

Check:

- DbContext is minimal;
- no fake business tables;
- no generic repository;
- no UnitOfWork wrapper;
- migration baseline is clean;
- migration executes successfully.

If the DbContext contains domain tables from future phases:

flag as premature.

---

# 16. Migration Review

Inspect every migration.

Expected:

- one small baseline migration;
- no speculative schema;
- deterministic;
- meaningful naming.

Reject unnecessary schema such as:

```text
Students
Invoices
Payments
Attendance
```

during bootstrap.

---

# 17. Database Configuration Review

Verify:

- no production password committed;
- local connection configuration is documented;
- secrets are externalized;
- Azure SQL compatibility maintained.

Look for:

```text
Password=
User Id=
client secret=
```

in tracked files.

Any committed secret is:

# FAIL — SECURITY

---

# 18. Managed Identity Readiness

Verify production configuration is compatible with future:

```text
App Service
→ Managed Identity
→ Azure SQL
```

Do not require that actual Azure resources exist yet.

Check that application architecture does not assume username/password-only SQL connections.

---

# 19. Review Area G — Multi-Tenancy Readiness

Business tenant functionality is not yet implemented.

However, verify Codex did NOT prematurely create unsafe tenant patterns.

Flag:

```text
TenantId read directly from query string and trusted
```

or:

```text
global CurrentTenant static variable
```

or:

```text
hard-coded tenant
```

At this stage, an explicit placeholder boundary may be acceptable only if it does not create fake behavior.

Do not implement tenant domain during this gate.

---

# 20. Review Area H — Angular Bootstrap

Verify:

```text
Angular 22
TypeScript strict
```

Review:

- package.json;
- angular.json;
- tsconfig;
- routes;
- application shell;
- HTTP configuration;
- linting;
- tests.

Ensure one application exists.

Reject unnecessary:

```text
admin-app
teacher-app
parent-app
```

separation.

---

# 21. Angular Dependency Review

List every non-standard npm dependency.

For each:

- explain purpose;
- verify current use;
- evaluate whether Angular already provides equivalent capability.

Look especially for premature:

- NgRx;
- Redux libraries;
- UI frameworks;
- chart libraries;
- date libraries;
- form libraries;
- utility mega-libraries.

Remove anything not needed yet.

---

# 22. Angular UX Review

Bootstrap UI should remain minimal.

Reject:

- fake dashboard;
- fake students screen;
- fake billing screen;
- production marketing landing page;
- placeholder modules.

Accept:

- application shell;
- routing;
- simple system status page if useful;
- responsive structure.

---

# 23. Angular Architecture Review

Verify frontend organization does not already become:

```text
components/
services/
models/
helpers/
```

containing unrelated global code.

Prefer feature-oriented organization when features begin.

Bootstrap may remain very small.

---

# 24. Frontend API Integration

Validate the frontend can call the backend in development.

Check:

- API base URL configuration;
- environment handling;
- errors;
- CORS compatibility.

No hard-coded production URL.

---

# 25. Review Area I — Security

Perform bootstrap security review.

Check:

- no secrets;
- HTTPS;
- production CORS not permissive;
- no fake authentication;
- no home-grown password storage;
- no sensitive values in frontend config;
- no telemetry secrets logged.

Reject:

```text
AllowAnyOrigin
```

in production config.

---

# 26. Identity Review

Production Entra implementation should NOT yet exist unless specifically authorized.

Verify Codex did not create:

- custom User password table;
- local login;
- JWT signing system;
- home-grown identity provider;
- fake token service.

Identity architecture must remain consistent with the approved Entra direction.

---

# 27. Key Vault Review

If Key Vault integration exists:

verify it is environment-safe.

If it does not yet exist:

ensure configuration allows it later without redesign.

Do not require cloud access for local development.

---

# 28. Review Area J — Observability

Verify OpenTelemetry initialization.

Expected instrumentation:

- ASP.NET Core;
- outgoing HTTP;
- useful runtime/system signals;
- database instrumentation if configured safely.

Application must still run without cloud telemetry credentials.

---

# 29. Telemetry Minimalism

Review whether Codex introduced excessive telemetry configuration.

Remove:

- unnecessary custom metrics;
- fake domain metrics;
- dozens of dashboards;
- provider metrics for providers not implemented.

Bootstrap telemetry should answer:

- is API running?
- are requests failing?
- is SQL slow/failing?
- are outbound calls failing?

---

# 30. Logging Review

Verify structured logging.

Ensure no secrets.

Check whether correlation/trace ID is available.

Do not require a separate logging framework if built-in logging satisfies current needs.

---

# 31. Review Area K — CI

Inspect:

```text
.github/workflows/ci.yml
```

Required:

```text
backend restore
backend build
backend tests
frontend install
frontend lint
frontend tests
frontend build
```

Architecture tests must run.

No:

```text
continue-on-error: true
```

for required checks.

---

# 32. CI Version Pinning

Verify:

- .NET 10 setup;
- Node version compatible with Angular 22;
- deterministic dependency install;
- `npm ci` rather than uncontrolled install when lockfile exists.

Avoid floating actions/dependencies where a stable version should be pinned.

---

# 33. CI Simplicity

Reject multiple workflows doing essentially the same thing.

For bootstrap, prefer one clean CI workflow.

Deployment workflow may remain separate later.

---

# 34. Review Area L — Tests

Run all test commands.

Backend:

```bash
dotnet test
```

Frontend:

```bash
npm test
```

or approved equivalent.

Do not accept test projects that compile but do not execute.

---

# 35. Test Quality

Check bootstrap tests are meaningful.

Good:

- architecture boundary;
- API startup;
- health endpoint;
- DB connectivity/migrations.

Bad:

```text
Assert.True(true)
```

or generated meaningless tests.

Remove meaningless tests rather than inflate coverage.

---

# 36. Integration Testing Database

Verify relational persistence testing uses SQL Server-compatible behavior.

Reject reliance on:

```text
EF Core InMemory
```

as proof of relational correctness.

SQLite may also be insufficient where SQL Server-specific behavior matters.

Use a real SQL Server-compatible test strategy for integration persistence tests.

---

# 37. Review Area M — README / Developer Experience

Follow README from a clean perspective.

Can a developer determine:

1. prerequisites;
2. restore;
3. local SQL setup;
4. migration command;
5. API run;
6. Angular run;
7. tests?

If not:

fix documentation.

---

# 38. Developer Setup Complexity

Count unnecessary manual steps.

Look for opportunities to remove:

- manual file copying;
- duplicate configuration;
- undocumented environment variables;
- repeated commands.

Goal:

```text
Clone
→ Configure local DB/secrets
→ Migrate
→ Run
```

---

# 39. Review Area N — Configuration

Inspect all appsettings/environment files.

Look for duplication.

Ensure:

- base config contains non-sensitive defaults;
- development overrides only what differs;
- production-specific config is not hard-coded.

Do not create many environment files unnecessarily.

---

# 40. Review Area O — Architecture Alignment

Compare implementation against every Gate 1.1 decision.

Validate:

- .NET 10;
- ASP.NET Core 10;
- EF Core 10;
- Angular 22;
- Azure SQL compatibility;
- Modular Monolith;
- one Angular application;
- no broker;
- no Redis;
- no microservices;
- no Kubernetes;
- no domain implementation.

Every deviation must be listed.

---

# 41. Review Area P — AGENTS.md Compliance

Review against AGENTS.md.

Specifically identify whether Codex:

- invented business behavior;
- added unnecessary abstraction;
- introduced synonyms;
- bypassed boundaries;
- created premature domain code;
- used speculative infrastructure.

Any violation must be corrected or explicitly approved through ADR.

---

# 42. Review Area Q — SDD Compliance

Check implementation does not contradict:

- architecture style;
- module boundaries;
- platform constraints;
- identity direction;
- tenant direction;
- integration isolation;
- testing philosophy.

---

# 43. Review Area R — ADR Integrity

Verify all approved Gate 1.1 ADRs exist:

```text
ADR-016
ADR-017
ADR-018
ADR-019
ADR-020
ADR-021
ADR-022
ADR-023
ADR-024
ADR-025
```

Each ADR must be concise and match the actual implementation.

No ADR should claim a technology was implemented if it was not.

---

# 44. Review Area S — Unnecessary Complexity Audit

Perform a final deletion-oriented review.

For every:

- project;
- package;
- folder;
- interface;
- base class;
- middleware;
- custom abstraction;
- configuration option;
- workflow;
- test helper

ask:

> Would the bootstrap stop working if this were removed?

If no:

strongly consider removal.

---

# 45. Review Area T — Premature Scalability Audit

Search for premature:

- distributed cache;
- broker;
- event bus;
- sharding;
- database-per-tenant;
- microservice communication;
- eventual consistency framework;
- retry orchestration;
- saga infrastructure.

Remove unless explicitly approved.

---

# 46. Review Area U — Naming

Review naming consistency:

```text
SchoolERP
SchoolERP.Api
SchoolERP.Domain
SchoolERP.Application
SchoolERP.Infrastructure
SchoolERP.Web
```

Avoid unnecessary variants.

Check namespaces match intended ownership.

---

# 47. Review Area V — Build Reproducibility

From clean state validate:

```bash
dotnet restore
dotnet build
dotnet test
```

and frontend:

```bash
npm ci
npm run lint
npm test
npm run build
```

Record actual results.

---

# 48. Review Area W — Local Runtime Validation

Run:

- backend;
- frontend;
- local database.

Verify:

```text
API starts
Angular starts
Angular reaches API
health/live succeeds
health/ready succeeds
```

Document failures accurately.

---

# 49. Review Area X — Failure Validation

At minimum test:

## Database unavailable

Expected:

```text
health/live = healthy
health/ready = unhealthy
```

or equivalent appropriate behavior.

Application must not leak connection information.

---

# 50. Review Area Y — Security Scan

Review repository for obvious secrets.

Search for patterns related to:

```text
password
secret
token
apikey
connectionstring
private key
BEGIN CERTIFICATE
BEGIN PRIVATE KEY
```

Do not publish sensitive values in the review output if found.

Report file and remediation only.

---

# 51. Review Area Z — Dependency Vulnerabilities

Check available package vulnerability information where tooling permits.

Flag high/critical vulnerabilities.

Do not upgrade major framework versions outside approved stack merely to fix an unrelated issue without evaluating compatibility.

---

# 52. Required Corrections

Classify every issue:

## BLOCKER

Must fix before Phase 2.0.

Examples:

- broken build;
- domain → infrastructure reference;
- committed secret;
- incorrect framework version;
- fake domain implementation;
- CI failing;
- unsafe database setup.

## REQUIRED

Must fix before gate closes but not architectural failure.

Examples:

- README error;
- missing architecture test;
- lint configuration issue;
- duplicate package.

## RECOMMENDED

Useful improvement that does not block first vertical slice.

Keep this list short.

---

# 53. Do Not Expand Scope

During review, do NOT implement:

- Person;
- Student;
- Enrollment;
- Attendance;
- Billing;
- Payments;
- DGII;
- Authentication;
- Parent Portal.

This is a review/fix gate.

Do not turn it into Phase 2.0.

---

# 54. Allowed Fixes

Allowed fixes include:

- remove dependency;
- correct project reference;
- fix CI;
- fix configuration;
- fix logging;
- fix health checks;
- fix migrations;
- improve README;
- correct telemetry setup;
- clean frontend bootstrap;
- add high-value architecture test;
- remove speculative scaffolding.

---

# 55. Architecture Review Scorecard

Provide:

| Area | Result |
|---|---|
| Repository simplicity | PASS / WARN / FAIL |
| Dependency direction | PASS / WARN / FAIL |
| Domain purity | PASS / WARN / FAIL |
| Backend bootstrap | PASS / WARN / FAIL |
| EF Core/database | PASS / WARN / FAIL |
| Angular bootstrap | PASS / WARN / FAIL |
| Security | PASS / WARN / FAIL |
| Observability | PASS / WARN / FAIL |
| CI/CD | PASS / WARN / FAIL |
| Tests | PASS / WARN / FAIL |
| Documentation | PASS / WARN / FAIL |
| SDD compliance | PASS / WARN / FAIL |
| AGENTS.md compliance | PASS / WARN / FAIL |

---

# 56. Architecture Debt Register

List only architecture debt that actually exists.

Format:

```text
AD-001
Issue:
Reason accepted:
Impact:
When it must be resolved:
```

Do not invent architecture debt for hypothetical improvements.

Preferred outcome:

```text
No accepted architecture debt.
```

---

# 57. Deviation Register

For every deviation from approved documents:

```text
DEV-###
Expected:
Actual:
Reason:
Decision:
```

Decision must be:

```text
FIX
ACCEPT VIA ADR
REMOVE
DEFER
```

No silent deviations.

---

# 58. Dependency Inventory

Provide concise inventory.

Example:

```text
Backend
- Microsoft.EntityFrameworkCore.SqlServer — required for Azure SQL
- OpenTelemetry... — required for approved observability

Frontend
- Angular framework dependencies
- ...
```

Every non-standard dependency needs rationale.

---

# 59. Project Inventory

Report final projects and their purpose.

Example:

```text
SchoolERP.Api
Composition root and HTTP host.

SchoolERP.Application
Application use-case boundary.

SchoolERP.Domain
Pure business domain.

SchoolERP.Infrastructure
Persistence and external technical integrations.
```

If a project has no clear purpose:

remove it.

---

# 60. Final Validation Commands

Execute and report exact result for:

```bash
dotnet --version
dotnet restore
dotnet build
dotnet test
```

Frontend:

```bash
node --version
npm --version
npm ci
npm run lint
npm test
npm run build
```

If a command does not exist:

report why.

Do not claim PASS without execution.

---

# 61. Runtime Validation

Report:

```text
API Startup: PASS/FAIL
Frontend Startup: PASS/FAIL
Database Connection: PASS/FAIL
Migration Apply: PASS/FAIL
Health Live: PASS/FAIL
Health Ready: PASS/FAIL
Frontend → Backend: PASS/FAIL
```

---

# 62. Phase 1.3 Final Report

Return exactly:

## 1. Executive Result

```text
PASS
PASS WITH REQUIRED FIXES
FAIL
```

## 2. Architecture Scorecard

## 3. Blockers

## 4. Required Fixes

## 5. Recommended Improvements

Maximum five.

## 6. Dependencies Review

## 7. Project Structure Review

## 8. Security Review

## 9. Test & CI Results

## 10. Runtime Validation

## 11. Architecture Debt

## 12. Deviations

## 13. Files Changed During Review

## 14. Final Gate Decision

---

# 63. PASS Criteria

Phase 1.3 passes only if:

```text
Build passes
+
Tests pass
+
Angular build passes
+
CI is valid
+
DB bootstrap works
+
No committed secrets
+
Architecture boundaries hold
+
No speculative business code
+
No critical architecture deviation
```

---

# 64. Gate Decision on PASS

If PASS:

output:

```text
PHASE 1.3 — BOOTSTRAP ARCHITECTURE REVIEW

STATUS: PASSED

AUTHORIZED NEXT PHASE:

Phase 2.0 — Core Domain Foundation
First Vertical Slice:
Person + Student Profile + Enrollment
```

Do NOT begin Phase 2.0 automatically.

---

# 65. Gate Decision on Failure

If FAIL:

state explicitly:

```text
PHASE 2.0 IS BLOCKED
```

Then list only the fixes necessary to unblock it.

Do not use failure as an excuse to redesign unrelated architecture.

---

# 66. Final Architecture Question

Before closing the gate ask:

> Can we begin implementing the first business requirement without first cleaning up technical debt created during bootstrap?

If the answer is NO:

the gate must not pass.

---

# 67. Final Mindset

The bootstrap architecture should be boring in the best possible way.

It should be:

```text
small
predictable
secure
testable
understandable
replaceable at the edges
strict at the core
```

It should not attempt to impress future engineers with sophistication.

It should allow them to understand the solution quickly and confidently modify it without hidden machinery.

Always apply:

> **“Make every single detail perfect, and limit the number of details.”**

A perfect bootstrap is not one with everything.

A perfect bootstrap is one with **exactly what the product needs to begin safely, and nothing else.**

# END — PHASE 1.3 BOOTSTRAP ARCHITECTURE REVIEW