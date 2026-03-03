#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "${SCRIPT_DIR}/../../.." && pwd)"
OBS_DIR="${REPO_ROOT}/documentation/Observability"
API_PROJECT="${REPO_ROOT}/EBOS.CRM.Api/EBOS.CRM.Api.csproj"
API_PROJECT_REL="EBOS.CRM.Api/EBOS.CRM.Api.csproj"
COMPOSE_FILE="${OBS_DIR}/docker-compose.observability.yml"
API_PORT="${API_PORT:-5013}"
JOB_NAME="ebos-crm-api"
CI_PROM_DIR_NAME=".ci-prometheus"
CI_PROM_DIR_PATH="${OBS_DIR}/${CI_PROM_DIR_NAME}"
API_CONTAINER_NAME="ebos-crm-api-smoke"
SQL_CONTAINER_NAME="ebos-crm-sql-smoke"
COMPOSE_NETWORK_NAME="observability_default"
SQL_SA_PASSWORD="${SQL_SA_PASSWORD:-StrongP@ssw0rd123!}"

API_PID=""

cleanup() {
  set +e
  docker rm -f "${API_CONTAINER_NAME}" >/dev/null 2>&1 || true
  docker rm -f "${SQL_CONTAINER_NAME}" >/dev/null 2>&1 || true
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

# Ensure Prometheus scrapes the API container running inside compose network.
sed -i "/targets:/a\\          - \"${API_CONTAINER_NAME}:${API_PORT}\"" "${CI_PROM_DIR_PATH}/prometheus.yml"

OBS_PROM_DIR="${CI_PROM_DIR_NAME}"

echo "[observability-ci] Starting alertmanager+grafana..."
(cd "${OBS_DIR}" && OBS_PROM_DIR="${OBS_PROM_DIR}" OBS_GRAFANA_DIR="${OBS_GRAFANA_DIR}" \
  docker compose -f "${COMPOSE_FILE}" up -d alertmanager grafana)

echo "[observability-ci] Starting SQL Server container..."
docker run -d \
  --name "${SQL_CONTAINER_NAME}" \
  --network "${COMPOSE_NETWORK_NAME}" \
  -e ACCEPT_EULA=Y \
  -e MSSQL_SA_PASSWORD="${SQL_SA_PASSWORD}" \
  -e MSSQL_PID=Developer \
  mcr.microsoft.com/mssql/server:2022-latest > /dev/null

echo "[observability-ci] Waiting for SQL Server readiness..."
sql_ready=false
for _ in $(seq 1 120); do
  if docker logs "${SQL_CONTAINER_NAME}" 2>&1 | grep -q "SQL Server is now ready for client connections"; then
    sql_ready=true
    break
  fi
  sleep 2
done

if [[ "${sql_ready}" != "true" ]]; then
  echo "[observability-ci] ERROR: SQL Server did not become ready." >&2
  docker logs "${SQL_CONTAINER_NAME}" --tail 200 >&2 || true
  exit 1
fi

echo "[observability-ci] Starting API container inside compose network..."
docker run -d \
  --name "${API_CONTAINER_NAME}" \
  --network "${COMPOSE_NETWORK_NAME}" \
  -e ASPNETCORE_ENVIRONMENT=Development \
  -e Authentication__Enabled=false \
  -e ASPNETCORE_URLS="http://0.0.0.0:${API_PORT}" \
  -e ConnectionStrings__CrmConnection="Server=${SQL_CONTAINER_NAME},1433;Database=crm_smoke;User Id=sa;Password=${SQL_SA_PASSWORD};TrustServerCertificate=true;Encrypt=false;" \
  -v "${REPO_ROOT}:/src" \
  -w /src \
  mcr.microsoft.com/dotnet/sdk:8.0 \
  bash -lc "dotnet run --project ${API_PROJECT_REL}" > "${OBS_DIR}/.ci-api.log" 2>&1

echo "[observability-ci] Waiting for API metrics from compose network..."
api_ready=false
for _ in $(seq 1 180); do
  if docker run --rm --network "${COMPOSE_NETWORK_NAME}" curlimages/curl:8.10.1 -fsS \
      "http://${API_CONTAINER_NAME}:${API_PORT}/metrics" | grep -q 'customer360_merge_total'; then
    api_ready=true
    break
  fi
  sleep 2
done

if [[ "${api_ready}" != "true" ]]; then
  echo "[observability-ci] ERROR: API /metrics did not become ready from compose network." >&2
  echo "[observability-ci] DEBUG: API container state:" >&2
  docker ps -a --filter "name=${API_CONTAINER_NAME}" >&2 || true
  echo "[observability-ci] DEBUG: Last API logs:" >&2
  docker logs "${API_CONTAINER_NAME}" --tail 300 >&2 || true
  exit 1
fi

echo "[observability-ci] Starting prometheus..."
(cd "${OBS_DIR}" && OBS_PROM_DIR="${OBS_PROM_DIR}" OBS_GRAFANA_DIR="${OBS_GRAFANA_DIR}" \
  docker compose -f "${COMPOSE_FILE}" up -d prometheus)

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
  docker logs "${API_CONTAINER_NAME}" --tail 200 >&2 || true
  exit 1
fi

echo "[observability-ci] Validating alert rules group is loaded..."
wait_until 120 2 "Prometheus rules group customer360-operability was not loaded." \
  "curl -fsS 'http://localhost:9090/api/v1/rules' | grep -q 'customer360-operability'"

echo "[observability-ci] Smoke test passed."
