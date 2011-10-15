@echo off

REM
REM Run the ps script to build all the nuget packages for this guy.
REM

cd ..\..

powershell -file LINQToTTree\BuildScripts\cibuilder.ps1 -NameSuffix -%COMPUTERNAME%

