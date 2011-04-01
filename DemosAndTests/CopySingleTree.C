///
/// This guy is a tool to help with creating a shortened ttree. This is useful for testing
///

/// How many events to copy over
int NumEvents = 100;

/// Name of the one tree from the file that you will be pulling out.
string treeName = "vtuple";

/// Input and output files
string inputFName = "NTUP_BTAG.255221._000065.root";
string outputFName = "shortNTuple.root";


////
//// Shouldn't need to modify anything after that!
////

void CopySingleTree()
{
	cout << "hi" << endl;

	TFile *finput = new TFile (inputFName.c_str(), "READ");
	TTree *tin = (TTree*) finput->Get(treeName.c_str());
	tin->SetBranchStatus("*", 1);

	TFile *foutput = new TFile(outputFName.c_str(), "RECREATE");
	TTree *tout = tin->CloneTree(NumEvents);

	foutput->Write();
	foutput->Close();
}
