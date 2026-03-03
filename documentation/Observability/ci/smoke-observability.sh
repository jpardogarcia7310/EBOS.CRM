#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "${SCRIPT_DIR}/../../.." && pwd)"
OBS_DIR="${REPO_ROOT}/documentation/Observability"
API_PROJECT="${REPO_ROOT}/EBOS.CRM.Api/EBOS.CRM.Api.csproj"
COMPOSE_FILE="${OBS_DIR}/docker-compose.observability.yml"
API_PORT="${API_PORT:-5013}"
JOB_NAME="ebos-crm-api"
CI_PROM_DIR_NAME=".ci-prometheus"
CI_PROM_DIR_PATH="${OBS_DIR}/${CI_PROM_DIR_NAME}"

API_PID=""

cleanup() {
  set +e
  if [[ -n "${API_PID}" ]] && kill -0 "${API_PID}" >/dev/null 2>&1; then
    kill "${API_PID}" >/dev/null 2>&1 || true
  fi
  (cd "${OBS_DIR}" && OBS_PROM_DIR="${OBS_PROM_DIR}" OBS_GRAFANA_DIR="${OBS_GRAFANA_DIR}" \
    docker compose -f "${COMPOSE_FILE}" down -v >/dev/null 2>&1) || true
  rm -rf "${CI_PROM_DIR_PATH}" >/dev/null 2>&1 || true
}
trap cleanup EXIT

wait_until() {
  local timeout_seconds="$1"
  local sleep_seconds="$2"
  local message="$3"
  local command="$4"
  local start_ts
  start_ts="$(date +%s)"
  while true; do
    if bash -c "${command}" >/dev/null 2>&1; then
      return 0
    fi
    if (( "$(date +%s)" - start_ts >= timeout_seconds )); then
      echo "[observability-ci] ERROR: ${message}" >&2
      exit 1
    fi
    sleep "${sleep_seconds}"
  done
}

if [[ -d "${OBS_DIR}/Prometheus" ]]; then
  SOURCE_PROM_DIR="${OBS_DIR}/Prometheus"
elif [[ -d "${OBS_DIR}/prometheus" ]]; then
  SOURCE_PROM_DIR="${OBS_DIR}/prometheus"
else
  echo "[observability-ci] ERROR: Prometheus folder not found under ${OBS_DIR}" >&2
  exit 1
fi

if [[ -d "${OBS_DIR}/Grafana" ]]; then
  OBS_GRAFANA_DIR="Grafana"
elif [[ -d "${OBS_DIR}/grafana" ]]; then
  OBS_GRAFANA_DIR="grafana"
else
  echo "[observability-ci] ERROR: Grafana folder not found under ${OBS_DIR}" >&2
  exit 1
fi

# Prepare CI-specific Prometheus config.
rm -rf "${CI_PROM_DIR_PATH}"
mkdir -p "${CI_PROM_DIR_PATH}"
cp "${SOURCE_PROM_DIR}/prometheus.yml" "${CI_PROM_DIR_PATH}/prometheus.yml"
cp "${SOURCE_PROM_DIR}/customer360-alert-rules.yml" "${CI_PROM_DIR_PATH}/customer360-alert-rules.yml"
cp "${SOURCE_PROM_DIR}/alertmanager.yml" "${CI_PROM_DIR_PATH}/alertmanager.yml"

OBS_PROM_DIR="${CI_PROM_DIR_NAME}"

echo "[observability-ci] Starting API for smoke test..."
(
  cd "${REPO_ROOT}"
  ASPNETCORE_ENVIRONMENT=Development \
  Authentication__Enabled=false \
  ASPNETCORE_URLS="http://0.0.0.0:${API_PORT}" \
  dotnet run --project "${API_PROJECT}" --no-build
) > "${OBS_DIR}/.ci-api.log" 2>&1 &
API_PID=$!

echo "[observability-ci] Waiting for API metrics..."
wait_until 180 2 "API /metrics did not become ready." \
  "curl -fsS 'http://localhost:${API_PORT}/metrics' | grep -q 'customer360_merge_total'"

echo "[observability-ci] Starting observability stack..."
(cd "${OBS_DIR}" && OBS_PROM_DIR="${OBS_PROM_DIR}" OBS_GRAFANA_DIR="${OBS_GRAFANA_DIR}" \
  docker compose -f "${COMPOSE_FILE}" up -d)

echo "[observability-ci] Waiting for Prometheus readiness..."
wait_until 180 2 "Prometheus did not become ready." \
  "curl -fsS 'http://localhost:9090/-/ready'"

QUERY="up{job=\"${JOB_NAME}\"}"
ENCODED_QUERY="$(python3 -c "import urllib.parse,sys; print(urllib.parse.quote(sys.argv[1], safe=''))" "${QUERY}")"
PROM_QUERY_URL="http://localhost:9090/api/v1/query?query=${ENCODED_QUERY}"

echo "[observability-ci] Validating exact matcher query: ${QUERY}"
query_ok=false
for _ in $(seq 1 90); do
  payload="$(curl -fsS "${PROM_QUERY_URL}" || true)"
  if [[ "${payload}" == *"\"job\":\"${JOB_NAME}\""* && "${payload}" == *"\"1\""* ]]; then
    query_ok=true
    break
  fi
  sleep 2
done

if [[ "${query_ok}" != "true" ]]; then
  echo "[observability-ci] ERROR: Prometheus query did not return up=1 for ${JOB_NAME}." >&2
  echo "[observability-ci] DEBUG: Prometheus targets payload:" >&2
  curl -fsS "http://localhost:9090/api/v1/targets" >&2 || true
  echo "[observability-ci] DEBUG: Last API logs:" >&2
  tail -n 200 "${OBS_DIR}/.ci-api.log" >&2 || true
  exit 1
fi

echo "[observability-ci] Validating alert rules group is loaded..."
wait_until 120 2 "Prometheus rules group customer360-operability was not loaded." \
  "curl -fsS 'http://localhost:9090/api/v1/rules' | grep -q 'customer360-operability'"

echo "[observability-ci] Smoke test passed."
