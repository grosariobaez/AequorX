# Project Memory — School ERP RD

Last synchronized: 2026-08-11 (America/Toronto)

## Durable source documents

The original attachments have been preserved byte-for-byte in the repository:

| Project file | Role | SHA-256 |
|---|---|---|
| `docs/SDD.md` | Approved functional, domain, security, multi-tenancy, fiscal, payment, workflow, and architecture baseline | `8C39D3F89F96F02D5E00302992E3830749DD427CE416E4950AD179A6CC3027BE` |
| `docs/architecture/technical-architecture-gate-1.1.md` | Approved technical stack and Gate 1.1 decisions | `97BD5C9CA98D6B401715C69620E8B403F776956577038FB5D4389B1BA1708174` |
| `docs/requirements/phase-1.2-solution-bootstrap.md` | Exact execution contract and Definition of Done for Phase 1.2 | `0592868DC60005B77D23F563CEF9B6B51322ED78F6D0C11B311376BBA4E47F6A` |
| `docs/architecture/bootstrap-architecture-review-gate-1.3.md` | Exact review contract required after Phase 1.2 and before any business slice | `1BBFA23B23BBDFCC75F1F9D22ED7257A376C4F70A58782DFB32607A8F12EC82E` |
| `docs/requirements/phase-1.2-execution-prompt.md` | Explicit implementation authorization and operational contract for executing Phase 1.2 | `371FB260D5DCBF8C02FFC6E7664DA03645E3CC3E522404A18E30CACE042C53A5` |

Do not rely on this summary in place of the full documents. Read the applicable source before making changes.

## Current project state and authorization

- Product: multi-tenant SaaS ERP for private schools in the Dominican Republic.
- Architecture: modular monolith; one backend deployment, one relational database, one Angular application.
- Phase 1.2 status: **implemented and locally validated on 2026-08-11**.
- Phase 1.3 status: **passed on 2026-08-11 after correcting two gate defects and rerunning all validation**.
- Phase 2.0 status: **implemented and locally validated on its dedicated branch on 2026-08-11; Draft PR review is pending**.
- Phase 2.0 was explicitly authorized by `Phase 2.0 — Core Domain Foundation.md` (attachment SHA-256 `0BD63664FBAAAB79F1A96E7477F6B4B0CADDDE8A4A35C18224808870DD95C748`).
- The repository was initially empty except for `.git`; the governing documents and this memory layer are the first project files.
- Phase 1.3 independently reviewed the implemented bootstrap; its pass advances only the architecture gate and does not implement or authorize a business slice.

## Phase 1.2 outcome

The bootstrap must prove that the approved architecture can compile, run, connect to local SQL, apply an initial migration without fake domain tables, expose `/health/live` and `/health/ready`, provide development OpenAPI, serve a minimal Angular shell, run automated and architecture tests, initialize telemetry without cloud credentials, and pass GitHub Actions CI.

The target solution contains four backend projects (`Api`, `Application`, `Domain`, `Infrastructure`), four test projects (`Domain.Tests`, `Application.Tests`, `IntegrationTests`, `ArchitectureTests`), and one Angular application (`Web`). It must remain intentionally small.

## Implemented bootstrap state

- .NET SDK 10.0.302; ASP.NET Core and EF Core packages 10.0.10.
- One empty Domain boundary and one empty Application boundary; no business types exist.
- `SchoolERPDbContext` has no business `DbSet`; migration `20260811162159_InitialBootstrap` creates only EF migration history.
- Local SQL Server 2022 Express connectivity and migration application were validated.
- API exposes `/health/live`, `/health/ready`, and development OpenAPI. Liveness remains healthy with SQL unavailable; readiness becomes 503 without leaking connection details.
- OpenTelemetry instruments ASP.NET Core, HTTP, runtime, and SQL; Azure Monitor export activates only when environment configuration is present.
- Angular 22.1 uses strict TypeScript, one standalone application, an explicit root `Home` route, a responsive Spanish technical shell, and typed health integration through the development proxy.
- Four .NET test projects execute seven tests total; Angular executes two meaningful Vitest shell/HTTP/routing tests.
- One GitHub Actions workflow restores, builds, tests, lints, and uses an ephemeral SQL Server container with a generated test password.
- Phase 1.3 corrected two required issues: `/` previously redirected to the not-found route while bootstrap content lived globally, and the migration test could reuse an already migrated database. The shell now uses an explicit `Home` route, and the migration test creates, migrates, verifies, and deletes a unique database on every run.
- Phase 1.3 local results: clean .NET restore/build with 0 warnings, 7/7 .NET tests, format verification, EF migration listing/application against a new database, npm reproducible install with 0 vulnerabilities, lint, 2/2 Angular tests, production build, API/frontend startup, OpenAPI, live/ready health, frontend proxy, controlled SQL failure, vulnerability scan, and secret scan all passed.
- GitHub-hosted CI execution awaits a push or pull request and is not claimed as locally executed.

## Phase 2.0 implementation state

- The implemented domain is limited to Tenant, Campus, Person, StudentProfile, StudentRelationship, AcademicYear, GradeLevel, Section, and Enrollment.
- Tenant scope comes from the server-side `ITenantContext`; API contracts do not accept a tenant identifier, and EF Core global filters enforce tenant-scoped reads.
- Domain constructors enforce same-tenant relationships, valid academic-year dates, and section/year consistency. Tenant-aware keys and foreign keys reinforce these rules in SQL.
- Migration `20260811194820_Phase20CoreDomainFoundation` creates the nine authorized tables and their constraints.
- The API provides the minimal Phase 2.0 query/create surface. Development-only tenant provisioning is explicit; production tenant configuration remains external.
- Angular provides six minimal management screens with Spanish as the default language and an English switch.
- Local Phase 2.0 validation passed: clean .NET build with 0 warnings/errors, 16/16 .NET tests, format verification, Angular lint, 2/2 Angular tests, and production build.
- No next phase is authorized. Attendance, grades, billing, payments, fiscal work, portals, admissions, notifications, AI, and advanced workflows remain deferred.

## Accepted bootstrap architecture debt

- xUnit 2.9.3 is reported as legacy by NuGet. Migration to xUnit v3 is deferred to a focused test-tooling change because current tests pass and the vulnerability scan is clean.
- EF Core SQL Server 10.0.10 resolves through Microsoft.Data.SqlClient 6.1.1 to deprecated Azure.Identity 1.14.2 and Microsoft.Identity.Client 4.73.1 packages. They have no reported vulnerability in the configured NuGet sources. Avoid speculative direct overrides; re-evaluate when EF Core/SqlClient publishes a compatible parent update.

## Non-negotiable constraints

- No Student, Person, Enrollment, Attendance, Assessment, Grade, Charge, Invoice, Payment, Fiscal, DGII, AZUL, CardNet, or Parent Portal implementation during bootstrap.
- No fake domain tables, fake production adapters, speculative provider contracts, broad shared kernel, generic repository, UnitOfWork wrapper, or unused abstraction.
- No microservices, Kubernetes, AKS, message broker, distributed cache, NoSQL database, event sourcing, full CQRS, generic workflow/rules engine, native mobile framework, or AI runtime for the MVP.
- Preserve tenant isolation, historical integrity, financial/fiscal idempotency, authorization, privacy of minors, auditability, and regulated-rule traceability in all later work.
- Do not invent defaults for ambiguous business, fiscal, regulatory, provider, destructive migration, architectural, or security behavior. Stop the affected work and report it.

## Product/domain direction for later phases

- English canonical vocabulary; Spanish UI; initial currency DOP.
- Unified `Person` identity with profiles and relationships.
- `Household` and `BillingAccount` are distinct.
- `Enrollment` is historical; attendance uses exception-based entry; published grades cannot be silently overwritten.
- `Invoice` and `FiscalDocument` are distinct; `PaymentAllocation` is first-class.
- Billing, payment callbacks, and fiscal submissions must be idempotent.
- Correct financial history through explicit reversal, credit, refund, or adjustment—not destructive deletion.
- Rules and deterministic automation precede AI.
- First business slice after platform foundation: Student Enrollment; then Attendance by Exception.

## Decisions intentionally deferred

Background-job framework, notification vendor, IaC tool, frontend component library, E2E framework, exact Entra topology, document storage, production networking/private endpoints, and final CardNet contract remain deferred until a real requirement demands them.

## Working rule

Phase 1.3 independently audited the technical foundation. Phase 2.0 adds only its explicitly authorized core-domain slice and remains subject to Draft PR review.

The next phase must remain proposal-only until explicitly requested. Do not infer further authorization from Phase 2.0 implementation.
