#
# Commands that can be run
# from the nuget command line window to
# do various things...
#
function Get-RelativePath ($Folder, $filePath)
{
	process {
	   $from = $Folder = split-path $Folder -NoQualifier -Resolve
	   $to = $filePath = split-path $filePath -NoQualifier -Resolve

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
}

#
# A string of fileinfo's, insert them in the project with the time type!
#
function add-to-project ($itemType, $project, $namespace = "")
{
	process
	{
		$rpath = Get-RelativePath $project.DirectoryPath $_.FullName
		$bogus = $project.Xml.AddItem($itemType, $rpath)
		if ($namespace)
		{
			$bogus1 = $bogus.AddMetadata("Namespace", $namespace)
		}
	}
}

# Parse a file
function Write-TTree-MetaData ($Path = $(throw "-Path must be supplied"), $SubDirName = "", $Namespace = "ROOTLINQ")
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
	
	#
	# The destination directory is the project + filename, unless we had a flag...
	#

	$destDir = ([System.IO.FileInfo] $p.FullName).Directory.FullName
	if (-not $SubDirName)
	{
		if ($Namespace)
		{
			$SubDirName = $Namespace
		}
		else
		{
			$SubDirName = split-path -leaf $Path
		}
	}
	$destDir = join-path $destDir $SubDirName
	
	#
	# Run the parse now
	#
	
	CmdTFileParser -d $destDir $Path
	
	#
	# Now, attempt to insert them all into the project
	#
	
	$allFiles = Get-ChildItem -Path $destDir
	$allFiles | ? {$_.Extension -eq ".ntup"} | add-to-project "TTreeGroupSpec" $ms
	$allFiles | ? {$_.Extension -eq ".ntupom"} | add-to-project "ROOTFileDataModel" $ms $Namespace

	#
	# Make sure everything is saved!
	#
	
	$ms.Save()
	$p.Save()
}

#
# Make availible in the nuget command prompt.
#
Export-ModuleMember Write-TTree-MetaData
