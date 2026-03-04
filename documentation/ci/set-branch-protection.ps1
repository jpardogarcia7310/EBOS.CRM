param(
    [string]$Owner = "",
    [string]$Repo = "",
    [string[]]$Branches = @("main", "master")
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

if (-not (Get-Command gh -ErrorAction SilentlyContinue)) {
    throw "GitHub CLI (gh) no está instalado."
}

if ([string]::IsNullOrWhiteSpace($Owner) -or [string]::IsNullOrWhiteSpace($Repo)) {
    $nameWithOwner = gh repo view --json nameWithOwner -q .nameWithOwner
    if ([string]::IsNullOrWhiteSpace($nameWithOwner)) {
        throw "No se pudo resolver owner/repo automáticamente."
    }
    $parts = $nameWithOwner.Split("/")
    $Owner = $parts[0]
    $Repo = $parts[1]
}

$contexts = @(
    "API suite",
    "Integration suite",
    "Concurrency suite",
    "Stress suite",
    "Test summary"
)

$payload = @{
    required_status_checks = @{
        strict   = $true
        contexts = $contexts
    }
    enforce_admins                  = $true
    required_pull_request_reviews   = @{
        dismiss_stale_reviews           = $true
        require_code_owner_reviews      = $false
        required_approving_review_count = 1
    }
    restrictions                    = $null
    required_linear_history         = $true
    allow_force_pushes              = $false
    allow_deletions                 = $false
    block_creations                 = $false
    required_conversation_resolution = $true
    lock_branch                     = $false
    allow_fork_syncing              = $false
}

$json = $payload | ConvertTo-Json -Depth 10

foreach ($branch in $Branches) {
    Write-Host "Configuring protection: $Owner/$Repo:$branch"
    try {
        gh api "repos/$Owner/$Repo/branches/$branch" --silent | Out-Null
    }
    catch {
        Write-Warning "Branch '$branch' no existe. Se omite."
        continue
    }

    $tmp = New-TemporaryFile
    Set-Content -Path $tmp -Value $json -Encoding UTF8
    gh api -X PUT "repos/$Owner/$Repo/branches/$branch/protection" --input $tmp | Out-Null
    Remove-Item $tmp -Force
    Write-Host "OK: $branch"
}

Write-Host ""
Write-Host "Current protection (main):"
gh api "repos/$Owner/$Repo/branches/main/protection" --jq '.required_status_checks.contexts'
