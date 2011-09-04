#
# Commands that can be run
# from the nuget command line window to
# do various things...
#

#
# A string of fileinfo's, insert them in the project with the time type!
#
function add-to-project ($itemType, $project)
{
	process
	{
		$bogus = $project.Xml.AddItem($itemType, $_)
	}
}

# Parse a file
function Write-TTree-MetaData ($Path = $(throw "-Path must be supplied"))
{
	# Config
	if (-not (Test-Path $Path))
	{
		Write-Host "The path '$Path' was not found."
		return
	}
	
	$p = Get-Project
	if (-not $p)
	{
		Write-Host "No default project is set in the nuget execution window!"
	}
	Write-Host "Inserting the results of the parsing into project" $p.Name
	$ms = Get-MSBuildProject
	
	$destDir = ([System.IO.FileInfo] $p.FullName).Directory
	
	#
	# Run the parse now
	#
	
	CmdTFileParser -d $destDir.FullName $Path
	
	#
	# Now, attempt to insert them all into the project
	#
	
	$allFiles = Get-ChildItem -Path $destDir.FullName
	$allFiles | ? {$_.Extension -eq ".ntup"} | add-to-project "TTreeGroupSpec" $ms
	$allFiles | ? {$_.Extension -eq ".ntupom"} | add-to-project "ROOTFileDataModel" $ms
}

#
# Make availible in the nuget command prompt.
#
Export-ModuleMember Write-TTree-MetaData
