#
# Module that contains an interface for source control
#

# SVN in order to download rootdotnet
# We depend on svn path from http://sourceforge.net/projects/win32svn/files/

$SVNDir = "C:\Users\gwatts\Documents\ATLAS\Projects\svn\svn-win32-1.6.17\bin"
if (-not (test-path $SVNDir))
{
    $SVNDIR = "D:\users\gwatts\ROOT.NET\svn-win32-1.6.17\bin"
}

if (-not (test-path $SVNDir))
{
    throw "Unable to find the svn executable"
}


#
# Get the revision number from a SVN repository. Use the svnversion executable to
# get this.
#
function get-svn-revision($directory)
{
    $vexe = "$SVNDir\svnversion.exe"
    return & $vexe $directory
}

#
# Update a given directory. Specify revision (or it defaults to HEAD). Works with svn and hg. We do
# our best to keep things "even" here, and simple. But the two do have different working styles.
#
function update-svn ($directory, [switch] $TopLevelOnly, $revision="HEAD")
{
    $rflag = "infinity"
    if ($TopLevelOnly)
    {
        $rflag = "immediates"
    }
    return & "$SVNDir\svn.exe" update -r $revision --depth $rflag $directory
}

#
# Update the repository to a particular revision
#
function update-hg ($directory, [switch] $TopLevelOnly, $revision)
{
	# First, make sure we have everything and pull it!
	$pullLog = & hg.exe pull -R $directory
	
	# Next, we can now update to the particular revision.
	$updateFlag = ""
	if ($revision -ne "HEAD")
	{
		$updateFlag = $revision
	}
	$updateLog = & hg.exe -R $directory update $updateFlag
	
	return $pullLog + $updateFlag
}


#
# Set the revision for a directory from a repository path.
#
function set-svn-revision ($directory, $repositoryPath, $revision, [Switch] $TopLevelOnly)
{
    $rflag = "infinity"
    if ($TopLevelOnly)
    {
        $rflag = "immediates"
    }
    return & "$SVNDir\svn.exe" checkout -r $revision --depth $rflag $repositoryPath $directory
}

#
# Set the revision for a directory - this is done when the directory doens't, basically, exist.
# we only pull the to the revision we need.
#
function set-hg-revision ($directory, $repositoryPath, $revision, [Switch] $TopLevelOnly)
{
	$rflag = ""
	$rinfo = ""
	if ($revision -ne "HEAD")
	{
		$rflag = "-r"
		$rinfo = $revision
	}
	
    return & "hg.exe" clone $rflag $rinfo $repositoryPath $directory
}

#################################
#
# The functions that we will export - so we can switch out what ss we are using
# as time goes by.
#

#
# Get the current revision number for the repo
#
function get-revision ($directory)
{
    if (-not (test-path $directory))
    {
        return -1
    }
    return get-svn-revision $directory
}

#
# A repository root is stored in $dir. See if we can figure out what repository type it is
# (svn or hg). If the directory must exist.
#
function guess-repository-manager ($Path = "", $URL = "")
{
	# Is it a path?
	if ($Path -ne "")
	{
		if (-not (Test-Path $Path))
		{
			throw "Unable to determine source control type of directory '$dir' becuase it doesn't exist"
		}
		
		if (Test-Path "$Path\.hg")
		{
			return "hg"
		}
		
		if (Test-Path "$Path\.svn")
		{
			return "svn"
		}
		throw "Directory '$dir' has unknown source control type (not svn or hg)"
	}
	
	# Is it a url?
	if ($URL -ne "")
	{
		if ($URL.Contains("svn"))
		{
			return "svn"
		}
		if ($URL.Contains("hg"))
		{
			return "hg"
		}
		throw "URL '$URL' has unknown source control type (not svn or hg)"
	}
	
	throw "Ask to determine the svn/hg flavor of nothing! Bad!"
}

#
# Set a directory to a particular revision. This means co if it hasn't been
# setup already
#
function set-revision ($directory, [Switch] $TopLevelOnly, $repositoryPath = "", $revision = "HEAD")
{
    if (test-path $directory)
    {
		if ((guess-repository-manager -Path $directory) -eq "svn")
		{
	        update-svn $directory -revision $revision -TopLevelOnly:$TopLevelOnly
		}
		else
		{
	        update-hg $directory -revision $revision -TopLevelOnly:$TopLevelOnly
		}
    }
    else
    {
		if ((guess-repository-manager -URL $repositoryPath) -eq "svn")
		{
	        set-svn-revision $directory -repositoryPath $repositoryPath -revision $revision -TopLevelOnly:$TopLevelOnly
		}
		else
		{
	        set-hg-revision $directory -repositoryPath $repositoryPath -revision $revision -TopLevelOnly:$TopLevelOnly
		}
    }
}

export-modulemember get-revision
export-modulemember set-revision
