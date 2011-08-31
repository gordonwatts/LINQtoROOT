param($installPath, $toolsPath, $package, $project) 
#
# Setup environment for dealing with the the LINQToTTree environment.
#

Import-Module -Force "$toolsPath\LINQToTTreeCommands.psm1"

Write-Host "Availible commands for LINQToTTree package:"
Write-Host "Write-TTree-MetaData <path-to-root-file>"
Write-Host "  Parses all TTree's in root file and generates meta data needed"
Write-Host "  to run the TTree. Inserts resulting files in the current default project."
