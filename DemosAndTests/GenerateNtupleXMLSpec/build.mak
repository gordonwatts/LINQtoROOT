#
# Builds the xml file for a ROOT file. Normally this isn't part of the build
# but since I expect things to evolve "rapidly" I suspect I'll be
# appreciative of automating this. of course, this is go paths hard wired
# into it!!
#

DemoJetShapesDir = ..\DemoJetShapes
SolutionDir = ..
ConverterImage = ..\..\LINQToTTree\CmdTFileParser\bin\Debug\CmdTFileParser.exe
ClassConverterImage = ..\..\LINQToTTree\CmdGenerateLINQClasses\bin\Debug\CmdGenerateLINQClasses.exe
NtupleDLLs = ..\NtupleClasses\MuonInBJet_cpp.dll ..\NtupleClasses\BTagJet_cpp.dll

ClassCSFile = $(DemoJetShapesDir)\ntupleDataModel.cs

#
# Rules
#

all: $(ClassCSFile)

$(DemoJetShapesDir)\ntuple.ntupom : $(SolutionDir)\output.root $(ConverterImage) $(NtupleDLLs)
	$(ConverterImage) $(NtupleDLLs) $(SolutionDir)\output.root -o $(DemoJetShapesDir)\ntuple.ntupom

$(ClassCSFile) : $(DemoJetShapesDir)\ntuple.ntupom $(ClassConverterImage)
	$(ClassConverterImage) $(DemoJetShapesDir)\ntuple.ntupom $(ClassCSFile)
