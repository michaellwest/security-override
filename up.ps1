<#
    .SYNOPSIS
        Spins up the containers.

    .PARAMETER SkipBuild
        Specifies that the images should not be built prior to starting up.

    .PARAMETER IncludeSps
        Species that the Sitecore Publishing Service should be included.

    .PARAMETER IncludeSpe
        Specifies that the Sitecore PowerShell Extensions module should be included.

    .PARAMETER IncludeSxa
        Specifies that the Sitecore Exerience Accelerator modules should be included.

    .PARAMETER IncludePackages
        Specifies that custom packages should be included during the build and/or after startup.

        Packages contained within .\docker\build\releases will be included in the built images.
        Packages contained within .\docker\releases will be deployed after the containers startup.
#>

[CmdletBinding()]
param(
    [switch]$SkipIndexing
)

if (-not (docker ps)) {
    Write-Host "Please verify Docker is running and try again." -ForegroundColor Red
    break
}


$composeArgs = @("compose", "-f", ".\docker-compose.yml")

if(Test-Path -Path (Join-Path -Path $PSScriptRoot -ChildPath "docker-compose.override.yml")) {
    $composeArgs += "-f"
    $composeArgs += ".\docker-compose.override.yml"
}

Write-Host "Starting Sitecore environment..." -ForegroundColor Green
docker $composeArgs up -d

Write-Host "Waiting for CM to become available..." -ForegroundColor Green
$startTime = Get-Date
do {
    Start-Sleep -Milliseconds 300
    try {
        $status = Invoke-RestMethod "http://localhost:8079/api/http/routers/cm-secure@docker"
    } catch {
        if ($_.Exception.Response.StatusCode.value__ -ne "404") {
            $containers = docker ps -a --format "table {{.Names}}"
            $container = $containers | ConvertFrom-Csv | Where-Object { $_.NAMES -match "-cm" } | Select-Object -First 1 -ExpandProperty NAMES
            docker exec $container curl http://cm
            throw
        }
    }
} while ($status.status -ne "enabled" -and $startTime.AddSeconds(15) -gt (Get-Date))
if (-not $status.status -eq "enabled") {
    $status
    Write-Error "Timeout waiting for Sitecore CM to become available via Traefik proxy. Check CM container logs."
}


Write-Host "Restoring Sitecore CLI..." -ForegroundColor Green
dotnet tool restore
Write-Host "Installing Sitecore CLI Plugins..."
dotnet sitecore --help | Out-Null
if ($LASTEXITCODE -ne 0) {
    Write-Error "Unexpected error installing Sitecore CLI Plugins"
}

Import-Module .\tools\DockerToolsLite
$envPath = Join-Path -Path $PSScriptRoot -ChildPath ".env"
$cmHost = Get-EnvFileVariable -Variable "CM_HOST" -Path $envPath
$idHost = Get-EnvFileVariable -Variable "ID_HOST" -Path $envPath

Write-Host "Logging into Sitecore..." -ForegroundColor Green
dotnet sitecore login --cm https://$cmHost --allow-write true --auth https://$idHost

if ($LASTEXITCODE -ne 0) {
    Write-Error "Unable to log into Sitecore, did the Sitecore environment start correctly? See logs above."
}

if (-not $SkipIndexing) {
    # Populate Solr managed schemas to avoid errors during item deploy
    Write-Host "Populating Solr managed schema..." -ForegroundColor Green
    dotnet sitecore index schema-populate
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Populating Solr managed schema failed, see errors above."
    }

    # Rebuild indexes
    Write-Host "Rebuilding indexes ..." -ForegroundColor Green
    dotnet sitecore index rebuild
}

Write-Host "Good luck!" -ForegroundColor Green

Write-Host "Opening site..." -ForegroundColor Green
Start-Process "https://$($cmHost)/sitecore"