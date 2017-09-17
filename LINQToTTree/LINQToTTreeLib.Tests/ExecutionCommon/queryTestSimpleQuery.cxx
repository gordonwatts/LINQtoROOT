///
/// Auto-generated template by the LINQToTTree utility. Do not modify. Anyway, it is
/// good only for one query, so no use in modifying it!!
///
/// Uses the Velociy template engine.
///
#include "ntuple_CollectionTree.h"

#include "TH1I.h"
#include <TFile.h>

#include <string>
#include <stdexcept>

using std::string;

#ifdef __MAKECINT__
#endif


class queryTestSimpleQuery : public ntuple_CollectionTree
{
public:
	/// So there is some init that CINT will know about
	/// (I don't know why this is required, but it is).
	queryTestSimpleQuery()
	{
	}

	/// Called when we are starting a run
	void SlaveBegin(TTree *t)
	{
		ntuple_CollectionTree::SlaveBegin(t);

		/// Init the variables that we are going to be carrying along with us.
		aInt32_1 = 0;
	}

	/// Called when we are closign the file and shutting down on the slave
	void SlaveTerminate()
	{
		ntuple_CollectionTree::SlaveTerminate();

		TH1I *aInt32_1_hist = new TH1I("aInt32_1", "var transport", 1, 0.0, 1.0);
		aInt32_1_hist->SetDirectory(0);
		aInt32_1_hist->SetBinContent(1, aInt32_1);
		Book(aInt32_1_hist);
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
		  if (((*this).RunNumber)>0)
		  {
		    aInt32_1=aInt32_1+1;
		  }
		}
	}

	/// Here are the variables that hold things we need to keep around
	/// between entries of the ntuple. So things like the result that has
	/// to be filled on each entry.

	int aInt32_1;

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
			GetOutputList()->Add(o);
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
