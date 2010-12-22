// TTreeParserCPP.Tests.h

#pragma once

#include <string>
#include <sstream>

#include <TTree.h>
#include <TLorentzVector.h>
#include <TList.h>

using std::string;
using std::ostringstream;

using namespace System;

namespace TTreeParserCPPTests {

	///
	/// Helps to create tree's for testing
	public ref class CreateTrees
	{
	public:

		///
		/// Create a tree with some number of int's.
		static ROOTNET::NTTree ^CreateWithIntOnly(System::String ^filename, int numberOfInts)
		{
			int bogus;

			TTree *t = new TTree("dude", "left field");

			string name ("item_");
			for (int i = 0; i < numberOfInts; i++) {
				t->Branch (name.c_str(), &bogus);
				name = name + "a";
			}
			bogus = 10;
			t->Fill();

			return gcnew ROOTNET::NTTree(t);
		}

		/// Create a tree with some number of TLZ's.
		static ROOTNET::NTTree ^CreateWithTLZOnly(System::String ^filename, int numberOfTLZ)
		{
			TLorentzVector bogus;

			TTree *t = new TTree("dude", "left field");

			string name ("item_");
			for (int i = 0; i < numberOfTLZ; i++) {
				t->Branch (name.c_str(), &bogus);
				name = name + "a";
			}

			return gcnew ROOTNET::NTTree(t);
		}
	};
}
