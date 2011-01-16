///
/// Auto-generated template by the LINQToTTree utility. Do not modify. Anyway, it is
/// good only for one query, so no use in modifying it!!
///
/// Uses the Velociy template engine.
///
\#include "$baseClassInclude"

class query : public $baseClassName
{
	/// Called when we are starting a run
	void query::SlaveBegin(TTree*)
	{
	}

	/// Called when we are closign the file and shutting down on the slave
	void query::SlaveTerminate()
	{
	}

	/// Called with all plots at hand
	void query::Terminate()
	{
	}

	/// Called to process an entry
	bool query::Process(Long64_t entry)
	{
		$baseClassName::Process(entry);

		return true;
	}
};
