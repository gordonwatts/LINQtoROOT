///
/// Auto-generated template by the LINQToTTree utility. Do not modify. Anyway, it is
/// good only for one query, so no use in modifying it!!
///
/// Uses the Velociy template engine.
///
#include "ntuple_CollectionTree.h"

#include "TLorentzVector.h"
#include "TVector3.h"
#include "TH1F.h"
#include <TFile.h>

#include <string>
#include <stdexcept>

using std::string;

#ifdef __MAKECINT__
#endif


class query0 : public ntuple_CollectionTree
{
public:
	/// So there is some init that CINT will know about
	/// (I don't know why this is required, but it is).
	query0()
	{
	}

	/// Called when we are starting a run
	void SlaveBegin(TTree *t)
	{
		ntuple_CollectionTree::SlaveBegin(t);

		/// Init the variables that we are going to be carrying along with us.
		aNTH1F_3 = LoadFromInputList<TH1F*>("aNTH1F_4");
		aNTH1F_7 = LoadFromInputList<TH1F*>("aNTH1F_8");
		aNTH1F_10 = LoadFromInputList<TH1F*>("aNTH1F_11");
		aNTH1F_18 = LoadFromInputList<TH1F*>("aNTH1F_19");
		aNTH1F_26 = LoadFromInputList<TH1F*>("aNTH1F_27");
		aNTH1F_30 = LoadFromInputList<TH1F*>("aNTH1F_31");
		aNTH1F_33 = LoadFromInputList<TH1F*>("aNTH1F_34");
		aNTH1F_41 = LoadFromInputList<TH1F*>("aNTH1F_42");
	}

	/// Called when we are closign the file and shutting down on the slave
	void SlaveTerminate()
	{
		ntuple_CollectionTree::SlaveTerminate();

		aNTH1F_3->SetName("aNTH1F_3");
		Book(aNTH1F_3);
		aNTH1F_7->SetName("aNTH1F_7");
		Book(aNTH1F_7);
		aNTH1F_10->SetName("aNTH1F_10");
		Book(aNTH1F_10);
		aNTH1F_18->SetName("aNTH1F_18");
		Book(aNTH1F_18);
		aNTH1F_26->SetName("aNTH1F_26");
		Book(aNTH1F_26);
		aNTH1F_30->SetName("aNTH1F_30");
		Book(aNTH1F_30);
		aNTH1F_33->SetName("aNTH1F_33");
		Book(aNTH1F_33);
		aNTH1F_41->SetName("aNTH1F_41");
		Book(aNTH1F_41);
	}

	/// Called with all plots at hand
	void Terminate()
	{
		ntuple_CollectionTree::Terminate();
	}

	/// Called to process an entry
	bool Process(Long64_t entry)
	{
		///
		/// Get to the proper entry
		///

		fDirector.SetReadEntry(entry);

		///
		/// Run the processing code
		///

		ExecuteQueryBlock0 ();

		///
		/// Always return true - we want to go onto the next entry, afterall.
		///

		return true;
	}

private:

	///
	/// The query block functions
	///
	void ExecuteQueryBlock0 ()
	{
		{
		  int aInt32_48 = (*this).McEventCollection_p4_GEN_EVENT.m_genParticles.GetEntries();
		  for (int aInt32_2=0; aInt32_2 < aInt32_48; aInt32_2++)
		  {
		    TLorentzVector* aNTLorentzVector_5;
		    TLorentzVector tlz0;
		    tlz0.SetXYZM((((*this).McEventCollection_p4_GEN_EVENT.m_genParticles.m_px)[aInt32_2]), (((*this).McEventCollection_p4_GEN_EVENT.m_genParticles.m_py)[aInt32_2]), (((*this).McEventCollection_p4_GEN_EVENT.m_genParticles.m_pz)[aInt32_2]), (((*this).McEventCollection_p4_GEN_EVENT.m_genParticles.m_m)[aInt32_2]));
		    aNTLorentzVector_5 = &tlz0;
		    (*aNTH1F_3).Fill(((*aNTLorentzVector_5).Pt())/1000.0,1.0);
		    (*aNTH1F_7).Fill(((double)((*this).McEventCollection_p4_GEN_EVENT.m_genParticles.m_pdgId)[aInt32_2]),1.0);
		    if ((((*this).McEventCollection_p4_GEN_EVENT.m_genParticles.m_endVtx)[aInt32_2])!=0)
		    {
		      bool aBoolean_13=false;
		      int aInt32_14=-1;
		      int aInt32_49 = (*this).McEventCollection_p4_GEN_EVENT.m_genVertices.GetEntries();
		      for (int aInt32_12=0; aInt32_12 < aInt32_49; aInt32_12++)
		      {
		        if ((((*this).McEventCollection_p4_GEN_EVENT.m_genParticles.m_endVtx)[aInt32_2])==(((*this).McEventCollection_p4_GEN_EVENT.m_genVertices.m_barcode)[aInt32_12]))
		        {
		          if (!aBoolean_13) {
		            aInt32_14 = aInt32_12;
		            aBoolean_13 = true;
		          }
		        }
		      }
		      if (!aBoolean_13) {
		        throw std::runtime_error("First predicate executed on a null sequence");
		      }
		      TVector3 aNTVector3_15(((*this).McEventCollection_p4_GEN_EVENT.m_genVertices.m_x)[aInt32_14],((*this).McEventCollection_p4_GEN_EVENT.m_genVertices.m_y)[aInt32_14],((*this).McEventCollection_p4_GEN_EVENT.m_genVertices.m_z)[aInt32_14]);
		      TVector3 *aNTVector3_16 = &aNTVector3_15;
		      (*aNTH1F_10).Fill((*aNTVector3_16).Mag(),1.0);
		      TVector3 aNTVector3_23(((*this).McEventCollection_p4_GEN_EVENT.m_genVertices.m_x)[aInt32_14],((*this).McEventCollection_p4_GEN_EVENT.m_genVertices.m_y)[aInt32_14],((*this).McEventCollection_p4_GEN_EVENT.m_genVertices.m_z)[aInt32_14]);
		      TVector3 *aNTVector3_24 = &aNTVector3_23;
		      (*aNTH1F_18).Fill((*aNTVector3_24).Mag(),1.0);
		    }
		    if ((((*this).McEventCollection_p4_GEN_EVENT.m_genParticles.m_pdgId)[aInt32_2])==36)
		    {
		      TLorentzVector* aNTLorentzVector_28;
		      TLorentzVector tlz1;
		      tlz1.SetXYZM((((*this).McEventCollection_p4_GEN_EVENT.m_genParticles.m_px)[aInt32_2]), (((*this).McEventCollection_p4_GEN_EVENT.m_genParticles.m_py)[aInt32_2]), (((*this).McEventCollection_p4_GEN_EVENT.m_genParticles.m_pz)[aInt32_2]), (((*this).McEventCollection_p4_GEN_EVENT.m_genParticles.m_m)[aInt32_2]));
		      aNTLorentzVector_28 = &tlz1;
		      (*aNTH1F_26).Fill(((*aNTLorentzVector_28).Pt())/1000.0,1.0);
		      (*aNTH1F_30).Fill(((double)((*this).McEventCollection_p4_GEN_EVENT.m_genParticles.m_pdgId)[aInt32_2]),1.0);
		      if ((((*this).McEventCollection_p4_GEN_EVENT.m_genParticles.m_endVtx)[aInt32_2])!=0)
		      {
		        bool aBoolean_36=false;
		        int aInt32_37=-1;
		        int aInt32_50 = (*this).McEventCollection_p4_GEN_EVENT.m_genVertices.GetEntries();
		        for (int aInt32_35=0; aInt32_35 < aInt32_50; aInt32_35++)
		        {
		          if ((((*this).McEventCollection_p4_GEN_EVENT.m_genParticles.m_endVtx)[aInt32_2])==(((*this).McEventCollection_p4_GEN_EVENT.m_genVertices.m_barcode)[aInt32_35]))
		          {
		            if (!aBoolean_36) {
		              aInt32_37 = aInt32_35;
		              aBoolean_36 = true;
		            }
		          }
		        }
		        if (!aBoolean_36) {
		          throw std::runtime_error("First predicate executed on a null sequence");
		        }
		        TVector3 aNTVector3_38(((*this).McEventCollection_p4_GEN_EVENT.m_genVertices.m_x)[aInt32_37],((*this).McEventCollection_p4_GEN_EVENT.m_genVertices.m_y)[aInt32_37],((*this).McEventCollection_p4_GEN_EVENT.m_genVertices.m_z)[aInt32_37]);
		        TVector3 *aNTVector3_39 = &aNTVector3_38;
		        (*aNTH1F_33).Fill((*aNTVector3_39).Mag(),1.0);
		        TVector3 aNTVector3_46(((*this).McEventCollection_p4_GEN_EVENT.m_genVertices.m_x)[aInt32_37],((*this).McEventCollection_p4_GEN_EVENT.m_genVertices.m_y)[aInt32_37],((*this).McEventCollection_p4_GEN_EVENT.m_genVertices.m_z)[aInt32_37]);
		        TVector3 *aNTVector3_47 = &aNTVector3_46;
		        (*aNTH1F_41).Fill((*aNTVector3_47).Mag(),1.0);
		      }
		    }
		  }
		}
	}

	/// Here are the variables that hold things we need to keep around
	/// between entries of the ntuple. So things like the result that has
	/// to be filled on each entry.

	TH1F* aNTH1F_3;
	TH1F* aNTH1F_7;
	TH1F* aNTH1F_10;
	TH1F* aNTH1F_18;
	TH1F* aNTH1F_26;
	TH1F* aNTH1F_30;
	TH1F* aNTH1F_33;
	TH1F* aNTH1F_41;

	/// Store an object to send back. We encase it in a FlowObject because the list
	/// of objects that goes back is "flat" and FlowObject holds onto
	/// a tag that tells us where this should be stored later. Helps!
	void Book(TObject *o)
	{
		string objName("");
		if (o->InheritsFrom("TNamed")) {
			TNamed *n = static_cast<TNamed*>(o);
			objName = n->GetName();
		} else {
			objName = o->ClassName();
		}

		///
		/// If this is a replacement, then boom!
		///

		if (dynamic_cast<TObject*> (GetOutputList()->FindObject(objName.c_str())) == 0) {
			GetOutputList()->Add(o->Clone());
		}
	}

	///
	/// Load from the input list a name of some item
	///
	template<class T>
	T LoadFromInputList(const std::string &name)
	{
		if (fInput == 0) {
			std::cout << "Unable to load (object '" << name << "') from the input list - the list is null!" << std::endl;
			throw std::runtime_error(("Unable to load from input list the object called '" + name + "'.").c_str());
		}

		T result = static_cast<T>(fInput->FindObject(name.c_str()));
		if (result == 0) {
			std::cout << "Unable to find object '" << name << "' in the input list!" << std::endl;
			throw std::runtime_error(("Unable to load object '" + name + "' from the input list - not found!").c_str());
		}

		return result;
	}
};
