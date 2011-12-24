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
# Next, add the import statements
#

$buildTargetFile = Join-Path $toolsPath "LINQTargets.targets"
$projectFolder = ([System.IO.FileInfo] $project.FullName).DirectoryName
$relBuildTargetFile = Get-RelativePath $projectFolder $buildTargetFile 

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
