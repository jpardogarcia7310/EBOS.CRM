#!/usr/bin/env bash
set -euo pipefail

API_PORT="${API_PORT:-5013}"
JOB_NAME="${JOB_NAME:-ebos-crm-api}"
SKIP_API_START="${SKIP_API_START:-false}"
SKIP_COMPOSE="${SKIP_COMPOSE:-false}"
NO_WAIT="${NO_WAIT:-false}"

while [[ $# -gt 0 ]]; do
  case "$1" in
    --api-port)
      API_PORT="$2"
      shift 2
      ;;
    --job-name)
      JOB_NAME="$2"
      shift 2
      ;;
    --skip-api-start)
      SKIP_API_START=true
      shift
      ;;
    --skip-compose)
      SKIP_COMPOSE=true
      shift
      ;;
    --no-wait)
      NO_WAIT=true
      shift
      ;;
    *)
      echo "[observability] Unknown argument: $1" >&2
      exit 1
      ;;
  esac
done

log() {
  echo "[observability] $*"
}

wait_until() {
  local timeout_seconds="$1"
  local sleep_seconds="$2"
  local timeout_message="$3"
  local start_ts
  start_ts="$(date +%s)"

  while true; do
    if "$4"; then
      return 0
    fi

    if (( "$(date +%s)" - start_ts >= timeout_seconds )); then
      echo "[observability] $timeout_message" >&2
      exit 1
    fi

    sleep "$sleep_seconds"
  done
}

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "${SCRIPT_DIR}/../.." && pwd)"
API_PROJECT="${REPO_ROOT}/EBOS.CRM.Api/EBOS.CRM.Api.csproj"
COMPOSE_FILE="${SCRIPT_DIR}/docker-compose.observability.yml"
PID_FILE="${SCRIPT_DIR}/.observability-api.pid"

API_BASE_URL="http://localhost:${API_PORT}"
METRICS_URL="${API_BASE_URL}/metrics"
PROM_BASE_URL="http://localhost:9090"
PROM_READY_URL="${PROM_BASE_URL}/-/ready"
PROM_QUERY_URL="${PROM_BASE_URL}/api/v1/query"

if [[ "${SKIP_API_START}" != "true" ]]; then
  log "Starting API using launch profile 'http'..."
  (
    cd "${REPO_ROOT}"
    nohup dotnet run --project "${API_PROJECT}" --launch-profile http --no-build > "${SCRIPT_DIR}/.observability-api.log" 2>&1 &
    echo $! > "${PID_FILE}"
  )
  log "API process started with PID $(cat "${PID_FILE}")."
else
  log "Skipping API start (requested)."
fi

if [[ "${SKIP_COMPOSE}" != "true" ]]; then
  log "Starting Prometheus/Alertmanager/Grafana with docker compose..."
  docker compose -f "${COMPOSE_FILE}" up -d
else
  log "Skipping docker compose up (requested)."
fi

if [[ "${NO_WAIT}" == "true" ]]; then
  log "NoWait enabled. Startup commands finished."
  exit 0
fi

log "Waiting for API metrics endpoint: ${METRICS_URL}"
wait_until 180 2 "API metrics endpoint did not become ready." bash -c \
  "curl -fsS '${METRICS_URL}' | grep -q 'customer360_merge_total'"

log "Waiting for Prometheus readiness: ${PROM_READY_URL}"
wait_until 180 2 "Prometheus did not become ready." bash -c \
  "curl -fsS '${PROM_READY_URL}' >/dev/null"

QUERY="up{job=\"${JOB_NAME}\"}"
ENCODED_QUERY="$(python3 -c 'import urllib.parse,sys; print(urllib.parse.quote(sys.argv[1], safe=""))' "${QUERY}")"
PROM_UP_QUERY_URL="${PROM_QUERY_URL}?query=${ENCODED_QUERY}"

log "Validating exact matcher query in Prometheus: ${QUERY}"
wait_until 180 2 "Prometheus query did not return up=1 for job '${JOB_NAME}'." bash -c \
  "curl -fsS '${PROM_UP_QUERY_URL}' | grep -q '\"job\":\"${JOB_NAME}\"' && curl -fsS '${PROM_UP_QUERY_URL}' | grep -q '\"1\"'"

log "OK: observability stack is running and query up{job=\"${JOB_NAME}\"} returns 1."
log "Prometheus: ${PROM_BASE_URL} | Alertmanager: http://localhost:9093 | Grafana: http://localhost:3000"
