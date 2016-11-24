#
# BuildUtils.psm1
#
#  This module contains the various functions and other things needed to build
# the LINQToTTree libraries
#

#
# Check that all the files exist as dll's in the
# destination directory. Fail badly if not.
#
function check-exists ($dir, $flist)
{
    $fullnames = $flist | % { "$dir\${_}.dll" }
    $bad = $false
    foreach ($f in $fullnames)
    {
        if (-not (Test-Path $f))
        {
            Write-Host "Unable to find file $f to build nuget package"
            $bad = $true
        }
    }
    if ($bad)
    {
        throw "Unable to find all files to build nuget packages"
    }
}

#
# Return a list of all library files we can find
#
function get-files-for-library ($dir, $flist, [Switch]$PDB)
{
    $fullnames = $flist | % { "$dir\${_}.dll", "$dir\${_}.xml" }
    if ($PDB)
    {
        $fullnames += $flist | % { "$dir\${_}.pdb" }
    }
    return $fullnames | ? { Test-Path $_ }
}

#
# Loads in the list of packages from package configuration directory as
# a PSObject.
#
function get-solution-nuget-dependencies ($solDir, $packageFile = "packages.config")
{
    $xml = [Xml] (Get-Content "$solDir\$packageFile")
    $prop = $xml.SelectNodes("/packages/package") | % { @{"Id" = $_.id; "Version" = $_.version} }
    return $prop | % {New-Object PSObject -Property $_}
}

#
# Given a nuget package spec and a directory location, write
# out the spec and then run nuget to build the package
#
function build-nuget-package ($PackageSpecification, $BuildDir, $NuGetExe)
{
    #
    # Build up package name, etc. This should be of the form "name-ROOTVERSION"
    #

    $version = $PackageSpecification["version"]
    $packageName = $PackageSpecification["Name"]
    $path = "$BuildDir\$packageName.$version.nuspec"
    
    #
    # Now write out the spec file
    #

    '<?xml version="1.0"?>' > $path
    '<package xmlns="http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd">' >> $path
    '  <metadata>' >> $path
    "    <id>$packageName</id>" >> $path
    "    <title>LINQToROOT Analysis Utilities</title>" >> $path
    "    <version>$version</version>" >> $path
    "    <authors>Gordon Watts</authors>" >> $path
    "    <owners>Gordon Watts</owners>" >> $path
    "    <licenseUrl>https://github.com/gordonwatts/LINQtoROOT/blob/master/LICENSE</licenseUrl>" >> $path
    "    <projectUrl>https://github.com/gordonwatts/LINQtoROOT</projectUrl>" >> $path
    "    <requireLicenseAcceptance>false</requireLicenseAcceptance>" >> $path
    "    <description>Use LINQ to query TTree's. LINQ queries are converted to C++ code. VS2013 C++ tools must be installed.</description>" >> $path
    "    <tags>ROOT Data Analysis Science TTree LINQ</tags>" >> $path
    if ($PackageSpecification["Dependencies"])
    {
        "    <dependencies>" >> $path
        foreach ($dep in $PackageSpecification["Dependencies"])
        {
            $pname = $dep.Id
            $pversion = $dep.Version
            "      <dependency id=`"$pname`" version=`"$pversion`" />" >> $path
        }
        "    </dependencies>" >> $path
    }
    "  </metadata>" >> $path
    
    #
    # First, the library files
    #
    
    "  <files>" >> $path
    
    foreach ($l in $PackageSpecification["Libraries"])
    {
        "    <file src=`"$l`" target=`"lib\net45`" />" >> $path
    }
    foreach ($l in $PackageSpecification["Tools"])
    {
        "    <file src=`"$l`" target=`"tools`" />" >> $path
    }
    foreach ($l in $PackageSpecification["ContentFiles"])
    {
        $dest = $l.DestDir
        $src = $l.SourceFile
        "    <file src=`"$src`" target=`"content\$dest`" />" >> $path
    }
    foreach ($l in $PackageSpecification["SourceRootDirectories"])
    {
        $destDir = split-path -Leaf $l
        "    <file src=`"$l\**\*.cs`" target=`"src\$destDir`" />" >> $path
    }
	foreach ($l in $PackageSpecification["TargetFile"])
	{
		"    <file src=`"$l`" target=`"build\$packageName.targets`" />" >> $path
	}
	foreach ($l in $PackageSpecification["BuildFiles"])
	{
		"    <file src=`"$l`" target=`"build`" />" >> $path
	}
    
    #
    # Done!
    #
    
    "  </files>" >> $path
    "</package>" >> $path
    
    #
    # Final task, run nuget on the thing to actually build it!
    #
    
    $results = & $NuGetExe pack $path -OutputDirectory $BuildDir -Symbols 2>&1
    if (-not (($results | ? {$_.GetType() -eq [System.String]} | ? { $_.Contains("Successfully created") } ).Length -gt 0 ))
    {
        Write-Host $results
        throw "Failed to build nuget package!"
    }
    
    return $path.Replace(".nuspec", ".nupkg")
}

#
# Build a nuget packages for this library. We assume the build has already been done
# at this point - so we fail if we can't find what we are looking for!
#
function build-LINQToTTree-nuget-packages ($SolutionDirectory, $BuildDir, $Version, $Release = "Debug", $nugetDistroDirectory = "", [Switch]$PDB, $NameSuffix = "", $ROOTPackage = "")
{
    if (-not (Test-Path $SolutionDirectory))
    {
        throw "Unable to find solution at $solutionDirectory"
    }

    #
    # We are going to be building for the LINQToTTree and the Helper library packages, and wrap them
    # up in one big package. We will also be doing the C# build stuff.
    #
    # Note that doodle causes an error when we upload the symbols file later on as we don't have the source
    # for it included here.
    #
    
    $mainLibrarySolutionDir = "$solutionDirectory\LINQToTTree\LINQToTTreeLib"
    $mainLibrary = "$mainLibrarySolutionDir\bin\$release"
    $mainLibraryFiles = "LinqToTTreeInterfacesLib", "LINQToTTreeLib"
    $mainLibrarySourceFiles = "LINQToTTree\LINQToTTreeLib"
    check-exists $mainLibrary $mainLibraryFiles

    $helperLibrarySolutionDir = "$solutionDirectory\LINQToTTree\LINQToTreeHelpers"
    $helperLibrary = "$helperLibrarySolutionDir\bin\$release"
    $helperLibraryFiles = "LINQToTreeHelpers", "Doddle.Reporting"
    $helperLibrarySourceFiles = "LINQToTTree\LINQToTreeHelpers"
    check-exists $helperLibrary $helperLibraryFiles

    $interfaceLibrarySourceFiles = "LINQToTTree\LinqToTTreeInterfacesLib"
    
    $mainLibraries = get-files-for-library $mainLibrary $mainLibraryFiles -PDB:$PDB
    $helperLibraries = get-files-for-library $helperLibrary $helperLibraryFiles -PDB:$PDB
    $allLibraries = $mainLibraries + $helperLibraries | ? {!$_.Contains("Doddle.Reporting.pdb")}
    $allSourceDirectories = ($mainLibrarySourceFiles, $helperLibrarySourceFiles, $interfaceLibrarySourceFiles)
    Write-Host $allSourceDirectories
    
    #
    # There are some configuration data files that we need to add in.
    #
    
    $methodConfigFile = New-Object PSObject -Property @{DestDir = "ConfigData"; SourceFile = "$mainLibrary\ConfigData\default.classmethodmappings" }
    $TSelectorTemplate = New-Object PSObject -Property @{DestDir = "Templates"; SourceFile = "$mainLibrary\Templates\TSelectorTemplate.cxx" }
    
    $contentList = $methodConfigFile, $TSelectorTemplate

    #
    # We need to include the executable that will parse the ntuples. Make sure to filter out PDB files if so requested!
    #
    
    $cmdExeFiles = Get-ChildItem "$solutionDirectory\LINQToTTree\CmdTFileParser\bin\$release"
    $msbuildTaskFiles = Get-ChildItem "$solutionDirectory\LINQToTTree\MSBuildTasks\bin\$release" | ? { $_.Extension -ne ".targets" }
    $targetFiles = Get-ChildItem "$solutionDirectory\LINQToTTree\MSBuildTasks\bin\$release" | ? { $_.Extension -eq ".targets" } | % {$_.FullName}
    $installToolFiles = "msbuild.psm1", "Install.ps1", "Init.ps1", "LINQToTTreeCommands.psm1" | % { [System.IO.FileInfo] "$solutionDirectory\LINQToTTree\BuildScripts\$_" }

    $toolFiles = ($cmdExeFiles + $installToolFiles) | Sort-Object -Property Name -Unique
    $toolFiles = $toolFiles | ? { $_.Extension -ne ".pdb" }
    $toolFiles = $toolFiles | % {$_.FullName}
	$msbuildTaskFiles = $msbuildTaskFiles | ? { $_.Extension -ne ".pdb"} | % {$_.FullName}

    #
    # Next, figure out what the dependent libraries are for nuget. These are things that nuget will
    # have to also fetch in order for us to "work" correctly.
    #
    
    $mainPackageDependencies = get-solution-nuget-dependencies $mainLibrarySolutionDir
    $helperPackageDependencies = get-solution-nuget-dependencies $helperLibrarySolutionDir

    $allPackageDependencies = $mainPackageDependencies + $helperPackageDependencies | sort -Property "Id","Version" -Unique

    #
    # Replace ROOT package versions so we can run with multiple root version numbers.
    #
    
    if ($ROOTPackage -ne "")
    {
        $allPackageDependencies = $allPackageDependencies | replace-ROOT-Package $ROOTPackage
    }
    
    #
    # Extract the root version number. We depend on these to build, so it will be stored
    # in one of those dependencies...
    #
    
    $ROOTNames = $allPackageDependencies | ? {$_.Id.Contains("ROOT")} | % {$_.Id}
        
    #
    # We have gathered all the basic information we need to build the main nuget package.
    #
    
    $packageSpec = @{
        "Name" = "LINQToTTree" + $NameSuffix
        "Version" = $Version
        "Dependencies" = $allPackageDependencies
        "Libraries" = $allLibraries
        "Tools" = $toolFiles
		"TargetFile" = $targetFiles
		"BuildFiles" = $msbuildTaskFiles
        "ContentFiles" = $contentList
        "SourceRootDirectories" = $allSourceDirectories
    }
    
    $pkg = build-nuget-package -PackageSpecification $packageSpec -BuildDir $buildDir -NuGetExe "nuget.exe"
    
    #
    # Copy it over if requested. Return the final location of the file.
    #
    
    if ($nugetDistroDirectory)
    {
        return Copy-Item $pkg $nugetDistroDirectory
    }
    
    return $pkg
}

#
# Given a nuget repository directory, figure out what versions of root are present there
#
function get-ROOT-versions ($NuGetExe)
{
    # Get the RSS list of everything that is available
    #$pkgInfo = [Xml] (new-object net.webclient).downloadstring($URL)
    $pkgList = & $NuGetExe list ROOTNET-Core
    # List of all the core package names.
    #$corePackages = $pkgInfo.feed.entry | ? {$_.Title.InnerText.Contains("ROOTNET-Core")} | %{ @{ RVersion = $_.Title.InnerText.SubString(13); RDNVersion = $_.GetElementsByTagName("d:Version").Item(0).InnerText} } | % {New-Object PSObject -Property $_ }
    $corePackages = $pkgList | %{ ,($_ -split " ") } | % { @{RVersion = $_[0].SubString(13); RDNVersion=$_[1]} } | % {New-Object PSObject -Property $_ }
    return $corePackages
}

#
# Return the list of ROOT versions from our official nuget feed. Use the most recent version of ROOT
#
function get-ROOT-Version-Names ($NuGetExe)
{
    #return $pkgs = get-ROOT-versions "https://go.microsoft.com/fwlink/?LinkID=206669" | %{$_.RVersion}
    return $pkgs = get-ROOT-versions $NuGetExe | %{$_.RVersion}
}

#
# Scans the packages.config for RDN packages and returns their name ("Core", etc.).
#
function get-ROOTDOTNET-packages ($ProjectDir)
{
    $pkgInfo = [Xml] (Get-Content "$ProjectDir\packages.config")
    return $pkgInfo.packages.package | ? {$_.id.StartsWith("ROOTNET")} | % {($_.id -split "-")[1]}
}

#
# Use the packages.config to drive a package installation
#
function install-packages ($URL, $nuget)
{
    process
    {
        [System.IO.Directory]::SetCurrentDirectory("$_\..")
        Set-Location "$_\.."
        Write-Output (& $nuget install -OutputDirectory .\packages -Source $URL "$_\packages.config")
    }
}

#
# Make sure that all packages are correctly installed for all the projects.
#
function configure-nuget ($BuildPath, $URL, $nuget)
{
    $installLogs = Get-ChildItem -Recurse -Path "$BuildPath\*\packages.config" | % {$_.Directory} | install-packages $URL $nuget
    return $installLogs
}

#
# Make sure that the proper nuget packages are installed for
# a particular build we are going to do.
#
function configure-nuget-all ($BuildPath)
{
    # See if we can figure out where nuget.exe is.
    $nuget = "nuget.exe"
    #if (-not (Test-Path $nuget))
    #{
    #    throw "Unable to locate nuget.exe - though it would be here: $nuget"
    #}
    
    $nugetRepository = "https://go.microsoft.com/fwlink/?LinkID=206669"
    
    $log1 = configure-nuget "$BuildPath\LINQToTTree" $nugetRepository $nuget
    $log2 = configure-nuget "$BuildPath\LINQToTTreeHelpers" $nugetRepository $nuget
    
    return $log1, $log2
}

#
# Do a build
#
function build-project ($release)
{
    process
    {
        $solFile = Get-ChildItem $_\..\*.sln
        $pname = ([System.IO.FileInfo] $_).Name
        & devenv /nologo $solFile /project $pname /build "$release"
    }
}

#
# See if the last build at dir is the same as the
# current one. return true if the build and the revision are the same.
#
function check-build ($dir)
{
    if (Test-Path $dir)
    {
        if (Test-Path "$dir\build.txt")
        {
            $itm = Get-Content "$dir\build.txt"
            $newItem = get-revision $dir
            return ($itm -eq $newItem)
        }
    }
    return $false
}

#
# Update the build number file.
function update-build ($dir)
{
    get-revision $dir > "$dir\build.txt"
}

$loc = Split-Path -parent $MyInvocation.MyCommand.Definition
Import-Module "$loc\source-control.psm1"

#
# Given the main distribution directory, build everything needed for
# making our nuget libraries, and generate the nuget package!
#
function build-LINQToTTree ($BuildPath, $Release = "x86", $Tag = "HEAD", $nugetPackageDir = "", [Switch]$PDB, $NameSuffix = "")
{	
    #
    # Build the libraries
    #
    
    $colog = set-revision $BuildPath -repositoryPath "https://hg01.codeplex.com/linqtoroot" -revision $tag
    if (-not (check-build "$BuildPath"))
    {
        $lognuget = configure-nuget-all $BuildPath
        $buildLog = "LINQToTTree\LINQToTTreeLib", "LINQToTTreeHelpers\LINQToTreeHelpers", "LINQToTTree\CmdTFileParser", "LINQToTTree\MSBuildTasks"   | % { "$BuildPath\$_" } | build-project $release

        #
        # Get the version number
        #

        $version = (Get-Item "$BuildPath\LINQToTTree\LINQToTTreeLib\bin\$release\LINQToTTreeLib.dll").VersionInfo.ProductVersion

        #
        # Next, make the nuget package
        #
        
        $nugetCreateLog = build-LINQToTTree-nuget-packages $BuildPath $BuildPath $version -nugetDistroDirectory $nugetPackageDir -PDB:$PDB -NameSuffix $NameSuffix -Release $Release
        
        update-build "$BuildPath"
        
        return $colog, $lognuget, $buildLog, $nugetCreateLog
    }
    else
    {
        return $colog
    }
}

#build-LINQToTTree-nuget-packages "C:\Users\gwatts\Documents\ATLAS\Projects\LINQToROOT" "C:\Users\gwatts\Documents\ATLAS\Projects\LINQToROOT" "0.42" -nugetDistroDirectory "C:\Users\gwatts\Documents\nuget"
Export-ModuleMember build-LINQToTTree
Export-ModuleMember build-LINQToTTree-nuget-packages
Export-ModuleMember get-ROOT-Version-Names
#build-LINQToTTree C:\Users\gwatts\Desktop\bogus\linqtoroot
