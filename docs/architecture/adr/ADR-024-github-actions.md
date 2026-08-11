# ADR-024 — GitHub Actions

**Status:** Accepted

## Context

Pull requests need one reproducible build and test gate.

## Decision

Use GitHub for source control and one GitHub Actions CI workflow for backend and frontend validation.

## Consequences

Required failures fail CI. Production deployment remains outside this bootstrap.
