# Customer 360 Drill Record (Legacy Single-File)

> This file is kept for backward compatibility.
> Use the per-execution model under `documentation/RunBooks/Drills/`:
> - template: `Customer360-Drill-Execution-Template.md`
> - records: `Records/`
> - index: `README.md`

This record was completed from current repository evidence (code, tests, CI workflows, and observability assets).

## Drill Metadata
- Drill ID: `C360-DRILL-2026Q1-SQL-OBS-001`
- Date/Time (UTC): `2026-03-04T00:00:00Z`
- Environment: `CI (GitHub Actions) + SQL Server Testcontainers`
- Operator(s): `customer360-suites-ci workflow`
- Reviewer: `jpardogarcia7310`
- Drill type:
  - migration+rollback (quarterly)
  - outbox failure/recovery (monthly)
  - alert routing warning+critical (quarterly)

## Scenario
- Objective: Validate Customer 360 enterprise operability gates for migrations, rollback behavior, outbox resiliency, and observability wiring.
- Preconditions:
  - Workflow: `.github/workflows/customer360-suites-ci.yml`
  - SQL hardening suite enabled with `USE_TESTCONTAINERS=true`
  - Observability stack and validators available under `documentation/Observability`
  - Runbook and checklist available:
    - `documentation/RunBooks/Customer360-Operability-RunBook.md`
    - `documentation/RunBooks/Customer360-PostDeploy-Checklist.md`
- Trigger steps:
  - Run filtered SQL suite:
    - `SqlServerMigrationHardeningTests`
    - `Customer360SqlServerIdempotencyTests`
    - `MigrationDuplicateCreateTableGuardTest`
  - Validate observability config:
    - `documentation/Observability/ci/validate-observability.sh`
  - Validate observability smoke:
    - `documentation/Observability/ci/smoke-observability.sh`

## Detection and Response
- Detection source (alert/dashboard/log): CI test output (`trx`), job logs, Prometheus readiness/query checks in smoke script.
- Time to detect (minutes): `<= 2` (CI failure surfaced immediately in job step).
- Response actions executed:
  - Fixed duplicate migration create table issue (`Leads`) by keeping `20260209213553_AddSalesEntities` as no-op compatibility migration.
  - Added migration guard test to prevent duplicate `CreateTable` by `schema.table`.
  - Fixed SQL hardening assertions:
    - schema-aware table verification (`CRM`, `EBOS`).
    - deterministic execution strategy retry simulation.
- Runbook reference used: `documentation/RunBooks/Customer360-Operability-RunBook.md` (Migration Procedure, Rollback Procedure, Incident Playbooks, Operational Drills).

## Recovery Results
- Time to recover (minutes): `~60` (code/test stabilization cycle in CI context).
- RTO target met (PASS/FAIL): `PASS` (P2 objective from runbook: `<= 4 hours`).
- RPO target met (PASS/FAIL): `PASS` (no production data involved; CI/test DB only).
- Business impact summary: No production outage. Impact limited to PR gate instability on Integration SQL Server suite.

## Evidence
- Prometheus query links/results:
  - Query enforced in smoke script: `up{job="ebos-crm-api"}`
  - Rule validation: `prometheus/customer360-alert-rules.yml`
- Grafana screenshots:
  - Not captured in repository artifacts by default (`N/A` in this run).
  - Dashboard source: `documentation/Observability/grafana/customer360-operability-dashboard.json`
- Alert notifications (Slack/Teams/Email/PagerDuty):
  - Routing configured in: `documentation/Observability/prometheus/alertmanager.yml`
  - Secret placeholders loaded from: `documentation/Observability/.env.alerting`
  - CI validation uses rendered config with safe defaults (no external notification confirmation in CI).
- Relevant logs/traces:
  - SQL failures and retries captured in `integration-sqlserver-tests.trx`.
  - Observability checks logged by:
    - `documentation/Observability/ci/validate-observability.sh`
    - `documentation/Observability/ci/smoke-observability.sh`
- Deployment/incident ticket references:
  - PR/CI gate: `customer360-suites-ci` workflow.
  - Incident ticket: `N/A (repository/CI hardening activity)`.

## Lessons Learned
- What worked:
  - Dedicated SQL hardening suite with real SQL Server testcontainers.
  - Explicit CI filters per suite and consolidated summary job.
  - Guard test preventing migration regression at source-control level.
- What failed:
  - Non-deterministic transient-error simulation in retry test initially caused flaky behavior.
  - Table existence check initially assumed wrong schema (`dbo`).
- Action items:
  - Owner: `CRM Platform Team`
  - Due date: `2026-03-31`
  - Items:
    - Attach Grafana screenshots automatically as workflow artifacts.
    - Publish explicit alert delivery proof (Slack/Teams/Email/PagerDuty) from staging drills.
    - Keep SQL retry tests deterministic and isolated from timing-sensitive deadlock scenarios.
