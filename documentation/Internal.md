# EBOS.CRM Internal Architecture (Current)

This document is the internal technical reference for the current enterprise baseline of the EBOS.CRM solution.

## Solution Modules

- `EBOS.CRM.Api`
  - HTTP surface, versioned endpoints, policies, middleware, Swagger, health/readiness, metrics endpoint.
- `EBOS.CRM.Application`
  - Use cases (commands/queries), validators, handlers, orchestration rules.
- `EBOS.CRM.Domain`
  - Aggregates/entities, invariants, domain services, domain contracts.
- `EBOS.CRM.Infrastructure`
  - EF Core persistence, migrations, repositories, outbox, integrations, telemetry wiring.
- `EBOS.CRM.Contracts`
  - API/application DTO contracts and compatibility-critical payload models.

## API Layer (EBOS.CRM.Api)

- Controllers are split by domain:
  - `Controllers/CRM/*`
  - `Controllers/EBOS/*`
  - `Controllers/Observability/*`
  - `Controllers/Operations/*`
- Standards:
  - versioned routes (`/api/v{version}/...`)
  - centralized error handling middleware
  - correlation ID middleware
  - policy-based authorization (including Customer 360 sensitive operations)
  - OpenAPI generated and guarded with snapshot compatibility tests

## Application Layer (EBOS.CRM.Application)

- Pattern:
  - command/query DTO
  - validator
  - handler
  - repository abstractions + tenant/current-user context
- Scope:
  - CRM modules (Customer 360, Sales, Service, master entities)
  - EBOS governance modules
  - privacy workflows (request, execute, retention)
  - merge lineage and dedupe operations

## Domain Layer (EBOS.CRM.Domain)

- Enterprise focus:
  - invariants enforced by domain methods
  - transition-based state changes
  - reduced anemic mutability in key Customer 360 entities
  - concurrency-aware entities (row version where required)

## Infrastructure Layer (EBOS.CRM.Infrastructure)

- EF Core:
  - mappings per aggregate
  - SQL Server migrations
  - snapshot/designer artifacts
- Repositories:
  - CRM and EBOS repository implementations
  - base repository contract behavior (tenant, pagination, soft-delete/erased filters)
- Customer 360 hardening:
  - merge lineage persistence (`CustomerMergeHistories`)
  - privacy request persistence (`CustomerPrivacyRequests`)
  - outbox persistence (`AuditOutboxMessages`)
  - dedupe strategy/index hardening
- Outbox:
  - dispatch service + dispatcher background process
  - retry/transient-failure behavior covered by tests

## Testing Suites

- `tests/EBOS.CRM.ApiTests`
  - controller tests, validators/handlers tests, domain invariants tests, contract tests, infra/service unit tests.
- `tests/EBOS.CRM.IntegrationTests`
  - endpoint behavior, auth/tenant isolation, Customer 360 E2E, SQL Server hardening/idempotency, OpenAPI compatibility.
- `tests/EBOS.CRM.ConcurrencyTests`
  - endpoint and infra/app concurrency scenarios (outbox dispatcher, retention service, repository conflicts).
- `tests/EBOS.CRM.StressTests`
  - high-volume Customer 360 scenarios (outbox backlog, merge/dedupe, retention throughput/latency).

## CI/CD Quality Gates

- Workflow: `.github/workflows/customer360-suites-ci.yml`
- Separate jobs:
  - API suite
  - Integration suite
  - Concurrency suite
  - Stress suite
  - Integration SQL Server suite (`USE_TESTCONTAINERS=true`)
- SQL Server hardening includes:
  - migration apply/rollback checks
  - write contention and consistency scenarios
  - idempotency checks
  - migration duplicate `CreateTable` guard test

## Observability and Operability

- Assets:
  - `documentation/Observability/prometheus/*`
  - `documentation/Observability/grafana/*`
  - `documentation/Observability/docker-compose.observability.yml`
- Provisioning:
  - Grafana datasource + dashboard provisioning
  - Prometheus alert rules + Alertmanager routing
- CI validation scripts:
  - `documentation/Observability/ci/validate-observability.sh`
  - `documentation/Observability/ci/smoke-observability.sh`

## Runbooks and Operational Documents

- `documentation/RunBooks/Customer360-Operability-RunBook.md`
- `documentation/RunBooks/Customer360-PostDeploy-Checklist.md`
- Legacy single-file drill template:
  - `documentation/RunBooks/Customer360-Drill-Record-Template.md`
- Per-execution drill model (recommended):
  - `documentation/RunBooks/Drills/README.md`
  - `documentation/RunBooks/Drills/Customer360-Drill-Execution-Template.md`
  - `documentation/RunBooks/Drills/Records/`
- Spanish counterparts available under the same folder with `_ES` suffix.

## Internal Notes

- Treat migrations as immutable historical artifacts; fix forward where possible.
- Keep security/policy checks strict in tests; avoid disabling global audit/auth unless explicitly scoped for a test fixture.
- Prefer deterministic tests over timing-sensitive behavior for CI stability.
