# Observability and Resilience Drill Record Template

Use this template for each drill execution.
Store records under `documentation/RunBooks/Drills/Records/`.

## Drill Metadata
- Drill ID:
- Date/Time (UTC):
- Environment:
- Operator(s):
- Reviewer:
- Drill type:
  - dependency timeout + circuit breaker
  - high error-rate triage by correlation/trace
  - rollback (app + config)

## Scenario
- Objective:
- Preconditions:
- Trigger steps:

## Detection and Response
- Detection source (alert/dashboard/log):
- Time to detect (minutes):
- First failing `correlationId`:
- Representative `traceId`:
- Impacted endpoint(s):
- Actions executed:
- Runbook section used:

## Recovery Results
- Time to recover (minutes):
- RTO target met (PASS/FAIL):
- RPO target met (PASS/FAIL):
- Business impact summary:

## Evidence
- Logs query evidence (by `correlationId`):
- Traces evidence (by `traceId`):
- Metric query links/results:
- Alert notifications/routing evidence:
- Deployment/incident ticket references:

## Lessons Learned
- What worked:
- What failed:
- Action items:
  - Owner:
  - Due date:
  - Tasks:
