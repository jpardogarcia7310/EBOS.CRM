# Observability and Resilience Drill Execution Template

Use this file as the base template for each drill execution record.

## Drill Metadata
- Drill ID:
- Date/Time (UTC):
- Environment:
- Operator(s):
- Reviewer:
- Frequency:
  - monthly
  - quarterly
- Drill type:
  - dependency timeout + circuit breaker
  - high error-rate triage (correlationId/traceId)
  - rollback (application + resilience config)

## Scope and Objective
- Objective:
- In-scope components:
- Out-of-scope components:

## Preconditions
- Runbook used:
- Required access verified:
- Feature flags/configuration:
- Data/tenant setup:

## Execution Steps
1.
2.
3.

## Detection and Response
- Detection source (alert/dashboard/log):
- Time to detect (minutes):
- First failing `correlationId`:
- Representative `traceId`:
- Response actions:
- Escalation needed (Yes/No):

## Recovery and Validation
- Time to recover (minutes):
- RTO target met (PASS/FAIL):
- RPO target met (PASS/FAIL):
- Functional validation done:
- Business impact summary:

## Evidence
- CI run / pipeline URL:
- Prometheus queries/results:
- Grafana screenshots:
- Alert notifications:
- Logs query by `correlationId`:
- Traces query by `traceId`:
- Related tickets:

## Lessons Learned and Actions
- What worked:
- What failed:
- Action items:
  - Owner:
  - Due date:
  - Status:
