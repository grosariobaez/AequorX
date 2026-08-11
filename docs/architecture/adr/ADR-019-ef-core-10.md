# ADR-019 — Entity Framework Core 10

**Status:** Accepted

## Context

The solution needs one primary persistence and migration mechanism.

## Decision

Use Entity Framework Core 10 with the SQL Server provider and committed migrations.

## Consequences

No generic repository or UnitOfWork wrapper is added. Dapper requires a demonstrated later need.
