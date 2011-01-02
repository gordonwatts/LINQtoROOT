#ifndef __BTagJet__
#define __BTagJet__
///
/// Simple class to hold info about a jet in a b-tagging event
///

#include "MuonInBJet.h"
#include <vector>
#include <TLorentzVector.h>

class BTagJet : public TLorentzVector
{
public:
	~BTagJet();
	float wSV0;
	float wJetProb;
	float wTrackCounting;

	int label;

	std::vector<MuonInBJet> muons;

	ClassDef(BTagJet, 1);
};

#endif
