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
function Write-TTree-MetaData
{
<#
.SYNOPSIS
   Parse a ROOT file for all TTrees and insert ROOTLINQ support files into current project.
.DESCRIPTION
   Reads a ROOT file for all TTree's located at the top level directory. For each TTree it generates the meta-data
   files required to use the ROOTLINQ query system and inserts them into the current project. Once the project has
   been built you can write queries against these TTree's.
.LINK
	http://linqtoroot.codeplex.com
.PARAMETER Path
   Path to the ROOT file
.PARAMETER SubDirName
   If you wish the output files to be stored in a different sub-directory than the default. Defaults to the Namespace.
.PARAMETER Namespace
   The namespcae where the query objects should exist. Defaults to ROOTLINQ. You are strongly encouraged to use this!
.Example
   Write-TTree-MetaData EVNT-short.root -Namespace HVDATA

   Will scan the file EVNT-short.root (located in the solution directory) for all TTree's and generate the XML files that
   contain the required metadata. These XML files are inserted in your current project in a folder called "HVDAATA". The
   objects will also exist in the HVDATA namespace.   
#>
	[CmdletBinding()]
	param(
	   [Parameter(Mandatory=$true, Position=0)]
	   [Alias("RootFile")]
	   [string]$Path,

	   [Parameter()]
	   [string]$SubDirName="",

	   [Parameter]
	   [string]$Namespace="ROOTLINQ"
	)

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
