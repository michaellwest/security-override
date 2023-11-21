Write-Host "Performing file cleanup" -ForegroundColor Yellow

# Clean data folders
Write-Host "- Cleaning data folders"
Get-ChildItem -Path (Join-Path $PSScriptRoot "docker\data") -Directory | ForEach-Object {
    $dataPath = $_.FullName

    Get-ChildItem -Path $dataPath -Exclude ".gitkeep" -Recurse | Remove-Item -Force -Recurse
}

# Clean deploy folders
Write-Host "- Cleaning deploy folders"
Get-Item -Path (Join-Path $PSScriptRoot "docker\deploy\*") -Exclude ".gitkeep" | Remove-Item -Force -Recurse