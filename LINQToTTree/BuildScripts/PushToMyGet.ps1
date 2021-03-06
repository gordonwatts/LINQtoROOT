#
# Push non-symbol files up
#
param([string] $APIKey)
set-alias nuget LINQToTTree\.nuget\nuget.exe

$packages = get-childItem *.nupkg | ? {!$_.FullName.Contains("symbols")}

foreach ($p in $packages)
{
	nuget push $p $APIKey -Source "https://www.myget.org/F/rootdotnet/"
}

