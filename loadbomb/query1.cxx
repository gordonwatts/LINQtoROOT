///
/// Auto-generated template by the LINQToTTree utility. Do not modify. Anyway, it is
/// good only for one query, so no use in modifying it!!
///
/// Uses the Velociy template engine.
///
#include "ntuple_CollectionTree.h"
#include "FlowOutputObject.h"

#include "TH1I.h"
#include <TFile.h>

#include <string>

using std::string;

#ifdef __MAKECINT__
#endif


class query1 : public ntuple_CollectionTree
{
public:
	/// So there is some init that CINT will know about
	/// (I don't know why this is required, but it is).
	query1()
	{
	}

	/// Called when we are starting a run
	void SlaveBegin(TTree *t)
	{
		ntuple_CollectionTree::SlaveBegin(t);

		/// Init the variables that we are going to be carrying along with us.
		anint_133 = 0;

	}

	/// Called when we are closign the file and shutting down on the slave
	void SlaveTerminate()
	{
		ntuple_CollectionTree::SlaveTerminate();

		TH1I *anint_133_hist = new TH1I("anint_133", "var transport", 1, 0.0, 1.0);
		anint_133_hist->SetBinContent(1, anint_133);
		Book(anint_133_hist);
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
		  int Int32_127;
		  int anint_128=0;
		  Int32_127=(*(*this).IP_x).size();
		  for (int Int32_126=0; Int32_126 < Int32_127; Int32_126++)
		  {
		    anint_128++;
		    if (anint_128 <= 1)
		    {
		      int Int32_131;
		      int anint_132=0;
		      Int32_131=(*(*this).IP_trk_pT)[Int32_126].size();
		      for (int Int32_130=0; Int32_130 < Int32_131; Int32_130++)
		      {
		        if ((*(*this).IP_trk_pT)[Int32_126][Int32_130]/1000>0.5)
		        {
		          anint_132++;
		        }
		      }
		      if (anint_132>3)
		      {
		        anint_133++;
		      }
		    }
		  }
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

	int anint_133;

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
