#!/usr/bin/env bash
set -euo pipefail

OWNER="${1:-}"
REPO="${2:-}"
BRANCHES=("main" "master")

if ! command -v gh >/dev/null 2>&1; then
  echo "GitHub CLI (gh) no está instalado." >&2
  exit 1
fi

if [[ -z "$OWNER" || -z "$REPO" ]]; then
  NAME_WITH_OWNER="$(gh repo view --json nameWithOwner -q .nameWithOwner)"
  OWNER="${NAME_WITH_OWNER%%/*}"
  REPO="${NAME_WITH_OWNER##*/}"
fi

PAYLOAD_FILE="$(mktemp)"
cat > "$PAYLOAD_FILE" <<'JSON'
{
  "required_status_checks": {
    "strict": true,
    "contexts": [
      "API suite",
      "Integration suite",
      "Concurrency suite",
      "Stress suite",
      "Test summary"
    ]
  },
  "enforce_admins": true,
  "required_pull_request_reviews": {
    "dismiss_stale_reviews": true,
    "require_code_owner_reviews": false,
    "required_approving_review_count": 1
  },
  "restrictions": null,
  "required_linear_history": true,
  "allow_force_pushes": false,
  "allow_deletions": false,
  "block_creations": false,
  "required_conversation_resolution": true,
  "lock_branch": false,
  "allow_fork_syncing": false
}
JSON

for BRANCH in "${BRANCHES[@]}"; do
  echo "Configuring protection: ${OWNER}/${REPO}:${BRANCH}"
  if ! gh api "repos/${OWNER}/${REPO}/branches/${BRANCH}" --silent >/dev/null 2>&1; then
    echo "WARN: branch '${BRANCH}' no existe. Se omite."
    continue
  fi
  gh api -X PUT "repos/${OWNER}/${REPO}/branches/${BRANCH}/protection" --input "$PAYLOAD_FILE" >/dev/null
  echo "OK: ${BRANCH}"
done

rm -f "$PAYLOAD_FILE"

echo
echo "Current protection (main):"
gh api "repos/${OWNER}/${REPO}/branches/main/protection" --jq '.required_status_checks.contexts'
