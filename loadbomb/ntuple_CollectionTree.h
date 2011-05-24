/////////////////////////////////////////////////////////////////////////
//   This class has been automatically generated 
//   (at Fri May 13 17:21:22 2011 by ROOT version 5.28/00)
//   from TTree CollectionTree/CollectionTree
//   found on file: C:\Users\gwatts\Documents\ATLAS\Projects\data\user.Sidoti.105011.J2_pythia_jetjet.e574_s1086_s1100_r2269.HVESD_ntuple_v1.110428125959\user.Sidoti.000072.AANT._00001.root
/////////////////////////////////////////////////////////////////////////


#ifndef ntuple_CollectionTree_h
#define ntuple_CollectionTree_h

// System Headers needed by the proxy
#if defined(__CINT__) && !defined(__MAKECINT__)
   #define ROOT_Rtypes
   #define ROOT_TError
#endif
#include <TROOT.h>
#include <TChain.h>
#include <TFile.h>
#include <TPad.h>
#include <TH1.h>
#include <TSelector.h>
#include <TBranchProxy.h>
#include <TBranchProxyDirector.h>
#include <TBranchProxyTemplate.h>
#include <TFriendProxy.h>
using namespace ROOT;

// forward declarations needed by this particular proxy


// Header needed by this particular proxy
#include <vector>
#include "string"


class junk_macro_parsettree_CollectionTree_Interface {
   // This class defines the list of methods that are directly used by ntuple_CollectionTree,
   // and that can be overloaded in the user's script
public:
   void junk_macro_parsettree_CollectionTree_Begin(TTree*) {}
   void junk_macro_parsettree_CollectionTree_SlaveBegin(TTree*) {}
   Bool_t junk_macro_parsettree_CollectionTree_Notify() { return kTRUE; }
   Bool_t junk_macro_parsettree_CollectionTree_Process(Long64_t) { return kTRUE; }
   void junk_macro_parsettree_CollectionTree_SlaveTerminate() {}
   void junk_macro_parsettree_CollectionTree_Terminate() {}
};


class ntuple_CollectionTree : public TSelector, public junk_macro_parsettree_CollectionTree_Interface {
public :
   TTree          *fChain;         //!pointer to the analyzed TTree or TChain
   TH1            *htemp;          //!pointer to the histogram
   TBranchProxyDirector fDirector; //!Manages the proxys

   // Optional User methods
   TClass         *fClass;    // Pointer to this class's description

   // Wrapper class for each unwounded class
   struct TStlPx_string
   {
      TStlPx_string(TBranchProxyDirector* director,const char *top,const char *mid=0) :
         ffPrefix(top,mid),
         obj(director, top, mid)
      {};
      TStlPx_string(TBranchProxyDirector* director, TBranchProxy *parent, const char *membername, const char *top=0, const char *mid=0) :
         ffPrefix(top,mid),
         obj(director, parent, membername)
      {};
      TBranchProxyHelper ffPrefix;
      InjecTBranchProxyInterface();
      const string& At(UInt_t i) {
         static string default_val;
         if (!obj.Read()) return default_val;
         string *temp = & obj.GetPtr()->at(i);
         if (temp) return *temp; else return default_val;
      }
      const string& operator[](Int_t i) { return At(i); }
      const string& operator[](UInt_t i) { return At(i); }
      Int_t GetEntries() { return obj.GetPtr()->size(); }
      const vector<string>* operator->() { return obj.GetPtr(); }
      operator vector<string>*() { return obj.GetPtr(); }
      TObjProxy<vector<string> > obj;

   };
   struct TStlPx_vector_float_
   {
      TStlPx_vector_float_(TBranchProxyDirector* director,const char *top,const char *mid=0) :
         ffPrefix(top,mid),
         obj(director, top, mid)
      {};
      TStlPx_vector_float_(TBranchProxyDirector* director, TBranchProxy *parent, const char *membername, const char *top=0, const char *mid=0) :
         ffPrefix(top,mid),
         obj(director, parent, membername)
      {};
      TBranchProxyHelper ffPrefix;
      InjecTBranchProxyInterface();
      const vector<float>& At(UInt_t i) {
         static vector<float> default_val;
         if (!obj.Read()) return default_val;
         vector<float> *temp = & obj.GetPtr()->at(i);
         if (temp) return *temp; else return default_val;
      }
      const vector<float>& operator[](Int_t i) { return At(i); }
      const vector<float>& operator[](UInt_t i) { return At(i); }
      Int_t GetEntries() { return obj.GetPtr()->size(); }
      const vector<vector<float> >* operator->() { return obj.GetPtr(); }
      operator vector<vector<float> >*() { return obj.GetPtr(); }
      TObjProxy<vector<vector<float> > > obj;

   };
   struct TStlPx_vector_int_
   {
      TStlPx_vector_int_(TBranchProxyDirector* director,const char *top,const char *mid=0) :
         ffPrefix(top,mid),
         obj(director, top, mid)
      {};
      TStlPx_vector_int_(TBranchProxyDirector* director, TBranchProxy *parent, const char *membername, const char *top=0, const char *mid=0) :
         ffPrefix(top,mid),
         obj(director, parent, membername)
      {};
      TBranchProxyHelper ffPrefix;
      InjecTBranchProxyInterface();
      const vector<int>& At(UInt_t i) {
         static vector<int> default_val;
         if (!obj.Read()) return default_val;
         vector<int> *temp = & obj.GetPtr()->at(i);
         if (temp) return *temp; else return default_val;
      }
      const vector<int>& operator[](Int_t i) { return At(i); }
      const vector<int>& operator[](UInt_t i) { return At(i); }
      Int_t GetEntries() { return obj.GetPtr()->size(); }
      const vector<vector<int> >* operator->() { return obj.GetPtr(); }
      operator vector<vector<int> >*() { return obj.GetPtr(); }
      TObjProxy<vector<vector<int> > > obj;

   };

   // Proxy for each of the branches, leaves and friends of the tree
   TIntProxy                                  RunNumber;
   TIntProxy                                  EventNumber;
   TCharProxy                                 StreamESD_ref;
   TCharProxy                                 Stream1_ref;
   TCharProxy                                 Token;
   TStlSimpleProxy<vector<float> >            CalRatio_eta;
   TStlSimpleProxy<vector<float> >            CalRatio_phi;
   TStlSimpleProxy<vector<float> >            CalRatio_erat;
   TStlSimpleProxy<vector<float> >            CalRatio_ET;
   TStlSimpleProxy<vector<float> >            RoICluster_eta;
   TStlSimpleProxy<vector<float> >            RoICluster_phi;
   TStlSimpleProxy<vector<float> >            RoICluster_nRoI;
   TStlSimpleProxy<vector<float> >            Trackless_eta;
   TStlSimpleProxy<vector<float> >            Trackless_phi;
   TStlSimpleProxy<vector<float> >            Trackless_ET;
   TStlSimpleProxy<vector<float> >            AllEvents_nRoI;
   TStlSimpleProxy<vector<int> >              MBTSTimeFilter;
   TStlSimpleProxy<vector<int> >              mubcid;
   TStlPx_string                              LVL1MuonRoIName;
   TStlSimpleProxy<vector<float> >            LVL1MuonRoIeta;
   TStlSimpleProxy<vector<float> >            LVL1MuonRoIphi;
   TStlSimpleProxy<vector<long> >             rpc_prd_station;
   TStlSimpleProxy<vector<long> >             rpc_prd_eta;
   TStlSimpleProxy<vector<long> >             rpc_prd_phi;
   TStlSimpleProxy<vector<long> >             rpc_prd_doublr;
   TStlSimpleProxy<vector<long> >             rpc_prd_doublz;
   TStlSimpleProxy<vector<long> >             rpc_prd_doublphi;
   TStlSimpleProxy<vector<long> >             rpc_prd_gasgap;
   TStlSimpleProxy<vector<long> >             rpc_prd_measphi;
   TStlSimpleProxy<vector<long> >             rpc_prd_strip;
   TStlSimpleProxy<vector<float> >            rpc_prd_time;
   TStlSimpleProxy<vector<float> >            rpc_prd_stripx;
   TStlSimpleProxy<vector<float> >            rpc_prd_stripy;
   TStlSimpleProxy<vector<float> >            rpc_prd_stripz;
   TStlSimpleProxy<vector<float> >            rpc_prd_triggerInfo;
   TStlSimpleProxy<vector<float> >            rpc_prd_ambigFlag;
   TStlSimpleProxy<vector<float> >            IP_x;
   TStlSimpleProxy<vector<float> >            IP_y;
   TStlSimpleProxy<vector<float> >            IP_z;
   TStlSimpleProxy<vector<int> >              IP_nTracks;
   TStlPx_vector_float_                       IP_trk_pT;
   TStlPx_vector_float_                       IP_trk_eta;
   TStlPx_vector_float_                       IP_trk_nPix;
   TStlPx_vector_float_                       IP_trk_nSCT;
   TStlSimpleProxy<vector<float> >            L2Track_pt;
   TStlSimpleProxy<vector<float> >            L2Track_eta;
   TStlSimpleProxy<vector<float> >            L2Track_phi;
   TStlPx_string                              L2Track_author;
   TStlSimpleProxy<vector<float> >            L2Track_a0;
   TStlSimpleProxy<vector<float> >            L2Track_z0;
   TStlSimpleProxy<vector<float> >            L2Track_chi2;
   TStlSimpleProxy<vector<float> >            L2Track_nPixel;
   TStlSimpleProxy<vector<float> >            L2Track_nSCT;
   TStlSimpleProxy<vector<float> >            L2Track_nTRT;
   TIntProxy                                  PassedRoICluster;
   TIntProxy                                  PassedRoIClusterFirstEmpty;
   TIntProxy                                  PassedRoIClusterUnpairedIso;
   TIntProxy                                  PassedRoIClusterUnpairedNonIso;
   TIntProxy                                  PassedCalRatio;
   TIntProxy                                  PassedCalRatioFirstEmpty;
   TIntProxy                                  PassedCalRatioUnpairedIso;
   TIntProxy                                  PassedCalRatioUnpairedNonIso;
   TIntProxy                                  PassedTracklessJet;
   TIntProxy                                  PassedTracklessJetFirstEmpty;
   TIntProxy                                  PassedTracklessJetUnpairedIso;
   TIntProxy                                  PassedTracklessJetUnpairedNonIso;
   TIntProxy                                  PassedMultiMuon;
   TIntProxy                                  PassGammaMuon;
   TIntProxy                                  PassedMultiMuonTD;
   TIntProxy                                  PassedMultiMuonTDFirstEmpty;
   TIntProxy                                  PassedMultiMuonTDUnpairedIso;
   TIntProxy                                  PassedMultiMuonTDUnpairedNonIso;
   TIntProxy                                  PassGammaMuonTD;
   TIntProxy                                  PassGammaMuonTDFirstEmpty;
   TIntProxy                                  PassGammaMuonTDUnpairedIso;
   TIntProxy                                  PassGammaMuonTDUnpairedNonIso;
   TStlSimpleProxy<vector<int> >              isGood;
   TStlSimpleProxy<vector<double> >           ms_t;
   TStlSimpleProxy<vector<double> >           ms_x;
   TStlSimpleProxy<vector<double> >           ms_y;
   TStlSimpleProxy<vector<double> >           ms_z;
   TStlSimpleProxy<vector<double> >           ms_dx;
   TStlSimpleProxy<vector<double> >           ms_dy;
   TStlSimpleProxy<vector<double> >           ms_dz;
   TStlSimpleProxy<vector<double> >           hcell_t;
   TStlSimpleProxy<vector<double> >           hcell_e;
   TStlSimpleProxy<vector<double> >           hcell_x;
   TStlSimpleProxy<vector<double> >           hcell_y;
   TStlSimpleProxy<vector<double> >           hcell_z;
   TStlSimpleProxy<vector<float> >            Track_pt;
   TStlSimpleProxy<vector<float> >            Track_eta;
   TStlSimpleProxy<vector<float> >            Track_phi;
   TStlSimpleProxy<vector<float> >            Track_chi2;
   TStlSimpleProxy<vector<float> >            Track_nDoF;
   TStlSimpleProxy<vector<float> >            Track_d0;
   TStlSimpleProxy<vector<float> >            Track_z0;
   TStlSimpleProxy<vector<float> >            Track_nPixel;
   TStlSimpleProxy<vector<float> >            Track_nSCT;
   TStlSimpleProxy<vector<float> >            Track_nTRT;
   TStlSimpleProxy<vector<float> >            Track_nTRT_Barrel;
   TStlPx_vector_float_                       Track_nTRT_Barrel_Hits_r;
   TStlPx_vector_float_                       Track_nTRT_Barrel_Hits_phi;
   TStlSimpleProxy<vector<int> >              Track_TRT_etaregion;
   TFloatProxy                                MET_base;
   TFloatProxy                                MET_base_phi;
   TFloatProxy                                MET_RefFinal;
   TFloatProxy                                MET_RefFinal_phi;
   TIntProxy                                  llp_N;
   TStlSimpleProxy<vector<int> >              llp_pdgid;
   TStlSimpleProxy<vector<int> >              llp_barcode;
   TStlSimpleProxy<vector<int> >              llp_mother_barcode;
   TStlSimpleProxy<vector<float> >            llp_eta;
   TStlSimpleProxy<vector<float> >            llp_phi;
   TStlSimpleProxy<vector<float> >            llp_x;
   TStlSimpleProxy<vector<float> >            llp_y;
   TStlSimpleProxy<vector<float> >            llp_z;
   TStlSimpleProxy<vector<float> >            llp_m;
   TStlSimpleProxy<vector<float> >            llp_e;
   TStlSimpleProxy<vector<float> >            llp_px;
   TStlSimpleProxy<vector<float> >            llp_py;
   TStlSimpleProxy<vector<float> >            llp_pz;
   TStlSimpleProxy<vector<int> >              llp_ndecay;
   TUIntProxy                                 caloCluster_nClusters;
   TFloatProxy                                caloCluster_totE;
   TStlSimpleProxy<vector<float> >            caloCluster_energy;
   TStlSimpleProxy<vector<float> >            caloCluster_eta;
   TStlSimpleProxy<vector<float> >            caloCluster_phi;
   TStlSimpleProxy<vector<float> >            caloCluster_hadEnergy;
   TStlSimpleProxy<vector<float> >            caloCluster_emEnergy;
   TStlSimpleProxy<vector<float> >            caloCluster_emB0;
   TStlSimpleProxy<vector<float> >            caloCluster_emB1;
   TStlSimpleProxy<vector<float> >            caloCluster_emB2;
   TStlSimpleProxy<vector<float> >            caloCluster_emB3;
   TStlSimpleProxy<vector<float> >            caloCluster_emE0;
   TStlSimpleProxy<vector<float> >            caloCluster_emE1;
   TStlSimpleProxy<vector<float> >            caloCluster_emE2;
   TStlSimpleProxy<vector<float> >            caloCluster_emE3;
   TStlSimpleProxy<vector<float> >            caloCluster_emF0;
   TStlSimpleProxy<vector<float> >            caloCluster_emF1;
   TStlSimpleProxy<vector<float> >            caloCluster_emF2;
   TStlSimpleProxy<vector<float> >            caloCluster_hadB0;
   TStlSimpleProxy<vector<float> >            caloCluster_hadB1;
   TStlSimpleProxy<vector<float> >            caloCluster_hadB2;
   TStlSimpleProxy<vector<float> >            caloCluster_hadExB0;
   TStlSimpleProxy<vector<float> >            caloCluster_hadExB1;
   TStlSimpleProxy<vector<float> >            caloCluster_hadExB2;
   TStlSimpleProxy<vector<float> >            caloCluster_hadE0;
   TStlSimpleProxy<vector<float> >            caloCluster_hadE1;
   TStlSimpleProxy<vector<float> >            caloCluster_hadE2;
   TStlSimpleProxy<vector<float> >            caloCluster_hadE3;
   TStlSimpleProxy<vector<float> >            caloCluster_hadGap1;
   TStlSimpleProxy<vector<float> >            caloCluster_hadGap2;
   TStlSimpleProxy<vector<float> >            caloCluster_hadGap3;
   TStlSimpleProxy<vector<float> >            caloCluster_emB0_eta;
   TStlSimpleProxy<vector<float> >            caloCluster_emB1_eta;
   TStlSimpleProxy<vector<float> >            caloCluster_emB2_eta;
   TStlSimpleProxy<vector<float> >            caloCluster_emB3_eta;
   TStlSimpleProxy<vector<float> >            caloCluster_emE0_eta;
   TStlSimpleProxy<vector<float> >            caloCluster_emE1_eta;
   TStlSimpleProxy<vector<float> >            caloCluster_emE2_eta;
   TStlSimpleProxy<vector<float> >            caloCluster_emE3_eta;
   TStlSimpleProxy<vector<float> >            caloCluster_emF0_eta;
   TStlSimpleProxy<vector<float> >            caloCluster_emF1_eta;
   TStlSimpleProxy<vector<float> >            caloCluster_emF2_eta;
   TStlSimpleProxy<vector<float> >            caloCluster_hadB0_eta;
   TStlSimpleProxy<vector<float> >            caloCluster_hadB1_eta;
   TStlSimpleProxy<vector<float> >            caloCluster_hadB2_eta;
   TStlSimpleProxy<vector<float> >            caloCluster_hadExB0_eta;
   TStlSimpleProxy<vector<float> >            caloCluster_hadExB1_eta;
   TStlSimpleProxy<vector<float> >            caloCluster_hadExB2_eta;
   TStlSimpleProxy<vector<float> >            caloCluster_hadE0_eta;
   TStlSimpleProxy<vector<float> >            caloCluster_hadE1_eta;
   TStlSimpleProxy<vector<float> >            caloCluster_hadE2_eta;
   TStlSimpleProxy<vector<float> >            caloCluster_hadE3_eta;
   TStlSimpleProxy<vector<float> >            caloCluster_hadGap1_eta;
   TStlSimpleProxy<vector<float> >            caloCluster_hadGap2_eta;
   TStlSimpleProxy<vector<float> >            caloCluster_hadGap3_eta;
   TStlSimpleProxy<vector<float> >            caloCluster_emB0_phi;
   TStlSimpleProxy<vector<float> >            caloCluster_emB1_phi;
   TStlSimpleProxy<vector<float> >            caloCluster_emB2_phi;
   TStlSimpleProxy<vector<float> >            caloCluster_emB3_phi;
   TStlSimpleProxy<vector<float> >            caloCluster_emE0_phi;
   TStlSimpleProxy<vector<float> >            caloCluster_emE1_phi;
   TStlSimpleProxy<vector<float> >            caloCluster_emE2_phi;
   TStlSimpleProxy<vector<float> >            caloCluster_emE3_phi;
   TStlSimpleProxy<vector<float> >            caloCluster_emF0_phi;
   TStlSimpleProxy<vector<float> >            caloCluster_emF1_phi;
   TStlSimpleProxy<vector<float> >            caloCluster_emF2_phi;
   TStlSimpleProxy<vector<float> >            caloCluster_hadB0_phi;
   TStlSimpleProxy<vector<float> >            caloCluster_hadB1_phi;
   TStlSimpleProxy<vector<float> >            caloCluster_hadB2_phi;
   TStlSimpleProxy<vector<float> >            caloCluster_hadExB0_phi;
   TStlSimpleProxy<vector<float> >            caloCluster_hadExB1_phi;
   TStlSimpleProxy<vector<float> >            caloCluster_hadExB3_phi;
   TStlSimpleProxy<vector<float> >            caloCluster_hadE0_phi;
   TStlSimpleProxy<vector<float> >            caloCluster_hadE1_phi;
   TStlSimpleProxy<vector<float> >            caloCluster_hadE2_phi;
   TStlSimpleProxy<vector<float> >            caloCluster_hadE3_phi;
   TStlSimpleProxy<vector<float> >            caloCluster_hadGap1_phi;
   TStlSimpleProxy<vector<float> >            caloCluster_hadGap2_phi;
   TStlSimpleProxy<vector<float> >            caloCluster_hadGap3_phi;
   TStlSimpleProxy<vector<float> >            caloCluster_larB;
   TStlSimpleProxy<vector<float> >            caloCluster_larE;
   TStlSimpleProxy<vector<float> >            caloCluster_fCal;
   TStlSimpleProxy<vector<float> >            caloCluster_hadE;
   TStlSimpleProxy<vector<float> >            caloCluster_tileB;
   TStlSimpleProxy<vector<float> >            caloCluster_tileG;
   TStlSimpleProxy<vector<float> >            caloCluster_tileExB;
   TUIntProxy                                 caloTower_nTowers;
   TFloatProxy                                caloTower_totE;
   TStlSimpleProxy<vector<float> >            caloTower_energy;
   TStlSimpleProxy<vector<float> >            caloTower_eta;
   TStlSimpleProxy<vector<float> >            caloTower_phi;
   TStlSimpleProxy<vector<float> >            AntiKt4TopoJets_CalibE;
   TStlSimpleProxy<vector<float> >            AntiKt4TopoJets_Calibpt;
   TStlSimpleProxy<vector<float> >            AntiKt4TopoJets_CalibEt;
   TStlSimpleProxy<vector<float> >            AntiKt4TopoJets_Calibpx;
   TStlSimpleProxy<vector<float> >            AntiKt4TopoJets_Calibpy;
   TStlSimpleProxy<vector<float> >            AntiKt4TopoJets_Calibpz;
   TStlSimpleProxy<vector<float> >            AntiKt4TopoJets_Calibm;
   TStlSimpleProxy<vector<float> >            AntiKt4TopoJets_Calibscale;
   TStlSimpleProxy<vector<float> >            AntiKt4TopoJets_Calibeta;
   TStlSimpleProxy<vector<float> >            AntiKt4TopoJets_Calibphi;
   TUIntProxy                                 AntiKt4TopoJets_nJets;
   TStlSimpleProxy<vector<float> >            AntiKt4TopoJets_E;
   TStlSimpleProxy<vector<float> >            AntiKt4TopoJets_p;
   TStlSimpleProxy<vector<float> >            AntiKt4TopoJets_Et;
   TStlSimpleProxy<vector<float> >            AntiKt4TopoJets_pt;
   TStlSimpleProxy<vector<float> >            AntiKt4TopoJets_m;
   TStlSimpleProxy<vector<float> >            AntiKt4TopoJets_y;
   TStlSimpleProxy<vector<float> >            AntiKt4TopoJets_tanth;
   TStlSimpleProxy<vector<float> >            AntiKt4TopoJets_eta;
   TStlSimpleProxy<vector<float> >            AntiKt4TopoJets_phi;
   TStlSimpleProxy<vector<float> >            AntiKt4TopoJets_Eh;
   TStlSimpleProxy<vector<float> >            AntiKt4TopoJets_Eem;
   TStlSimpleProxy<vector<float> >            AntiKt4TopoJets_px;
   TStlSimpleProxy<vector<float> >            AntiKt4TopoJets_py;
   TStlSimpleProxy<vector<float> >            AntiKt4TopoJets_pz;
   TStlSimpleProxy<vector<int> >              AntiKt4TopoJets_Ncon;
   TStlPx_vector_float_                       AntiKt4TopoJets_ptcon;
   TStlPx_vector_float_                       AntiKt4TopoJets_econ;
   TStlPx_vector_float_                       AntiKt4TopoJets_etacon;
   TStlPx_vector_float_                       AntiKt4TopoJets_phicon;
   TStlPx_vector_float_                       AntiKt4TopoJets_weightcon;
   TStlSimpleProxy<vector<float> >            AntiKt4TopoJets_emfrac;
   TStlPx_string                              momentList;
   TStlPx_vector_float_                       AntiKt4TopoJets_moments;
   TStlPx_string                              AntiKt4TopoJets_author;
   TStlPx_string                              AntiKt4TopoJets_calibTags;
   TStlSimpleProxy<vector<float> >            AntiKt4TopoJets_e_PreSamplerB;
   TStlSimpleProxy<vector<float> >            AntiKt4TopoJets_e_EMB1;
   TStlSimpleProxy<vector<float> >            AntiKt4TopoJets_e_EMB2;
   TStlSimpleProxy<vector<float> >            AntiKt4TopoJets_e_EMB3;
   TStlSimpleProxy<vector<float> >            AntiKt4TopoJets_e_PreSamplerE;
   TStlSimpleProxy<vector<float> >            AntiKt4TopoJets_e_EME1;
   TStlSimpleProxy<vector<float> >            AntiKt4TopoJets_e_EME2;
   TStlSimpleProxy<vector<float> >            AntiKt4TopoJets_e_EME3;
   TStlSimpleProxy<vector<float> >            AntiKt4TopoJets_e_HEC0;
   TStlSimpleProxy<vector<float> >            AntiKt4TopoJets_e_HEC1;
   TStlSimpleProxy<vector<float> >            AntiKt4TopoJets_e_HEC2;
   TStlSimpleProxy<vector<float> >            AntiKt4TopoJets_e_HEC3;
   TStlSimpleProxy<vector<float> >            AntiKt4TopoJets_e_TileBar0;
   TStlSimpleProxy<vector<float> >            AntiKt4TopoJets_e_TileBar1;
   TStlSimpleProxy<vector<float> >            AntiKt4TopoJets_e_TileBar2;
   TStlSimpleProxy<vector<float> >            AntiKt4TopoJets_e_TileGap1;
   TStlSimpleProxy<vector<float> >            AntiKt4TopoJets_e_TileGap2;
   TStlSimpleProxy<vector<float> >            AntiKt4TopoJets_e_TileGap3;
   TStlSimpleProxy<vector<float> >            AntiKt4TopoJets_e_TileExt0;
   TStlSimpleProxy<vector<float> >            AntiKt4TopoJets_e_TileExt1;
   TStlSimpleProxy<vector<float> >            AntiKt4TopoJets_e_TileExt2;
   TStlSimpleProxy<vector<float> >            AntiKt4TopoJets_e_FCAL0;
   TStlSimpleProxy<vector<float> >            AntiKt4TopoJets_e_FCAL1;
   TStlSimpleProxy<vector<float> >            AntiKt4TopoJets_e_FCAL2;
   TStlSimpleProxy<vector<float> >            AntiKt4TopoJets_time_calo;
   TStlSimpleProxy<vector<float> >            AntiKt4TopoJets_fbadQ;
   TStlSimpleProxy<vector<float> >            AntiKt4TopoJets_hecF;
   TStlSimpleProxy<vector<float> >            AntiKt4TopoJets_TileGap3F;
   TStlSimpleProxy<vector<float> >            AntiKt4TopoJetsjet_truepdg;
   TStlSimpleProxy<vector<float> >            AntiKt4TopoJetsjet_truelabel;
   TStlPx_string                              trig_triggers;
   TStlSimpleProxy<vector<float> >            trig_prescales;
   TStlSimpleProxy<vector<unsigned int> >     trig_bits;
   TStlSimpleProxy<vector<unsigned int> >     trig_passed;
   TStlSimpleProxy<vector<float> >            trig_mbts_times;
   TStlSimpleProxy<vector<float> >            trig_mbts_energy;
   TStlSimpleProxy<vector<int> >              trig_mbts_quality;
   TStlSimpleProxy<vector<int> >              trig_mbts_eta;
   TStlSimpleProxy<vector<int> >              trig_mbts_phi;
   TStlSimpleProxy<vector<int> >              trig_mbts_channel;
   TStlSimpleProxy<vector<int> >              trig_calo_ncellA;
   TStlSimpleProxy<vector<int> >              trig_calo_ncellC;
   TStlSimpleProxy<vector<float> >            trig_calo_timeA;
   TStlSimpleProxy<vector<float> >            trig_calo_timeC;
   TStlSimpleProxy<vector<float> >            trig_calo_energyA;
   TStlSimpleProxy<vector<float> >            trig_calo_energyC;
   TStlSimpleProxy<vector<int> >              cell_number;
   TStlPx_vector_int_                         cell_sampling;
   TStlPx_vector_float_                       cell_time;
   TStlPx_vector_float_                       cell_energy;
   TIntProxy                                  Mu_N;
   TStlSimpleProxy<vector<float> >            Mu_E;
   TStlSimpleProxy<vector<float> >            Mu_p;
   TStlSimpleProxy<vector<float> >            Mu_p_T;
   TStlSimpleProxy<vector<float> >            Mu_p_x;
   TStlSimpleProxy<vector<float> >            Mu_p_y;
   TStlSimpleProxy<vector<float> >            Mu_p_z;
   TStlSimpleProxy<vector<float> >            Mu_phi;
   TStlSimpleProxy<vector<float> >            Mu_eta;
   TStlSimpleProxy<vector<float> >            Mu_m;
   TStlSimpleProxy<vector<float> >            Mu_charge;
   TStlSimpleProxy<vector<float> >            Mu_cone20;
   TStlSimpleProxy<vector<float> >            Mu_cone30;
   TStlSimpleProxy<vector<float> >            Mu_cone40;
   TStlSimpleProxy<vector<float> >            Mu_trk10;
   TStlSimpleProxy<vector<float> >            Mu_trk20;
   TStlSimpleProxy<vector<float> >            Mu_trk30;
   TStlSimpleProxy<vector<float> >            Mu_trk40;
   TStlSimpleProxy<vector<float> >            Mu_matchchi2;
   TStlSimpleProxy<vector<float> >            Mu_matchchi2DoverF;
   TStlSimpleProxy<vector<float> >            Mu_chi2;
   TStlSimpleProxy<vector<float> >            Mu_chi2DoverF;
   TStlSimpleProxy<vector<float> >            Mu_energyDeposit;
   TStlSimpleProxy<vector<float> >            Mu_energySigma;
   TStlSimpleProxy<vector<float> >            Mu_x;
   TStlSimpleProxy<vector<float> >            Mu_y;
   TStlSimpleProxy<vector<float> >            Mu_z;
   TStlSimpleProxy<vector<float> >            Mu_d0;
   TStlSimpleProxy<vector<float> >            Mu_z0sinTh;
   TStlSimpleProxy<vector<float> >            Mu_a0;
   TStlSimpleProxy<vector<float> >            Mu_d0err;
   TStlSimpleProxy<vector<int> >              Mu_isCombined;
   TStlSimpleProxy<vector<int> >              Mu_isStandAlone;
   TStlSimpleProxy<vector<int> >              Mu_isLowPt;
   TStlSimpleProxy<vector<int> >              Mu_isLoose;
   TStlSimpleProxy<vector<int> >              Mu_isMedium;
   TStlSimpleProxy<vector<int> >              Mu_isTight;
   TStlSimpleProxy<vector<float> >            Mu_isMuonLikelihood;
   TStlSimpleProxy<vector<int> >              Mu_author;
   TStlSimpleProxy<vector<unsigned short> >   Mu_allAuthors;
   TStlSimpleProxy<vector<float> >            Mu_InDet_p_T;
   TStlSimpleProxy<vector<float> >            Mu_InDet_phi;
   TStlSimpleProxy<vector<float> >            Mu_InDet_eta;
   TStlSimpleProxy<vector<int> >              Mu_InDet_trk_Index;
   TStlSimpleProxy<vector<float> >            Mu_InDet_pixelhits;
   TStlSimpleProxy<vector<float> >            Mu_InDet_scthits;
   TStlSimpleProxy<vector<float> >            Mu_InDet_blayerhits;
   TStlSimpleProxy<vector<float> >            Mu_InDet_trthits;
   TStlSimpleProxy<vector<float> >            Mu_InDet_trththits;
   TStlSimpleProxy<vector<float> >            Mu_MuonSpec_p_T;
   TStlSimpleProxy<vector<float> >            Mu_MuonSpec_phi;
   TStlSimpleProxy<vector<float> >            Mu_MuonSpec_eta;
   TStlSimpleProxy<vector<float> >            Mu_MuonSpec_d0;
   TStlSimpleProxy<vector<float> >            Mu_MuonSpec_z0;
   TStlSimpleProxy<vector<float> >            Mu_MuonSpec_phi0;
   TStlSimpleProxy<vector<float> >            Mu_MuonSpec_theta;
   TStlSimpleProxy<vector<float> >            Mu_MuonSpec_qOp;
   TStlSimpleProxy<vector<float> >            Mu_MuonSpec_chi2;
   TStlSimpleProxy<vector<float> >            Mu_MuonSpec_ndof;
   TStlPx_vector_float_                       Mu_MuonSpecSurf_x;
   TStlPx_vector_float_                       Mu_MuonSpecSurf_y;
   TStlPx_vector_float_                       Mu_MuonSpecSurf_z;
   TStlPx_vector_float_                       Mu_MuonSpecSurf_px;
   TStlPx_vector_float_                       Mu_MuonSpecSurf_py;
   TStlPx_vector_float_                       Mu_MuonSpecSurf_pz;
   TStlPx_vector_float_                       Mu_MuonExtr_CellE;
   TStlPx_vector_float_                       Mu_MuonExtr_CellEt;
   TStlPx_vector_float_                       Mu_MuonExtr_CellPhi;
   TStlPx_vector_float_                       Mu_MuonExtr_CellEta;
   TStlPx_vector_int_                         Mu_MuonExtr_CellSampl;
   TStlSimpleProxy<vector<float> >            Mu_MuonExtr_cellEtIsol20;
   TStlSimpleProxy<vector<float> >            Mu_MuonExtr_cellEtIsol30;
   TStlSimpleProxy<vector<float> >            Mu_MuonExtr_cellEtIsol40;
   TStlSimpleProxy<vector<float> >            Mu_MuonExtr_p_T;
   TStlSimpleProxy<vector<float> >            Mu_MuonExtr_phi;
   TStlSimpleProxy<vector<float> >            Mu_MuonExtr_eta;
   TStlSimpleProxy<vector<float> >            Mu_MuonExtr_p;
   TStlSimpleProxy<vector<float> >            Mu_MuonExtr_d0;
   TStlSimpleProxy<vector<float> >            Mu_MuonExtr_z0;
   TStlSimpleProxy<vector<float> >            Mu_MuonExtr_phi0;
   TStlSimpleProxy<vector<float> >            Mu_MuonExtr_theta;
   TStlSimpleProxy<vector<float> >            Mu_MuonExtr_qOp;
   TStlSimpleProxy<vector<float> >            Mu_MuonExtr_charge;
   TStlSimpleProxy<vector<float> >            Mu_Combined_p_T;
   TStlSimpleProxy<vector<float> >            Mu_Combined_phi;
   TStlSimpleProxy<vector<float> >            Mu_Combined_eta;
   TStlSimpleProxy<vector<float> >            Mu_beta;
   TStlSimpleProxy<vector<float> >            Mu_beta_L2;
   TStlSimpleProxy<vector<int> >              Mu_mdthits_N;
   TStlSimpleProxy<vector<int> >              Mu_cscetahits_N;
   TStlSimpleProxy<vector<int> >              Mu_cschits_N;
   TStlSimpleProxy<vector<int> >              Mu_rpchits_N;
   TStlSimpleProxy<vector<int> >              Mu_tgchits_N;
   TStlPx_string                              muMSonly_author;
   TStlSimpleProxy<vector<float> >            muMSonly_px;
   TStlSimpleProxy<vector<float> >            muMSonly_py;
   TStlSimpleProxy<vector<float> >            muMSonly_pz;
   TStlSimpleProxy<vector<float> >            muMSonly_eta;
   TStlSimpleProxy<vector<float> >            muMSonly_phi;
   TStlSimpleProxy<vector<float> >            muMSonly_e;
   TStlSimpleProxy<vector<float> >            muMSonly_charge;
   TStlSimpleProxy<vector<float> >            muMSonly_chi2;
   TStlSimpleProxy<vector<float> >            muMSonly_ndof;
   TStlSimpleProxy<vector<float> >            MSvx_R;
   TStlSimpleProxy<vector<float> >            MSvx_z;
   TStlSimpleProxy<vector<float> >            MSvx_theta;
   TStlSimpleProxy<vector<float> >            MSvx_phi;
   TStlSimpleProxy<vector<float> >            MSvx_nTrks;
   TStlSimpleProxy<vector<float> >            MSvx_chi2prob;
   TStlSimpleProxy<vector<int> >              MSvx_author;
   TStlPx_vector_float_                       MSvx_trk_px;
   TStlPx_vector_float_                       MSvx_trk_py;
   TStlPx_vector_float_                       MSvx_trk_pz;
   TStlPx_vector_float_                       MSvx_trk_charge;
   TStlSimpleProxy<vector<float> >            MSvx_nMDT;
   TStlSimpleProxy<vector<float> >            MSvx_nIMDT;
   TStlSimpleProxy<vector<float> >            MSvx_nMMDT;
   TStlSimpleProxy<vector<float> >            MSvx_nOMDT;
   TStlSimpleProxy<vector<float> >            MSvx_nRPC;
   TStlSimpleProxy<vector<float> >            MSvx_nTGC;
   TStlSimpleProxy<vector<float> >            HVtrk_x;
   TStlSimpleProxy<vector<float> >            HVtrk_y;
   TStlSimpleProxy<vector<float> >            HVtrk_z;
   TStlSimpleProxy<vector<float> >            HVtrk_pT;
   TStlSimpleProxy<vector<float> >            HVtrk_pz;
   TStlSimpleProxy<vector<float> >            HVtrk_charge;
   TStlPx_vector_float_                       MDT_x;
   TStlPx_vector_float_                       MDT_y;
   TStlPx_vector_float_                       MDT_z;
   TStlSimpleProxy<vector<int> >              MDT_ml;
   TStlPx_string                              MDT_chamber;
   TStlSimpleProxy<vector<float> >            Pix_Seg_Phi;
   TStlSimpleProxy<vector<float> >            Pix_Seg_Pt;
   TStlSimpleProxy<vector<float> >            Pix_Seg_Eta;
   TStlSimpleProxy<vector<float> >            Pix_Seg_z0;
   TStlPx_vector_float_                       Pix_Seg_Hits_z;
   TStlSimpleProxy<vector<float> >            SCT_Seg_Phi;
   TStlSimpleProxy<vector<float> >            SCT_Seg_Pt;
   TStlSimpleProxy<vector<float> >            SCT_Seg_Eta;
   TStlSimpleProxy<vector<float> >            SCT_Seg_z0;
   TStlPx_vector_float_                       SCT_Seg_Hits_z;
   TStlSimpleProxy<vector<float> >            TRT_Seg_PosEta_Phi;
   TStlSimpleProxy<vector<float> >            TRT_Seg_NegEta_Phi;
   TStlSimpleProxy<vector<float> >            TRT_Seg_PosEta_d0;
   TStlSimpleProxy<vector<float> >            TRT_Seg_NegEta_d0;
   TStlSimpleProxy<vector<float> >            TRT_Seg_PosEta_Pt;
   TStlSimpleProxy<vector<float> >            TRT_Seg_NegEta_Pt;
   TStlPx_vector_float_                       TRT_Seg_PosEta_Hits_R;
   TStlPx_vector_float_                       TRT_Seg_PosEta_Hits_Phi;
   TStlPx_vector_float_                       TRT_Seg_NegEta_Hits_R;
   TStlPx_vector_float_                       TRT_Seg_NegEta_Hits_Phi;
   TStlSimpleProxy<vector<float> >            TRT_Vtx_PosEta_r;
   TStlSimpleProxy<vector<float> >            TRT_Vtx_NegEta_r;
   TStlSimpleProxy<vector<float> >            TRT_Vtx_PosEta_Phi;
   TStlSimpleProxy<vector<float> >            TRT_Vtx_NegEta_Phi;
   TStlSimpleProxy<vector<float> >            TRT_Vtx_PosEta_ConeAngle;
   TStlSimpleProxy<vector<float> >            TRT_Vtx_NegEta_ConeAngle;
   TStlSimpleProxy<vector<float> >            TRT_Vtx_PosEta_numInLowerCone;
   TStlSimpleProxy<vector<float> >            TRT_Vtx_NegEta_numInLowerCone;
   TStlSimpleProxy<vector<float> >            TRT_Vtx_PosEta_numInUpperCone;
   TStlSimpleProxy<vector<float> >            TRT_Vtx_NegEta_numInUpperCone;
   TStlSimpleProxy<vector<float> >            TRT_Vtx_PosEta_numInPreHitConeFromIP_L;
   TStlSimpleProxy<vector<float> >            TRT_Vtx_NegEta_numInPreHitConeFromIP_L;
   TStlSimpleProxy<vector<float> >            TRT_Vtx_PosEta_numInPreHitConeFromIP_R;
   TStlSimpleProxy<vector<float> >            TRT_Vtx_NegEta_numInPreHitConeFromIP_R;
   TStlSimpleProxy<vector<float> >            TRT_Vtx_PosEta_fracInLowerCone;
   TStlSimpleProxy<vector<float> >            TRT_Vtx_NegEta_fracInLowerCone;
   TStlSimpleProxy<vector<float> >            TRT_Vtx_PosEta_fracInUpperCone;
   TStlSimpleProxy<vector<float> >            TRT_Vtx_NegEta_fracInUpperCone;
   TStlSimpleProxy<vector<float> >            TRT_Vtx_PosEta_fracInPreHitConeFromIP_L;
   TStlSimpleProxy<vector<float> >            TRT_Vtx_NegEta_fracInPreHitConeFromIP_L;
   TStlSimpleProxy<vector<float> >            TRT_Vtx_PosEta_fracInPreHitConeFromIP_R;
   TStlSimpleProxy<vector<float> >            TRT_Vtx_NegEta_fracInPreHitConeFromIP_R;
   TIntProxy                                  Run;
   TIntProxy                                  Event;
   TIntProxy                                  Time;
   TIntProxy                                  LumiBlock;
   TIntProxy                                  BCID;
   TIntProxy                                  LVL1ID;
   TDoubleProxy                               Weight;
   TIntProxy                                  IEvent;
   TIntProxy                                  StatusElement;
   TIntProxy                                  LVL1TriggerType;
   TStlSimpleProxy<vector<unsigned int> >     LVL1TriggerInfo;
   TStlSimpleProxy<vector<unsigned int> >     LVL2TriggerInfo;
   TStlSimpleProxy<vector<unsigned int> >     EventFilterInfo;
   TStlPx_string                              StreamTagName;
   TStlPx_string                              StreamTagType;


   ntuple_CollectionTree(TTree *tree=0) : 
      fChain(0),
      htemp(0),
      fDirector(tree,-1),
      fClass                (TClass::GetClass("ntuple_CollectionTree")),
      RunNumber                                 (&fDirector,"RunNumber"),
      EventNumber                               (&fDirector,"EventNumber"),
      StreamESD_ref                             (&fDirector,"StreamESD_ref"),
      Stream1_ref                               (&fDirector,"Stream1_ref"),
      Token                                     (&fDirector,"Token"),
      CalRatio_eta                              (&fDirector,"CalRatio_eta"),
      CalRatio_phi                              (&fDirector,"CalRatio_phi"),
      CalRatio_erat                             (&fDirector,"CalRatio_erat"),
      CalRatio_ET                               (&fDirector,"CalRatio_ET"),
      RoICluster_eta                            (&fDirector,"RoICluster_eta"),
      RoICluster_phi                            (&fDirector,"RoICluster_phi"),
      RoICluster_nRoI                           (&fDirector,"RoICluster_nRoI"),
      Trackless_eta                             (&fDirector,"Trackless_eta"),
      Trackless_phi                             (&fDirector,"Trackless_phi"),
      Trackless_ET                              (&fDirector,"Trackless_ET"),
      AllEvents_nRoI                            (&fDirector,"AllEvents_nRoI"),
      MBTSTimeFilter                            (&fDirector,"MBTSTimeFilter"),
      mubcid                                    (&fDirector,"mubcid"),
      LVL1MuonRoIName                           (&fDirector,"LVL1MuonRoIName"),
      LVL1MuonRoIeta                            (&fDirector,"LVL1MuonRoIeta"),
      LVL1MuonRoIphi                            (&fDirector,"LVL1MuonRoIphi"),
      rpc_prd_station                           (&fDirector,"rpc_prd_station"),
      rpc_prd_eta                               (&fDirector,"rpc_prd_eta"),
      rpc_prd_phi                               (&fDirector,"rpc_prd_phi"),
      rpc_prd_doublr                            (&fDirector,"rpc_prd_doublr"),
      rpc_prd_doublz                            (&fDirector,"rpc_prd_doublz"),
      rpc_prd_doublphi                          (&fDirector,"rpc_prd_doublphi"),
      rpc_prd_gasgap                            (&fDirector,"rpc_prd_gasgap"),
      rpc_prd_measphi                           (&fDirector,"rpc_prd_measphi"),
      rpc_prd_strip                             (&fDirector,"rpc_prd_strip"),
      rpc_prd_time                              (&fDirector,"rpc_prd_time"),
      rpc_prd_stripx                            (&fDirector,"rpc_prd_stripx"),
      rpc_prd_stripy                            (&fDirector,"rpc_prd_stripy"),
      rpc_prd_stripz                            (&fDirector,"rpc_prd_stripz"),
      rpc_prd_triggerInfo                       (&fDirector,"rpc_prd_triggerInfo"),
      rpc_prd_ambigFlag                         (&fDirector,"rpc_prd_ambigFlag"),
      IP_x                                      (&fDirector,"IP_x"),
      IP_y                                      (&fDirector,"IP_y"),
      IP_z                                      (&fDirector,"IP_z"),
      IP_nTracks                                (&fDirector,"IP_nTracks"),
      IP_trk_pT                                 (&fDirector,"IP_trk_pT"),
      IP_trk_eta                                (&fDirector,"IP_trk_eta"),
      IP_trk_nPix                               (&fDirector,"IP_trk_nPix"),
      IP_trk_nSCT                               (&fDirector,"IP_trk_nSCT"),
      L2Track_pt                                (&fDirector,"L2Track_pt"),
      L2Track_eta                               (&fDirector,"L2Track_eta"),
      L2Track_phi                               (&fDirector,"L2Track_phi"),
      L2Track_author                            (&fDirector,"L2Track_author"),
      L2Track_a0                                (&fDirector,"L2Track_a0"),
      L2Track_z0                                (&fDirector,"L2Track_z0"),
      L2Track_chi2                              (&fDirector,"L2Track_chi2"),
      L2Track_nPixel                            (&fDirector,"L2Track_nPixel"),
      L2Track_nSCT                              (&fDirector,"L2Track_nSCT"),
      L2Track_nTRT                              (&fDirector,"L2Track_nTRT"),
      PassedRoICluster                          (&fDirector,"PassedRoICluster"),
      PassedRoIClusterFirstEmpty                (&fDirector,"PassedRoIClusterFirstEmpty"),
      PassedRoIClusterUnpairedIso               (&fDirector,"PassedRoIClusterUnpairedIso"),
      PassedRoIClusterUnpairedNonIso            (&fDirector,"PassedRoIClusterUnpairedNonIso"),
      PassedCalRatio                            (&fDirector,"PassedCalRatio"),
      PassedCalRatioFirstEmpty                  (&fDirector,"PassedCalRatioFirstEmpty"),
      PassedCalRatioUnpairedIso                 (&fDirector,"PassedCalRatioUnpairedIso"),
      PassedCalRatioUnpairedNonIso              (&fDirector,"PassedCalRatioUnpairedNonIso"),
      PassedTracklessJet                        (&fDirector,"PassedTracklessJet"),
      PassedTracklessJetFirstEmpty              (&fDirector,"PassedTracklessJetFirstEmpty"),
      PassedTracklessJetUnpairedIso             (&fDirector,"PassedTracklessJetUnpairedIso"),
      PassedTracklessJetUnpairedNonIso          (&fDirector,"PassedTracklessJetUnpairedNonIso"),
      PassedMultiMuon                           (&fDirector,"PassedMultiMuon"),
      PassGammaMuon                             (&fDirector,"PassGammaMuon"),
      PassedMultiMuonTD                         (&fDirector,"PassedMultiMuonTD"),
      PassedMultiMuonTDFirstEmpty               (&fDirector,"PassedMultiMuonTDFirstEmpty"),
      PassedMultiMuonTDUnpairedIso              (&fDirector,"PassedMultiMuonTDUnpairedIso"),
      PassedMultiMuonTDUnpairedNonIso           (&fDirector,"PassedMultiMuonTDUnpairedNonIso"),
      PassGammaMuonTD                           (&fDirector,"PassGammaMuonTD"),
      PassGammaMuonTDFirstEmpty                 (&fDirector,"PassGammaMuonTDFirstEmpty"),
      PassGammaMuonTDUnpairedIso                (&fDirector,"PassGammaMuonTDUnpairedIso"),
      PassGammaMuonTDUnpairedNonIso             (&fDirector,"PassGammaMuonTDUnpairedNonIso"),
      isGood                                    (&fDirector,"isGood"),
      ms_t                                      (&fDirector,"ms_t"),
      ms_x                                      (&fDirector,"ms_x"),
      ms_y                                      (&fDirector,"ms_y"),
      ms_z                                      (&fDirector,"ms_z"),
      ms_dx                                     (&fDirector,"ms_dx"),
      ms_dy                                     (&fDirector,"ms_dy"),
      ms_dz                                     (&fDirector,"ms_dz"),
      hcell_t                                   (&fDirector,"hcell_t"),
      hcell_e                                   (&fDirector,"hcell_e"),
      hcell_x                                   (&fDirector,"hcell_x"),
      hcell_y                                   (&fDirector,"hcell_y"),
      hcell_z                                   (&fDirector,"hcell_z"),
      Track_pt                                  (&fDirector,"Track_pt"),
      Track_eta                                 (&fDirector,"Track_eta"),
      Track_phi                                 (&fDirector,"Track_phi"),
      Track_chi2                                (&fDirector,"Track_chi2"),
      Track_nDoF                                (&fDirector,"Track_nDoF"),
      Track_d0                                  (&fDirector,"Track_d0"),
      Track_z0                                  (&fDirector,"Track_z0"),
      Track_nPixel                              (&fDirector,"Track_nPixel"),
      Track_nSCT                                (&fDirector,"Track_nSCT"),
      Track_nTRT                                (&fDirector,"Track_nTRT"),
      Track_nTRT_Barrel                         (&fDirector,"Track_nTRT_Barrel"),
      Track_nTRT_Barrel_Hits_r                  (&fDirector,"Track_nTRT_Barrel_Hits_r"),
      Track_nTRT_Barrel_Hits_phi                (&fDirector,"Track_nTRT_Barrel_Hits_phi"),
      Track_TRT_etaregion                       (&fDirector,"Track_TRT_etaregion"),
      MET_base                                  (&fDirector,"MET_base"),
      MET_base_phi                              (&fDirector,"MET_base_phi"),
      MET_RefFinal                              (&fDirector,"MET_RefFinal"),
      MET_RefFinal_phi                          (&fDirector,"MET_RefFinal_phi"),
      llp_N                                     (&fDirector,"llp_N"),
      llp_pdgid                                 (&fDirector,"llp_pdgid"),
      llp_barcode                               (&fDirector,"llp_barcode"),
      llp_mother_barcode                        (&fDirector,"llp_mother_barcode"),
      llp_eta                                   (&fDirector,"llp_eta"),
      llp_phi                                   (&fDirector,"llp_phi"),
      llp_x                                     (&fDirector,"llp_x"),
      llp_y                                     (&fDirector,"llp_y"),
      llp_z                                     (&fDirector,"llp_z"),
      llp_m                                     (&fDirector,"llp_m"),
      llp_e                                     (&fDirector,"llp_e"),
      llp_px                                    (&fDirector,"llp_px"),
      llp_py                                    (&fDirector,"llp_py"),
      llp_pz                                    (&fDirector,"llp_pz"),
      llp_ndecay                                (&fDirector,"llp_ndecay"),
      caloCluster_nClusters                     (&fDirector,"caloCluster_nClusters"),
      caloCluster_totE                          (&fDirector,"caloCluster_totE"),
      caloCluster_energy                        (&fDirector,"caloCluster_energy"),
      caloCluster_eta                           (&fDirector,"caloCluster_eta"),
      caloCluster_phi                           (&fDirector,"caloCluster_phi"),
      caloCluster_hadEnergy                     (&fDirector,"caloCluster_hadEnergy"),
      caloCluster_emEnergy                      (&fDirector,"caloCluster_emEnergy"),
      caloCluster_emB0                          (&fDirector,"caloCluster_emB0"),
      caloCluster_emB1                          (&fDirector,"caloCluster_emB1"),
      caloCluster_emB2                          (&fDirector,"caloCluster_emB2"),
      caloCluster_emB3                          (&fDirector,"caloCluster_emB3"),
      caloCluster_emE0                          (&fDirector,"caloCluster_emE0"),
      caloCluster_emE1                          (&fDirector,"caloCluster_emE1"),
      caloCluster_emE2                          (&fDirector,"caloCluster_emE2"),
      caloCluster_emE3                          (&fDirector,"caloCluster_emE3"),
      caloCluster_emF0                          (&fDirector,"caloCluster_emF0"),
      caloCluster_emF1                          (&fDirector,"caloCluster_emF1"),
      caloCluster_emF2                          (&fDirector,"caloCluster_emF2"),
      caloCluster_hadB0                         (&fDirector,"caloCluster_hadB0"),
      caloCluster_hadB1                         (&fDirector,"caloCluster_hadB1"),
      caloCluster_hadB2                         (&fDirector,"caloCluster_hadB2"),
      caloCluster_hadExB0                       (&fDirector,"caloCluster_hadExB0"),
      caloCluster_hadExB1                       (&fDirector,"caloCluster_hadExB1"),
      caloCluster_hadExB2                       (&fDirector,"caloCluster_hadExB2"),
      caloCluster_hadE0                         (&fDirector,"caloCluster_hadE0"),
      caloCluster_hadE1                         (&fDirector,"caloCluster_hadE1"),
      caloCluster_hadE2                         (&fDirector,"caloCluster_hadE2"),
      caloCluster_hadE3                         (&fDirector,"caloCluster_hadE3"),
      caloCluster_hadGap1                       (&fDirector,"caloCluster_hadGap1"),
      caloCluster_hadGap2                       (&fDirector,"caloCluster_hadGap2"),
      caloCluster_hadGap3                       (&fDirector,"caloCluster_hadGap3"),
      caloCluster_emB0_eta                      (&fDirector,"caloCluster_emB0_eta"),
      caloCluster_emB1_eta                      (&fDirector,"caloCluster_emB1_eta"),
      caloCluster_emB2_eta                      (&fDirector,"caloCluster_emB2_eta"),
      caloCluster_emB3_eta                      (&fDirector,"caloCluster_emB3_eta"),
      caloCluster_emE0_eta                      (&fDirector,"caloCluster_emE0_eta"),
      caloCluster_emE1_eta                      (&fDirector,"caloCluster_emE1_eta"),
      caloCluster_emE2_eta                      (&fDirector,"caloCluster_emE2_eta"),
      caloCluster_emE3_eta                      (&fDirector,"caloCluster_emE3_eta"),
      caloCluster_emF0_eta                      (&fDirector,"caloCluster_emF0_eta"),
      caloCluster_emF1_eta                      (&fDirector,"caloCluster_emF1_eta"),
      caloCluster_emF2_eta                      (&fDirector,"caloCluster_emF2_eta"),
      caloCluster_hadB0_eta                     (&fDirector,"caloCluster_hadB0_eta"),
      caloCluster_hadB1_eta                     (&fDirector,"caloCluster_hadB1_eta"),
      caloCluster_hadB2_eta                     (&fDirector,"caloCluster_hadB2_eta"),
      caloCluster_hadExB0_eta                   (&fDirector,"caloCluster_hadExB0_eta"),
      caloCluster_hadExB1_eta                   (&fDirector,"caloCluster_hadExB1_eta"),
      caloCluster_hadExB2_eta                   (&fDirector,"caloCluster_hadExB2_eta"),
      caloCluster_hadE0_eta                     (&fDirector,"caloCluster_hadE0_eta"),
      caloCluster_hadE1_eta                     (&fDirector,"caloCluster_hadE1_eta"),
      caloCluster_hadE2_eta                     (&fDirector,"caloCluster_hadE2_eta"),
      caloCluster_hadE3_eta                     (&fDirector,"caloCluster_hadE3_eta"),
      caloCluster_hadGap1_eta                   (&fDirector,"caloCluster_hadGap1_eta"),
      caloCluster_hadGap2_eta                   (&fDirector,"caloCluster_hadGap2_eta"),
      caloCluster_hadGap3_eta                   (&fDirector,"caloCluster_hadGap3_eta"),
      caloCluster_emB0_phi                      (&fDirector,"caloCluster_emB0_phi"),
      caloCluster_emB1_phi                      (&fDirector,"caloCluster_emB1_phi"),
      caloCluster_emB2_phi                      (&fDirector,"caloCluster_emB2_phi"),
      caloCluster_emB3_phi                      (&fDirector,"caloCluster_emB3_phi"),
      caloCluster_emE0_phi                      (&fDirector,"caloCluster_emE0_phi"),
      caloCluster_emE1_phi                      (&fDirector,"caloCluster_emE1_phi"),
      caloCluster_emE2_phi                      (&fDirector,"caloCluster_emE2_phi"),
      caloCluster_emE3_phi                      (&fDirector,"caloCluster_emE3_phi"),
      caloCluster_emF0_phi                      (&fDirector,"caloCluster_emF0_phi"),
      caloCluster_emF1_phi                      (&fDirector,"caloCluster_emF1_phi"),
      caloCluster_emF2_phi                      (&fDirector,"caloCluster_emF2_phi"),
      caloCluster_hadB0_phi                     (&fDirector,"caloCluster_hadB0_phi"),
      caloCluster_hadB1_phi                     (&fDirector,"caloCluster_hadB1_phi"),
      caloCluster_hadB2_phi                     (&fDirector,"caloCluster_hadB2_phi"),
      caloCluster_hadExB0_phi                   (&fDirector,"caloCluster_hadExB0_phi"),
      caloCluster_hadExB1_phi                   (&fDirector,"caloCluster_hadExB1_phi"),
      caloCluster_hadExB3_phi                   (&fDirector,"caloCluster_hadExB3_phi"),
      caloCluster_hadE0_phi                     (&fDirector,"caloCluster_hadE0_phi"),
      caloCluster_hadE1_phi                     (&fDirector,"caloCluster_hadE1_phi"),
      caloCluster_hadE2_phi                     (&fDirector,"caloCluster_hadE2_phi"),
      caloCluster_hadE3_phi                     (&fDirector,"caloCluster_hadE3_phi"),
      caloCluster_hadGap1_phi                   (&fDirector,"caloCluster_hadGap1_phi"),
      caloCluster_hadGap2_phi                   (&fDirector,"caloCluster_hadGap2_phi"),
      caloCluster_hadGap3_phi                   (&fDirector,"caloCluster_hadGap3_phi"),
      caloCluster_larB                          (&fDirector,"caloCluster_larB"),
      caloCluster_larE                          (&fDirector,"caloCluster_larE"),
      caloCluster_fCal                          (&fDirector,"caloCluster_fCal"),
      caloCluster_hadE                          (&fDirector,"caloCluster_hadE"),
      caloCluster_tileB                         (&fDirector,"caloCluster_tileB"),
      caloCluster_tileG                         (&fDirector,"caloCluster_tileG"),
      caloCluster_tileExB                       (&fDirector,"caloCluster_tileExB"),
      caloTower_nTowers                         (&fDirector,"caloTower_nTowers"),
      caloTower_totE                            (&fDirector,"caloTower_totE"),
      caloTower_energy                          (&fDirector,"caloTower_energy"),
      caloTower_eta                             (&fDirector,"caloTower_eta"),
      caloTower_phi                             (&fDirector,"caloTower_phi"),
      AntiKt4TopoJets_CalibE                    (&fDirector,"AntiKt4TopoJets_CalibE"),
      AntiKt4TopoJets_Calibpt                   (&fDirector,"AntiKt4TopoJets_Calibpt"),
      AntiKt4TopoJets_CalibEt                   (&fDirector,"AntiKt4TopoJets_CalibEt"),
      AntiKt4TopoJets_Calibpx                   (&fDirector,"AntiKt4TopoJets_Calibpx"),
      AntiKt4TopoJets_Calibpy                   (&fDirector,"AntiKt4TopoJets_Calibpy"),
      AntiKt4TopoJets_Calibpz                   (&fDirector,"AntiKt4TopoJets_Calibpz"),
      AntiKt4TopoJets_Calibm                    (&fDirector,"AntiKt4TopoJets_Calibm"),
      AntiKt4TopoJets_Calibscale                (&fDirector,"AntiKt4TopoJets_Calibscale"),
      AntiKt4TopoJets_Calibeta                  (&fDirector,"AntiKt4TopoJets_Calibeta"),
      AntiKt4TopoJets_Calibphi                  (&fDirector,"AntiKt4TopoJets_Calibphi"),
      AntiKt4TopoJets_nJets                     (&fDirector,"AntiKt4TopoJets_nJets"),
      AntiKt4TopoJets_E                         (&fDirector,"AntiKt4TopoJets_E"),
      AntiKt4TopoJets_p                         (&fDirector,"AntiKt4TopoJets_p"),
      AntiKt4TopoJets_Et                        (&fDirector,"AntiKt4TopoJets_Et"),
      AntiKt4TopoJets_pt                        (&fDirector,"AntiKt4TopoJets_pt"),
      AntiKt4TopoJets_m                         (&fDirector,"AntiKt4TopoJets_m"),
      AntiKt4TopoJets_y                         (&fDirector,"AntiKt4TopoJets_y"),
      AntiKt4TopoJets_tanth                     (&fDirector,"AntiKt4TopoJets_tanth"),
      AntiKt4TopoJets_eta                       (&fDirector,"AntiKt4TopoJets_eta"),
      AntiKt4TopoJets_phi                       (&fDirector,"AntiKt4TopoJets_phi"),
      AntiKt4TopoJets_Eh                        (&fDirector,"AntiKt4TopoJets_Eh"),
      AntiKt4TopoJets_Eem                       (&fDirector,"AntiKt4TopoJets_Eem"),
      AntiKt4TopoJets_px                        (&fDirector,"AntiKt4TopoJets_px"),
      AntiKt4TopoJets_py                        (&fDirector,"AntiKt4TopoJets_py"),
      AntiKt4TopoJets_pz                        (&fDirector,"AntiKt4TopoJets_pz"),
      AntiKt4TopoJets_Ncon                      (&fDirector,"AntiKt4TopoJets_Ncon"),
      AntiKt4TopoJets_ptcon                     (&fDirector,"AntiKt4TopoJets_ptcon"),
      AntiKt4TopoJets_econ                      (&fDirector,"AntiKt4TopoJets_econ"),
      AntiKt4TopoJets_etacon                    (&fDirector,"AntiKt4TopoJets_etacon"),
      AntiKt4TopoJets_phicon                    (&fDirector,"AntiKt4TopoJets_phicon"),
      AntiKt4TopoJets_weightcon                 (&fDirector,"AntiKt4TopoJets_weightcon"),
      AntiKt4TopoJets_emfrac                    (&fDirector,"AntiKt4TopoJets_emfrac"),
      momentList                                (&fDirector,"momentList"),
      AntiKt4TopoJets_moments                   (&fDirector,"AntiKt4TopoJets_moments"),
      AntiKt4TopoJets_author                    (&fDirector,"AntiKt4TopoJets_author"),
      AntiKt4TopoJets_calibTags                 (&fDirector,"AntiKt4TopoJets_calibTags"),
      AntiKt4TopoJets_e_PreSamplerB             (&fDirector,"AntiKt4TopoJets_e_PreSamplerB"),
      AntiKt4TopoJets_e_EMB1                    (&fDirector,"AntiKt4TopoJets_e_EMB1"),
      AntiKt4TopoJets_e_EMB2                    (&fDirector,"AntiKt4TopoJets_e_EMB2"),
      AntiKt4TopoJets_e_EMB3                    (&fDirector,"AntiKt4TopoJets_e_EMB3"),
      AntiKt4TopoJets_e_PreSamplerE             (&fDirector,"AntiKt4TopoJets_e_PreSamplerE"),
      AntiKt4TopoJets_e_EME1                    (&fDirector,"AntiKt4TopoJets_e_EME1"),
      AntiKt4TopoJets_e_EME2                    (&fDirector,"AntiKt4TopoJets_e_EME2"),
      AntiKt4TopoJets_e_EME3                    (&fDirector,"AntiKt4TopoJets_e_EME3"),
      AntiKt4TopoJets_e_HEC0                    (&fDirector,"AntiKt4TopoJets_e_HEC0"),
      AntiKt4TopoJets_e_HEC1                    (&fDirector,"AntiKt4TopoJets_e_HEC1"),
      AntiKt4TopoJets_e_HEC2                    (&fDirector,"AntiKt4TopoJets_e_HEC2"),
      AntiKt4TopoJets_e_HEC3                    (&fDirector,"AntiKt4TopoJets_e_HEC3"),
      AntiKt4TopoJets_e_TileBar0                (&fDirector,"AntiKt4TopoJets_e_TileBar0"),
      AntiKt4TopoJets_e_TileBar1                (&fDirector,"AntiKt4TopoJets_e_TileBar1"),
      AntiKt4TopoJets_e_TileBar2                (&fDirector,"AntiKt4TopoJets_e_TileBar2"),
      AntiKt4TopoJets_e_TileGap1                (&fDirector,"AntiKt4TopoJets_e_TileGap1"),
      AntiKt4TopoJets_e_TileGap2                (&fDirector,"AntiKt4TopoJets_e_TileGap2"),
      AntiKt4TopoJets_e_TileGap3                (&fDirector,"AntiKt4TopoJets_e_TileGap3"),
      AntiKt4TopoJets_e_TileExt0                (&fDirector,"AntiKt4TopoJets_e_TileExt0"),
      AntiKt4TopoJets_e_TileExt1                (&fDirector,"AntiKt4TopoJets_e_TileExt1"),
      AntiKt4TopoJets_e_TileExt2                (&fDirector,"AntiKt4TopoJets_e_TileExt2"),
      AntiKt4TopoJets_e_FCAL0                   (&fDirector,"AntiKt4TopoJets_e_FCAL0"),
      AntiKt4TopoJets_e_FCAL1                   (&fDirector,"AntiKt4TopoJets_e_FCAL1"),
      AntiKt4TopoJets_e_FCAL2                   (&fDirector,"AntiKt4TopoJets_e_FCAL2"),
      AntiKt4TopoJets_time_calo                 (&fDirector,"AntiKt4TopoJets_time_calo"),
      AntiKt4TopoJets_fbadQ                     (&fDirector,"AntiKt4TopoJets_fbadQ"),
      AntiKt4TopoJets_hecF                      (&fDirector,"AntiKt4TopoJets_hecF"),
      AntiKt4TopoJets_TileGap3F                 (&fDirector,"AntiKt4TopoJets_TileGap3F"),
      AntiKt4TopoJetsjet_truepdg                (&fDirector,"AntiKt4TopoJetsjet_truepdg"),
      AntiKt4TopoJetsjet_truelabel              (&fDirector,"AntiKt4TopoJetsjet_truelabel"),
      trig_triggers                             (&fDirector,"trig_triggers"),
      trig_prescales                            (&fDirector,"trig_prescales"),
      trig_bits                                 (&fDirector,"trig_bits"),
      trig_passed                               (&fDirector,"trig_passed"),
      trig_mbts_times                           (&fDirector,"trig_mbts_times"),
      trig_mbts_energy                          (&fDirector,"trig_mbts_energy"),
      trig_mbts_quality                         (&fDirector,"trig_mbts_quality"),
      trig_mbts_eta                             (&fDirector,"trig_mbts_eta"),
      trig_mbts_phi                             (&fDirector,"trig_mbts_phi"),
      trig_mbts_channel                         (&fDirector,"trig_mbts_channel"),
      trig_calo_ncellA                          (&fDirector,"trig_calo_ncellA"),
      trig_calo_ncellC                          (&fDirector,"trig_calo_ncellC"),
      trig_calo_timeA                           (&fDirector,"trig_calo_timeA"),
      trig_calo_timeC                           (&fDirector,"trig_calo_timeC"),
      trig_calo_energyA                         (&fDirector,"trig_calo_energyA"),
      trig_calo_energyC                         (&fDirector,"trig_calo_energyC"),
      cell_number                               (&fDirector,"cell_number"),
      cell_sampling                             (&fDirector,"cell_sampling"),
      cell_time                                 (&fDirector,"cell_time"),
      cell_energy                               (&fDirector,"cell_energy"),
      Mu_N                                      (&fDirector,"Mu_N"),
      Mu_E                                      (&fDirector,"Mu_E"),
      Mu_p                                      (&fDirector,"Mu_p"),
      Mu_p_T                                    (&fDirector,"Mu_p_T"),
      Mu_p_x                                    (&fDirector,"Mu_p_x"),
      Mu_p_y                                    (&fDirector,"Mu_p_y"),
      Mu_p_z                                    (&fDirector,"Mu_p_z"),
      Mu_phi                                    (&fDirector,"Mu_phi"),
      Mu_eta                                    (&fDirector,"Mu_eta"),
      Mu_m                                      (&fDirector,"Mu_m"),
      Mu_charge                                 (&fDirector,"Mu_charge"),
      Mu_cone20                                 (&fDirector,"Mu_cone20"),
      Mu_cone30                                 (&fDirector,"Mu_cone30"),
      Mu_cone40                                 (&fDirector,"Mu_cone40"),
      Mu_trk10                                  (&fDirector,"Mu_trk10"),
      Mu_trk20                                  (&fDirector,"Mu_trk20"),
      Mu_trk30                                  (&fDirector,"Mu_trk30"),
      Mu_trk40                                  (&fDirector,"Mu_trk40"),
      Mu_matchchi2                              (&fDirector,"Mu_matchchi2"),
      Mu_matchchi2DoverF                        (&fDirector,"Mu_matchchi2DoverF"),
      Mu_chi2                                   (&fDirector,"Mu_chi2"),
      Mu_chi2DoverF                             (&fDirector,"Mu_chi2DoverF"),
      Mu_energyDeposit                          (&fDirector,"Mu_energyDeposit"),
      Mu_energySigma                            (&fDirector,"Mu_energySigma"),
      Mu_x                                      (&fDirector,"Mu_x"),
      Mu_y                                      (&fDirector,"Mu_y"),
      Mu_z                                      (&fDirector,"Mu_z"),
      Mu_d0                                     (&fDirector,"Mu_d0"),
      Mu_z0sinTh                                (&fDirector,"Mu_z0sinTh"),
      Mu_a0                                     (&fDirector,"Mu_a0"),
      Mu_d0err                                  (&fDirector,"Mu_d0err"),
      Mu_isCombined                             (&fDirector,"Mu_isCombined"),
      Mu_isStandAlone                           (&fDirector,"Mu_isStandAlone"),
      Mu_isLowPt                                (&fDirector,"Mu_isLowPt"),
      Mu_isLoose                                (&fDirector,"Mu_isLoose"),
      Mu_isMedium                               (&fDirector,"Mu_isMedium"),
      Mu_isTight                                (&fDirector,"Mu_isTight"),
      Mu_isMuonLikelihood                       (&fDirector,"Mu_isMuonLikelihood"),
      Mu_author                                 (&fDirector,"Mu_author"),
      Mu_allAuthors                             (&fDirector,"Mu_allAuthors"),
      Mu_InDet_p_T                              (&fDirector,"Mu_InDet_p_T"),
      Mu_InDet_phi                              (&fDirector,"Mu_InDet_phi"),
      Mu_InDet_eta                              (&fDirector,"Mu_InDet_eta"),
      Mu_InDet_trk_Index                        (&fDirector,"Mu_InDet_trk_Index"),
      Mu_InDet_pixelhits                        (&fDirector,"Mu_InDet_pixelhits"),
      Mu_InDet_scthits                          (&fDirector,"Mu_InDet_scthits"),
      Mu_InDet_blayerhits                       (&fDirector,"Mu_InDet_blayerhits"),
      Mu_InDet_trthits                          (&fDirector,"Mu_InDet_trthits"),
      Mu_InDet_trththits                        (&fDirector,"Mu_InDet_trththits"),
      Mu_MuonSpec_p_T                           (&fDirector,"Mu_MuonSpec_p_T"),
      Mu_MuonSpec_phi                           (&fDirector,"Mu_MuonSpec_phi"),
      Mu_MuonSpec_eta                           (&fDirector,"Mu_MuonSpec_eta"),
      Mu_MuonSpec_d0                            (&fDirector,"Mu_MuonSpec_d0"),
      Mu_MuonSpec_z0                            (&fDirector,"Mu_MuonSpec_z0"),
      Mu_MuonSpec_phi0                          (&fDirector,"Mu_MuonSpec_phi0"),
      Mu_MuonSpec_theta                         (&fDirector,"Mu_MuonSpec_theta"),
      Mu_MuonSpec_qOp                           (&fDirector,"Mu_MuonSpec_qOp"),
      Mu_MuonSpec_chi2                          (&fDirector,"Mu_MuonSpec_chi2"),
      Mu_MuonSpec_ndof                          (&fDirector,"Mu_MuonSpec_ndof"),
      Mu_MuonSpecSurf_x                         (&fDirector,"Mu_MuonSpecSurf_x"),
      Mu_MuonSpecSurf_y                         (&fDirector,"Mu_MuonSpecSurf_y"),
      Mu_MuonSpecSurf_z                         (&fDirector,"Mu_MuonSpecSurf_z"),
      Mu_MuonSpecSurf_px                        (&fDirector,"Mu_MuonSpecSurf_px"),
      Mu_MuonSpecSurf_py                        (&fDirector,"Mu_MuonSpecSurf_py"),
      Mu_MuonSpecSurf_pz                        (&fDirector,"Mu_MuonSpecSurf_pz"),
      Mu_MuonExtr_CellE                         (&fDirector,"Mu_MuonExtr_CellE"),
      Mu_MuonExtr_CellEt                        (&fDirector,"Mu_MuonExtr_CellEt"),
      Mu_MuonExtr_CellPhi                       (&fDirector,"Mu_MuonExtr_CellPhi"),
      Mu_MuonExtr_CellEta                       (&fDirector,"Mu_MuonExtr_CellEta"),
      Mu_MuonExtr_CellSampl                     (&fDirector,"Mu_MuonExtr_CellSampl"),
      Mu_MuonExtr_cellEtIsol20                  (&fDirector,"Mu_MuonExtr_cellEtIsol20"),
      Mu_MuonExtr_cellEtIsol30                  (&fDirector,"Mu_MuonExtr_cellEtIsol30"),
      Mu_MuonExtr_cellEtIsol40                  (&fDirector,"Mu_MuonExtr_cellEtIsol40"),
      Mu_MuonExtr_p_T                           (&fDirector,"Mu_MuonExtr_p_T"),
      Mu_MuonExtr_phi                           (&fDirector,"Mu_MuonExtr_phi"),
      Mu_MuonExtr_eta                           (&fDirector,"Mu_MuonExtr_eta"),
      Mu_MuonExtr_p                             (&fDirector,"Mu_MuonExtr_p"),
      Mu_MuonExtr_d0                            (&fDirector,"Mu_MuonExtr_d0"),
      Mu_MuonExtr_z0                            (&fDirector,"Mu_MuonExtr_z0"),
      Mu_MuonExtr_phi0                          (&fDirector,"Mu_MuonExtr_phi0"),
      Mu_MuonExtr_theta                         (&fDirector,"Mu_MuonExtr_theta"),
      Mu_MuonExtr_qOp                           (&fDirector,"Mu_MuonExtr_qOp"),
      Mu_MuonExtr_charge                        (&fDirector,"Mu_MuonExtr_charge"),
      Mu_Combined_p_T                           (&fDirector,"Mu_Combined_p_T"),
      Mu_Combined_phi                           (&fDirector,"Mu_Combined_phi"),
      Mu_Combined_eta                           (&fDirector,"Mu_Combined_eta"),
      Mu_beta                                   (&fDirector,"Mu_beta"),
      Mu_beta_L2                                (&fDirector,"Mu_beta_L2"),
      Mu_mdthits_N                              (&fDirector,"Mu_mdthits_N"),
      Mu_cscetahits_N                           (&fDirector,"Mu_cscetahits_N"),
      Mu_cschits_N                              (&fDirector,"Mu_cschits_N"),
      Mu_rpchits_N                              (&fDirector,"Mu_rpchits_N"),
      Mu_tgchits_N                              (&fDirector,"Mu_tgchits_N"),
      muMSonly_author                           (&fDirector,"muMSonly_author"),
      muMSonly_px                               (&fDirector,"muMSonly_px"),
      muMSonly_py                               (&fDirector,"muMSonly_py"),
      muMSonly_pz                               (&fDirector,"muMSonly_pz"),
      muMSonly_eta                              (&fDirector,"muMSonly_eta"),
      muMSonly_phi                              (&fDirector,"muMSonly_phi"),
      muMSonly_e                                (&fDirector,"muMSonly_e"),
      muMSonly_charge                           (&fDirector,"muMSonly_charge"),
      muMSonly_chi2                             (&fDirector,"muMSonly_chi2"),
      muMSonly_ndof                             (&fDirector,"muMSonly_ndof"),
      MSvx_R                                    (&fDirector,"MSvx_R"),
      MSvx_z                                    (&fDirector,"MSvx_z"),
      MSvx_theta                                (&fDirector,"MSvx_theta"),
      MSvx_phi                                  (&fDirector,"MSvx_phi"),
      MSvx_nTrks                                (&fDirector,"MSvx_nTrks"),
      MSvx_chi2prob                             (&fDirector,"MSvx_chi2prob"),
      MSvx_author                               (&fDirector,"MSvx_author"),
      MSvx_trk_px                               (&fDirector,"MSvx_trk_px"),
      MSvx_trk_py                               (&fDirector,"MSvx_trk_py"),
      MSvx_trk_pz                               (&fDirector,"MSvx_trk_pz"),
      MSvx_trk_charge                           (&fDirector,"MSvx_trk_charge"),
      MSvx_nMDT                                 (&fDirector,"MSvx_nMDT"),
      MSvx_nIMDT                                (&fDirector,"MSvx_nIMDT"),
      MSvx_nMMDT                                (&fDirector,"MSvx_nMMDT"),
      MSvx_nOMDT                                (&fDirector,"MSvx_nOMDT"),
      MSvx_nRPC                                 (&fDirector,"MSvx_nRPC"),
      MSvx_nTGC                                 (&fDirector,"MSvx_nTGC"),
      HVtrk_x                                   (&fDirector,"HVtrk_x"),
      HVtrk_y                                   (&fDirector,"HVtrk_y"),
      HVtrk_z                                   (&fDirector,"HVtrk_z"),
      HVtrk_pT                                  (&fDirector,"HVtrk_pT"),
      HVtrk_pz                                  (&fDirector,"HVtrk_pz"),
      HVtrk_charge                              (&fDirector,"HVtrk_charge"),
      MDT_x                                     (&fDirector,"MDT_x"),
      MDT_y                                     (&fDirector,"MDT_y"),
      MDT_z                                     (&fDirector,"MDT_z"),
      MDT_ml                                    (&fDirector,"MDT_ml"),
      MDT_chamber                               (&fDirector,"MDT_chamber"),
      Pix_Seg_Phi                               (&fDirector,"Pix_Seg_Phi"),
      Pix_Seg_Pt                                (&fDirector,"Pix_Seg_Pt"),
      Pix_Seg_Eta                               (&fDirector,"Pix_Seg_Eta"),
      Pix_Seg_z0                                (&fDirector,"Pix_Seg_z0"),
      Pix_Seg_Hits_z                            (&fDirector,"Pix_Seg_Hits_z"),
      SCT_Seg_Phi                               (&fDirector,"SCT_Seg_Phi"),
      SCT_Seg_Pt                                (&fDirector,"SCT_Seg_Pt"),
      SCT_Seg_Eta                               (&fDirector,"SCT_Seg_Eta"),
      SCT_Seg_z0                                (&fDirector,"SCT_Seg_z0"),
      SCT_Seg_Hits_z                            (&fDirector,"SCT_Seg_Hits_z"),
      TRT_Seg_PosEta_Phi                        (&fDirector,"TRT_Seg_PosEta_Phi"),
      TRT_Seg_NegEta_Phi                        (&fDirector,"TRT_Seg_NegEta_Phi"),
      TRT_Seg_PosEta_d0                         (&fDirector,"TRT_Seg_PosEta_d0"),
      TRT_Seg_NegEta_d0                         (&fDirector,"TRT_Seg_NegEta_d0"),
      TRT_Seg_PosEta_Pt                         (&fDirector,"TRT_Seg_PosEta_Pt"),
      TRT_Seg_NegEta_Pt                         (&fDirector,"TRT_Seg_NegEta_Pt"),
      TRT_Seg_PosEta_Hits_R                     (&fDirector,"TRT_Seg_PosEta_Hits_R"),
      TRT_Seg_PosEta_Hits_Phi                   (&fDirector,"TRT_Seg_PosEta_Hits_Phi"),
      TRT_Seg_NegEta_Hits_R                     (&fDirector,"TRT_Seg_NegEta_Hits_R"),
      TRT_Seg_NegEta_Hits_Phi                   (&fDirector,"TRT_Seg_NegEta_Hits_Phi"),
      TRT_Vtx_PosEta_r                          (&fDirector,"TRT_Vtx_PosEta_r"),
      TRT_Vtx_NegEta_r                          (&fDirector,"TRT_Vtx_NegEta_r"),
      TRT_Vtx_PosEta_Phi                        (&fDirector,"TRT_Vtx_PosEta_Phi"),
      TRT_Vtx_NegEta_Phi                        (&fDirector,"TRT_Vtx_NegEta_Phi"),
      TRT_Vtx_PosEta_ConeAngle                  (&fDirector,"TRT_Vtx_PosEta_ConeAngle"),
      TRT_Vtx_NegEta_ConeAngle                  (&fDirector,"TRT_Vtx_NegEta_ConeAngle"),
      TRT_Vtx_PosEta_numInLowerCone             (&fDirector,"TRT_Vtx_PosEta_numInLowerCone"),
      TRT_Vtx_NegEta_numInLowerCone             (&fDirector,"TRT_Vtx_NegEta_numInLowerCone"),
      TRT_Vtx_PosEta_numInUpperCone             (&fDirector,"TRT_Vtx_PosEta_numInUpperCone"),
      TRT_Vtx_NegEta_numInUpperCone             (&fDirector,"TRT_Vtx_NegEta_numInUpperCone"),
      TRT_Vtx_PosEta_numInPreHitConeFromIP_L    (&fDirector,"TRT_Vtx_PosEta_numInPreHitConeFromIP_L"),
      TRT_Vtx_NegEta_numInPreHitConeFromIP_L    (&fDirector,"TRT_Vtx_NegEta_numInPreHitConeFromIP_L"),
      TRT_Vtx_PosEta_numInPreHitConeFromIP_R    (&fDirector,"TRT_Vtx_PosEta_numInPreHitConeFromIP_R"),
      TRT_Vtx_NegEta_numInPreHitConeFromIP_R    (&fDirector,"TRT_Vtx_NegEta_numInPreHitConeFromIP_R"),
      TRT_Vtx_PosEta_fracInLowerCone            (&fDirector,"TRT_Vtx_PosEta_fracInLowerCone"),
      TRT_Vtx_NegEta_fracInLowerCone            (&fDirector,"TRT_Vtx_NegEta_fracInLowerCone"),
      TRT_Vtx_PosEta_fracInUpperCone            (&fDirector,"TRT_Vtx_PosEta_fracInUpperCone"),
      TRT_Vtx_NegEta_fracInUpperCone            (&fDirector,"TRT_Vtx_NegEta_fracInUpperCone"),
      TRT_Vtx_PosEta_fracInPreHitConeFromIP_L   (&fDirector,"TRT_Vtx_PosEta_fracInPreHitConeFromIP_L"),
      TRT_Vtx_NegEta_fracInPreHitConeFromIP_L   (&fDirector,"TRT_Vtx_NegEta_fracInPreHitConeFromIP_L"),
      TRT_Vtx_PosEta_fracInPreHitConeFromIP_R   (&fDirector,"TRT_Vtx_PosEta_fracInPreHitConeFromIP_R"),
      TRT_Vtx_NegEta_fracInPreHitConeFromIP_R   (&fDirector,"TRT_Vtx_NegEta_fracInPreHitConeFromIP_R"),
      Run                                       (&fDirector,"Run"),
      Event                                     (&fDirector,"Event"),
      Time                                      (&fDirector,"Time"),
      LumiBlock                                 (&fDirector,"LumiBlock"),
      BCID                                      (&fDirector,"BCID"),
      LVL1ID                                    (&fDirector,"LVL1ID"),
      Weight                                    (&fDirector,"Weight"),
      IEvent                                    (&fDirector,"IEvent"),
      StatusElement                             (&fDirector,"StatusElement"),
      LVL1TriggerType                           (&fDirector,"LVL1TriggerType"),
      LVL1TriggerInfo                           (&fDirector,"LVL1TriggerInfo"),
      LVL2TriggerInfo                           (&fDirector,"LVL2TriggerInfo"),
      EventFilterInfo                           (&fDirector,"EventFilterInfo"),
      StreamTagName                             (&fDirector,"StreamTagName"),
      StreamTagType                             (&fDirector,"StreamTagType")
      { }
   ~ntuple_CollectionTree();
   Int_t   Version() const {return 1;}
   void    Begin(::TTree *tree);
   void    SlaveBegin(::TTree *tree);
   void    Init(::TTree *tree);
   Bool_t  Notify();
   Bool_t  Process(Long64_t entry);
   void    SlaveTerminate();
   void    Terminate();

   ClassDef(ntuple_CollectionTree,0);


//inject the user's code
#include "junk_macro_parsettree_CollectionTree.C"
};

#endif


#ifdef __MAKECINT__
#pragma link C++ class ntuple_CollectionTree::TStlPx_string-;
#pragma link C++ class ntuple_CollectionTree::TStlPx_vector_float_-;
#pragma link C++ class ntuple_CollectionTree::TStlPx_vector_int_-;
#pragma link6 C++ class vector<float>;
#pragma link6 C++ class vector<long>;
#pragma link6 C++ class vector<unsigned short>;
#pragma link C++ class vector<float>;
#pragma link C++ class vector<vector<float> >;
#pragma link C++ class vector<vector<int> >;
#pragma link C++ class ntuple_CollectionTree;
#endif


inline ntuple_CollectionTree::~ntuple_CollectionTree() {
   // destructor. Clean up helpers.

}

inline void ntuple_CollectionTree::Init(TTree *tree)
{
//   Set branch addresses
   if (tree == 0) return;
   fChain = tree;
   fDirector.SetTree(fChain);
   if (htemp == 0) {
      htemp = fDirector.CreateHistogram(GetOption());
      htemp->SetTitle("junk_macro_parsettree_CollectionTree.C");
      fObject = htemp;
   }
}

Bool_t ntuple_CollectionTree::Notify()
{
   // Called when loading a new file.
   // Get branch pointers.
   fDirector.SetTree(fChain);
   junk_macro_parsettree_CollectionTree_Notify();
   
   return kTRUE;
}
   

inline void ntuple_CollectionTree::Begin(TTree *tree)
{
   // The Begin() function is called at the start of the query.
   // When running with PROOF Begin() is only called on the client.
   // The tree argument is deprecated (on PROOF 0 is passed).

   TString option = GetOption();
   junk_macro_parsettree_CollectionTree_Begin(tree);

}

inline void ntuple_CollectionTree::SlaveBegin(TTree *tree)
{
   // The SlaveBegin() function is called after the Begin() function.
   // When running with PROOF SlaveBegin() is called on each slave server.
   // The tree argument is deprecated (on PROOF 0 is passed).

   Init(tree);

   junk_macro_parsettree_CollectionTree_SlaveBegin(tree);

}

inline Bool_t ntuple_CollectionTree::Process(Long64_t entry)
{
   // The Process() function is called for each entry in the tree (or possibly
   // keyed object in the case of PROOF) to be processed. The entry argument
   // specifies which entry in the currently loaded tree is to be processed.
   // It can be passed to either TTree::GetEntry() or TBranch::GetEntry()
   // to read either all or the required parts of the data. When processing
   // keyed objects with PROOF, the object is already loaded and is available
   // via the fObject pointer.
   //
   // This function should contain the "body" of the analysis. It can contain
   // simple or elaborate selection criteria, run algorithms on the data
   // of the event and typically fill histograms.

   // WARNING when a selector is used with a TChain, you must use
   //  the pointer to the current TTree to call GetEntry(entry).
   //  The entry is always the local entry number in the current tree.
   //  Assuming that fChain is the pointer to the TChain being processed,
   //  use fChain->GetTree()->GetEntry(entry).


   fDirector.SetReadEntry(entry);
   junk_macro_parsettree_CollectionTree();
   junk_macro_parsettree_CollectionTree_Process(entry);
   return kTRUE;

}

inline void ntuple_CollectionTree::SlaveTerminate()
{
   // The SlaveTerminate() function is called after all entries or objects
   // have been processed. When running with PROOF SlaveTerminate() is called
   // on each slave server.
   junk_macro_parsettree_CollectionTree_SlaveTerminate();
}

inline void ntuple_CollectionTree::Terminate()
{
   // Function called at the end of the event loop.
   htemp = (TH1*)fObject;
   Int_t drawflag = (htemp && htemp->GetEntries()>0);
   
   if (gPad && !drawflag && !fOption.Contains("goff") && !fOption.Contains("same")) {
      gPad->Clear();
   } else {
      if (fOption.Contains("goff")) drawflag = false;
      if (drawflag) htemp->Draw(fOption);
   }
   junk_macro_parsettree_CollectionTree_Terminate();
}
