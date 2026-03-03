# Customer 360 Post-Deploy Checklist

Use this checklist after each deployment in staging/production.
Mark each line with `PASS`, `FAIL`, or `N/A` and attach evidence.

## 1) Platform and API
- API process is running and stable for 10+ minutes.
- `GET /health/live` returns `200`.
- `GET /health/ready` returns `200` (or expected `503` with documented reason).
- No startup migration errors in API logs.

## 2) Security and Access
- `/metrics` is not publicly exposed without required auth/policy in target environment.
- Operational endpoints require policy and return expected `401/403/200`.
- Tenant header/subdomain resolution still works for Customer 360 endpoints.

## 3) Customer 360 Functional Smoke
- Dedupe query endpoint responds successfully.
- Merge command endpoint responds successfully (or controlled business validation).
- Consent add/revoke endpoints respond successfully.
- Privacy request register/execute endpoints respond successfully.

## 4) Outbox and Concurrency
- `OperationalReadiness/dashboard` shows expected outbox pending/failed values.
- `OperationalReadiness/alerts` does not show unexpected critical flags.
- Concurrency failures are within normal baseline.

## 5) Observability
- Prometheus target `up{job="ebos-crm-api"}` is `1`.
- Rules group `customer360-operability` is loaded in Prometheus.
- Grafana dashboard `Customer360 Operability` loads without datasource errors.
- At least one metric point is visible for:
  - `customer360_merge_total`
  - `customer360_audit_outbox_total`
  - `customer360_concurrency_total`

## 6) Alert Routing
- Warning test alert is routed to expected channel.
- Critical test alert is routed to expected channel.
- Alert resolve notifications are delivered.

## 7) Closeout
- Incident/runbook references updated if deviations occurred.
- Deployment ticket includes all evidence links.
- Final status approved by on-call/operator.

