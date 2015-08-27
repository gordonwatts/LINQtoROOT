///
/// Auto-generated template by the LINQToTTree utility. Do not modify. Anyway, it is
/// good only for one query, so no use in modifying it!!
///
/// Uses the Velociy template engine.
///
\#include "$baseClassInclude"

#foreach($f in $IncludeFiles)
\#include "$f"
#end
\#include <TFile.h>

\#include <string>
\#include <stdexcept>

using std::string;

#ifdef __MAKECINT__
#foreach($l in $CINTLines)
$l
#end
#endif


class query$QueryIndex : public $baseClassName
{
public:
	/// So there is some init that CINT will know about
	/// (I don't know why this is required, but it is).
	query$QueryIndex()
	{
	}

	/// Called when we are starting a run
	void SlaveBegin(TTree *t)
	{
		$baseClassName::SlaveBegin(t);

		/// Init the variables that we are going to be carrying along with us.
#foreach($v in $ResultVariables)
		$v.VariableName = $v.InitialValue;
#end
	}

	/// Called when we are closign the file and shutting down on the slave
	void SlaveTerminate()
	{
		$baseClassName::SlaveTerminate();

#foreach($s in $SlaveTerminateStatements)
		$s
#end
	}

	/// Called with all plots at hand
	void Terminate()
	{
		$baseClassName::Terminate();
	}

	/// Called to process an entry
	bool Process(Long64_t entry)
	{
		///
		/// Get to the proper entry, reset internal cache
		///

		fDirector.SetReadEntry(entry);
		ResetCache();

		///
		/// Run the processing code
		///

#set ( $blockIndex = 0 )
#foreach($block in $QueryFunctionBlocks)
		ExecuteQueryBlock$blockIndex ();
#set ( $blockIndex = $blockIndex + 1 )
#end

		///
		/// Always return true - we want to go onto the next entry, afterall.
		///

		return true;
	}

private:

	///
	/// The query block functions
	///
#set ( $blockIndex = 0 )
#foreach($block in $QueryFunctionBlocks)
	void ExecuteQueryBlock$blockIndex ()
	{
#foreach($s in $block)
		$s
#end
	}

#set ( $blockIndex = $blockIndex + 1 )
#end

	///
	/// The member functions that the query blocks will be calling
	///

#foreach($s in $QueryMemberFunctions)
	$s
#end

	///
	/// Reset all cached items before we get going
	///

	void ResetCache()
	{
#foreach($s in $QueryCacheBools)
		$s = false;
#end
	}

	/// Here are the variables that hold things we need to keep around
	/// between entries of the ntuple. So things like the result that has
	/// to be filled on each entry.

#foreach($v in $ResultVariables)
	$v.VariableType $v.VariableName;
#end

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
