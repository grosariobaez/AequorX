# ADR-023 — OpenTelemetry and Application Insights

**Status:** Accepted

## Context

The system needs portable request, dependency, runtime, and failure telemetry.

## Decision

Instrument with OpenTelemetry and export to Azure Monitor/Application Insights when configured.

## Consequences

Local startup does not require cloud credentials; exporter activation is environment-driven.
