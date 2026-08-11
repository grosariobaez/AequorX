# ADR-022 — Azure Key Vault and Managed Identity

**Status:** Accepted

## Context

Production credentials must not live in source control or long-lived application configuration.

## Decision

Use Managed Identity for supported Azure authentication and Key Vault for required production secrets.

## Consequences

Local development remains cloud-independent and uses environment variables or .NET user secrets.
