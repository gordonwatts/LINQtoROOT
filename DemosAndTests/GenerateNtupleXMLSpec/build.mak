#
# Builds the xml file for a ROOT file. Normally this isn't part of the build
# but since I expect things to evolve "rapidly" I suspect I'll be
# appreciative of automating this. of course, this is go paths hard wired
# into it!!
#

DemoJetShapesDir = ..\DemoJetShapes
SolutionDir = ..
ConverterImage = ..\..\LINQToTTree\CmdTFileParser\bin\Debug\CmdTFileParser.exe
NtupleDLLs = ..\NtupleClasses\MuonInBJet_cpp.dll ..\NtupleClasses\BTagJet_cpp.dll

#
# Rules
#

all: $(DemoJetShapesDir)\ntuple.ntupom

$(DemoJetShapesDir)\ntuple.ntupom : $(SolutionDir)\output.root $(ConverterImage) $(NtupleDLLs)
	$(ConverterImage) $(NtupleDLLs) $(SolutionDir)\output.root -o $(DemoJetShapesDir)\ntuple.ntupom

	