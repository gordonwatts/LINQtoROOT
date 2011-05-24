///
/// Auto-generated template by the LINQToTTree utility. Do not modify. Anyway, it is
/// good only for one query, so no use in modifying it!!
///
/// Uses the Velociy template engine.
///
#include "ntuple_CollectionTree.h"
#include "FlowOutputObject.h"

#include "TH1F.h"
#include <TFile.h>

#include <string>

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
		NTH1F_115 = LoadFromInputList<TH1F*>("NTH1F_116");

	}

	/// Called when we are closign the file and shutting down on the slave
	void SlaveTerminate()
	{
		ntuple_CollectionTree::SlaveTerminate();

		NTH1F_115->SetName("NTH1F_115");
		Book(NTH1F_115);
	}

	/// Called with all plots at hand
	void Terminate()
	{
		string outputRootFilename ("queryplots.root");
		TFile *output = new TFile(outputRootFilename.c_str(), "RECREATE");

		TIter next (GetOutputList());
		TObject *o;
		while ((o = next.Next())) {
			if (o->InheritsFrom("FlowOutputObject")) {
				o->Write();
			}
		}
		output->Write();
		output->Close();
		delete output;
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

		{
		  int Int32_119;
		  int anint_124=0;
		  Int32_119=(*(*this).IP_x).size();
		  for (int Int32_118=0; Int32_118 < Int32_119; Int32_118++)
		  {
		    int Int32_122;
		    int anint_123=0;
		    Int32_122=(*(*this).IP_trk_pT)[Int32_118].size();
		    for (int Int32_121=0; Int32_121 < Int32_122; Int32_121++)
		    {
		      if ((*(*this).IP_trk_pT)[Int32_118][Int32_121]/1000>0.5)
		      {
		        anint_123++;
		      }
		    }
		    if (anint_123>3)
		    {
		      anint_124++;
		    }
		  }
		  (*NTH1F_115).Fill(anint_124);
		}

		///
		/// Always return true - we want to go onto the next entry, afterall.
		///

		return true;
	}

private:
	/// Here are the variables that hold things we need to keep around
	/// between entries of the ntuple. So things like the result that has
	/// to be filled on each entry.

	TH1F* NTH1F_115;

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
			GetOutputList()->Add(new FlowOutputObject(o, objName.c_str(), ""));
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
			throw std::exception(("Unable to load from input list the object called '" + name + "'.").c_str());
		}

		T result = static_cast<T>(fInput->FindObject(name.c_str()));
		if (result == 0) {
			std::cout << "Unable to find object '" << name << "' in the input list!" << std::endl;
			throw std::exception(("Unable to load object '" + name + "' from the input list - not found!").c_str());
		}

		return result;
	}
};
