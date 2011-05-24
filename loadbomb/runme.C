///
/// Repro the load error
///

void runme()
{
	///
	/// Prevent duplicate error messages (and this is how we are running
	/// in the main code anyhow).
	///

	gEnv->SetValue("ACLiC.LinkLibs", 1);

	///
	/// Some infrastructure objects we don't care about
	///

	gSystem->CompileMacro("FlowOutputObject.cpp", "k");

	///
	/// First load works just fine...
	///

	gSystem->Exec("del query0_cxx.dll");
	gSystem->CompileMacro("query0.cxx", "kf");

	///
	/// Now, run the TSelector on our file(s)
	///

	TChain *f = new TChain("CollectionTree");
	f->Add("user.Sidoti.000260.AANT._00008.root");

	TClass *selector = TClass::GetClass("query0");
	if (selector == 0) {
		cout << "Error loading up query0" << endl;
		return 0;
	}

	TSelector *s = (TSelector*) selector->New();
	TH1F *h = new TH1F("NTH1F_116", "dummy histo", 10, 0.0, 100.0);
	TList *inputs = new TList();
	inputs->Add(h);
	s->SetInputList(inputs);

	f->Process(s);

	delete s;
	delete inputs;
	delete f;

	///
	/// Now, unload that query guy.
	///

	gSystem->Unload("query0_cxx");

	///
	/// Second build causes all heck to break loose and fails.
	///

	gSystem->Exec("del query1_cxx.dll");
	gSystem->CompileMacro("query1.cxx", "kf");

}
