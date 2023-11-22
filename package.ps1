Clear-Host

[System.IO.Directory]::SetCurrentDirectory($PSScriptRoot)
$releases = Join-Path -Path $PSScriptRoot -ChildPath "docker/releases"

Remove-Item -Path "_out" -Recurse
Write-Host "Generate IAR file"
dotnet tool restore
dotnet sitecore plugin list
dotnet sitecore ser pull
dotnet sitecore itemres create -o _out/so --overwrite

Copy-Item -Path "_out/items.master.so.dat" -Destination "_out/items.web.so.dat"

Write-Host "Generate packages from running Sitecore instance."

Import-Module -Name SPE

$sharedSecret = '7AF6F59C14A05786E97012F054D1FB98AC756A2E54E5C9ACBAEE147D9ED0E0DB'
$name = 'sitecore\admin'
$hostname = "https://so-cm.dev.local"

$session = New-ScriptSession -Username $name -SharedSecret $sharedSecret -ConnectionUri $hostname
$packageScript = Get-Content -Raw -Path .\package-spe.ps1
$packageScriptBlock = [scriptblock]::Create($packageScript)
Invoke-RemoteScript -ScriptBlock $packageScriptBlock -Session $session -Raw
Stop-ScriptSession -Session $session

Write-Host "Swap out IAR files"

Add-Type -AssemblyName "System.IO.Compression.FileSystem"
$file = Get-ChildItem -Path $releases -Filter "Sitecore.Security.Access.Overrides-*-IAR.zip" | Select-Object -ExpandProperty FullName
$zip = [System.IO.Compression.ZipFile]::Open($file, [System.IO.Compression.ZipArchiveMode]::Update)
$packageZipEntry = $zip.Entries | Where-Object { $_.Name -eq "package.zip" }

$stream = $packageZipEntry.Open()
$packageArchive = New-Object System.IO.Compression.ZipArchive($stream, [System.IO.Compression.ZipArchiveMode]::Update)
$iarEntries = $packageArchive.Entries | Where-Object { $_.Name -like "*.so.dat*" }
foreach($iarEntry in $iarEntries) {
    $fullname = $iarEntry.FullName.Replace(".tmp", "")
    $iarEntry.Delete()
    $iarEntry = $packageArchive.CreateEntry($fullname)

    if($fullname.StartsWith("files")) {
        $name = [System.IO.Path]::GetFileName($fullname)
        $content = [System.IO.File]::ReadAllBytes((Join-Path -Path ".\_out" -ChildPath $name))
        $ms = New-Object System.IO.MemoryStream(,$content)
        $zipStream = $iarEntry.Open()
        $ms.CopyTo($zipStream)
        $zipStream.Dispose()
        $zipStream.Close()
        $ms.Dispose()
        $ms.Close()
    }
}

$packageArchive.Dispose()

$stream.Close()
$stream.Dispose()

$zip.Dispose()