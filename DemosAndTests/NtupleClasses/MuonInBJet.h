#ifndef __MuonInBJet__
#define __MuonInBJet__
///
/// Simple class to hold info about a jet in a b-tagging event
///

#include <TLorentzVector.h>
#include <vector>

class MuonInBJet : public TLorentzVector
{
public:
	float ptRel;
	~MuonInBJet();

	ClassDef(MuonInBJet, 1)
};

#endif
