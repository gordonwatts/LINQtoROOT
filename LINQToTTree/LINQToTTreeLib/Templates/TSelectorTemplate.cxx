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
	void SlaveBegin(TTree *t)
	{
		$baseClassName::SlaveBegin(t);

		/// Init the variables that we are going to be carrying along with us.
		$ResultVariable.VariableName = $ResultVariable.InitialValue;

	}

	/// Called when we are closign the file and shutting down on the slave
	void SlaveTerminate()
	{
		$baseClassName::SlaveTerminate();
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
		/// Run the processing code
		///

#foreach($s in $ProcessStatements)
		$s
#end

		///
		/// Always return true - we want to go onto the next entry, afterall.
		///

		return true;
	}

private:
	/// Here are the variables that hold things we need to keep around
	/// between entries of the ntuple. So things like the result that has
	/// to be filled on each entry.

	$ResultVariable.VariableType $ResultVariable.VariableName;
};
