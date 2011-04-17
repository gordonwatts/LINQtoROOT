#
# Build in a directory (LINQ) everything needed to send
# this onto a user's machine in the form of a ZIP file.
#
# You need to have 7-zip installed for this make file to work as-is.
#

zip7App="C:\Program Files\7-Zip\7z.exe"

ResultName=LINQ
ResultDirectoryName=$(BuildConfig)\$(ResultName)

LINQToTTreeLibDir=$(SolutionDir)\LINQToTTreeLib\bin\$(BuildConfig)
CmdGenerateLINQClassesDir=$(SolutionDir)\CmdGenerateLINQClasses\bin\$(BuildConfig)
CmdTFileParser=$(SolutionDir)\CmdTFileParser\bin\$(BuildConfig)
MSBuildTask=$(SolutionDir)\MSBuildTasks\bin\$(BuildConfig)

# Rules

all: $(SolutionDir)\$(ResultName)-$(BuildConfig).zip

clean:
	del /Q $(SolutionDir)\$(ResultName)-$(BuildConfig).zip
	rmdir /S /Q $(ResultDirectoryName)

#
# Create a zip file for uploading
#

$(SolutionDir)\$(ResultName)-$(BuildConfig).zip : $(ResultDirectoryName)\date.txt
	cd $(ResultDirectoryName)\..
	$(zip7App) a $(SolutionDir)\$(ResultName)-$(BuildConfig).zip ".\$(ResultName)"

#
# Create the directory that we will be zipping
#

$(ResultDirectoryName)\date.txt : $(LINQToTTreeLibDir)\LINQToTTreeLib.dll \
		$(CmdGenerateLINQClassesDir)\CmdGenerateLINQClasses.exe \
		$(CmdTFileParser)\CmdTFileParser.exe \
		$(MSBuildTask)\MSBuildTasks.dll \
		$(SolutionDir)\LINQToTTreeLib\Templates\TSelectorTemplate.cxx \
		$(SolutionDir)\LINQToTTreeLib\ConfigData\default.classmethodmappings
	if NOT EXIST $(ResultDirectoryName) mkdir $(ResultDirectoryName)
	if NOT EXIST $(ResultDirectoryName)\bin mkdir $(ResultDirectoryName)\bin
	if NOT EXIST $(ResultDirectoryName)\lib mkdir $(ResultDirectoryName)\lib
	if NOT EXIST $(ResultDirectoryName)\build mkdir $(ResultDirectoryName)\build
	copy $(LINQToTTreeLibDir) $(ResultDirectoryName)\lib
	copy $(CmdGenerateLINQClassesDir) $(ResultDirectoryName)\bin
	copy $(CmdTFileParser) $(ResultDirectoryName)\bin
	copy $(MSBuildTask) $(ResultDirectoryName)\build
	if not exist $(ResultDirectoryName)\AppData\Local\LINQToTTree\Config mkdir $(ResultDirectoryName)\AppData\Local\LINQToTTree\Config
	copy $(SolutionDir)\LINQToTTreeLib\ConfigData\default.classmethodmappings $(ResultDirectoryName)\AppData\Local\LINQToTTree\Config
	xcopy /Y "$(SolutionDir)\LINQToTTreeLib\InfrastructureSourceFiles" "$(ResultDirectoryName)\AppData\Local\LINQToTTree\InfrastructureSourceFiles\"
	xcopy /Y "$(SolutionDir)\LINQToTTreeLib\Templates" "$(ResultDirectoryName)\AppData\Local\LINQToTTree\Templates\"
	time /T > $(ResultDirectoryName)\date.txt
	date /T >> $(ResultDirectoryName)\date.txt

	