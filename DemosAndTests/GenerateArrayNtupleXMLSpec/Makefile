#
# Builds the xml file for a ROOT file. Normally this isn't part of the build
# but since I expect things to evolve "rapidly" I suspect I'll be
# appreciative of automating this. of course, this is go paths hard wired
# into it!!
#

clean: cleanall

DemoJetShapesDir = $(SolutionDir)\DemoNtupleArrays
ClassConverterImage = $(SolutionDir)\..\LINQToTTree\CmdGenerateLINQClasses\bin\Debug\CmdGenerateLINQClasses.exe

ClassCSFile = $(DemoJetShapesDir)\ntupleDataModel.cs
ClassCSFile1 = $(DemoJetShapesDir)\vtupleDataModel.cs

ConverterImage = $(SolutionDir)\..\LINQToTTree\CmdTFileParser\bin\Debug\CmdTFileParser.exe

#
# Rules
#

all: $(ClassCSFile) $(ClassCSFile1)

$(ClassCSFile) : $(DemoJetShapesDir)\hvsample.ntupom $(DemoJetShapesDir)\CollectionTreeConfig-000.ntup $(ClassConverterImage) 
    $(ClassConverterImage) $(DemoJetShapesDir)\hvsample.ntupom $(ClassCSFile)

$(ClassCSFile1) : $(DemoJetShapesDir)\btag-slim.ntupom $(DemoJetShapesDir)\vtupleConfig-000.ntup $(ClassConverterImage) 
    $(ClassConverterImage) $(DemoJetShapesDir)\btag-slim.ntupom $(ClassCSFile1)

#
# Clean up the above files to force a restart
#
cleanall:
    del /Q $(DemoJetShapesDir)\ntuple.ntupom
    del /Q $(ClassCSFile)