param(
  [string]$Owner,
  [string]$Repo
)

$ErrorActionPreference = 'Stop'

if (-not $Owner -or -not $Repo) {
  $remote = git remote get-url origin
  if ($remote -match 'github\.com[:/](?<owner>[^/]+)/(?<repo>[^/.]+)') {
    if (-not $Owner) { $Owner = $Matches.owner }
    if (-not $Repo) { $Repo = $Matches.repo }
  }
}

if (-not $Owner -or -not $Repo) {
  throw "No se pudo detectar owner/repo. Ejemplo: .\\scripts\\Protect-GitHubBranches.ps1 -Owner tu-org -Repo tu-repo"
}

Write-Host "Configurando protecciones para $Owner/$Repo ..."

# main: solo PR, sin push directo, sin borrado, con check obligatorio del workflow
$mainProtection = @{
  required_status_checks = @{
    strict   = $true
    contexts = @('Validate source branch is develop')
  }
  enforce_admins = $true
  required_pull_request_reviews = @{
    required_approving_review_count = 1
    dismiss_stale_reviews            = $true
    require_code_owner_reviews       = $true
    require_last_push_approval       = $false
  }
  restrictions = $null
  required_linear_history = $true
  allow_force_pushes = $false
  allow_deletions = $false
  block_creations = $false
  required_conversation_resolution = $true
  lock_branch = $false
  allow_fork_syncing = $true
}

$mainJson = $mainProtection | ConvertTo-Json -Depth 10 -Compress
$mainTmp = [System.IO.Path]::GetTempFileName()
$mainJson | Set-Content -Path $mainTmp -Encoding utf8

gh api --method PUT "repos/$Owner/$Repo/branches/main/protection" `
  --header "Accept: application/vnd.github+json" `
  --input $mainTmp | Out-Null

# develop: no borrado, no force-push
$developProtection = @{
  required_status_checks = $null
  enforce_admins = $true
  required_pull_request_reviews = $null
  restrictions = $null
  required_linear_history = $false
  allow_force_pushes = $false
  allow_deletions = $false
  block_creations = $false
  required_conversation_resolution = $false
  lock_branch = $false
  allow_fork_syncing = $true
}

$developJson = $developProtection | ConvertTo-Json -Depth 10 -Compress
$developTmp = [System.IO.Path]::GetTempFileName()
$developJson | Set-Content -Path $developTmp -Encoding utf8

gh api --method PUT "repos/$Owner/$Repo/branches/develop/protection" `
  --header "Accept: application/vnd.github+json" `
  --input $developTmp | Out-Null

# Evita el borrado automatico de ramas origen al mergear PRs
gh api --method PATCH "repos/$Owner/$Repo" `
  --header "Accept: application/vnd.github+json" `
  --field delete_branch_on_merge=false | Out-Null

Remove-Item $mainTmp,$developTmp -Force

Write-Host "Protección aplicada:"
Write-Host "- main: solo PR + check obligatorio + sin borrado"
Write-Host "- develop: sin borrado + sin force-push"
Write-Host "- repo: auto-delete de rama origen desactivado"
