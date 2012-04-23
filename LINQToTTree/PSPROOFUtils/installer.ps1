#Author: gwatts
#Date: 4/22/2012 10:14:17 PM
#Script: installer
#
# Fragment to correctly register our snapin. Needs to happen only one time.
#
[CmdletBinding()]
param (
	[Parameter(mandatory=$true)]
	[string] $assemblyPath
)

# Param checks

if (-not $(Test-Path "$assemblyPath"))
{
	Write-Host "Unable to locate assembly $assemblyPath"
	return 1
}

# What we do depends on what bit-nes we are running under.
$FrameworkName = "Framework"
if ([IntPtr]::Size -eq 8)
{
	$FrameworkName = "Framework64"
}

# Now, run the thing
Set-Alias installutil $env:windir\Microsoft.NET\$FrameworkName\v4.0.30319\installutil
installutil "$assemblyPath"

# Just to show off
Get-PSSnapin -Registered
