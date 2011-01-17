///
/// Auto-generated template by the LINQToTTree utility. Do not modify. Anyway, it is
/// good only for one query, so no use in modifying it!!
///
/// Uses the Velociy template engine.
///
\#include "$baseClassInclude"

class query : public $baseClassName
{
public:
	/// So there is some init that CINT will know about
	/// (I don't know why this is required, but it is).
	query()
	{
	}

	/// Called when we are starting a run
	void SlaveBegin(TTree*)
	{
	}

	/// Called when we are closign the file and shutting down on the slave
	void SlaveTerminate()
	{
	}

	/// Called with all plots at hand
	void Terminate()
	{
	}

	/// Called to process an entry
	bool Process(Long64_t entry)
	{
		///
		/// Get to the proper entry
		///

		fDirector.SetReadEntry(entry);

		///
		/// Always return true - we want to go onto the next entry, afterall.
		///

		return true;
	}
};
