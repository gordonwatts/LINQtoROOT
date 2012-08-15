param($newVersion)
#
# We expect to be run from a root directory! And "$newVersion" is "v5.30.01" or
# similar.
#

#
# Get a list of all packages.config files.
#

$allfiles = Get-ChildItem -Recurse packages.config

if (-not $allfiles)
{
    Write-Host "No packages.config files found!"
}

#
# Find all the ROOT version numbers that are in those files.
# We will replace all of them and update them to the version that
# was passed in on the command line.
#

$oldVersions = $allfiles | Get-Content | ? {$_ -match "-(v[\.0-9]+)\.win32"} | % {$Matches[1]} | Sort-Object -Unique

#
# For each old version, update the file! :-)
#

foreach ($oldV in $oldVersions)
{
    foreach ($f in $allfiles)
    {
        $newFile = Get-Content $f | % {$_ -replace $oldV, $newVersion}
        Write-Host "Updating $f"
        $newFile | Set-Content $f
    }
}