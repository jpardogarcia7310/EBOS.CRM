#!/usr/bin/env python3
import json
import re
import sys
from pathlib import Path


def fail(message: str) -> None:
    print(f"[observability-ci] ERROR: {message}", file=sys.stderr)
    sys.exit(1)


repo_root = Path(__file__).resolve().parents[3]
obs_root = repo_root / "documentation" / "Observability"
candidate_paths = [
    obs_root / "Grafana" / "customer360-operability-dashboard.json",
    obs_root / "grafana" / "customer360-operability-dashboard.json",
]
dashboard_path = next((path for path in candidate_paths if path.exists()), candidate_paths[0])

if not dashboard_path.exists():
    fail(f"Dashboard file not found: {dashboard_path}")

try:
    dashboard = json.loads(dashboard_path.read_text(encoding="utf-8"))
except Exception as ex:
    fail(f"Invalid dashboard JSON: {ex}")

panels = dashboard.get("panels", [])
if not isinstance(panels, list) or len(panels) == 0:
    fail("Dashboard has no panels.")

expressions = []
for panel in panels:
    targets = panel.get("targets", [])
    if not isinstance(targets, list):
        continue
    for target in targets:
        expr = target.get("expr")
        if isinstance(expr, str) and expr.strip():
            expressions.append(expr)

if not expressions:
    fail("Dashboard has no PromQL expressions.")

expected_metrics = [
    "customer360_merge_total",
    "customer360_dedupe_query_total",
    "customer360_consent_event_total",
    "customer360_audit_outbox_total",
    "customer360_concurrency_total",
]

for metric in expected_metrics:
    if not any(metric in expr for expr in expressions):
        fail(f"Expected metric '{metric}' is not used in dashboard expressions.")

if any(re.search(r'job\s*=~', expr) for expr in expressions):
    fail("Dashboard contains regex matcher for job label (job=~...). Use exact matcher.")

print("[observability-ci] Dashboard JSON validation passed.")
