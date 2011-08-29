#
# BuildUtils.psm1
#
#  This module contains the various functions and other things needed to build
# the LINQToTTree libraries
#


#
# Check that all the files exist as dll's in the
# dest directory. Fail badly if not.
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
function get-files-for-library ($dir, $flist)
{
	$fullnames = $flist | % { "$dir\${_}.dll", "$dir\${_}.xml" }
	return $fullnames | ? { Test-Path $_ }
}

#
# Loads in the list of packages from package config directory
#
function get-solution-nuget-dependencies ($solDir, $packageFile = "packages.config")
{
	$xml = [Xml] (Get-Content "$solDir\$packageFile")
	return $xml.SelectNodes("/packages/package") | % { @{"Id" = $_.id; "Version" = $_.version} }
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
	$packageName = $PackageSpecification["Name"] + "-" + $PackageSpecification["ROOTVersion"]
	$path = "$BuildDir\$packageName.$version.nuspec"
	
	#
	# Now write out the spec file
	#

    '<?xml version="1.0"?>' > $path
    '<package xmlns="http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd">' >> $path
    '  <metadata>' >> $path
    "    <id>$packageName</id>" >> $path
    #"    <title>ROOT.NET $packageSinglename for ROOT version $rootVersion</title>" >> $path
    "    <version>$version</version>" >> $path
    "    <authors>Gordon Watts</authors>" >> $path
    "    <owners>Gordon Watts</owners>" >> $path
    "    <licenseUrl>http://rootdotnet.codeplex.com/license</licenseUrl>" >> $path
    "    <projectUrl>http://rootdotnet.codeplex.com/</projectUrl>" >> $path
    #"    <iconUrl>http://ICON_URL_HERE_OR_DELETE_THIS_LINE</iconUrl>" >> $path
    "    <requireLicenseAcceptance>false</requireLicenseAcceptance>" >> $path
    "    <description>Use LINQ to query TTree's</description>" >> $path
    "    <tags>ROOT Data Analysis Science</tags>" >> $path
    if ($PackageSpecification["Dependencies"])
    {
        "    <dependencies>" >> $path
        foreach ($dep in $PackageSpecification["Dependencies"])
        {
			$pname = $dep["Id"]
			$pversion = $dep["Version"]
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
        "    <file src=`"$l`" target=`"lib\net40`" />" >> $path
    }
    
    #
    # Done!
    #
    
    "  </files>" >> $path
    "</package>" >> $path
    
    #
    # Final task, run nuget on the thing to actually build it!
    #
    
    $results = & $NuGetExe pack $path -OutputDirectory $BuildDir 2>&1
	if (-not (($results | ? { $_.Contains("Successfully created") } ).Length -gt 0 ))
	{
		Write-Host $results
		throw "Failed to build nuget package!"
	}
	
	return $path.Replace(".nuspec", ".nupkg")
}

#
# Given the name of a root package, extract the version number.
#
function get-root-version
{
	process
	{
		$l = $_ -split "-"
		$vstr = $l[2] -split "\."
		#return $vstr[0..2] -join "."
		return $vstr -join "."
	}
}

#
# Build a nuget packages for this library. We assume the build has already been done
# at this point - so we fail if we can't find what we are looking for!
#
function build-LINQToTTree-nuget-packages ($SolutionDirectory, $BuildDir, $Version, $release = "Debug", $nugetDistroDirectory = "")
{
	if (-not (Test-Path $solutionDirectory))
	{
		throw "Unable to find solution at $solutionDirectory"
	}

	#
	# We are going to be building for the LINQToTTree and the Helper library packages, and wrap them
	# up in one big package. We will also be doing the C# build stuff.
	#
	
	$mainLibrarySolutionDir = "$solutionDirectory\LINQToTTree\LINQToTTreeLib"
	$mainLibrary = "$mainLibrarySolutionDir\bin\$release"
	$mainLibraryFiles = "LinqToTTreeInterfacesLib", "LINQToTTreeLib", "Remotion.Linq"
	check-exists $mainLibrary $mainLibraryFiles

	$helperLibrarySolutionDir = "$solutionDirectory\LINQToTTreeHelpers\LINQToTreeHelpers"
	$helperLibrary = "$helperLibrarySolutionDir\bin\$release"
	$helperLibraryFiles = "LINQToTreeHelpers", "Doddle.Reporting"
	check-exists $helperLibrary $helperLibraryFiles
	
	$mainLibraries = get-files-for-library $mainLibrary $mainLibraryFiles
	$helperLibraries = get-files-for-library $helperLibrary $helperLibraryFiles
	$allLibraries = $mainLibraries + $helperLibraries

	#
	# Next, figure out what the dependent libraries are for nuget. These are things that nuget will
	# have to also fetch in order for us to "work" correctly.
	#
	
	$mainPackageDependencies = get-solution-nuget-dependencies $mainLibrarySolutionDir
	$helperPackageDependencies = get-solution-nuget-dependencies $helperLibrarySolutionDir

	$allPackageDependencies = $mainPackageDependencies + $helperPackageDependencies
	
	#
	# Extract the root version number. We depend on these to build, so it will be stored
	# in one of those dependencies...
	#
	
	$ROOTNames = $allPackageDependencies | ? {$_.Id.Contains("ROOT")} | % {$_.Id}
	$ROOTVersion = $ROOTNames[0] | get-root-version
	
	#
	# We have gathered all the basic information we need to build the main nuget package.
	#
	
	$packageSpec = @{
		"Name" = "LINQToTTree"
		"Version" = $Version
		"ROOTVersion" = $ROOTVersion
		"Dependencies" = $allPackageDependencies
		"Libraries" = $allLibraries
	}
	
	$pkg = build-nuget-package -PackageSpecification $packageSpec -BuildDir $buildDir -NuGetExe "$solutionDirectory\LINQToTTree\nuget.exe"
	
	#
	# Copy it over if requested. Return the final location of the file.
	#
	
	if ($nugetDistroDirectory)
	{
		return Copy-Item $pkg $nugetDistroDirectory
	}
	
	return $pkg
}

build-LINQToTTree-nuget-packages "C:\Users\gwatts\Documents\ATLAS\Projects\LINQToROOT" "C:\Users\gwatts\Documents\ATLAS\Projects\LINQToROOT" "0.42" -nugetDistroDirectory "C:\Users\gwatts\Documents\nuget"
