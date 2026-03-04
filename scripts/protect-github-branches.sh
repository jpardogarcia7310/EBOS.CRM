#!/usr/bin/env bash
set -euo pipefail

OWNER="${1:-}"
REPO="${2:-}"

if [[ -z "$OWNER" || -z "$REPO" ]]; then
  remote_url="$(git remote get-url origin)"
  if [[ "$remote_url" =~ github.com[:/]([^/]+)/([^/.]+) ]]; then
    [[ -z "$OWNER" ]] && OWNER="${BASH_REMATCH[1]}"
    [[ -z "$REPO" ]] && REPO="${BASH_REMATCH[2]}"
  fi
fi

if [[ -z "$OWNER" || -z "$REPO" ]]; then
  echo "No se pudo detectar owner/repo. Uso: ./scripts/protect-github-branches.sh <owner> <repo>" >&2
  exit 1
fi

echo "Configurando protecciones para $OWNER/$REPO ..."

main_tmp="$(mktemp)"
develop_tmp="$(mktemp)"
trap 'rm -f "$main_tmp" "$develop_tmp"' EXIT

cat > "$main_tmp" <<'JSON'
{
  "required_status_checks": {
    "strict": true,
    "contexts": ["Validate source branch is develop"]
  },
  "enforce_admins": true,
  "required_pull_request_reviews": {
    "required_approving_review_count": 1,
    "dismiss_stale_reviews": true,
    "require_code_owner_reviews": true,
    "require_last_push_approval": false
  },
  "restrictions": null,
  "required_linear_history": true,
  "allow_force_pushes": false,
  "allow_deletions": false,
  "block_creations": false,
  "required_conversation_resolution": true,
  "lock_branch": false,
  "allow_fork_syncing": true
}
JSON

gh api --method PUT "repos/$OWNER/$REPO/branches/main/protection" \
  --header "Accept: application/vnd.github+json" \
  --input "$main_tmp" >/dev/null

cat > "$develop_tmp" <<'JSON'
{
  "required_status_checks": null,
  "enforce_admins": true,
  "required_pull_request_reviews": null,
  "restrictions": null,
  "required_linear_history": false,
  "allow_force_pushes": false,
  "allow_deletions": false,
  "block_creations": false,
  "required_conversation_resolution": false,
  "lock_branch": false,
  "allow_fork_syncing": true
}
JSON

gh api --method PUT "repos/$OWNER/$REPO/branches/develop/protection" \
  --header "Accept: application/vnd.github+json" \
  --input "$develop_tmp" >/dev/null

# Evita el borrado automatico de ramas origen al mergear PRs
gh api --method PATCH "repos/$OWNER/$REPO" \
  --header "Accept: application/vnd.github+json" \
  --field delete_branch_on_merge=false >/dev/null

echo "Protección aplicada:"
echo "- main: solo PR + check obligatorio + sin borrado"
echo "- develop: sin borrado + sin force-push"
echo "- repo: auto-delete de rama origen desactivado"
