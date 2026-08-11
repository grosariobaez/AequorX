# School ERP RD — Agent Instructions

## Required reading

Before modifying this repository, read these documents completely:

1. `docs/SDD.md`
2. `docs/architecture/technical-architecture-gate-1.1.md`
3. `docs/requirements/phase-1.2-solution-bootstrap.md`
4. `docs/requirements/phase-1.2-execution-prompt.md` when implementing or validating Phase 1.2
5. `docs/architecture/bootstrap-architecture-review-gate-1.3.md` when reviewing or completing the bootstrap
6. every applicable file under `docs/architecture/adr/` once that directory exists
7. `PROJECT_MEMORY.md`

The full documents are authoritative; this file is only a navigation and safety layer. If instructions conflict with the approved SDD or an approved ADR, stop only the affected work, report the conflict, and continue independent safe work.

## Current phase

**Phase 2.1 — Attendance Foundation is complete and merged.** Phase 1.2 and Phase 1.3 are complete. Phase 2.0 is complete and merged.

Phase 2.1 is limited to AttendanceRecord, four attendance exception statuses, Present-by-absence, correction through the defined application operation, server-derived audit identity, the minimal attendance API, and one localized attendance screen. Do not implement schedules, periods, subjects/classes, teacher assignment, notifications, absence justification workflows, medical notes, dashboards, grades, assessments, billing, or AI.

Do not invent business concepts, placeholder entities, fake provider implementations, speculative interfaces, or empty architecture ceremony. No next phase is authorized.

## Fixed technical baseline

- .NET 10 LTS, ASP.NET Core 10, C#
- Entity Framework Core 10
- Azure SQL Database in production and a SQL Server-compatible local environment
- Angular 22 with TypeScript, as one application
- Azure App Service
- Microsoft Entra platform / External ID direction; production identity behavior is deferred
- Azure Key Vault and Managed Identity
- OpenTelemetry with Azure Monitor / Application Insights
- GitHub and GitHub Actions
- Modular monolith with API, Application, Domain, and Infrastructure boundaries

## Architecture guardrails

- Keep the solution small, explicit, and free of speculative abstractions.
- Domain must not depend on Infrastructure, ASP.NET Core, EF Core, Azure SDKs, or external providers.
- Application must not depend on concrete Infrastructure implementations.
- API composes dependencies; Infrastructure implements Application/Domain-facing contracts only when currently required.
- Do not introduce microservices, Kubernetes/AKS, brokers, distributed caches, NoSQL primary persistence, event sourcing, full CQRS, generic workflow/rules engines, generic repositories, UnitOfWork wrappers, service mesh, or AI-agent infrastructure.
- MediatR, AutoMapper, FluentValidation, Dapper, extra UI libraries, and similar dependencies require a concrete present need and explicit justification.
- Never commit secrets. Keep local secrets in environment variables, user secrets, or another documented local-secret mechanism.
- Use English canonical domain vocabulary and Spanish user-facing UI.
- Report actual validation results; never claim a command passed unless it was run.

## Governing principle

> Make every single detail perfect, and limit the number of details.

For every addition ask: **Does this solve a requirement in the current bootstrap?** If not, do not add it.
