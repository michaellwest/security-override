Clear-Host

Add-Type -Assembly 'System.IO.Compression.FileSystem'

[System.IO.Directory]::SetCurrentDirectory($PSScriptRoot)
Remove-Item -Path "security-overrides.zip"
Remove-Item -Path "_out" -Recurse
Write-Host "Generate IAR file"
dotnet tool restore
dotnet sitecore plugin list
dotnet sitecore ser pull
dotnet sitecore itemres create -o _out/so --overwrite

New-Item -Path "_out/sitecore modules/items/master/" -ItemType Directory -ErrorAction SilentlyContinue > $null
New-Item -Path "_out/sitecore modules/items/web/" -ItemType Directory -ErrorAction SilentlyContinue > $null
Copy-Item -Path "_out/items.master.so.dat" -Destination "_out/sitecore modules/items/web/items.web.so.dat"
Move-Item -Path "_out/items.master.so.dat" -Destination "_out/sitecore modules/items/master/" -Force
Copy-Item -Path "docker/deploy/*" -Destination "_out/" -Recurse

[System.IO.Compression.ZipFile]::CreateFromDirectory("_out", "security-overrides.zip")