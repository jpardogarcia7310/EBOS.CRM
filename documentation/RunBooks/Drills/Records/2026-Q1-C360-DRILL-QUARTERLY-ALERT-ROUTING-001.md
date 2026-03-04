# Customer 360 Drill Record

## Drill Metadata
- Drill ID: `C360-DRILL-QUARTERLY-ALERT-ROUTING-001`
- Date/Time (UTC): `2026-03-04T00:00:00Z`
- Environment: `CI observability validation + docker compose stack`
- Operator(s): `CRM Platform Team`
- Reviewer: `jpardogarcia7310`
- Frequency: `quarterly`
- Drill type: `alert routing warning+critical`

## Scope and Objective
- Objective: Verify Prometheus rules, Alertmanager routing config, and observability smoke path.
- In-scope components: `prometheus.yml`, `customer360-alert-rules.yml`, `alertmanager.yml`, Grafana provisioning.
- Out-of-scope components: external provider delivery proof (Slack/Teams/Email/PagerDuty in CI).

## Preconditions
- Runbook used: `documentation/RunBooks/Customer360-Operability-RunBook.md`
- Required access verified: Yes
- Feature flags/configuration: `.env.alerting` placeholders for CI-safe validation.
- Data/tenant setup: job matcher `job="ebos-crm-api"` exact.

## Execution Steps
1. Run `documentation/Observability/ci/validate-observability.sh`.
2. Run `documentation/Observability/ci/smoke-observability.sh`.
3. Verify Prometheus ready state and `up{job="ebos-crm-api"}` query success.

## Detection and Response
- Detection source: script outputs and non-zero exit codes.
- Time to detect (minutes): `<= 2`
- Response actions: path fixes, config rendering fixes, exact job matcher enforcement, smoke stabilization.
- Escalation needed: `No`

## Recovery and Validation
- Time to recover (minutes): `~45`
- RTO target met: `PASS`
- RPO target met: `PASS`
- Functional validation done: yes (config validation + smoke stack + rule load checks).
- Business impact summary: observability CI gate stabilized.

## Evidence
- CI run / pipeline URL: observability validation jobs/workflow steps.
- Prometheus queries/results: `up{job="ebos-crm-api"}`.
- Grafana screenshots: dashboard JSON/provisioning validated.
- Alert notifications: routing config validated syntactically and structurally.
- Logs/traces: `validate-observability.sh` and `smoke-observability.sh` outputs.
- Related tickets: `N/A`

## Lessons Learned and Actions
- What worked: strict CI validators and exact matcher strategy.
- What failed: initial path/mount/config drift and environment rendering pitfalls.
- Action items:
  - Owner: `CRM Platform Team`
  - Due date: `2026-03-31`
  - Status: `Open`
