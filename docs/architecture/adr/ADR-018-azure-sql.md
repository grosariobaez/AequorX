# ADR-018 — Azure SQL Database

**Status:** Accepted

## Context

Academic and financial records require relational integrity and transactions.

## Decision

Use Azure SQL Database in production and SQL Server-compatible databases for development and tests.

## Consequences

Relational behavior is validated against SQL Server, not EF InMemory or SQLite substitutes.
