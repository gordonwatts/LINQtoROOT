#
# Simple script to build the nuget package
# when run from the continuous integration server. The result
# of this guy will be become a build artifact.
#
param([int] $BuildNumber = 0, [string] $NameSuffix = "-ci", [string] $release = "x86\Release", [switch] PDB)

$mod = Resolve-Path .\LINQToTTree\BuildScripts\BuildUtils.psm1
$policy = Get-ExecutionPolicy
Import-Module -DisableNameChecking $mod

#
# The CI builder has already done all the hard work for us, so we need to just go
# and get everything.
#

# First, we need to put together the version number that we will use to register
# this nuget package

$FileVersion = (Get-Item "LINQToTTree\LINQToTTreeLib\bin\$release\LINQToTTreeLib.dll").VersionInfo.ProductVersion -split "\."
$FileVersion[3] = $BuildNumber
$version = $FileVersion -join "."

# Next, do the nuget build, one for each ROOT package.

#-nugetDistroDirectory $nugetPackageDir 
$currentPath = Resolve-Path "."
#get-ROOT-Version-Names ".\LINQToTTree\.nuget\nuget.exe" | %{build-LINQToTTree-nuget-packages $currentPath $currentPath $version -PDB:$PDB -NameSuffix $NameSuffix -Release $release -ROOTPackage $_}

# For CI, only build the version that is on the server for now.
build-LINQToTTree-nuget-packages $currentPath $currentPath $version -PDB:$PDB -NameSuffix $NameSuffix -Release $release


