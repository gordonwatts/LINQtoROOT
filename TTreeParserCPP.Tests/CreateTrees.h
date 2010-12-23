// TTreeParserCPP.Tests.h

#pragma once

#include <string>
#include <sstream>
#include <vector>

#include <TTree.h>
#include <TLorentzVector.h>
#include <TList.h>

using std::string;
using std::ostringstream;
using std::vector;

using namespace System;

namespace TTreeParserCPPTests {
			struct stuff
		{
			int junk1;
			double junk2;
			short junk3;
		};


	///
	/// Helps to create tree's for testing
	public ref class CreateTrees
	{
	public:

		///
		/// Create a tree with some number of int's.
		static ROOTNET::NTTree ^CreateWithIntOnly(int numberOfInts)
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
		static ROOTNET::NTTree ^CreateWithTLZOnly(int numberOfTLZ)
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

		/// Create a tree with some number of TLZ's.
		static ROOTNET::NTTree ^CreateWithTLZVector()
		{
			vector<TLorentzVector> bogus;
			TLorentzVector v;

			TTree *t = new TTree("dude", "left field");
			/// Shoot - doesn't seem to know about this either! :(

			t->Branch ("myvectoroftlz", &bogus);
			int n = t->GetListOfBranches()->GetEntries();

			return gcnew ROOTNET::NTTree(t);
		}

		/// Create a tree with some number of vector's in it.
		static ROOTNET::NTTree ^CreateVectorTree()
		{
			vector<double> dvector;
			vector<int> ivector;
			//vector<short> svector; // Not known to root by default!
			vector<bool> bvector;
			//vector<float> fvector; // Not in this default root impl! Very odd!

			TTree *t = new TTree("dude", "left field");

			t->Branch("d_double", &dvector);
			t->Branch("i_vector", &ivector);
			//t->Branch("s_vector", &svector);
			t->Branch("b_vector", &bvector);
			//t->Branch("f_fector", &fvector);

			return gcnew ROOTNET::NTTree(t);
		}

		/// Create a tree with some number of vector's in it.
		static ROOTNET::NTTree ^CreateVectorVectorTree()
		{
			vector<vector<double> > dvector;

			TTree *t = new TTree("dude", "left field");

			t->Branch("vector", &dvector);

			return gcnew ROOTNET::NTTree(t);
		}

		static ROOTNET::NTTree ^CreateListOfLeavesTree()
		{
			TTree *t = new TTree("dude", "left field");
			stuff temp;

			t->Branch("mybranch", &(temp.junk1), "junk1/I:junk2/D:junk3/F");

			return gcnew ROOTNET::NTTree(t);
		}
	};
}
