# Customer 360 Operability Runbook

## Scope
- Customer 360 dedupe, merge, consent, and audit outbox operability.
- Applies to `EBOS.CRM.Api` and dependent infrastructure services.

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

## Troubleshooting
- Readiness degraded/unhealthy:
  - check `/api/v2.0/OperationalReadiness/dashboard`
  - inspect `outbox.pending`, `outbox.failed`, and stale dispatch
- Outbox failures increasing:
  - validate `AuditService:BaseUrl`
  - verify network connectivity and auth to audit service
  - inspect `AuditOutboxMessage.LastError`
- High concurrency failures:
  - identify hot aggregates/endpoints
  - tune `CommandExecution` retry settings
  - review conflicting writes and business workflow

## Verification Checklist
- `dotnet build EBOS.CRM.slnx -c Debug`
- `dotnet test tests/EBOS.CRM.IntegrationTests/EBOS.CRM.IntegrationTests.csproj -c Debug --filter "FullyQualifiedName~Customer360"`
- readiness and dashboard endpoints accessible in deployed environment.
