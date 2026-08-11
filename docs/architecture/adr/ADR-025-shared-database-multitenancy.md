# ADR-025 — Shared-database multi-tenancy

**Status:** Accepted

## Context

The MVP needs tenant isolation without database-per-tenant operational complexity.

## Decision

Use one shared Azure SQL database and schema with explicit `TenantId` ownership on future tenant-scoped data.

## Consequences

Application-level tenant enforcement and automated isolation tests are mandatory when tenant behavior is implemented. No tenant domain code is introduced during bootstrap.
