Import-Function -Name New-PackagePostStep
Import-Function -Name Compress-Archive

# Setup Chrome
Get-Item -Path "master:{FE669C6E-5CE5-4A7D-B4D3-5039B4C6AE75}" | Invoke-Script
$currentYear = [datetime]::Today.ToString("yyyy")

# Build Standard Package
$package = New-Package "Sitecore Security Access Overrides"
$package.Sources.Clear();

$package.Metadata.Author = "Michael West"
$package.Metadata.Publisher = "Michael West"
$Version = "1.0"
$package.Metadata.Version = $Version

$readMeBuilder = New-Object System.Text.StringBuilder
$readMeBuilder.AppendLine("Thank you for your support.") > $null
$readMeBuilder.AppendLine("") > $null
$readMeBuilder.AppendLine("Copyright (c) 2023-$($currentYear) Michael West") > $null
$readMeBuilder.AppendLine("") > $null
$readMeBuilder.AppendLine("Additional Resources:") > $null
$readMeBuilder.AppendLine("https://michaellwest.blogspot.com/") > $null
$package.Metadata.Readme = $readMeBuilder.ToString()

# Files
$source = Get-Item "$AppPath\App_Config\Include\So\*.*" | New-ExplicitFileSource -Name "Configs"
$package.Sources.Add($source)

$source = Get-Item "$AppPath\bin\So.dll" | New-ExplicitFileSource -Name "Binaries"
$package.Sources.Add($source)

$iarSource = Get-ChildItem -Path "$AppPath\sitecore modules\items\" -Include "*.so.dat.tmp" -Recurse | New-ExplicitFileSource -Name "IAR"
$package.Sources.Add($iarSource)

$packageName = "$($package.Name)-$Version" -replace " ",".";

# Package without items
Export-Package -Project $package -Path "$packageName-IAR.xml"
Export-Package -Project $package -Path "$packageName-IAR.zip" -Zip

# Package with items
$package.Sources.Remove($iarSource) > $null

$source = Get-Item 'master:\templates\Modules\Security Override\*' | New-ExplicitItemSource -Name 'Templates' -InstallMode Overwrite
$package.Sources.Add($source)

$source = Get-Item 'master:\system\Settings\Rules\Security Override\*' | New-ExplicitItemSource -Name 'Rules' -InstallMode Overwrite
$package.Sources.Add($source)

$source = Get-Item 'master:\system\Modules\Security Override' | New-ExplicitItemSource -Name 'Modules' -InstallMode Merge
$package.Sources.Add($source)

Export-Package -Project $package -Path "$packageName.xml"
Export-Package -Project $package -Path "$packageName.zip" -Zip