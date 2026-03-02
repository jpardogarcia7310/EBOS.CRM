# Customer 360 Observability Assets

This folder contains baseline observability assets for PR-6:

- Grafana dashboard JSON:
  - `grafana/customer360-operability-dashboard.json`
- Prometheus alert rules:
  - `prometheus/customer360-alert-rules.yml`

## Metric Names

The dashboard and alert rules assume OpenTelemetry-to-Prometheus naming using underscores:

- `customer360_merge_total`
- `customer360_dedupe_query_total`
- `customer360_consent_event_total`
- `customer360_audit_outbox_total`
- `customer360_concurrency_total`

If your Prometheus pipeline exposes different names, adjust the `expr` in the rules and panel queries.

## Labels used in queries

- `job` (Prometheus scrape job for this API)
- `instance`
- Optional attributes emitted by the app:
  - `tenant_id`
  - `operation`
  - `event`
  - `success`
  - `exhausted_retries`

## Production adaptation applied

- Dashboard queries are filtered by Grafana variable `job` (`job=~"$job"`).
- Alert rules include a production-oriented matcher:
  - `job=~"(?i).*(ebos.*crm.*api|crm.*api|ebos.*crm).*"`

If your scrape `job_name` differs, adjust this matcher in:
- `prometheus/customer360-alert-rules.yml`

## Import/Apply

1. Import dashboard JSON into Grafana.
2. Create/update a Prometheus rule file with `prometheus/customer360-alert-rules.yml`.
3. Reload Prometheus rule config.
4. Verify in Grafana:
   - dashboard panels show data
   - alert states are visible in Alertmanager/Grafana Alerting.
