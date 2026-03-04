# Observability and Resilience Post-Deploy Checklist

Use this checklist after each deployment in staging/production.
Status values: `PASS`, `FAIL`, `N/A`.

## Current baseline
- Baseline review date (UTC): `2026-03-04`
- Scope:
  - `documentation/RunBooks/Observability-Operability-RunBook.md`
  - `documentation/RunBooks/Observability-Drill-Record-Template.md`

## 1) Platform and API
- `PASS` API is stable for 10+ minutes.
- `PASS` `GET /health/live` returns `200`.
- `PASS` `GET /health/ready` returns `200` (or expected degraded state with documented reason).
- `PASS` Startup has no migration/configuration errors.

## 2) Correlation and Tracing
- `PASS` Responses include `correlationId` on success and failure paths.
- `PASS` Error responses include `traceId`.
- `PASS` `X-Correlation-Id` is accepted and propagated when provided.
- `PASS` `traceparent`/`tracestate` are accepted and propagated when provided.

## 3) Resilience Policies
- `PASS` Timeout policy is active and produces deterministic timeout payload.
- `PASS` Retry policy is active only for transient failures.
- `PASS` Circuit breaker opens/closes according to configured thresholds.
- `PASS` Rate limiting/degradation behavior is consistent with policy.

## 4) Observability Signals
- `PASS` Logs are queryable by `correlationId` and `traceId`.
- `PASS` Metrics pipeline is healthy and receiving data points.
- `PASS` Traces are visible end-to-end (API -> Application -> Infrastructure).
- `PASS` Dashboards load without datasource/query errors.

## 5) Alerting and Readiness
- `PASS` Availability/latency alerts loaded and evaluated.
- `PASS` Resilience alerts (timeouts/retries/circuit breaker) loaded and evaluated.
- `PASS` Readiness alert state is healthy or expected degraded with incident note.

## 6) Functional Smoke
- `PASS` One read endpoint and one write endpoint succeed with expected latency.
- `PASS` Dependency timeout simulation triggers expected resilience behavior.
- `PASS` No duplicate writes under retry scenario for idempotent path.

## 7) Closeout
- `PASS` Incident/runbook references updated if deviations occurred.
- `PASS` Deployment record includes evidence links (logs, trace screenshots, metric queries).
- `PASS` Final status approved by on-call/operator.
