# Customer 360 Observability Assets

This folder contains baseline observability assets for PR-6:

- Grafana dashboard JSON:
  - `grafana/customer360-operability-dashboard.json`
- Prometheus alert rules:
  - `prometheus/customer360-alert-rules.yml`
- Ready-to-use baseline config:
  - `prometheus/prometheus.yml`
  - `prometheus/alertmanager.yml`
  - `.env.alerting`
  - `docker-compose.observability.yml`
  - `grafana/provisioning/datasources/datasource.yml`
  - `grafana/provisioning/dashboards/dashboards.yml`

## 100% closed state (exact matcher)

Everything is pinned to the exact matcher:

- `job="ebos-crm-api"`

No regex and no job variable are used in dashboard or alerts.

## Expected metrics

- `customer360_merge_total`
- `customer360_dedupe_query_total`
- `customer360_consent_event_total`
- `customer360_audit_outbox_total`
- `customer360_concurrency_total`

## Is Prometheus embedded in the API?

No. Prometheus runs as an independent service and scrapes the API endpoint:

- `http://<api-host>:<port>/metrics`

The API in this repo already exposes `/metrics`.
For distributed tracing, enable OpenTelemetry in `EBOS.CRM.Api/appsettings*.json`:
`OpenTelemetry:Enabled=true` and set `OpenTelemetry:OtlpEndpoint` (for example `http://localhost:4317`).

## Quick local startup (Docker)

1. Start the API locally with `http` profile (`http://localhost:5013`).
2. `prometheus/prometheus.yml` is already preconfigured with `host.docker.internal:5013`.
3. From `documentation/Observability`, run:

```bash
docker compose -f docker-compose.observability.yml up -d
```

Before first startup, update `.env.alerting` with real credentials
(SMTP/Slack/Teams/PagerDuty) to enable real alert routing.

4. Open:
- Prometheus: `http://localhost:9090`
- Alertmanager: `http://localhost:9093`
- Grafana: `http://localhost:3000` (admin/admin)

5. Import dashboard:
- Manual import is not needed anymore. Grafana provisions it automatically on startup.

## Minimal verification

1. In Prometheus, query:

```promql
up{job="ebos-crm-api"}
```

This should return `1`.

2. Check one Customer 360 metric:

```promql
sum(rate(customer360_merge_total{job="ebos-crm-api"}[5m]))
```

3. Check rules loaded:
- Prometheus > `Status` > `Rules`, group `customer360-operability`.

## Production

- Keep exactly `job_name: ebos-crm-api` in `scrape_config`.
- If you use Kubernetes/ServiceMonitor, relabel so final `job` label is `ebos-crm-api`.
- If you change this job name, you must update dashboard and alert rules.
- `docker-compose.observability.yml` already includes persistent volumes:
  - `prometheus_data`
  - `alertmanager_data`
  - `grafana_data`

## Recommended Next Steps

1. Fill real values in `.env.alerting`.
2. Start the stack:

```bash
docker compose -f documentation/Observability/docker-compose.observability.yml up -d
```

3. Verify in Prometheus:

```promql
up{job="ebos-crm-api"}
```

## Alert routing (severity)

`prometheus/alertmanager.yml` routes by `severity`:

- `critical`:
  - PagerDuty
  - Slack (critical channel)
  - Teams (critical webhook)
  - Critical email
- `warning`:
  - Slack (warning channel)
  - Teams (warning webhook)
  - Warning email

## Operations Follow-up

- Runbook:
  - `documentation/RunBooks/Customer360-Operability-RunBook.md`
- Mandatory post-deploy checklist:
  - `documentation/RunBooks/Customer360-PostDeploy-Checklist.md`
- Recommended drill cadence:
  - Monthly outbox failure/recovery drill
  - Quarterly migration+rollback drill
  - Quarterly alert-routing drill
- CI workflow:
  - GitHub Actions workflow `Observability CI` can run on PR/push and manually (`workflow_dispatch`).
