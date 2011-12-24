// TTreeParserCPP.Tests.h

#pragma once

#include "ATestRootClass.h"

#include <string>
#include <sstream>
#include <vector>

#include <TTree.h>
#include <TLorentzVector.h>
#include <TList.h>
#include <TFile.h>

using std::string;
using std::ostringstream;
using std::vector;

using namespace System;
using namespace System::Runtime::InteropServices;

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

		/// Create a tree with double-colon name number of int's.
		static ROOTNET::NTTree ^CreateTreeWithDoubleColorName()
		{
			int bogus;

			TTree *t = new TTree("dude", "left field");

			auto b = t->Branch("fork", &bogus);
			b->SetName("dude::fork");
			b->SetTitle("dude::fork");

			for (int i = 0; i < 10; i++) {
				bogus = i;
				t->Fill();
			}

			return gcnew ROOTNET::NTTree(t);
		}

		static ROOTNET::NTTree ^CreateOneIntTree(int numberOfEntries)
		{
			int bogus;

			TTree *t = new TTree("dude", "left field");

			t->Branch ("run", &bogus);
			bogus = 10;
			for (int i = 0; i < numberOfEntries; i++) {
				t->Fill();
			}

			return gcnew ROOTNET::NTTree(t);
		}

		static ROOTNET::NTTree ^CreateWithIntName(String ^leaf_name)
		{
			int bogus;

			TTree *t = new TTree("dude", "left field");

			char *native_string = (char *)Marshal::StringToHGlobalAnsi(leaf_name).ToPointer();
			string name (native_string);
			Marshal::FreeHGlobal(IntPtr((void *)native_string));
			t->Branch (name.c_str(), &bogus);
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

#ifdef notyet
		/// Create a tree with custom ROOT class
		static ROOTNET::NTTree ^CreateCustomROOTClassTree()
		{
			ATestRootClass bogus;

			TTree *t = new TTree("dude", "left field");

			t->Branch ("test", &bogus);

			return gcnew ROOTNET::NTTree(t);
		}
#endif

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

		/// Create a tree with some number of TLZ's.
		static ROOTNET::NTTree ^CreateTreeWithSimpleSingleVector(int entries)
		{
			vector<int> bogus;

			TTree *t = new TTree("dude", "left field");
			auto brAddr = t->Branch ("myvectorofint", &bogus);

			for (int i = 0; i < entries; i++) {
				bogus.clear();
				for (int j = 0; j < 10; j++) {
					bogus.push_back(j);
				}
				t->Fill();
			}
			t->ResetBranchAddress(brAddr);

			return gcnew ROOTNET::NTTree(t);
		}

		/// Create a tree with some number of TLZ's.
		static ROOTNET::NTTree ^CreateTreeWithIndexedSimpleVector(int entries)
		{
			int bogus[20];
			int ientries = 0;

			TTree *t = new TTree("dude", "left field");
			auto brAddrEnt = t->Branch ("n", &ientries);
			auto brAddrArr = t->Branch ("arr[n]/I", &ientries);

			for (int i = 0; i < entries; i++) {
				ientries = 10;
				for (int j = 0; j < ientries; j++) {
					bogus[i] = j;
				}
				t->Fill();
			}
			t->ResetBranchAddress(brAddrEnt);
			t->ResetBranchAddress(brAddrArr);

			return gcnew ROOTNET::NTTree(t);
		}

		static ROOTNET::NTTree ^CreateTreeWithSimpleSingleVector(int entries, int vectorsize)
		{
			vector<int> bogus;

			TTree *t = new TTree("dude", "left field");
			auto brAddr = t->Branch ("myvectorofint", &bogus);

			for (int i = 0; i < entries; i++) {
				bogus.clear();
				for (int j = 0; j < vectorsize; j++) {
					bogus.push_back(j);
				}
				t->Fill();
			}
			t->ResetBranchAddress(brAddr);

			return gcnew ROOTNET::NTTree(t);
		}

		/// Create a tree with two vectors
		static ROOTNET::NTTree ^CreateTreeWithSimpleDoubleVector(int entries)
		{
			vector<int> bogus;
			vector<int> bogus1;

			TTree *t = new TTree("dude", "left field");
			auto brAddr = t->Branch ("myvectorofint", &bogus);
			auto brAddr1 = t->Branch ("myvectorofint1", &bogus1);

			for (int i = 0; i < entries; i++) {
				bogus.clear();
				bogus1.clear();
				for (int j = 0; j < 10; j++) {
					bogus.push_back(j);
				}
				for (int j = 0; j < 20; j++) {
					bogus1.push_back(j);
				}
				t->Fill();
			}
			t->ResetBranchAddress(brAddr);
			t->ResetBranchAddress(brAddr1);

			return gcnew ROOTNET::NTTree(t);
		}

		/// Create a tree with some number of TLZ's.
		static ROOTNET::NTTree ^CreateTreeWithNonVectorArray()
		{
			int myarr[20];
			int lenUsed;

			TTree *t = new TTree("dude", "left field");
			auto brAddrL = t->Branch("lenUsed", &lenUsed);
			auto brAddr = t->Branch ("myarr", &myarr, "myarr[lenUsed]/I");

			t->ResetBranchAddress(brAddr);
			t->ResetBranchAddress(brAddrL);

			return gcnew ROOTNET::NTTree(t);
		}

		/// Create a tree with an integer array and something "normal"
		static ROOTNET::NTTree ^CreateTreeWithSimpleSingleVectorAndItem(int entries)
		{
			vector<int> bogus;
			int other;

			TTree *t = new TTree("dude", "left field");
			auto brAddr = t->Branch ("myvectorofint", &bogus);
			auto brAddr1 = t->Branch("other", &other);

			for (int i = 0; i < entries; i++) {
				bogus.clear();
				other = i;
				for (int j = 0; j < 10; j++) {
					bogus.push_back(j);
				}
				t->Fill();
			}
			t->ResetBranchAddress(brAddr);
			t->ResetBranchAddress(brAddr1);

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
		static ROOTNET::NTTree ^CreateVectorTreeWithTypedef()
		{
			vector<Int32> ivector;

			TTree *t = new TTree("dude", "left field");

			t->Branch("i", &ivector);

			return gcnew ROOTNET::NTTree(t);
		}

		/// Create a tree with some number of vector's in it.
		static ROOTNET::NTTree ^CreateSingleItemTree()
		{
			int intVar;
			unsigned int uintVar;
			short shortVar;
			unsigned short ushortVar;
			Long64_t longVar;
			ULong64_t ulongVar;
			bool bVar;

			TTree *t = new TTree("dude", "left field");

			t->Branch("intvar", &intVar);
			t->Branch("shortvar", &shortVar);
			t->Branch("longvar", &longVar);

			t->Branch("uintvar", &uintVar);
			t->Branch("ushortvar", &ushortVar);
			t->Branch("ulongvar", &ulongVar);

			t->Branch("bvar", &bVar);

			return gcnew ROOTNET::NTTree(t);
		}

		/// Create a tree with things like unsigned int, unsigned short, etc.
		static ROOTNET::NTTree ^CreateWithOddTypes()
		{
			vector<unsigned int> uintVector;
			vector<Int_t> templateVector;

			TTree *t = new TTree("dude", "left field");
			t->Branch("uint_vector", &uintVector);
			t->Branch("i32_vector", &templateVector);

			return gcnew ROOTNET::NTTree(t);
		}

		/// Create a tree with things like unsigned int, unsigned short, etc.
		static ROOTNET::NTTree ^CreateWithStringTypes()
		{
			vector<string> strVector;

			TTree *t = new TTree("dude", "left field");
			t->Branch("str_vector", &strVector);

			return gcnew ROOTNET::NTTree(t);
		}

		/// Create a tree with some number of vector's in it.
		static ROOTNET::NTTree ^CreateVectorVectorTree()
		{
			vector<vector<double> > dvector;
			vector<vector<float> > fvector;

			TTree *t = new TTree("dude", "left field");

			t->Branch("vector", &dvector);
			t->Branch("vector2", &fvector);

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
