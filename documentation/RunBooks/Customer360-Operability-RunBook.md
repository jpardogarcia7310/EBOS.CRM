# Customer 360 Operability Runbook

## Scope
- Customer 360 dedupe, merge, consent, and audit outbox operability.
- Applies to `EBOS.CRM.Api` and dependent infrastructure services.

## Service Objectives (SLO)
- RTO (service recovery):
  - P1 (Customer 360 API unavailable): <= 30 minutes
  - P2 (degraded outbox/merge/consent): <= 4 hours
- RPO (acceptable data loss):
  - API + CRM DB: <= 5 minutes (transactional writes)
  - Audit outbox dispatch state: <= 15 minutes (retry window)
- Escalation:
  - P1 -> on-call immediately, incident channel, status updates every 15 minutes.
  - P2 -> business-hours response, status updates every 60 minutes.

## Operational Endpoints
- Readiness dashboard:
  - `GET /api/v2.0/OperationalReadiness/dashboard`
- Alert state summary:
  - `GET /api/v2.0/OperationalReadiness/alerts`
- Health checks:
  - Liveness: `GET /health/live`
  - Readiness: `GET /health/ready`

## Metrics (Customer 360)
- Source meter: `EBOS.CRM.Customer360`
- Counters:
  - `customer360.merge.total`
  - `customer360.dedupe.query.total`
  - `customer360.consent.event.total`
  - `customer360.audit.outbox.total`
  - `customer360.concurrency.total`

## Recommended Dashboard Panels
- Dedupe:
  - total dedupe queries per minute
  - dedupe candidates per query percentile
- Merge:
  - merge success/failure ratio
  - merged customers per operation
- Consent:
  - consent granted/revoked trend by type
- Outbox:
  - pending queue size
  - failed outbox messages
  - last successful dispatch timestamp
- Concurrency:
  - conflicts per minute
  - exhausted retries count

## Alerting Rules (baseline)
- Outbox critical:
  - `outbox.failed >= OutboxFailedCriticalThreshold`
- Outbox backlog critical:
  - `outbox.pending >= OutboxPendingCriticalThreshold`
- Outbox stale dispatch:
  - last dispatch older than `OutboxDispatchStaleMinutesThreshold` and pending > 0
- Concurrency critical:
  - `concurrency.failures.total >= ConcurrencyFailuresCriticalThreshold`

## Configuration
- Section: `OperationalReadiness`
  - `OutboxPendingWarningThreshold`
  - `OutboxPendingCriticalThreshold`
  - `OutboxFailedCriticalThreshold`
  - `ConcurrencyFailuresCriticalThreshold`
  - `OutboxDispatchStaleMinutesThreshold`

## Migration Procedure
1. Backup DB and capture current schema version.
2. Deploy API with migration artifacts.
3. Run migrations automatically on startup or execute `dotnet ef database update`.
4. Validate:
   - `/health/ready` returns `Healthy` or expected `Degraded`.
   - critical Customer 360 endpoints respond correctly.
5. Monitor outbox and concurrency alerts for at least 30 minutes.

## Rollback Procedure
1. Stop incoming writes if possible.
2. Roll back app version.
3. If schema rollback is required, execute target rollback migration in controlled window.
4. Validate health endpoints and primary Customer 360 flows.
5. Reprocess outbox backlog if pending messages remain.

## Incident Playbooks
- Readiness degraded/unhealthy:
  - check `/api/v2.0/OperationalReadiness/dashboard`
  - inspect `outbox.pending`, `outbox.failed`, and stale dispatch
  - if DB/migrations related, stop rollout and execute rollback.
- Outbox failures increasing:
  - validate `AuditService:BaseUrl`
  - verify network connectivity and auth to audit service
  - inspect `AuditOutboxMessage.LastError`
  - pause non-critical writes if backlog grows above critical threshold.
- High concurrency failures:
  - identify hot aggregates/endpoints
  - tune `CommandExecution` retry settings
  - review conflicting writes and business workflow
  - enable temporary traffic shaping for conflicting endpoints if needed.

## Operational Drills
- Frequency:
  - Monthly: outbox failure simulation and recovery.
  - Quarterly: migration + rollback rehearsal in staging-like environment.
  - Quarterly: alert routing verification (warning + critical end-to-end).
- Minimum evidence to store:
  - execution date/time, operator, simulated scenario, detection time, recovery time, lessons learned.
  - use template: `documentation/RunBooks/Customer360-Drill-Record-Template.md`
- Exit criteria:
  - alert fired and routed correctly,
  - runbook steps reproducible by another operator,
  - measured RTO/RPO within objective.

## Post-Deploy Checklist
- Run: `documentation/RunBooks/Customer360-PostDeploy-Checklist.md`
- Mark each item as `PASS/FAIL/N/A` and attach evidence links (logs, screenshots, query results).

## Verification Checklist
- `dotnet build EBOS.CRM.slnx -c Debug`
- `dotnet test tests/EBOS.CRM.IntegrationTests/EBOS.CRM.IntegrationTests.csproj -c Debug --filter "FullyQualifiedName~Customer360"`
- readiness and dashboard endpoints accessible in deployed environment.
