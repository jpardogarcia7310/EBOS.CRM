# Customer 360 Drill Record

## Drill Metadata
- Drill ID: `C360-DRILL-QUARTERLY-MIGRATION-ROLLBACK-001`
- Date/Time (UTC): `2026-03-04T00:00:00Z`
- Environment: `GitHub Actions + SQL Server Testcontainers`
- Operator(s): `customer360-suites-ci`
- Reviewer: `jpardogarcia7310`
- Frequency: `quarterly`
- Drill type: `migration+rollback`

## Scope and Objective
- Objective: Validate migration apply/rollback and post-failure consistency on SQL Server.
- In-scope components: EF migrations, SQL hardening tests, migration guard test.
- Out-of-scope components: production rollout windows.

## Preconditions
- Runbook used: `documentation/RunBooks/Customer360-Operability-RunBook.md`
- Required access verified: Yes
- Feature flags/configuration: `USE_TESTCONTAINERS=true`
- Data/tenant setup: isolated per-test SQL DB names

## Execution Steps
1. Run `SqlServerMigrationHardeningTests`.
2. Run `Customer360SqlServerIdempotencyTests`.
3. Run `MigrationDuplicateCreateTableGuardTest`.

## Detection and Response
- Detection source: CI failures in `integration-sqlserver-tests`.
- Time to detect (minutes): `<= 2`
- Response actions:
  - fixed duplicated `CreateTable` migration path,
  - added guard test,
  - stabilized retry test behavior.
- Escalation needed: `No`

## Recovery and Validation
- Time to recover (minutes): `~60`
- RTO target met: `PASS`
- RPO target met: `PASS`
- Functional validation done: yes (migrations + rollback + idempotency + consistency).
- Business impact summary: PR gate reliability restored; no production impact.

## Evidence
- CI run / pipeline URL: `customer360-suites-ci / integration-sqlserver-tests`.
- Prometheus queries/results: not primary evidence for this drill.
- Grafana screenshots: N/A.
- Alert notifications: N/A.
- Logs/traces: `integration-sqlserver-tests.trx`.
- Related tickets: `N/A`

## Lessons Learned and Actions
- What worked: SQL Server dedicated suite with testcontainers.
- What failed: flaky transient simulation and duplicate migration artifact.
- Action items:
  - Owner: `CRM Platform Team`
  - Due date: `2026-03-31`
  - Status: `Open`
