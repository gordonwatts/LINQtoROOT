param($installPath, $toolsPath, $package, $project)
#
# install.ps1 - called each time a package is put into
# a project. We modify the projects .csproj build items to include
# targets that can do the LINQ class translation.
#

Import-Module (Join-Path $toolsPath msbuild.psm1)

#
# Helper function to do some relative path stuff
#

function Get-RelativePath ([string] $Folder, [String] $filePath, [Switch] $Resolve)
{
   Write-Verbose "Resolving paths relative to '$Folder'"
   $from = $Folder = split-path $Folder -NoQualifier -Resolve:$Resolve
   $to = $filePath = split-path $filePath -NoQualifier -Resolve:$Resolve

   while($from -and $to -and ($from -ne $to)) {
      if($from.Length -gt $to.Length) {
         $from = split-path $from
      } else {
         $to = split-path $to
      }
   }

   $filepath = $filepath -replace "^"+[regex]::Escape($to)+"\\"
   $from = $Folder
   while($from -and $to -and $from -gt $to ) {
      $from = split-path $from
      $filepath = join-path ".." $filepath
   }
   Write-Output $filepath
}

#
# Get the project
#

$buildProject = Get-MSBuildProject $project.Name

#
# Next, add the import statement to the project file. First,
# copy the file to the proper place.
#

$solutionTargetName = ".LINQToTTree"
$solution = $project.DTE.Solution

$buildTargetFile = Join-Path $toolsPath "LINQTargets.targets"
$solutionFolderName = ([System.IO.FileInfo] $solution.FullName).DirectoryName
$linqFolderName = "$solutionFolderName\$solutionTargetName"
$linqBuildTargetFile = "$linqFolderName\LINQTargets.targets"
if (-not (Test-Path "$linqFolderName"))
{
    New-Item -ItemType "Directory" $linqFolderName
}
Copy-Item $buildTargetFile $linqBuildTargetFile

#
# Now, add it into a folder in the solution. Make sure not to add it if it is already there!
#

$solutionFolderProject = $solution.Projects | ? { $_.ProjectName -eq $solutionTargetName }
if (-not $solutionFolderProject)
{
    $solution.AddSolutionFolder($solutionTargetName)
    $solutionFolderProject = $solution.Projects | ? { $_.ProjectName -eq $solutionTargetName }
}

$solutionFolderProject.ProjectItems.AddFromFile($linqBuildTargetFile)

#
# Finally, now that we have the location, add it into the project build instructions itself.
# Do it with a relative path to make sure this will survive going in and out of source control
# to differe
#

$projectFolder = ([System.IO.FileInfo] $project.FullName).DirectoryName
$relBuildTargetFile = Get-RelativePath $projectFolder $linqBuildTargetFile 
$buildProject.Xml.AddImport($relBuildTargetFile)

#
# Now, we have a few tempate files - we need to set them as no build and mark them
# copy if newer.
#

$projectItems = $project.ProjectItems
$cfgFile = $projectItems.Item("ConfigData").ProjectItems.Item("default.classmethodmappings")
$template = $projectItems.Item("Templates").ProjectItems.Item("TSelectorTemplate.cxx")

$cfgFile.Properties.Item("CopyToOutputDirectory").Value = 2
$cfgFile.Properties.Item("BuildAction").Value = 0

$template.Properties.Item("CopyToOutputDirectory").Value = 2
$template.Properties.Item("BuildAction").Value = 0

#
# now, update everything and make sure it sticks!
#

$project.Save() #persists the changes
