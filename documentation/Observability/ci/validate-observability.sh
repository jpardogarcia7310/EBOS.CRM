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
SMTP_SMARTHOST="${SMTP_SMARTHOST:-smtp.example.com:587}"
SMTP_FROM="${SMTP_FROM:-noreply@example.com}"
SMTP_AUTH_USERNAME="${SMTP_AUTH_USERNAME:-user}"
SMTP_AUTH_PASSWORD="${SMTP_AUTH_PASSWORD:-password}"
SLACK_WEBHOOK_URL="${SLACK_WEBHOOK_URL:-https://hooks.slack.test/services/T000/B000/XXXX}"
SLACK_DEFAULT_CHANNEL="${SLACK_DEFAULT_CHANNEL:-#alerts}"
SLACK_WARNING_CHANNEL="${SLACK_WARNING_CHANNEL:-#alerts-warning}"
SLACK_CRITICAL_CHANNEL="${SLACK_CRITICAL_CHANNEL:-#alerts-critical}"
TEAMS_WARNING_WEBHOOK_URL="${TEAMS_WARNING_WEBHOOK_URL:-https://example.test/teams/warning}"
TEAMS_CRITICAL_WEBHOOK_URL="${TEAMS_CRITICAL_WEBHOOK_URL:-https://example.test/teams/critical}"
WARNING_EMAIL_TO="${WARNING_EMAIL_TO:-warning@example.com}"
CRITICAL_EMAIL_TO="${CRITICAL_EMAIL_TO:-critical@example.com}"
PAGERDUTY_ROUTING_KEY="${PAGERDUTY_ROUTING_KEY:-dummy-routing-key}"

tmp_dir="$(mktemp -d)"
trap 'rm -rf "${tmp_dir}"' EXIT
rendered_alertmanager="${tmp_dir}/alertmanager.rendered.yml"
cp "${PROM_DIR}/alertmanager.yml" "${rendered_alertmanager}"
chmod 755 "${tmp_dir}"
chmod 644 "${rendered_alertmanager}"

replace_var() {
  local key="$1"
  local value="$2"
  local escaped
  escaped="$(printf '%s' "${value}" | sed -e 's/[\/&]/\\&/g')"
  sed -i "s|\${${key}}|${escaped}|g" "${rendered_alertmanager}"
}

replace_var "SMTP_SMARTHOST" "${SMTP_SMARTHOST}"
replace_var "SMTP_FROM" "${SMTP_FROM}"
replace_var "SMTP_AUTH_USERNAME" "${SMTP_AUTH_USERNAME}"
replace_var "SMTP_AUTH_PASSWORD" "${SMTP_AUTH_PASSWORD}"
replace_var "SLACK_WEBHOOK_URL" "${SLACK_WEBHOOK_URL}"
replace_var "SLACK_DEFAULT_CHANNEL" "${SLACK_DEFAULT_CHANNEL}"
replace_var "SLACK_WARNING_CHANNEL" "${SLACK_WARNING_CHANNEL}"
replace_var "SLACK_CRITICAL_CHANNEL" "${SLACK_CRITICAL_CHANNEL}"
replace_var "TEAMS_WARNING_WEBHOOK_URL" "${TEAMS_WARNING_WEBHOOK_URL}"
replace_var "TEAMS_CRITICAL_WEBHOOK_URL" "${TEAMS_CRITICAL_WEBHOOK_URL}"
replace_var "WARNING_EMAIL_TO" "${WARNING_EMAIL_TO}"
replace_var "CRITICAL_EMAIL_TO" "${CRITICAL_EMAIL_TO}"
replace_var "PAGERDUTY_ROUTING_KEY" "${PAGERDUTY_ROUTING_KEY}"

docker run --rm \
  --entrypoint amtool \
  -v "${tmp_dir}:/work:ro" \
  prom/alertmanager:v0.27.0 \
  check-config /work/alertmanager.rendered.yml

echo "[observability-ci] Validating Grafana dashboard JSON..."
python3 "${SCRIPT_DIR}/validate-dashboard.py"

echo "[observability-ci] All observability validation checks passed."
