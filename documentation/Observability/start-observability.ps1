param(
    [int]$ApiPort = 5013,
    [string]$JobName = "ebos-crm-api",
    [switch]$SkipApiStart,
    [switch]$SkipCompose,
    [switch]$NoWait
)

$ErrorActionPreference = "Stop"

function Write-Step([string]$message) {
    Write-Host "[observability] $message"
}

function Wait-Until {
    param(
        [scriptblock]$Condition,
        [int]$TimeoutSeconds = 120,
        [int]$SleepSeconds = 2,
        [string]$TimeoutMessage = "Timeout waiting for condition."
    )

    $deadline = (Get-Date).AddSeconds($TimeoutSeconds)
    while ((Get-Date) -lt $deadline) {
        try {
            if (& $Condition) {
                return $true
            }
        }
        catch {
            # keep waiting
        }
        Start-Sleep -Seconds $SleepSeconds
    }

    throw $TimeoutMessage
}

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = Resolve-Path (Join-Path $scriptDir "..\..")
$apiProject = Join-Path $repoRoot "EBOS.CRM.Api\EBOS.CRM.Api.csproj"
$composeFile = Join-Path $scriptDir "docker-compose.observability.yml"
$pidFile = Join-Path $scriptDir ".observability-api.pid"

$apiBaseUrl = "http://localhost:$ApiPort"
$metricsUrl = "$apiBaseUrl/metrics"
$promBaseUrl = "http://localhost:9090"
$promReadyUrl = "$promBaseUrl/-/ready"
$promQueryUrl = "$promBaseUrl/api/v1/query"
$query = "up{job=`"$JobName`"}"
$encodedQuery = [System.Uri]::EscapeDataString($query)
$promUpQueryUrl = "$promQueryUrl?query=$encodedQuery"

if (-not $SkipApiStart) {
    Write-Step "Starting API using launch profile 'http'..."
    $apiProcess = Start-Process -FilePath "dotnet" `
        -ArgumentList @("run", "--project", $apiProject, "--launch-profile", "http", "--no-build") `
        -WorkingDirectory $repoRoot `
        -PassThru

    Set-Content -Path $pidFile -Value $apiProcess.Id
    Write-Step "API process started with PID $($apiProcess.Id)."
}
else {
    Write-Step "Skipping API start (requested)."
}

if (-not $SkipCompose) {
    Write-Step "Starting Prometheus/Alertmanager/Grafana with docker compose..."
    & docker compose -f $composeFile up -d
    if ($LASTEXITCODE -ne 0) {
        throw "docker compose up failed."
    }
}
else {
    Write-Step "Skipping docker compose up (requested)."
}

if ($NoWait) {
    Write-Step "NoWait enabled. Startup commands finished."
    exit 0
}

Write-Step "Waiting for API metrics endpoint: $metricsUrl"
Wait-Until -TimeoutSeconds 180 -TimeoutMessage "API metrics endpoint did not become ready." -Condition {
    $response = Invoke-WebRequest -Uri $metricsUrl -UseBasicParsing -TimeoutSec 5
    return $response.StatusCode -eq 200 -and $response.Content -match "customer360_merge_total"
}

Write-Step "Waiting for Prometheus readiness: $promReadyUrl"
Wait-Until -TimeoutSeconds 180 -TimeoutMessage "Prometheus did not become ready." -Condition {
    $response = Invoke-WebRequest -Uri $promReadyUrl -UseBasicParsing -TimeoutSec 5
    return $response.StatusCode -eq 200
}

Write-Step "Validating exact matcher query in Prometheus: $query"
Wait-Until -TimeoutSeconds 180 -TimeoutMessage "Prometheus query did not return up=1 for job '$JobName'." -Condition {
    $result = Invoke-RestMethod -Method Get -Uri $promUpQueryUrl -TimeoutSec 10
    if ($null -eq $result -or $result.status -ne "success" -or $null -eq $result.data.result) {
        return $false
    }

    foreach ($series in $result.data.result) {
        if ($series.metric.job -eq $JobName -and [string]$series.value[1] -eq "1") {
            return $true
        }
    }

    return $false
}

Write-Step "OK: observability stack is running and query up{job=`"$JobName`"} returns 1."
Write-Step "Prometheus: $promBaseUrl | Alertmanager: http://localhost:9093 | Grafana: http://localhost:3000"
