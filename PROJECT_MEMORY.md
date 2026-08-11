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

Do not rely on this summary in place of the full documents. Read the applicable source before making changes.

## Current project state and authorization

- Product: multi-tenant SaaS ERP for private schools in the Dominican Republic.
- Architecture: modular monolith; one backend deployment, one relational database, one Angular application.
- Current authorized work: **Phase 1.2 — Solution Bootstrap only**.
- Coding is authorized for technical bootstrap, not for business features.
- The repository was initially empty except for `.git`; the governing documents and this memory layer are the first project files.
- Phase 1.3 is a future strict review gate. It becomes applicable only after Phase 1.2 is implemented and validated; merely storing its document does not advance the project phase.

## Phase 1.2 outcome

The bootstrap must prove that the approved architecture can compile, run, connect to local SQL, apply an initial migration without fake domain tables, expose `/health/live` and `/health/ready`, provide development OpenAPI, serve a minimal Angular shell, run automated and architecture tests, initialize telemetry without cloud credentials, and pass GitHub Actions CI.

The target solution contains four backend projects (`Api`, `Application`, `Domain`, `Infrastructure`), four test projects (`Domain.Tests`, `Application.Tests`, `IntegrationTests`, `ArchitectureTests`), and one Angular application (`Web`). It must remain intentionally small.

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

Before declaring Phase 1.2 complete, execute and report the real results of backend restore/build/tests and frontend install/lint/tests/build, plus database, health, telemetry, and architecture validation. Document all deviations; if there are none, state `No deviations.`

After Phase 1.2, Phase 1.3 must independently audit repository simplicity, dependency direction, domain purity, packages, API host, health and failure behavior, EF Core/migrations, tenant readiness, Angular, security, observability, CI, tests, documentation, configuration, ADR integrity, reproducibility, runtime behavior, and vulnerabilities. A passing review requires real command/runtime evidence and does not itself start Phase 2.0.
