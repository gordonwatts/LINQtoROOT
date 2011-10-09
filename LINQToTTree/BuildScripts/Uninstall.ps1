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

$imports = $buildProject.XML.Imports | ? {([System.IO.FileInfo] $_.Project).Name -eq "LINQTargets.targets" }
if ($imports)
{
	foreach ($i in $imports)
	{
		$buildProject.XML.RemoveChild($i)
	}
}

$project.Save() #persists the changes
$buildProject.Save()
$project.Save() #persists the changes
