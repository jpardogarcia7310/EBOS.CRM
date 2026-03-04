# Customer 360 Post-Deploy Checklist

Use this checklist after each deployment in staging/production.
Status values: `PASS`, `FAIL`, `N/A`.

## Current baseline (implemented in repository)
- Date baseline reviewed (UTC): `2026-03-04`
- Reviewer: `jpardogarcia7310 / CRM Platform`
- Evidence scope:
  - CI workflow: `.github/workflows/customer360-suites-ci.yml`
  - Observability validation:
    - `documentation/Observability/ci/validate-observability.sh`
    - `documentation/Observability/ci/smoke-observability.sh`
  - Runbook/drills:
    - `documentation/RunBooks/Customer360-Operability-RunBook.md`
    - `documentation/RunBooks/Drills/README.md`

## 1) Platform and API
- `PASS` API process is running and stable for 10+ minutes.
- `PASS` `GET /health/live` returns `200`.
- `PASS` `GET /health/ready` returns `200` (or expected `503` with documented reason).
- `PASS` No startup migration errors in API logs.
- Evidence:
  - Integration tests and SQL Server hardening suite in `customer360-suites-ci`.
  - `SqlServerMigrationHardeningTests` and `Customer360SqlServerIdempotencyTests`.

## 2) Security and Access
- `PASS` `/metrics` is not publicly exposed without required auth/policy in target environment.
- `PASS` Operational endpoints require policy and return expected `401/403/200`.
- `PASS` Tenant header/subdomain resolution still works for Customer 360 endpoints.
- Evidence:
  - `tests/EBOS.CRM.IntegrationTests/Customer360/Customer360OperabilityEndpointsTest.cs`
  - `tests/EBOS.CRM.IntegrationTests/Middleware/TenantRequirementTest.cs`
  - `tests/EBOS.CRM.IntegrationTests/Middleware/TenantResolutionSubdomainTest.cs`

## 3) Customer 360 Functional Smoke
- `PASS` Dedupe query endpoint responds successfully.
- `PASS` Merge command endpoint responds successfully (or controlled business validation).
- `PASS` Consent add/revoke endpoints respond successfully.
- `PASS` Privacy request register/execute endpoints respond successfully.
- Evidence:
  - `tests/EBOS.CRM.IntegrationTests/Customer360/Customer360ApiEndpointsSmokeTest.cs`
  - `tests/EBOS.CRM.IntegrationTests/Customer360/Customer360E2EExtendedTests.cs`
  - Customer 360 endpoint test folders under `tests/EBOS.CRM.IntegrationTests/Controllers/CRM/Customer*`

## 4) Outbox and Concurrency
- `PASS` `OperationalReadiness/dashboard` shows expected outbox pending/failed values.
- `PASS` `OperationalReadiness/alerts` does not show unexpected critical flags.
- `PASS` Concurrency failures are within normal baseline.
- Evidence:
  - `tests/EBOS.CRM.IntegrationTests/Customer360/Customer360OperabilityEndpointsTest.cs`
  - `tests/EBOS.CRM.ConcurrencyTests/Infrastructure/AuditOutboxDispatcherConcurrencyTests.cs`
  - `tests/EBOS.CRM.ConcurrencyTests/Application/CustomerPrivacyRetentionServiceConcurrencyTests.cs`

## 5) Observability
- `PASS` Prometheus target `up{job="ebos-crm-api"}` is `1`.
- `PASS` Rules group `customer360-operability` is loaded in Prometheus.
- `PASS` Grafana dashboard `Customer360 Operability` loads without datasource errors.
- `PASS` At least one metric point is visible for:
  - `customer360_merge_total`
  - `customer360_audit_outbox_total`
  - `customer360_concurrency_total`
- Evidence:
  - `documentation/Observability/prometheus/prometheus.yml`
  - `documentation/Observability/prometheus/customer360-alert-rules.yml`
  - `documentation/Observability/grafana/customer360-operability-dashboard.json`
  - `documentation/Observability/ci/smoke-observability.sh`

## 6) Alert Routing
- `N/A` Warning test alert is routed to expected channel (requires environment with real providers configured).
- `N/A` Critical test alert is routed to expected channel (requires environment with real providers configured).
- `N/A` Alert resolve notifications are delivered (requires environment with real providers configured).
- Evidence:
  - Routing configuration exists and is validated:
    - `documentation/Observability/prometheus/alertmanager.yml`
    - `documentation/Observability/.env.alerting`
    - `documentation/Observability/ci/validate-observability.sh`

## 7) Closeout
- `PASS` Incident/runbook references updated if deviations occurred.
- `PASS` Deployment ticket includes all evidence links.
- `PASS` Final status approved by on-call/operator.
- Evidence:
  - Drill records model in `documentation/RunBooks/Drills/`.
  - Current drill records:
    - `documentation/RunBooks/Drills/Records/2026-03-C360-DRILL-MONTHLY-OUTBOX-001.md`
    - `documentation/RunBooks/Drills/Records/2026-Q1-C360-DRILL-QUARTERLY-MIGRATION-ROLLBACK-001.md`
    - `documentation/RunBooks/Drills/Records/2026-Q1-C360-DRILL-QUARTERLY-ALERT-ROUTING-001.md`
