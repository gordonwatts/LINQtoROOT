param($newVersion)
#
# We expect to be run from a root directory! And "$newVersion" is "v5.30.01" or
# similar.
#

#
# Get a list of all packages.config files.
#

$allConfigfiles = Get-ChildItem -Recurse -Filter packages.config .

if (-not $allConfigfiles)
{
    Write-Host "No packages.config files found!"
    return
}

#
# Find all the ROOT version numbers that are in those files.
# We will replace all of them and update them to the version that
# was passed in on the command line.
#

$oldVersions = $allConfigfiles | Get-Content | ? {$_ -match "-(v[\.0-9]+)\.win32"} | % {$Matches[1]} | Sort-Object -Unique

#
# We have to do this replacement in the project files as well as the
# nuget files.
#

$projectFilesCS = Get-ChildItem -Recurse -Filter *.csproj .
$projectFilesVC = Get-ChildItem -Recurse -Filter *.vcxproj .
$allfiles = $allConfigfiles + $projectFilesCS + $projectFilesVC

#
# For each old version, update the file! :-)
#

foreach ($oldV in $oldVersions)
{
    foreach ($f in $allfiles)
    {
        $newFile = Get-Content $f.FullName | % {$_ -replace $oldV, $newVersion}
        Write-Host "Updating $($f.FullName)"
        $newFile | Set-Content $f.FullName
    }
}