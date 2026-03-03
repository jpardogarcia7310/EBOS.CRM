#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "${SCRIPT_DIR}/../../.." && pwd)"
OBS_DIR="${REPO_ROOT}/documentation/Observability"
if [[ -d "${OBS_DIR}/Prometheus" ]]; then
  PROM_DIR="${OBS_DIR}/Prometheus"
elif [[ -d "${OBS_DIR}/prometheus" ]]; then
  PROM_DIR="${OBS_DIR}/prometheus"
else
  echo "[observability-ci] ERROR: Prometheus config folder not found under ${OBS_DIR}" >&2
  exit 1
fi

echo "[observability-ci] Validating Prometheus config..."
docker run --rm \
  --entrypoint promtool \
  -v "${PROM_DIR}:/etc/prometheus:ro" \
  prom/prometheus:v2.55.1 \
  check config /etc/prometheus/prometheus.yml

echo "[observability-ci] Validating Prometheus alert rules..."
docker run --rm \
  --entrypoint promtool \
  -v "${PROM_DIR}:/etc/prometheus:ro" \
  prom/prometheus:v2.55.1 \
  check rules /etc/prometheus/customer360-alert-rules.yml

echo "[observability-ci] Validating Alertmanager config..."
docker run --rm \
  --entrypoint amtool \
  -e SMTP_SMARTHOST="${SMTP_SMARTHOST:-smtp.example.com:587}" \
  -e SMTP_FROM="${SMTP_FROM:-noreply@example.com}" \
  -e SMTP_AUTH_USERNAME="${SMTP_AUTH_USERNAME:-user}" \
  -e SMTP_AUTH_PASSWORD="${SMTP_AUTH_PASSWORD:-password}" \
  -e SLACK_WEBHOOK_URL="${SLACK_WEBHOOK_URL:-https://hooks.slack.test/services/T000/B000/XXXX}" \
  -e SLACK_DEFAULT_CHANNEL="${SLACK_DEFAULT_CHANNEL:-#alerts}" \
  -e SLACK_WARNING_CHANNEL="${SLACK_WARNING_CHANNEL:-#alerts-warning}" \
  -e SLACK_CRITICAL_CHANNEL="${SLACK_CRITICAL_CHANNEL:-#alerts-critical}" \
  -e TEAMS_WARNING_WEBHOOK_URL="${TEAMS_WARNING_WEBHOOK_URL:-https://example.test/teams/warning}" \
  -e TEAMS_CRITICAL_WEBHOOK_URL="${TEAMS_CRITICAL_WEBHOOK_URL:-https://example.test/teams/critical}" \
  -e WARNING_EMAIL_TO="${WARNING_EMAIL_TO:-warning@example.com}" \
  -e CRITICAL_EMAIL_TO="${CRITICAL_EMAIL_TO:-critical@example.com}" \
  -e PAGERDUTY_ROUTING_KEY="${PAGERDUTY_ROUTING_KEY:-dummy-routing-key}" \
  -v "${PROM_DIR}:/etc/alertmanager:ro" \
  prom/alertmanager:v0.27.0 \
  check-config /etc/alertmanager/alertmanager.yml

echo "[observability-ci] Validating Grafana dashboard JSON..."
python3 "${SCRIPT_DIR}/validate-dashboard.py"

echo "[observability-ci] All observability validation checks passed."
