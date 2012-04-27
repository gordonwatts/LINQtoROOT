#Author: gwatts
#Date: 4/22/2012 10:14:17 PM
#Script: installer
#
# Fragment to correctly register our snapin. Needs to happen only one time.
#
[CmdletBinding()]
param (
	[Parameter()]
	[string] $assemblyPath="PSPROOFUtils.dll"
)

#
# Is it registered? If so, load it up!
#

$s = Get-PSSnapin -Registered | ? {$_.Name -eq "PROOFSnapIn"}
if ($s)
{
	Write-Host "Snap-in already declared"
	add-pssnapin "PROOFSnapIn"
	return
}

Write-Host "Snapin is not known... I hope you are running as admin for this!"

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

# Now, load it in for use
add-pssnapin "PROOFSnapIn"