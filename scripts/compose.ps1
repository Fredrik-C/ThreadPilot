param(
    [ValidateSet('up','down','logs','ps','smoke')]
    [string]$cmd = 'up'
)

$ErrorActionPreference = 'Stop'

$composeFile = Join-Path $PSScriptRoot "..\docker-compose.yml" | Resolve-Path -ErrorAction SilentlyContinue
if (-not $composeFile) {
    # Fallback to repo root relative to script when run in CI or different cwd
    $composeFile = Join-Path (Get-Location) "docker-compose.yml"
}

function Compose-Up {
    docker compose -f "$composeFile" up --build -d
}

function Compose-Down {
    docker compose -f "$composeFile" down -v --remove-orphans
}

function Compose-Logs {
    docker compose -f "$composeFile" logs -f --tail=200
}

function Compose-Ps {
    docker compose -f "$composeFile" ps
}

function Smoke-Test {
    Write-Host "Waiting for services to be healthy..." -ForegroundColor Cyan
    $timeout = [DateTime]::UtcNow.AddMinutes(3)
    while([DateTime]::UtcNow -lt $timeout){
        $veh = docker inspect --format='{{json .State.Health.Status}}' threadpilot-vehicles-api-1 2>$null
        $ins = docker inspect --format='{{json .State.Health.Status}}' threadpilot-insurances-api-1 2>$null
        if($veh -match 'healthy' -and $ins -match 'healthy'){ break }
        Start-Sleep -Seconds 3
    }

    Write-Host "Vehicles API /health/ready:" -NoNewline
    try { Invoke-WebRequest -Uri http://localhost:5123/health/ready -UseBasicParsing | Out-Null; Write-Host " OK" -ForegroundColor Green } catch { Write-Host " FAIL" -ForegroundColor Red }

    Write-Host "Insurances API /health/ready:" -NoNewline
    try { Invoke-WebRequest -Uri http://localhost:5261/health/ready -UseBasicParsing | Out-Null; Write-Host " OK" -ForegroundColor Green } catch { Write-Host " FAIL" -ForegroundColor Red }

    Write-Host "Vehicles sample GET /api/vehicles/ABC123:" -NoNewline
    try { (Invoke-WebRequest -Uri http://localhost:5123/api/vehicles/ABC123 -UseBasicParsing).StatusCode | Out-Null; Write-Host " OK" -ForegroundColor Green } catch { Write-Host " FAIL" -ForegroundColor Red }

    Write-Host "Insurances sample GET /api/insurances/640823-3234:" -NoNewline
    try { (Invoke-WebRequest -Uri http://localhost:5261/api/insurances/640823-3234 -UseBasicParsing).StatusCode | Out-Null; Write-Host " OK" -ForegroundColor Green } catch { Write-Host " FAIL" -ForegroundColor Red }
}

switch($cmd){
    'up'    { Compose-Up }
    'down'  { Compose-Down }
    'logs'  { Compose-Logs }
    'ps'    { Compose-Ps }
    'smoke' { Smoke-Test }
    default { Compose-Up }
}

