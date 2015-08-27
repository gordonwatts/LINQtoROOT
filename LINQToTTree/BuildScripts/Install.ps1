param($installPath, $toolsPath, $package, $project)
#
# install.ps1 - called each time a package is put into
# a project. We modify the projects .csproj build items to include
# targets that can do the LINQ class translation.
#

#
# Now, we have a few template files - we need to set them as no build and mark them
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
