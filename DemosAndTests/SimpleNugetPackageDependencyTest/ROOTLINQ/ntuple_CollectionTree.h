/////////////////////////////////////////////////////////////////////////
//   This class has been automatically generated 
//   (at Fri Aug 28 00:37:29 2015 by ROOT version 5.34/32)
//   from TTree CollectionTree/CollectionTree
//   found on file: D:\Code\ROOT\LINQtoROOT\DemosAndTests\hvsample.root
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
#include <string>


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
   struct TStlPx_vector_double_
   {
      TStlPx_vector_double_(TBranchProxyDirector* director,const char *top,const char *mid=0) :
         ffPrefix(top,mid),
         obj(director, top, mid)
      {};
      TStlPx_vector_double_(TBranchProxyDirector* director, TBranchProxy *parent, const char *membername, const char *top=0, const char *mid=0) :
         ffPrefix(top,mid),
         obj(director, parent, membername)
      {};
      TBranchProxyHelper ffPrefix;
      InjecTBranchProxyInterface();
      const vector<double>& At(UInt_t i) {
         static vector<double> default_val;
         if (!obj.Read()) return default_val;
         vector<double> *temp = & obj.GetPtr()->at(i);
         if (temp) return *temp; else return default_val;
      }
      const vector<double>& operator[](Int_t i) { return At(i); }
      const vector<double>& operator[](UInt_t i) { return At(i); }
      Int_t GetEntries() { return obj.GetPtr()->size(); }
      const vector<vector<double> >* operator->() { return obj.GetPtr(); }
      operator vector<vector<double> >*() { return obj.GetPtr(); }
      TObjProxy<vector<vector<double> > > obj;

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
   TArrayCharProxy                            StreamESD_ref;
   TArrayCharProxy                            Stream1_ref;
   TArrayCharProxy                            Token;
   TStlSimpleProxy<vector<double> >           CalRatio_eta;
   TStlSimpleProxy<vector<double> >           CalRatio_phi;
   TStlSimpleProxy<vector<double> >           CalRatio_erat;
   TStlSimpleProxy<vector<double> >           CalRatio_ET;
   TStlSimpleProxy<vector<double> >           RoICluster_eta;
   TStlSimpleProxy<vector<double> >           RoICluster_phi;
   TStlSimpleProxy<vector<double> >           RoICluster_nRoI;
   TStlSimpleProxy<vector<double> >           Trackless_eta;
   TStlSimpleProxy<vector<double> >           Trackless_phi;
   TStlSimpleProxy<vector<double> >           Trackless_ET;
   TStlSimpleProxy<vector<double> >           AllEvents_nRoI;
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
   TStlSimpleProxy<vector<double> >           IP_x;
   TStlSimpleProxy<vector<double> >           IP_y;
   TStlSimpleProxy<vector<double> >           IP_z;
   TStlSimpleProxy<vector<double> >           L2Track_pt;
   TStlSimpleProxy<vector<double> >           L2Track_eta;
   TStlSimpleProxy<vector<double> >           L2Track_phi;
   TStlPx_string                              L2Track_author;
   TStlSimpleProxy<vector<double> >           L2Track_a0;
   TStlSimpleProxy<vector<double> >           L2Track_z0;
   TStlSimpleProxy<vector<double> >           L2Track_chi2;
   TStlSimpleProxy<vector<double> >           L2Track_nPixel;
   TStlSimpleProxy<vector<double> >           L2Track_nSCT;
   TStlSimpleProxy<vector<double> >           L2Track_nTRT;
   TIntProxy                                  PassedRoICluster;
   TIntProxy                                  PassedCalRatio;
   TIntProxy                                  PassedTracklessJet;
   TIntProxy                                  PassedMultiMuon;
   TIntProxy                                  PassGammaMuon;
   TIntProxy                                  PassedCollisionVertex;
   TStlSimpleProxy<vector<int> >              isGood;
   TStlSimpleProxy<vector<int> >              isUgly;
   TStlSimpleProxy<vector<int> >              isBad;
   TStlSimpleProxy<vector<double> >           Track_pt;
   TStlSimpleProxy<vector<double> >           Track_eta;
   TStlSimpleProxy<vector<double> >           Track_phi;
   TStlSimpleProxy<vector<double> >           Track_E;
   TStlSimpleProxy<vector<double> >           Track_chi2;
   TStlSimpleProxy<vector<double> >           Track_nDoF;
   TStlSimpleProxy<vector<double> >           Track_d0;
   TStlSimpleProxy<vector<double> >           Track_z0;
   TStlSimpleProxy<vector<double> >           Track_nPixel;
   TStlSimpleProxy<vector<double> >           Track_nSCT;
   TStlSimpleProxy<vector<double> >           Track_nTRT;
   TFloatProxy                                MET_final;
   TFloatProxy                                MET_final_phi;
   TFloatProxy                                MET_base;
   TFloatProxy                                MET_base_phi;
   TFloatProxy                                MET_topo;
   TFloatProxy                                MET_topo_phi;
   TUIntProxy                                 AntiKt4TopoJets_nJets;
   TStlSimpleProxy<vector<double> >           AntiKt4TopoJets_E;
   TStlSimpleProxy<vector<double> >           AntiKt4TopoJets_p;
   TStlSimpleProxy<vector<double> >           AntiKt4TopoJets_Et;
   TStlSimpleProxy<vector<double> >           AntiKt4TopoJets_pt;
   TStlSimpleProxy<vector<double> >           AntiKt4TopoJets_m;
   TStlSimpleProxy<vector<double> >           AntiKt4TopoJets_y;
   TStlSimpleProxy<vector<double> >           AntiKt4TopoJets_tanth;
   TStlSimpleProxy<vector<double> >           AntiKt4TopoJets_eta;
   TStlSimpleProxy<vector<double> >           AntiKt4TopoJets_phi;
   TStlSimpleProxy<vector<double> >           AntiKt4TopoJets_Eh;
   TStlSimpleProxy<vector<double> >           AntiKt4TopoJets_Eem;
   TStlSimpleProxy<vector<double> >           AntiKt4TopoJets_px;
   TStlSimpleProxy<vector<double> >           AntiKt4TopoJets_py;
   TStlSimpleProxy<vector<double> >           AntiKt4TopoJets_pz;
   TStlSimpleProxy<vector<int> >              AntiKt4TopoJets_Ncon;
   TStlPx_vector_double_                      AntiKt4TopoJets_ptcon;
   TStlPx_vector_double_                      AntiKt4TopoJets_econ;
   TStlPx_vector_double_                      AntiKt4TopoJets_etacon;
   TStlPx_vector_double_                      AntiKt4TopoJets_phicon;
   TStlPx_vector_double_                      AntiKt4TopoJets_weightcon;
   TStlSimpleProxy<vector<double> >           AntiKt4TopoJets_emfrac;
   TStlPx_string                              momentList;
   TStlPx_vector_double_                      AntiKt4TopoJets_moments;
   TStlPx_string                              AntiKt4TopoJets_author;
   TStlPx_string                              AntiKt4TopoJets_calibTags;
   TStlSimpleProxy<vector<double> >           AntiKt4TopoJets_e_PreSamplerB;
   TStlSimpleProxy<vector<double> >           AntiKt4TopoJets_e_EMB1;
   TStlSimpleProxy<vector<double> >           AntiKt4TopoJets_e_EMB2;
   TStlSimpleProxy<vector<double> >           AntiKt4TopoJets_e_EMB3;
   TStlSimpleProxy<vector<double> >           AntiKt4TopoJets_e_PreSamplerE;
   TStlSimpleProxy<vector<double> >           AntiKt4TopoJets_e_EME1;
   TStlSimpleProxy<vector<double> >           AntiKt4TopoJets_e_EME2;
   TStlSimpleProxy<vector<double> >           AntiKt4TopoJets_e_EME3;
   TStlSimpleProxy<vector<double> >           AntiKt4TopoJets_e_HEC0;
   TStlSimpleProxy<vector<double> >           AntiKt4TopoJets_e_HEC1;
   TStlSimpleProxy<vector<double> >           AntiKt4TopoJets_e_HEC2;
   TStlSimpleProxy<vector<double> >           AntiKt4TopoJets_e_HEC3;
   TStlSimpleProxy<vector<double> >           AntiKt4TopoJets_e_TileBar0;
   TStlSimpleProxy<vector<double> >           AntiKt4TopoJets_e_TileBar1;
   TStlSimpleProxy<vector<double> >           AntiKt4TopoJets_e_TileBar2;
   TStlSimpleProxy<vector<double> >           AntiKt4TopoJets_e_TileGap1;
   TStlSimpleProxy<vector<double> >           AntiKt4TopoJets_e_TileGap2;
   TStlSimpleProxy<vector<double> >           AntiKt4TopoJets_e_TileGap3;
   TStlSimpleProxy<vector<double> >           AntiKt4TopoJets_e_TileExt0;
   TStlSimpleProxy<vector<double> >           AntiKt4TopoJets_e_TileExt1;
   TStlSimpleProxy<vector<double> >           AntiKt4TopoJets_e_TileExt2;
   TStlSimpleProxy<vector<double> >           AntiKt4TopoJets_e_FCAL0;
   TStlSimpleProxy<vector<double> >           AntiKt4TopoJets_e_FCAL1;
   TStlSimpleProxy<vector<double> >           AntiKt4TopoJets_e_FCAL2;
   TStlSimpleProxy<vector<double> >           AntiKt4TopoJets_time_calo;
   TStlSimpleProxy<vector<double> >           AntiKt4TopoJets_fbadQ;
   TStlSimpleProxy<vector<double> >           AntiKt4TopoJets_hecF;
   TStlSimpleProxy<vector<double> >           AntiKt4TopoJets_TileGap3F;
   TStlPx_string                              trig_triggers;
   TStlSimpleProxy<vector<float> >            trig_prescales;
   TStlSimpleProxy<vector<unsigned int> >     trig_bits;
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
   TStlSimpleProxy<vector<float> >            caloCluster_emB0_time;
   TStlSimpleProxy<vector<float> >            caloCluster_emB1_time;
   TStlSimpleProxy<vector<float> >            caloCluster_emB2_time;
   TStlSimpleProxy<vector<float> >            caloCluster_emB3_time;
   TStlSimpleProxy<vector<float> >            caloCluster_emE0_time;
   TStlSimpleProxy<vector<float> >            caloCluster_emE1_time;
   TStlSimpleProxy<vector<float> >            caloCluster_emE2_time;
   TStlSimpleProxy<vector<float> >            caloCluster_emE3_time;
   TStlSimpleProxy<vector<float> >            caloCluster_emF0_time;
   TStlSimpleProxy<vector<float> >            caloCluster_emF1_time;
   TStlSimpleProxy<vector<float> >            caloCluster_emF2_time;
   TStlSimpleProxy<vector<float> >            caloCluster_hadE0_time;
   TStlSimpleProxy<vector<float> >            caloCluster_hadE1_time;
   TStlSimpleProxy<vector<float> >            caloCluster_hadE2_time;
   TStlSimpleProxy<vector<float> >            caloCluster_hadE3_time;
   TStlSimpleProxy<vector<float> >            caloCluster_hadB0_time;
   TStlSimpleProxy<vector<float> >            caloCluster_hadB1_time;
   TStlSimpleProxy<vector<float> >            caloCluster_hadB2_time;
   TStlSimpleProxy<vector<float> >            caloCluster_hadGap1_time;
   TStlSimpleProxy<vector<float> >            caloCluster_hadGap2_time;
   TStlSimpleProxy<vector<float> >            caloCluster_hadGap3_time;
   TStlSimpleProxy<vector<float> >            caloCluster_hadExB0_time;
   TStlSimpleProxy<vector<float> >            caloCluster_hadExB1_time;
   TStlSimpleProxy<vector<float> >            caloCluster_hadExB2_time;
   TStlPx_vector_double_                      cell_emB0_time;
   TStlPx_vector_double_                      cell_emB1_time;
   TStlPx_vector_double_                      cell_emB2_time;
   TStlPx_vector_double_                      cell_emB3_time;
   TStlPx_vector_double_                      cell_emE0_time;
   TStlPx_vector_double_                      cell_emE1_time;
   TStlPx_vector_double_                      cell_emE2_time;
   TStlPx_vector_double_                      cell_emE3_time;
   TStlPx_vector_double_                      cell_emF0_time;
   TStlPx_vector_double_                      cell_emF1_time;
   TStlPx_vector_double_                      cell_emF2_time;
   TStlPx_vector_double_                      cell_hadE0_time;
   TStlPx_vector_double_                      cell_hadE1_time;
   TStlPx_vector_double_                      cell_hadE2_time;
   TStlPx_vector_double_                      cell_hadE3_time;
   TStlPx_vector_double_                      cell_hadB0_time;
   TStlPx_vector_double_                      cell_hadB1_time;
   TStlPx_vector_double_                      cell_hadB2_time;
   TStlPx_vector_double_                      cell_hadGap1_time;
   TStlPx_vector_double_                      cell_hadGap2_time;
   TStlPx_vector_double_                      cell_hadGap3_time;
   TStlPx_vector_double_                      cell_hadExB0_time;
   TStlPx_vector_double_                      cell_hadExB1_time;
   TStlPx_vector_double_                      cell_hadExB2_time;
   TStlSimpleProxy<vector<int> >              cell_number;
   TStlPx_vector_int_                         cell_sampling;
   TStlPx_vector_double_                      cell_time;
   TStlPx_vector_double_                      cell_energy;
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
   TStlSimpleProxy<vector<float> >            Mu_MuonExtr_p_T;
   TStlSimpleProxy<vector<float> >            Mu_MuonExtr_phi;
   TStlSimpleProxy<vector<float> >            Mu_MuonExtr_eta;
   TStlSimpleProxy<vector<float> >            Mu_MuonExtr_p;
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
   TStlSimpleProxy<vector<double> >           muMSonly_px;
   TStlSimpleProxy<vector<double> >           muMSonly_py;
   TStlSimpleProxy<vector<double> >           muMSonly_pz;
   TStlSimpleProxy<vector<double> >           muMSonly_eta;
   TStlSimpleProxy<vector<double> >           muMSonly_phi;
   TStlSimpleProxy<vector<double> >           muMSonly_e;
   TStlSimpleProxy<vector<double> >           muMSonly_charge;
   TStlSimpleProxy<vector<double> >           muMSonly_chi2;
   TStlSimpleProxy<vector<double> >           muMSonly_ndof;
   TStlSimpleProxy<vector<double> >           MSvx_R;
   TStlSimpleProxy<vector<double> >           MSvx_z;
   TStlSimpleProxy<vector<double> >           MSvx_theta;
   TStlSimpleProxy<vector<double> >           MSvx_phi;
   TStlSimpleProxy<vector<double> >           MSvx_nTrks;
   TStlSimpleProxy<vector<double> >           MSvx_chi2prob;
   TStlPx_vector_double_                      MSvx_trk_px;
   TStlPx_vector_double_                      MSvx_trk_py;
   TStlPx_vector_double_                      MSvx_trk_pz;
   TStlPx_vector_double_                      MSvx_trk_charge;
   TStlSimpleProxy<vector<double> >           MSvx_nMDT;
   TStlSimpleProxy<vector<double> >           MSvx_nIMDT;
   TStlSimpleProxy<vector<double> >           MSvx_nMMDT;
   TStlSimpleProxy<vector<double> >           MSvx_nOMDT;
   TStlSimpleProxy<vector<double> >           MSvx_nRPC;
   TStlSimpleProxy<vector<double> >           MSvx_nTGC;
   TStlSimpleProxy<vector<double> >           HVtrk_x;
   TStlSimpleProxy<vector<double> >           HVtrk_y;
   TStlSimpleProxy<vector<double> >           HVtrk_z;
   TStlSimpleProxy<vector<double> >           HVtrk_pT;
   TStlSimpleProxy<vector<double> >           HVtrk_pz;
   TStlSimpleProxy<vector<double> >           HVtrk_charge;


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
      PassedCalRatio                            (&fDirector,"PassedCalRatio"),
      PassedTracklessJet                        (&fDirector,"PassedTracklessJet"),
      PassedMultiMuon                           (&fDirector,"PassedMultiMuon"),
      PassGammaMuon                             (&fDirector,"PassGammaMuon"),
      PassedCollisionVertex                     (&fDirector,"PassedCollisionVertex"),
      isGood                                    (&fDirector,"isGood"),
      isUgly                                    (&fDirector,"isUgly"),
      isBad                                     (&fDirector,"isBad"),
      Track_pt                                  (&fDirector,"Track_pt"),
      Track_eta                                 (&fDirector,"Track_eta"),
      Track_phi                                 (&fDirector,"Track_phi"),
      Track_E                                   (&fDirector,"Track_E"),
      Track_chi2                                (&fDirector,"Track_chi2"),
      Track_nDoF                                (&fDirector,"Track_nDoF"),
      Track_d0                                  (&fDirector,"Track_d0"),
      Track_z0                                  (&fDirector,"Track_z0"),
      Track_nPixel                              (&fDirector,"Track_nPixel"),
      Track_nSCT                                (&fDirector,"Track_nSCT"),
      Track_nTRT                                (&fDirector,"Track_nTRT"),
      MET_final                                 (&fDirector,"MET_final"),
      MET_final_phi                             (&fDirector,"MET_final_phi"),
      MET_base                                  (&fDirector,"MET_base"),
      MET_base_phi                              (&fDirector,"MET_base_phi"),
      MET_topo                                  (&fDirector,"MET_topo"),
      MET_topo_phi                              (&fDirector,"MET_topo_phi"),
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
      trig_triggers                             (&fDirector,"trig_triggers"),
      trig_prescales                            (&fDirector,"trig_prescales"),
      trig_bits                                 (&fDirector,"trig_bits"),
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
      caloCluster_emB0_time                     (&fDirector,"caloCluster_emB0_time"),
      caloCluster_emB1_time                     (&fDirector,"caloCluster_emB1_time"),
      caloCluster_emB2_time                     (&fDirector,"caloCluster_emB2_time"),
      caloCluster_emB3_time                     (&fDirector,"caloCluster_emB3_time"),
      caloCluster_emE0_time                     (&fDirector,"caloCluster_emE0_time"),
      caloCluster_emE1_time                     (&fDirector,"caloCluster_emE1_time"),
      caloCluster_emE2_time                     (&fDirector,"caloCluster_emE2_time"),
      caloCluster_emE3_time                     (&fDirector,"caloCluster_emE3_time"),
      caloCluster_emF0_time                     (&fDirector,"caloCluster_emF0_time"),
      caloCluster_emF1_time                     (&fDirector,"caloCluster_emF1_time"),
      caloCluster_emF2_time                     (&fDirector,"caloCluster_emF2_time"),
      caloCluster_hadE0_time                    (&fDirector,"caloCluster_hadE0_time"),
      caloCluster_hadE1_time                    (&fDirector,"caloCluster_hadE1_time"),
      caloCluster_hadE2_time                    (&fDirector,"caloCluster_hadE2_time"),
      caloCluster_hadE3_time                    (&fDirector,"caloCluster_hadE3_time"),
      caloCluster_hadB0_time                    (&fDirector,"caloCluster_hadB0_time"),
      caloCluster_hadB1_time                    (&fDirector,"caloCluster_hadB1_time"),
      caloCluster_hadB2_time                    (&fDirector,"caloCluster_hadB2_time"),
      caloCluster_hadGap1_time                  (&fDirector,"caloCluster_hadGap1_time"),
      caloCluster_hadGap2_time                  (&fDirector,"caloCluster_hadGap2_time"),
      caloCluster_hadGap3_time                  (&fDirector,"caloCluster_hadGap3_time"),
      caloCluster_hadExB0_time                  (&fDirector,"caloCluster_hadExB0_time"),
      caloCluster_hadExB1_time                  (&fDirector,"caloCluster_hadExB1_time"),
      caloCluster_hadExB2_time                  (&fDirector,"caloCluster_hadExB2_time"),
      cell_emB0_time                            (&fDirector,"cell_emB0_time"),
      cell_emB1_time                            (&fDirector,"cell_emB1_time"),
      cell_emB2_time                            (&fDirector,"cell_emB2_time"),
      cell_emB3_time                            (&fDirector,"cell_emB3_time"),
      cell_emE0_time                            (&fDirector,"cell_emE0_time"),
      cell_emE1_time                            (&fDirector,"cell_emE1_time"),
      cell_emE2_time                            (&fDirector,"cell_emE2_time"),
      cell_emE3_time                            (&fDirector,"cell_emE3_time"),
      cell_emF0_time                            (&fDirector,"cell_emF0_time"),
      cell_emF1_time                            (&fDirector,"cell_emF1_time"),
      cell_emF2_time                            (&fDirector,"cell_emF2_time"),
      cell_hadE0_time                           (&fDirector,"cell_hadE0_time"),
      cell_hadE1_time                           (&fDirector,"cell_hadE1_time"),
      cell_hadE2_time                           (&fDirector,"cell_hadE2_time"),
      cell_hadE3_time                           (&fDirector,"cell_hadE3_time"),
      cell_hadB0_time                           (&fDirector,"cell_hadB0_time"),
      cell_hadB1_time                           (&fDirector,"cell_hadB1_time"),
      cell_hadB2_time                           (&fDirector,"cell_hadB2_time"),
      cell_hadGap1_time                         (&fDirector,"cell_hadGap1_time"),
      cell_hadGap2_time                         (&fDirector,"cell_hadGap2_time"),
      cell_hadGap3_time                         (&fDirector,"cell_hadGap3_time"),
      cell_hadExB0_time                         (&fDirector,"cell_hadExB0_time"),
      cell_hadExB1_time                         (&fDirector,"cell_hadExB1_time"),
      cell_hadExB2_time                         (&fDirector,"cell_hadExB2_time"),
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
      Mu_MuonExtr_p_T                           (&fDirector,"Mu_MuonExtr_p_T"),
      Mu_MuonExtr_phi                           (&fDirector,"Mu_MuonExtr_phi"),
      Mu_MuonExtr_eta                           (&fDirector,"Mu_MuonExtr_eta"),
      Mu_MuonExtr_p                             (&fDirector,"Mu_MuonExtr_p"),
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
      HVtrk_charge                              (&fDirector,"HVtrk_charge")
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
#pragma link C++ class ntuple_CollectionTree::TStlPx_vector_double_-;
#pragma link C++ class ntuple_CollectionTree::TStlPx_vector_int_-;
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
