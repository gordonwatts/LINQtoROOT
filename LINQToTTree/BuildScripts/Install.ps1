param($installPath, $toolsPath, $package, $project) 
#
# install.ps1 - called each time a package is put into
# a project. We modify the projects .csproj build items to include
# targets that can do the LINQ class translation.
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

$buildTargetFile = Join-Path $toolsPath "LINQTargets.targets"
$buildProject.Xml.AddImport($buildTargetFile)

#$target = $buildProject.Xml.AddTarget("MyCustomAfterBuild")
#$target.AfterTargets = "AfterBuild"
#task = $target.AddTask("Message")
#$task.SetParameter("Text", "Hello AfterBuild")

$project.Save() #persists the changes
