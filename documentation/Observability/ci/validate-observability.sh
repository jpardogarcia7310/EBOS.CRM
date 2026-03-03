#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "${SCRIPT_DIR}/../../.." && pwd)"
OBS_DIR="${REPO_ROOT}/documentation/Observability"
PROM_DIR="${OBS_DIR}/Prometheus"

echo "[observability-ci] Validating Prometheus config..."
docker run --rm \
  --entrypoint promtool \
  -v "${PROM_DIR}:/work:ro" \
  prom/prometheus:v2.55.1 \
  check config /work/prometheus.yml

echo "[observability-ci] Validating Prometheus alert rules..."
docker run --rm \
  --entrypoint promtool \
  -v "${PROM_DIR}:/work:ro" \
  prom/prometheus:v2.55.1 \
  check rules /work/customer360-alert-rules.yml

echo "[observability-ci] Validating Alertmanager config..."
docker run --rm \
  --entrypoint amtool \
  -v "${PROM_DIR}:/work:ro" \
  prom/alertmanager:v0.27.0 \
  check-config /work/alertmanager.yml

echo "[observability-ci] Validating Grafana dashboard JSON..."
python3 "${SCRIPT_DIR}/validate-dashboard.py"

echo "[observability-ci] All observability validation checks passed."
