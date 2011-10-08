#
# Simple script to build the nuget package
# when run from the continuous integration server. The result
# of this guy will be become a build artifact.
#


$mod = Resolve-Path .\LINQToTTree\BuildScripts\BuildUtils.psm1
Write-Host $mod
$policy = Get-ExecutionPolicy
Write-Host "Current exe policy: $policy"
Import-Module $mod

#
# Some config - which might oneday be passed in as arguments!
#

$release = "x86\Release"
$NameSuffix = "-ci"

#
# The CI builder has already done all the hard work for us, so we need to just go
# and get everything.
#

# First, we need to put together the version number that we will use to register
# this nuget package

$FileVersion = (Get-Item "LINQToTTree\LINQToTTreeLib\bin\$release\LINQToTTreeLib.dll").VersionInfo.ProductVersion
$version = $FileVersion

# Next, do the nuget build!

#-nugetDistroDirectory $nugetPackageDir 
build-LINQToTTree-nuget-packages "." "." $version -PDB:$PDB -NameSuffix $NameSuffix -Release $release
