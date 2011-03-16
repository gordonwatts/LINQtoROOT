#
# Build the ROOT objects into dll's
#

all: netwrapper
clean: cleanlibraries cleannetwrapper

#
# Some Defines to make life simpler
#

SolutionDir = ..\\

ROOT = root -l -b -q
ADDON = "c:\root\NETWrappers\bin\ROOT.NET Addon Library Converter.exe" c:\root\NETWrappers $(SolutionDir)

DLLLibraries = MuonInBJet_cpp.dll BTagJet_cpp.dll

#
# Build the project that translates
#

WrapperProjectName = NTupleWrappers

netwrapper: $(SolutionDir)$(WrapperProjectName)\\$(WrapperProjectName).vcxproj

$(SolutionDir)$(WrapperProjectName)\\$(WrapperProjectName).vcxproj: libraries 
	$(ADDON) $(WrapperProjectName) $(DLLLibraries)

cleannetwrapper:
	del /Q $(SolutionDir)$(WrapperProjectName)\\

#
# The root dll's that do the build
#

libraries: $(DLLLibraries)

MuonInBJet_cpp.dll BTagJet_cpp.dll : MuonInBJet.cpp MuonInBJet.h BTagJet.cpp BTagJet.h
	$(ROOT) compile.C(\"MuonInBJet.cpp\")
	$(ROOT) compile.C(\"BTagJet.cpp\")

cleanlibraries:
	del /Q MuonInBJet_cpp.dll
	del /Q BTagJet_cpp.dll
