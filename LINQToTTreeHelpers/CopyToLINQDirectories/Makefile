#
# Make file to copy/clean items from the LINQ helpers library to the local distro. This just makes developing this library a bit simpler.
#

LIB=$(SolutionDir)LINQToTreeHelpers\bin\$(Configuration)

filelist = $(LIB)\LINQToTreeHelpers.dll $(LIB)\LINQToTreeHelpers.xml $(LIB)\LINQToTreeHelpers.pdb $(LIB)\Sprache.dll

libDir = $(ROOTSYS)\NETWrappers\LINQ\lib
destList = $(libDir)\LINQToTreeHelpers.dll $(libDir)\LINQToTreeHelpers.xml $(libDir)\LINQToTreeHelpers.pdb $(libDir)\Sprache.dll

#
# Do the default building
#
all: $(destList)

$(destList) : $(filelist)
	!copy $? $(libDir)

#
# Clean up everything that got copied over.
#
clean:
	del $(destList)
 