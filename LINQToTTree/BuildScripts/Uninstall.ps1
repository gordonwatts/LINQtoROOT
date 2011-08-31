#
# The package is being uninstalled from
# the project. We will remove the add imports, etc.
# that were put in.
#
Import-Module (Join-Path $toolsPath msbuild.psm1)

Write-Host "This is a test"

#
# Get the project
#

$project = Get-Project
$buildProject = Get-MSBuildProject

#
# Next, add the import statements
#

$import = $buildProject.XML.Imports | ? {([System.IO.FileInfo] $_.Project).Name -eq "LINQTargets.targets" }
if ($import)
{
	$buildProject.XML.RemoveChild($import)
}

$project.Save() #persists the changes
