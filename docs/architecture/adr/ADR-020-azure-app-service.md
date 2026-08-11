# ADR-020 — Azure App Service

**Status:** Accepted

## Context

The modular monolith needs a managed, operationally simple production host.

## Decision

Host the ASP.NET Core backend on Azure App Service.

## Consequences

Kubernetes and container orchestration are excluded from the MVP production architecture.
