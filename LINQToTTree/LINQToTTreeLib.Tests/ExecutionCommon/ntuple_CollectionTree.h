/////////////////////////////////////////////////////////////////////////
//   This class has been automatically generated 
//   (at Wed Jan 04 13:39:42 2012 by ROOT version 5.30/01)
//   from TTree CollectionTree/CollectionTree
//   found on file: \\tango.phys.washington.edu\tev-scratch3\users\btag\mc11\group.perf-flavtag.105016.J7_pythia_jetjet.BTAG_D3PD.PCTF.r2725.0.111203145120\group.perf-flavtag.50938_004742.EXT0._00003.BTAG_D3PD.pool.root
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
   struct TStlPx_vector_short_
   {
      TStlPx_vector_short_(TBranchProxyDirector* director,const char *top,const char *mid=0) :
         ffPrefix(top,mid),
         obj(director, top, mid)
      {};
      TStlPx_vector_short_(TBranchProxyDirector* director, TBranchProxy *parent, const char *membername, const char *top=0, const char *mid=0) :
         ffPrefix(top,mid),
         obj(director, parent, membername)
      {};
      TBranchProxyHelper ffPrefix;
      InjecTBranchProxyInterface();
      const vector<short>& At(UInt_t i) {
         static vector<short> default_val;
         if (!obj.Read()) return default_val;
         vector<short> *temp = & obj.GetPtr()->at(i);
         if (temp) return *temp; else return default_val;
      }
      const vector<short>& operator[](Int_t i) { return At(i); }
      const vector<short>& operator[](UInt_t i) { return At(i); }
      Int_t GetEntries() { return obj.GetPtr()->size(); }
      const vector<vector<short> >* operator->() { return obj.GetPtr(); }
      operator vector<vector<short> >*() { return obj.GetPtr(); }
      TObjProxy<vector<vector<short> > > obj;

   };
   struct TStlPx_vector_unsignedint_
   {
      TStlPx_vector_unsignedint_(TBranchProxyDirector* director,const char *top,const char *mid=0) :
         ffPrefix(top,mid),
         obj(director, top, mid)
      {};
      TStlPx_vector_unsignedint_(TBranchProxyDirector* director, TBranchProxy *parent, const char *membername, const char *top=0, const char *mid=0) :
         ffPrefix(top,mid),
         obj(director, parent, membername)
      {};
      TBranchProxyHelper ffPrefix;
      InjecTBranchProxyInterface();
      const vector<unsigned int>& At(UInt_t i) {
         static vector<unsigned int> default_val;
         if (!obj.Read()) return default_val;
         vector<unsigned int> *temp = & obj.GetPtr()->at(i);
         if (temp) return *temp; else return default_val;
      }
      const vector<unsigned int>& operator[](Int_t i) { return At(i); }
      const vector<unsigned int>& operator[](UInt_t i) { return At(i); }
      Int_t GetEntries() { return obj.GetPtr()->size(); }
      const vector<vector<unsigned int> >* operator->() { return obj.GetPtr(); }
      operator vector<vector<unsigned int> >*() { return obj.GetPtr(); }
      TObjProxy<vector<vector<unsigned int> > > obj;

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
   struct TStlPx_vector_string_
   {
      TStlPx_vector_string_(TBranchProxyDirector* director,const char *top,const char *mid=0) :
         ffPrefix(top,mid),
         obj(director, top, mid)
      {};
      TStlPx_vector_string_(TBranchProxyDirector* director, TBranchProxy *parent, const char *membername, const char *top=0, const char *mid=0) :
         ffPrefix(top,mid),
         obj(director, parent, membername)
      {};
      TBranchProxyHelper ffPrefix;
      InjecTBranchProxyInterface();
      const vector<string>& At(UInt_t i) {
         static vector<string> default_val;
         if (!obj.Read()) return default_val;
         vector<string> *temp = & obj.GetPtr()->at(i);
         if (temp) return *temp; else return default_val;
      }
      const vector<string>& operator[](Int_t i) { return At(i); }
      const vector<string>& operator[](UInt_t i) { return At(i); }
      Int_t GetEntries() { return obj.GetPtr()->size(); }
      const vector<vector<string> >* operator->() { return obj.GetPtr(); }
      operator vector<vector<string> >*() { return obj.GetPtr(); }
      TObjProxy<vector<vector<string> > > obj;

   };

   // Wrapper class for each friend TTree
   struct TFriendPx_btagd3pd : public TFriendProxy {
      TFriendPx_btagd3pd(TBranchProxyDirector *director,TTree *tree,Int_t index) :
         TFriendProxy                              (director,tree,index),
         jet_antikt4topoem_n                       (&fDirector,"jet_antikt4topoem_n"),
         jet_antikt4topoem_E                       (&fDirector,"jet_antikt4topoem_E"),
         jet_antikt4topoem_pt                      (&fDirector,"jet_antikt4topoem_pt"),
         jet_antikt4topoem_m                       (&fDirector,"jet_antikt4topoem_m"),
         jet_antikt4topoem_eta                     (&fDirector,"jet_antikt4topoem_eta"),
         jet_antikt4topoem_phi                     (&fDirector,"jet_antikt4topoem_phi"),
         jet_antikt4topoem_EtaOrigin               (&fDirector,"jet_antikt4topoem_EtaOrigin"),
         jet_antikt4topoem_PhiOrigin               (&fDirector,"jet_antikt4topoem_PhiOrigin"),
         jet_antikt4topoem_MOrigin                 (&fDirector,"jet_antikt4topoem_MOrigin"),
         jet_antikt4topoem_EtaOriginEM             (&fDirector,"jet_antikt4topoem_EtaOriginEM"),
         jet_antikt4topoem_PhiOriginEM             (&fDirector,"jet_antikt4topoem_PhiOriginEM"),
         jet_antikt4topoem_MOriginEM               (&fDirector,"jet_antikt4topoem_MOriginEM"),
         jet_antikt4topoem_WIDTH                   (&fDirector,"jet_antikt4topoem_WIDTH"),
         jet_antikt4topoem_n90                     (&fDirector,"jet_antikt4topoem_n90"),
         jet_antikt4topoem_Timing                  (&fDirector,"jet_antikt4topoem_Timing"),
         jet_antikt4topoem_LArQuality              (&fDirector,"jet_antikt4topoem_LArQuality"),
         jet_antikt4topoem_nTrk                    (&fDirector,"jet_antikt4topoem_nTrk"),
         jet_antikt4topoem_sumPtTrk                (&fDirector,"jet_antikt4topoem_sumPtTrk"),
         jet_antikt4topoem_OriginIndex             (&fDirector,"jet_antikt4topoem_OriginIndex"),
         jet_antikt4topoem_HECQuality              (&fDirector,"jet_antikt4topoem_HECQuality"),
         jet_antikt4topoem_NegativeE               (&fDirector,"jet_antikt4topoem_NegativeE"),
         jet_antikt4topoem_AverageLArQF            (&fDirector,"jet_antikt4topoem_AverageLArQF"),
         jet_antikt4topoem_YFlip12                 (&fDirector,"jet_antikt4topoem_YFlip12"),
         jet_antikt4topoem_YFlip23                 (&fDirector,"jet_antikt4topoem_YFlip23"),
         jet_antikt4topoem_BCH_CORR_CELL           (&fDirector,"jet_antikt4topoem_BCH_CORR_CELL"),
         jet_antikt4topoem_BCH_CORR_DOTX           (&fDirector,"jet_antikt4topoem_BCH_CORR_DOTX"),
         jet_antikt4topoem_BCH_CORR_JET            (&fDirector,"jet_antikt4topoem_BCH_CORR_JET"),
         jet_antikt4topoem_BCH_CORR_JET_FORCELL    (&fDirector,"jet_antikt4topoem_BCH_CORR_JET_FORCELL"),
         jet_antikt4topoem_ENG_BAD_CELLS           (&fDirector,"jet_antikt4topoem_ENG_BAD_CELLS"),
         jet_antikt4topoem_N_BAD_CELLS             (&fDirector,"jet_antikt4topoem_N_BAD_CELLS"),
         jet_antikt4topoem_N_BAD_CELLS_CORR        (&fDirector,"jet_antikt4topoem_N_BAD_CELLS_CORR"),
         jet_antikt4topoem_BAD_CELLS_CORR_E        (&fDirector,"jet_antikt4topoem_BAD_CELLS_CORR_E"),
         jet_antikt4topoem_NumTowers               (&fDirector,"jet_antikt4topoem_NumTowers"),
         jet_antikt4topoem_SamplingMax             (&fDirector,"jet_antikt4topoem_SamplingMax"),
         jet_antikt4topoem_fracSamplingMax         (&fDirector,"jet_antikt4topoem_fracSamplingMax"),
         jet_antikt4topoem_hecf                    (&fDirector,"jet_antikt4topoem_hecf"),
         jet_antikt4topoem_tgap3f                  (&fDirector,"jet_antikt4topoem_tgap3f"),
         jet_antikt4topoem_isUgly                  (&fDirector,"jet_antikt4topoem_isUgly"),
         jet_antikt4topoem_isBadLoose              (&fDirector,"jet_antikt4topoem_isBadLoose"),
         jet_antikt4topoem_isBadMedium             (&fDirector,"jet_antikt4topoem_isBadMedium"),
         jet_antikt4topoem_isBadTight              (&fDirector,"jet_antikt4topoem_isBadTight"),
         jet_antikt4topoem_emfrac                  (&fDirector,"jet_antikt4topoem_emfrac"),
         jet_antikt4topoem_Offset                  (&fDirector,"jet_antikt4topoem_Offset"),
         jet_antikt4topoem_EMJES                   (&fDirector,"jet_antikt4topoem_EMJES"),
         jet_antikt4topoem_EMJES_EtaCorr           (&fDirector,"jet_antikt4topoem_EMJES_EtaCorr"),
         jet_antikt4topoem_EMJESnooffset           (&fDirector,"jet_antikt4topoem_EMJESnooffset"),
         jet_antikt4topoem_GCWJES                  (&fDirector,"jet_antikt4topoem_GCWJES"),
         jet_antikt4topoem_GCWJES_EtaCorr          (&fDirector,"jet_antikt4topoem_GCWJES_EtaCorr"),
         jet_antikt4topoem_CB                      (&fDirector,"jet_antikt4topoem_CB"),
         jet_antikt4topoem_emscale_E               (&fDirector,"jet_antikt4topoem_emscale_E"),
         jet_antikt4topoem_emscale_pt              (&fDirector,"jet_antikt4topoem_emscale_pt"),
         jet_antikt4topoem_emscale_m               (&fDirector,"jet_antikt4topoem_emscale_m"),
         jet_antikt4topoem_emscale_eta             (&fDirector,"jet_antikt4topoem_emscale_eta"),
         jet_antikt4topoem_emscale_phi             (&fDirector,"jet_antikt4topoem_emscale_phi"),
         jet_antikt4topoem_jvtx_x                  (&fDirector,"jet_antikt4topoem_jvtx_x"),
         jet_antikt4topoem_jvtx_y                  (&fDirector,"jet_antikt4topoem_jvtx_y"),
         jet_antikt4topoem_jvtx_z                  (&fDirector,"jet_antikt4topoem_jvtx_z"),
         jet_antikt4topoem_jvtxf                   (&fDirector,"jet_antikt4topoem_jvtxf"),
         jet_antikt4topoem_GSCFactorF              (&fDirector,"jet_antikt4topoem_GSCFactorF"),
         jet_antikt4topoem_WidthFraction           (&fDirector,"jet_antikt4topoem_WidthFraction"),
         jet_antikt4topoem_flavor_weight_Comb      (&fDirector,"jet_antikt4topoem_flavor_weight_Comb"),
         jet_antikt4topoem_flavor_weight_IP2D      (&fDirector,"jet_antikt4topoem_flavor_weight_IP2D"),
         jet_antikt4topoem_flavor_weight_IP3D      (&fDirector,"jet_antikt4topoem_flavor_weight_IP3D"),
         jet_antikt4topoem_flavor_weight_SV0       (&fDirector,"jet_antikt4topoem_flavor_weight_SV0"),
         jet_antikt4topoem_flavor_weight_SV1       (&fDirector,"jet_antikt4topoem_flavor_weight_SV1"),
         jet_antikt4topoem_flavor_weight_SV2       (&fDirector,"jet_antikt4topoem_flavor_weight_SV2"),
         jet_antikt4topoem_flavor_weight_JetProb   (&fDirector,"jet_antikt4topoem_flavor_weight_JetProb"),
         jet_antikt4topoem_flavor_weight_SoftMuonTag(&fDirector,"jet_antikt4topoem_flavor_weight_SoftMuonTag"),
         jet_antikt4topoem_flavor_weight_JetFitterTagNN(&fDirector,"jet_antikt4topoem_flavor_weight_JetFitterTagNN"),
         jet_antikt4topoem_flavor_weight_JetFitterCOMBNN(&fDirector,"jet_antikt4topoem_flavor_weight_JetFitterCOMBNN"),
         jet_antikt4topoem_flavor_weight_GbbNN     (&fDirector,"jet_antikt4topoem_flavor_weight_GbbNN"),
         jet_antikt4topoem_flavor_truth_label      (&fDirector,"jet_antikt4topoem_flavor_truth_label"),
         jet_antikt4topoem_flavor_truth_dRminToB   (&fDirector,"jet_antikt4topoem_flavor_truth_dRminToB"),
         jet_antikt4topoem_flavor_truth_dRminToC   (&fDirector,"jet_antikt4topoem_flavor_truth_dRminToC"),
         jet_antikt4topoem_flavor_truth_dRminToT   (&fDirector,"jet_antikt4topoem_flavor_truth_dRminToT"),
         jet_antikt4topoem_flavor_truth_BHadronpdg (&fDirector,"jet_antikt4topoem_flavor_truth_BHadronpdg"),
         jet_antikt4topoem_flavor_truth_vx_x       (&fDirector,"jet_antikt4topoem_flavor_truth_vx_x"),
         jet_antikt4topoem_flavor_truth_vx_y       (&fDirector,"jet_antikt4topoem_flavor_truth_vx_y"),
         jet_antikt4topoem_flavor_truth_vx_z       (&fDirector,"jet_antikt4topoem_flavor_truth_vx_z"),
         jet_antikt4topoem_flavor_putruth_label    (&fDirector,"jet_antikt4topoem_flavor_putruth_label"),
         jet_antikt4topoem_flavor_putruth_dRminToB (&fDirector,"jet_antikt4topoem_flavor_putruth_dRminToB"),
         jet_antikt4topoem_flavor_putruth_dRminToC (&fDirector,"jet_antikt4topoem_flavor_putruth_dRminToC"),
         jet_antikt4topoem_flavor_putruth_dRminToT (&fDirector,"jet_antikt4topoem_flavor_putruth_dRminToT"),
         jet_antikt4topoem_flavor_putruth_BHadronpdg(&fDirector,"jet_antikt4topoem_flavor_putruth_BHadronpdg"),
         jet_antikt4topoem_flavor_putruth_vx_x     (&fDirector,"jet_antikt4topoem_flavor_putruth_vx_x"),
         jet_antikt4topoem_flavor_putruth_vx_y     (&fDirector,"jet_antikt4topoem_flavor_putruth_vx_y"),
         jet_antikt4topoem_flavor_putruth_vx_z     (&fDirector,"jet_antikt4topoem_flavor_putruth_vx_z"),
         jet_antikt4topoem_flavor_component_assoctrk_n(&fDirector,"jet_antikt4topoem_flavor_component_assoctrk_n"),
         jet_antikt4topoem_flavor_component_assoctrk_index(&fDirector,"jet_antikt4topoem_flavor_component_assoctrk_index"),
         jet_antikt4topoem_flavor_component_assoctrk_signOfIP(&fDirector,"jet_antikt4topoem_flavor_component_assoctrk_signOfIP"),
         jet_antikt4topoem_flavor_component_assoctrk_signOfZIP(&fDirector,"jet_antikt4topoem_flavor_component_assoctrk_signOfZIP"),
         jet_antikt4topoem_flavor_component_assoctrk_ud0wrtPriVtx(&fDirector,"jet_antikt4topoem_flavor_component_assoctrk_ud0wrtPriVtx"),
         jet_antikt4topoem_flavor_component_assoctrk_ud0ErrwrtPriVtx(&fDirector,"jet_antikt4topoem_flavor_component_assoctrk_ud0ErrwrtPriVtx"),
         jet_antikt4topoem_flavor_component_assoctrk_uz0wrtPriVtx(&fDirector,"jet_antikt4topoem_flavor_component_assoctrk_uz0wrtPriVtx"),
         jet_antikt4topoem_flavor_component_assoctrk_uz0ErrwrtPriVtx(&fDirector,"jet_antikt4topoem_flavor_component_assoctrk_uz0ErrwrtPriVtx"),
         jet_antikt4topoem_flavor_component_assocmuon_n(&fDirector,"jet_antikt4topoem_flavor_component_assocmuon_n"),
         jet_antikt4topoem_flavor_component_assocmuon_index(&fDirector,"jet_antikt4topoem_flavor_component_assocmuon_index"),
         jet_antikt4topoem_flavor_component_gentruthlepton_n(&fDirector,"jet_antikt4topoem_flavor_component_gentruthlepton_n"),
         jet_antikt4topoem_flavor_component_gentruthlepton_index(&fDirector,"jet_antikt4topoem_flavor_component_gentruthlepton_index"),
         jet_antikt4topoem_flavor_component_gentruthlepton_origin(&fDirector,"jet_antikt4topoem_flavor_component_gentruthlepton_origin"),
         jet_antikt4topoem_flavor_component_gentruthlepton_slbarcode(&fDirector,"jet_antikt4topoem_flavor_component_gentruthlepton_slbarcode"),
         jet_antikt4topoem_flavor_component_ip2d_pu(&fDirector,"jet_antikt4topoem_flavor_component_ip2d_pu"),
         jet_antikt4topoem_flavor_component_ip2d_pb(&fDirector,"jet_antikt4topoem_flavor_component_ip2d_pb"),
         jet_antikt4topoem_flavor_component_ip2d_isValid(&fDirector,"jet_antikt4topoem_flavor_component_ip2d_isValid"),
         jet_antikt4topoem_flavor_component_ip2d_ntrk(&fDirector,"jet_antikt4topoem_flavor_component_ip2d_ntrk"),
         jet_antikt4topoem_flavor_component_ip3d_pu(&fDirector,"jet_antikt4topoem_flavor_component_ip3d_pu"),
         jet_antikt4topoem_flavor_component_ip3d_pb(&fDirector,"jet_antikt4topoem_flavor_component_ip3d_pb"),
         jet_antikt4topoem_flavor_component_ip3d_isValid(&fDirector,"jet_antikt4topoem_flavor_component_ip3d_isValid"),
         jet_antikt4topoem_flavor_component_ip3d_ntrk(&fDirector,"jet_antikt4topoem_flavor_component_ip3d_ntrk"),
         jet_antikt4topoem_flavor_component_sv1_pu (&fDirector,"jet_antikt4topoem_flavor_component_sv1_pu"),
         jet_antikt4topoem_flavor_component_sv1_pb (&fDirector,"jet_antikt4topoem_flavor_component_sv1_pb"),
         jet_antikt4topoem_flavor_component_sv1_isValid(&fDirector,"jet_antikt4topoem_flavor_component_sv1_isValid"),
         jet_antikt4topoem_flavor_component_sv2_pu (&fDirector,"jet_antikt4topoem_flavor_component_sv2_pu"),
         jet_antikt4topoem_flavor_component_sv2_pb (&fDirector,"jet_antikt4topoem_flavor_component_sv2_pb"),
         jet_antikt4topoem_flavor_component_sv2_isValid(&fDirector,"jet_antikt4topoem_flavor_component_sv2_isValid"),
         jet_antikt4topoem_flavor_component_jfit_pu(&fDirector,"jet_antikt4topoem_flavor_component_jfit_pu"),
         jet_antikt4topoem_flavor_component_jfit_pb(&fDirector,"jet_antikt4topoem_flavor_component_jfit_pb"),
         jet_antikt4topoem_flavor_component_jfit_pc(&fDirector,"jet_antikt4topoem_flavor_component_jfit_pc"),
         jet_antikt4topoem_flavor_component_jfit_isValid(&fDirector,"jet_antikt4topoem_flavor_component_jfit_isValid"),
         jet_antikt4topoem_flavor_component_jfitcomb_pu(&fDirector,"jet_antikt4topoem_flavor_component_jfitcomb_pu"),
         jet_antikt4topoem_flavor_component_jfitcomb_pb(&fDirector,"jet_antikt4topoem_flavor_component_jfitcomb_pb"),
         jet_antikt4topoem_flavor_component_jfitcomb_pc(&fDirector,"jet_antikt4topoem_flavor_component_jfitcomb_pc"),
         jet_antikt4topoem_flavor_component_jfitcomb_isValid(&fDirector,"jet_antikt4topoem_flavor_component_jfitcomb_isValid"),
         jet_antikt4topoem_flavor_component_gbbnn_nMatchingTracks(&fDirector,"jet_antikt4topoem_flavor_component_gbbnn_nMatchingTracks"),
         jet_antikt4topoem_flavor_component_gbbnn_trkJetWidth(&fDirector,"jet_antikt4topoem_flavor_component_gbbnn_trkJetWidth"),
         jet_antikt4topoem_flavor_component_gbbnn_trkJetMaxDeltaR(&fDirector,"jet_antikt4topoem_flavor_component_gbbnn_trkJetMaxDeltaR"),
         jet_antikt4topoem_flavor_component_jfit_nvtx(&fDirector,"jet_antikt4topoem_flavor_component_jfit_nvtx"),
         jet_antikt4topoem_flavor_component_jfit_nvtx1t(&fDirector,"jet_antikt4topoem_flavor_component_jfit_nvtx1t"),
         jet_antikt4topoem_flavor_component_jfit_ntrkAtVx(&fDirector,"jet_antikt4topoem_flavor_component_jfit_ntrkAtVx"),
         jet_antikt4topoem_flavor_component_jfit_efrc(&fDirector,"jet_antikt4topoem_flavor_component_jfit_efrc"),
         jet_antikt4topoem_flavor_component_jfit_mass(&fDirector,"jet_antikt4topoem_flavor_component_jfit_mass"),
         jet_antikt4topoem_flavor_component_jfit_sig3d(&fDirector,"jet_antikt4topoem_flavor_component_jfit_sig3d"),
         jet_antikt4topoem_flavor_component_jfit_deltaPhi(&fDirector,"jet_antikt4topoem_flavor_component_jfit_deltaPhi"),
         jet_antikt4topoem_flavor_component_jfit_deltaEta(&fDirector,"jet_antikt4topoem_flavor_component_jfit_deltaEta"),
         jet_antikt4topoem_flavor_component_ipplus_trk_n(&fDirector,"jet_antikt4topoem_flavor_component_ipplus_trk_n"),
         jet_antikt4topoem_flavor_component_ipplus_trk_index(&fDirector,"jet_antikt4topoem_flavor_component_ipplus_trk_index"),
         jet_antikt4topoem_flavor_component_ipplus_trk_d0val(&fDirector,"jet_antikt4topoem_flavor_component_ipplus_trk_d0val"),
         jet_antikt4topoem_flavor_component_ipplus_trk_d0sig(&fDirector,"jet_antikt4topoem_flavor_component_ipplus_trk_d0sig"),
         jet_antikt4topoem_flavor_component_ipplus_trk_z0val(&fDirector,"jet_antikt4topoem_flavor_component_ipplus_trk_z0val"),
         jet_antikt4topoem_flavor_component_ipplus_trk_z0sig(&fDirector,"jet_antikt4topoem_flavor_component_ipplus_trk_z0sig"),
         jet_antikt4topoem_flavor_component_ipplus_trk_w2D(&fDirector,"jet_antikt4topoem_flavor_component_ipplus_trk_w2D"),
         jet_antikt4topoem_flavor_component_ipplus_trk_w3D(&fDirector,"jet_antikt4topoem_flavor_component_ipplus_trk_w3D"),
         jet_antikt4topoem_flavor_component_ipplus_trk_pJP(&fDirector,"jet_antikt4topoem_flavor_component_ipplus_trk_pJP"),
         jet_antikt4topoem_flavor_component_ipplus_trk_pJPneg(&fDirector,"jet_antikt4topoem_flavor_component_ipplus_trk_pJPneg"),
         jet_antikt4topoem_flavor_component_ipplus_trk_grade(&fDirector,"jet_antikt4topoem_flavor_component_ipplus_trk_grade"),
         jet_antikt4topoem_flavor_component_ipplus_trk_isFromV0(&fDirector,"jet_antikt4topoem_flavor_component_ipplus_trk_isFromV0"),
         jet_antikt4topoem_flavor_component_svp_isValid(&fDirector,"jet_antikt4topoem_flavor_component_svp_isValid"),
         jet_antikt4topoem_flavor_component_svp_ntrkv(&fDirector,"jet_antikt4topoem_flavor_component_svp_ntrkv"),
         jet_antikt4topoem_flavor_component_svp_ntrkj(&fDirector,"jet_antikt4topoem_flavor_component_svp_ntrkj"),
         jet_antikt4topoem_flavor_component_svp_n2t(&fDirector,"jet_antikt4topoem_flavor_component_svp_n2t"),
         jet_antikt4topoem_flavor_component_svp_mass(&fDirector,"jet_antikt4topoem_flavor_component_svp_mass"),
         jet_antikt4topoem_flavor_component_svp_efrc(&fDirector,"jet_antikt4topoem_flavor_component_svp_efrc"),
         jet_antikt4topoem_flavor_component_svp_x  (&fDirector,"jet_antikt4topoem_flavor_component_svp_x"),
         jet_antikt4topoem_flavor_component_svp_y  (&fDirector,"jet_antikt4topoem_flavor_component_svp_y"),
         jet_antikt4topoem_flavor_component_svp_z  (&fDirector,"jet_antikt4topoem_flavor_component_svp_z"),
         jet_antikt4topoem_flavor_component_svp_err_x(&fDirector,"jet_antikt4topoem_flavor_component_svp_err_x"),
         jet_antikt4topoem_flavor_component_svp_err_y(&fDirector,"jet_antikt4topoem_flavor_component_svp_err_y"),
         jet_antikt4topoem_flavor_component_svp_err_z(&fDirector,"jet_antikt4topoem_flavor_component_svp_err_z"),
         jet_antikt4topoem_flavor_component_svp_cov_xy(&fDirector,"jet_antikt4topoem_flavor_component_svp_cov_xy"),
         jet_antikt4topoem_flavor_component_svp_cov_xz(&fDirector,"jet_antikt4topoem_flavor_component_svp_cov_xz"),
         jet_antikt4topoem_flavor_component_svp_cov_yz(&fDirector,"jet_antikt4topoem_flavor_component_svp_cov_yz"),
         jet_antikt4topoem_flavor_component_svp_chi2(&fDirector,"jet_antikt4topoem_flavor_component_svp_chi2"),
         jet_antikt4topoem_flavor_component_svp_ndof(&fDirector,"jet_antikt4topoem_flavor_component_svp_ndof"),
         jet_antikt4topoem_flavor_component_svp_ntrk(&fDirector,"jet_antikt4topoem_flavor_component_svp_ntrk"),
         jet_antikt4topoem_flavor_component_svp_trk_n(&fDirector,"jet_antikt4topoem_flavor_component_svp_trk_n"),
         jet_antikt4topoem_flavor_component_svp_trk_index(&fDirector,"jet_antikt4topoem_flavor_component_svp_trk_index"),
         jet_antikt4topoem_flavor_component_sv0p_isValid(&fDirector,"jet_antikt4topoem_flavor_component_sv0p_isValid"),
         jet_antikt4topoem_flavor_component_sv0p_ntrkv(&fDirector,"jet_antikt4topoem_flavor_component_sv0p_ntrkv"),
         jet_antikt4topoem_flavor_component_sv0p_ntrkj(&fDirector,"jet_antikt4topoem_flavor_component_sv0p_ntrkj"),
         jet_antikt4topoem_flavor_component_sv0p_n2t(&fDirector,"jet_antikt4topoem_flavor_component_sv0p_n2t"),
         jet_antikt4topoem_flavor_component_sv0p_mass(&fDirector,"jet_antikt4topoem_flavor_component_sv0p_mass"),
         jet_antikt4topoem_flavor_component_sv0p_efrc(&fDirector,"jet_antikt4topoem_flavor_component_sv0p_efrc"),
         jet_antikt4topoem_flavor_component_sv0p_x (&fDirector,"jet_antikt4topoem_flavor_component_sv0p_x"),
         jet_antikt4topoem_flavor_component_sv0p_y (&fDirector,"jet_antikt4topoem_flavor_component_sv0p_y"),
         jet_antikt4topoem_flavor_component_sv0p_z (&fDirector,"jet_antikt4topoem_flavor_component_sv0p_z"),
         jet_antikt4topoem_flavor_component_sv0p_err_x(&fDirector,"jet_antikt4topoem_flavor_component_sv0p_err_x"),
         jet_antikt4topoem_flavor_component_sv0p_err_y(&fDirector,"jet_antikt4topoem_flavor_component_sv0p_err_y"),
         jet_antikt4topoem_flavor_component_sv0p_err_z(&fDirector,"jet_antikt4topoem_flavor_component_sv0p_err_z"),
         jet_antikt4topoem_flavor_component_sv0p_cov_xy(&fDirector,"jet_antikt4topoem_flavor_component_sv0p_cov_xy"),
         jet_antikt4topoem_flavor_component_sv0p_cov_xz(&fDirector,"jet_antikt4topoem_flavor_component_sv0p_cov_xz"),
         jet_antikt4topoem_flavor_component_sv0p_cov_yz(&fDirector,"jet_antikt4topoem_flavor_component_sv0p_cov_yz"),
         jet_antikt4topoem_flavor_component_sv0p_chi2(&fDirector,"jet_antikt4topoem_flavor_component_sv0p_chi2"),
         jet_antikt4topoem_flavor_component_sv0p_ndof(&fDirector,"jet_antikt4topoem_flavor_component_sv0p_ndof"),
         jet_antikt4topoem_flavor_component_sv0p_ntrk(&fDirector,"jet_antikt4topoem_flavor_component_sv0p_ntrk"),
         jet_antikt4topoem_flavor_component_sv0p_trk_n(&fDirector,"jet_antikt4topoem_flavor_component_sv0p_trk_n"),
         jet_antikt4topoem_flavor_component_sv0p_trk_index(&fDirector,"jet_antikt4topoem_flavor_component_sv0p_trk_index"),
         jet_antikt4topoem_flavor_component_softmuoninfo_muon_n(&fDirector,"jet_antikt4topoem_flavor_component_softmuoninfo_muon_n"),
         jet_antikt4topoem_flavor_component_softmuoninfo_muon_index(&fDirector,"jet_antikt4topoem_flavor_component_softmuoninfo_muon_index"),
         jet_antikt4topoem_flavor_component_softmuoninfo_muon_w(&fDirector,"jet_antikt4topoem_flavor_component_softmuoninfo_muon_w"),
         jet_antikt4topoem_flavor_component_softmuoninfo_muon_pTRel(&fDirector,"jet_antikt4topoem_flavor_component_softmuoninfo_muon_pTRel"),
         jet_antikt4topoem_flavor_component_msvp_isValid(&fDirector,"jet_antikt4topoem_flavor_component_msvp_isValid"),
         jet_antikt4topoem_flavor_component_msvp_ntrkv(&fDirector,"jet_antikt4topoem_flavor_component_msvp_ntrkv"),
         jet_antikt4topoem_flavor_component_msvp_ntrkj(&fDirector,"jet_antikt4topoem_flavor_component_msvp_ntrkj"),
         jet_antikt4topoem_flavor_component_msvp_n2t(&fDirector,"jet_antikt4topoem_flavor_component_msvp_n2t"),
         jet_antikt4topoem_flavor_component_msvp_nVtx(&fDirector,"jet_antikt4topoem_flavor_component_msvp_nVtx"),
         jet_antikt4topoem_flavor_component_msvp_normWeightedDist(&fDirector,"jet_antikt4topoem_flavor_component_msvp_normWeightedDist"),
         jet_antikt4topoem_flavor_component_msvp_msvinjet_n(&fDirector,"jet_antikt4topoem_flavor_component_msvp_msvinjet_n"),
         jet_antikt4topoem_flavor_component_msvp_msvinjet_index(&fDirector,"jet_antikt4topoem_flavor_component_msvp_msvinjet_index"),
         jet_antikt4topoem_flavor_component_VKalbadtrack_n(&fDirector,"jet_antikt4topoem_flavor_component_VKalbadtrack_n"),
         jet_antikt4topoem_flavor_component_VKalbadtrack_index(&fDirector,"jet_antikt4topoem_flavor_component_VKalbadtrack_index"),
         jet_antikt4topoem_flavor_component_jfitvx_theta(&fDirector,"jet_antikt4topoem_flavor_component_jfitvx_theta"),
         jet_antikt4topoem_flavor_component_jfitvx_phi(&fDirector,"jet_antikt4topoem_flavor_component_jfitvx_phi"),
         jet_antikt4topoem_flavor_component_jfitvx_errtheta(&fDirector,"jet_antikt4topoem_flavor_component_jfitvx_errtheta"),
         jet_antikt4topoem_flavor_component_jfitvx_errphi(&fDirector,"jet_antikt4topoem_flavor_component_jfitvx_errphi"),
         jet_antikt4topoem_flavor_component_jfitvx_chi2(&fDirector,"jet_antikt4topoem_flavor_component_jfitvx_chi2"),
         jet_antikt4topoem_flavor_component_jfitvx_ndof(&fDirector,"jet_antikt4topoem_flavor_component_jfitvx_ndof"),
         jet_antikt4topoem_flavor_component_jfvxonjetaxis_n(&fDirector,"jet_antikt4topoem_flavor_component_jfvxonjetaxis_n"),
         jet_antikt4topoem_flavor_component_jfvxonjetaxis_index(&fDirector,"jet_antikt4topoem_flavor_component_jfvxonjetaxis_index"),
         jet_antikt4topoem_flavor_component_jftwotrackvertex_n(&fDirector,"jet_antikt4topoem_flavor_component_jftwotrackvertex_n"),
         jet_antikt4topoem_flavor_component_jftwotrackvertex_index(&fDirector,"jet_antikt4topoem_flavor_component_jftwotrackvertex_index"),
         jet_antikt4topoem_el_dr                   (&fDirector,"jet_antikt4topoem_el_dr"),
         jet_antikt4topoem_el_matched              (&fDirector,"jet_antikt4topoem_el_matched"),
         jet_antikt4topoem_mu_dr                   (&fDirector,"jet_antikt4topoem_mu_dr"),
         jet_antikt4topoem_mu_matched              (&fDirector,"jet_antikt4topoem_mu_matched"),
         jet_antikt4topoem_L1_dr                   (&fDirector,"jet_antikt4topoem_L1_dr"),
         jet_antikt4topoem_L1_matched              (&fDirector,"jet_antikt4topoem_L1_matched"),
         jet_antikt4topoem_L2_dr                   (&fDirector,"jet_antikt4topoem_L2_dr"),
         jet_antikt4topoem_L2_matched              (&fDirector,"jet_antikt4topoem_L2_matched"),
         jet_antikt4topoem_EF_dr                   (&fDirector,"jet_antikt4topoem_EF_dr"),
         jet_antikt4topoem_EF_matched              (&fDirector,"jet_antikt4topoem_EF_matched"),
         jet_antikt6topoem_n                       (&fDirector,"jet_antikt6topoem_n"),
         jet_antikt6topoem_E                       (&fDirector,"jet_antikt6topoem_E"),
         jet_antikt6topoem_pt                      (&fDirector,"jet_antikt6topoem_pt"),
         jet_antikt6topoem_m                       (&fDirector,"jet_antikt6topoem_m"),
         jet_antikt6topoem_eta                     (&fDirector,"jet_antikt6topoem_eta"),
         jet_antikt6topoem_phi                     (&fDirector,"jet_antikt6topoem_phi"),
         jet_antikt6topoem_EtaOrigin               (&fDirector,"jet_antikt6topoem_EtaOrigin"),
         jet_antikt6topoem_PhiOrigin               (&fDirector,"jet_antikt6topoem_PhiOrigin"),
         jet_antikt6topoem_MOrigin                 (&fDirector,"jet_antikt6topoem_MOrigin"),
         jet_antikt6topoem_EtaOriginEM             (&fDirector,"jet_antikt6topoem_EtaOriginEM"),
         jet_antikt6topoem_PhiOriginEM             (&fDirector,"jet_antikt6topoem_PhiOriginEM"),
         jet_antikt6topoem_MOriginEM               (&fDirector,"jet_antikt6topoem_MOriginEM"),
         jet_antikt6topoem_WIDTH                   (&fDirector,"jet_antikt6topoem_WIDTH"),
         jet_antikt6topoem_n90                     (&fDirector,"jet_antikt6topoem_n90"),
         jet_antikt6topoem_Timing                  (&fDirector,"jet_antikt6topoem_Timing"),
         jet_antikt6topoem_LArQuality              (&fDirector,"jet_antikt6topoem_LArQuality"),
         jet_antikt6topoem_nTrk                    (&fDirector,"jet_antikt6topoem_nTrk"),
         jet_antikt6topoem_sumPtTrk                (&fDirector,"jet_antikt6topoem_sumPtTrk"),
         jet_antikt6topoem_OriginIndex             (&fDirector,"jet_antikt6topoem_OriginIndex"),
         jet_antikt6topoem_HECQuality              (&fDirector,"jet_antikt6topoem_HECQuality"),
         jet_antikt6topoem_NegativeE               (&fDirector,"jet_antikt6topoem_NegativeE"),
         jet_antikt6topoem_AverageLArQF            (&fDirector,"jet_antikt6topoem_AverageLArQF"),
         jet_antikt6topoem_YFlip12                 (&fDirector,"jet_antikt6topoem_YFlip12"),
         jet_antikt6topoem_YFlip23                 (&fDirector,"jet_antikt6topoem_YFlip23"),
         jet_antikt6topoem_BCH_CORR_CELL           (&fDirector,"jet_antikt6topoem_BCH_CORR_CELL"),
         jet_antikt6topoem_BCH_CORR_DOTX           (&fDirector,"jet_antikt6topoem_BCH_CORR_DOTX"),
         jet_antikt6topoem_BCH_CORR_JET            (&fDirector,"jet_antikt6topoem_BCH_CORR_JET"),
         jet_antikt6topoem_BCH_CORR_JET_FORCELL    (&fDirector,"jet_antikt6topoem_BCH_CORR_JET_FORCELL"),
         jet_antikt6topoem_ENG_BAD_CELLS           (&fDirector,"jet_antikt6topoem_ENG_BAD_CELLS"),
         jet_antikt6topoem_N_BAD_CELLS             (&fDirector,"jet_antikt6topoem_N_BAD_CELLS"),
         jet_antikt6topoem_N_BAD_CELLS_CORR        (&fDirector,"jet_antikt6topoem_N_BAD_CELLS_CORR"),
         jet_antikt6topoem_BAD_CELLS_CORR_E        (&fDirector,"jet_antikt6topoem_BAD_CELLS_CORR_E"),
         jet_antikt6topoem_NumTowers               (&fDirector,"jet_antikt6topoem_NumTowers"),
         jet_antikt6topoem_SamplingMax             (&fDirector,"jet_antikt6topoem_SamplingMax"),
         jet_antikt6topoem_fracSamplingMax         (&fDirector,"jet_antikt6topoem_fracSamplingMax"),
         jet_antikt6topoem_hecf                    (&fDirector,"jet_antikt6topoem_hecf"),
         jet_antikt6topoem_tgap3f                  (&fDirector,"jet_antikt6topoem_tgap3f"),
         jet_antikt6topoem_isUgly                  (&fDirector,"jet_antikt6topoem_isUgly"),
         jet_antikt6topoem_isBadLoose              (&fDirector,"jet_antikt6topoem_isBadLoose"),
         jet_antikt6topoem_isBadMedium             (&fDirector,"jet_antikt6topoem_isBadMedium"),
         jet_antikt6topoem_isBadTight              (&fDirector,"jet_antikt6topoem_isBadTight"),
         jet_antikt6topoem_emfrac                  (&fDirector,"jet_antikt6topoem_emfrac"),
         jet_antikt6topoem_Offset                  (&fDirector,"jet_antikt6topoem_Offset"),
         jet_antikt6topoem_EMJES                   (&fDirector,"jet_antikt6topoem_EMJES"),
         jet_antikt6topoem_EMJES_EtaCorr           (&fDirector,"jet_antikt6topoem_EMJES_EtaCorr"),
         jet_antikt6topoem_EMJESnooffset           (&fDirector,"jet_antikt6topoem_EMJESnooffset"),
         jet_antikt6topoem_GCWJES                  (&fDirector,"jet_antikt6topoem_GCWJES"),
         jet_antikt6topoem_GCWJES_EtaCorr          (&fDirector,"jet_antikt6topoem_GCWJES_EtaCorr"),
         jet_antikt6topoem_CB                      (&fDirector,"jet_antikt6topoem_CB"),
         jet_antikt6topoem_emscale_E               (&fDirector,"jet_antikt6topoem_emscale_E"),
         jet_antikt6topoem_emscale_pt              (&fDirector,"jet_antikt6topoem_emscale_pt"),
         jet_antikt6topoem_emscale_m               (&fDirector,"jet_antikt6topoem_emscale_m"),
         jet_antikt6topoem_emscale_eta             (&fDirector,"jet_antikt6topoem_emscale_eta"),
         jet_antikt6topoem_emscale_phi             (&fDirector,"jet_antikt6topoem_emscale_phi"),
         jet_antikt6topoem_jvtx_x                  (&fDirector,"jet_antikt6topoem_jvtx_x"),
         jet_antikt6topoem_jvtx_y                  (&fDirector,"jet_antikt6topoem_jvtx_y"),
         jet_antikt6topoem_jvtx_z                  (&fDirector,"jet_antikt6topoem_jvtx_z"),
         jet_antikt6topoem_jvtxf                   (&fDirector,"jet_antikt6topoem_jvtxf"),
         jet_antikt6topoem_GSCFactorF              (&fDirector,"jet_antikt6topoem_GSCFactorF"),
         jet_antikt6topoem_WidthFraction           (&fDirector,"jet_antikt6topoem_WidthFraction"),
         jet_antikt6topoem_flavor_weight_Comb      (&fDirector,"jet_antikt6topoem_flavor_weight_Comb"),
         jet_antikt6topoem_flavor_weight_IP2D      (&fDirector,"jet_antikt6topoem_flavor_weight_IP2D"),
         jet_antikt6topoem_flavor_weight_IP3D      (&fDirector,"jet_antikt6topoem_flavor_weight_IP3D"),
         jet_antikt6topoem_flavor_weight_SV0       (&fDirector,"jet_antikt6topoem_flavor_weight_SV0"),
         jet_antikt6topoem_flavor_weight_SV1       (&fDirector,"jet_antikt6topoem_flavor_weight_SV1"),
         jet_antikt6topoem_flavor_weight_SV2       (&fDirector,"jet_antikt6topoem_flavor_weight_SV2"),
         jet_antikt6topoem_flavor_weight_JetProb   (&fDirector,"jet_antikt6topoem_flavor_weight_JetProb"),
         jet_antikt6topoem_flavor_weight_SoftMuonTag(&fDirector,"jet_antikt6topoem_flavor_weight_SoftMuonTag"),
         jet_antikt6topoem_flavor_weight_JetFitterTagNN(&fDirector,"jet_antikt6topoem_flavor_weight_JetFitterTagNN"),
         jet_antikt6topoem_flavor_weight_JetFitterCOMBNN(&fDirector,"jet_antikt6topoem_flavor_weight_JetFitterCOMBNN"),
         jet_antikt6topoem_flavor_weight_GbbNN     (&fDirector,"jet_antikt6topoem_flavor_weight_GbbNN"),
         jet_antikt6topoem_flavor_truth_label      (&fDirector,"jet_antikt6topoem_flavor_truth_label"),
         jet_antikt6topoem_flavor_truth_dRminToB   (&fDirector,"jet_antikt6topoem_flavor_truth_dRminToB"),
         jet_antikt6topoem_flavor_truth_dRminToC   (&fDirector,"jet_antikt6topoem_flavor_truth_dRminToC"),
         jet_antikt6topoem_flavor_truth_dRminToT   (&fDirector,"jet_antikt6topoem_flavor_truth_dRminToT"),
         jet_antikt6topoem_flavor_truth_BHadronpdg (&fDirector,"jet_antikt6topoem_flavor_truth_BHadronpdg"),
         jet_antikt6topoem_flavor_truth_vx_x       (&fDirector,"jet_antikt6topoem_flavor_truth_vx_x"),
         jet_antikt6topoem_flavor_truth_vx_y       (&fDirector,"jet_antikt6topoem_flavor_truth_vx_y"),
         jet_antikt6topoem_flavor_truth_vx_z       (&fDirector,"jet_antikt6topoem_flavor_truth_vx_z"),
         jet_antikt6topoem_flavor_putruth_label    (&fDirector,"jet_antikt6topoem_flavor_putruth_label"),
         jet_antikt6topoem_flavor_putruth_dRminToB (&fDirector,"jet_antikt6topoem_flavor_putruth_dRminToB"),
         jet_antikt6topoem_flavor_putruth_dRminToC (&fDirector,"jet_antikt6topoem_flavor_putruth_dRminToC"),
         jet_antikt6topoem_flavor_putruth_dRminToT (&fDirector,"jet_antikt6topoem_flavor_putruth_dRminToT"),
         jet_antikt6topoem_flavor_putruth_BHadronpdg(&fDirector,"jet_antikt6topoem_flavor_putruth_BHadronpdg"),
         jet_antikt6topoem_flavor_putruth_vx_x     (&fDirector,"jet_antikt6topoem_flavor_putruth_vx_x"),
         jet_antikt6topoem_flavor_putruth_vx_y     (&fDirector,"jet_antikt6topoem_flavor_putruth_vx_y"),
         jet_antikt6topoem_flavor_putruth_vx_z     (&fDirector,"jet_antikt6topoem_flavor_putruth_vx_z"),
         jet_antikt6topoem_flavor_component_assoctrk_n(&fDirector,"jet_antikt6topoem_flavor_component_assoctrk_n"),
         jet_antikt6topoem_flavor_component_assoctrk_index(&fDirector,"jet_antikt6topoem_flavor_component_assoctrk_index"),
         jet_antikt6topoem_flavor_component_assoctrk_signOfIP(&fDirector,"jet_antikt6topoem_flavor_component_assoctrk_signOfIP"),
         jet_antikt6topoem_flavor_component_assoctrk_signOfZIP(&fDirector,"jet_antikt6topoem_flavor_component_assoctrk_signOfZIP"),
         jet_antikt6topoem_flavor_component_assoctrk_ud0wrtPriVtx(&fDirector,"jet_antikt6topoem_flavor_component_assoctrk_ud0wrtPriVtx"),
         jet_antikt6topoem_flavor_component_assoctrk_ud0ErrwrtPriVtx(&fDirector,"jet_antikt6topoem_flavor_component_assoctrk_ud0ErrwrtPriVtx"),
         jet_antikt6topoem_flavor_component_assoctrk_uz0wrtPriVtx(&fDirector,"jet_antikt6topoem_flavor_component_assoctrk_uz0wrtPriVtx"),
         jet_antikt6topoem_flavor_component_assoctrk_uz0ErrwrtPriVtx(&fDirector,"jet_antikt6topoem_flavor_component_assoctrk_uz0ErrwrtPriVtx"),
         jet_antikt6topoem_flavor_component_assocmuon_n(&fDirector,"jet_antikt6topoem_flavor_component_assocmuon_n"),
         jet_antikt6topoem_flavor_component_assocmuon_index(&fDirector,"jet_antikt6topoem_flavor_component_assocmuon_index"),
         jet_antikt6topoem_flavor_component_gentruthlepton_n(&fDirector,"jet_antikt6topoem_flavor_component_gentruthlepton_n"),
         jet_antikt6topoem_flavor_component_gentruthlepton_index(&fDirector,"jet_antikt6topoem_flavor_component_gentruthlepton_index"),
         jet_antikt6topoem_flavor_component_gentruthlepton_origin(&fDirector,"jet_antikt6topoem_flavor_component_gentruthlepton_origin"),
         jet_antikt6topoem_flavor_component_gentruthlepton_slbarcode(&fDirector,"jet_antikt6topoem_flavor_component_gentruthlepton_slbarcode"),
         jet_antikt6topoem_flavor_component_ip2d_pu(&fDirector,"jet_antikt6topoem_flavor_component_ip2d_pu"),
         jet_antikt6topoem_flavor_component_ip2d_pb(&fDirector,"jet_antikt6topoem_flavor_component_ip2d_pb"),
         jet_antikt6topoem_flavor_component_ip2d_isValid(&fDirector,"jet_antikt6topoem_flavor_component_ip2d_isValid"),
         jet_antikt6topoem_flavor_component_ip2d_ntrk(&fDirector,"jet_antikt6topoem_flavor_component_ip2d_ntrk"),
         jet_antikt6topoem_flavor_component_ip3d_pu(&fDirector,"jet_antikt6topoem_flavor_component_ip3d_pu"),
         jet_antikt6topoem_flavor_component_ip3d_pb(&fDirector,"jet_antikt6topoem_flavor_component_ip3d_pb"),
         jet_antikt6topoem_flavor_component_ip3d_isValid(&fDirector,"jet_antikt6topoem_flavor_component_ip3d_isValid"),
         jet_antikt6topoem_flavor_component_ip3d_ntrk(&fDirector,"jet_antikt6topoem_flavor_component_ip3d_ntrk"),
         jet_antikt6topoem_flavor_component_sv1_pu (&fDirector,"jet_antikt6topoem_flavor_component_sv1_pu"),
         jet_antikt6topoem_flavor_component_sv1_pb (&fDirector,"jet_antikt6topoem_flavor_component_sv1_pb"),
         jet_antikt6topoem_flavor_component_sv1_isValid(&fDirector,"jet_antikt6topoem_flavor_component_sv1_isValid"),
         jet_antikt6topoem_flavor_component_sv2_pu (&fDirector,"jet_antikt6topoem_flavor_component_sv2_pu"),
         jet_antikt6topoem_flavor_component_sv2_pb (&fDirector,"jet_antikt6topoem_flavor_component_sv2_pb"),
         jet_antikt6topoem_flavor_component_sv2_isValid(&fDirector,"jet_antikt6topoem_flavor_component_sv2_isValid"),
         jet_antikt6topoem_flavor_component_jfit_pu(&fDirector,"jet_antikt6topoem_flavor_component_jfit_pu"),
         jet_antikt6topoem_flavor_component_jfit_pb(&fDirector,"jet_antikt6topoem_flavor_component_jfit_pb"),
         jet_antikt6topoem_flavor_component_jfit_pc(&fDirector,"jet_antikt6topoem_flavor_component_jfit_pc"),
         jet_antikt6topoem_flavor_component_jfit_isValid(&fDirector,"jet_antikt6topoem_flavor_component_jfit_isValid"),
         jet_antikt6topoem_flavor_component_jfitcomb_pu(&fDirector,"jet_antikt6topoem_flavor_component_jfitcomb_pu"),
         jet_antikt6topoem_flavor_component_jfitcomb_pb(&fDirector,"jet_antikt6topoem_flavor_component_jfitcomb_pb"),
         jet_antikt6topoem_flavor_component_jfitcomb_pc(&fDirector,"jet_antikt6topoem_flavor_component_jfitcomb_pc"),
         jet_antikt6topoem_flavor_component_jfitcomb_isValid(&fDirector,"jet_antikt6topoem_flavor_component_jfitcomb_isValid"),
         jet_antikt6topoem_flavor_component_gbbnn_nMatchingTracks(&fDirector,"jet_antikt6topoem_flavor_component_gbbnn_nMatchingTracks"),
         jet_antikt6topoem_flavor_component_gbbnn_trkJetWidth(&fDirector,"jet_antikt6topoem_flavor_component_gbbnn_trkJetWidth"),
         jet_antikt6topoem_flavor_component_gbbnn_trkJetMaxDeltaR(&fDirector,"jet_antikt6topoem_flavor_component_gbbnn_trkJetMaxDeltaR"),
         jet_antikt6topoem_flavor_component_jfit_nvtx(&fDirector,"jet_antikt6topoem_flavor_component_jfit_nvtx"),
         jet_antikt6topoem_flavor_component_jfit_nvtx1t(&fDirector,"jet_antikt6topoem_flavor_component_jfit_nvtx1t"),
         jet_antikt6topoem_flavor_component_jfit_ntrkAtVx(&fDirector,"jet_antikt6topoem_flavor_component_jfit_ntrkAtVx"),
         jet_antikt6topoem_flavor_component_jfit_efrc(&fDirector,"jet_antikt6topoem_flavor_component_jfit_efrc"),
         jet_antikt6topoem_flavor_component_jfit_mass(&fDirector,"jet_antikt6topoem_flavor_component_jfit_mass"),
         jet_antikt6topoem_flavor_component_jfit_sig3d(&fDirector,"jet_antikt6topoem_flavor_component_jfit_sig3d"),
         jet_antikt6topoem_flavor_component_jfit_deltaPhi(&fDirector,"jet_antikt6topoem_flavor_component_jfit_deltaPhi"),
         jet_antikt6topoem_flavor_component_jfit_deltaEta(&fDirector,"jet_antikt6topoem_flavor_component_jfit_deltaEta"),
         jet_antikt6topoem_flavor_component_ipplus_trk_n(&fDirector,"jet_antikt6topoem_flavor_component_ipplus_trk_n"),
         jet_antikt6topoem_flavor_component_ipplus_trk_index(&fDirector,"jet_antikt6topoem_flavor_component_ipplus_trk_index"),
         jet_antikt6topoem_flavor_component_ipplus_trk_d0val(&fDirector,"jet_antikt6topoem_flavor_component_ipplus_trk_d0val"),
         jet_antikt6topoem_flavor_component_ipplus_trk_d0sig(&fDirector,"jet_antikt6topoem_flavor_component_ipplus_trk_d0sig"),
         jet_antikt6topoem_flavor_component_ipplus_trk_z0val(&fDirector,"jet_antikt6topoem_flavor_component_ipplus_trk_z0val"),
         jet_antikt6topoem_flavor_component_ipplus_trk_z0sig(&fDirector,"jet_antikt6topoem_flavor_component_ipplus_trk_z0sig"),
         jet_antikt6topoem_flavor_component_ipplus_trk_w2D(&fDirector,"jet_antikt6topoem_flavor_component_ipplus_trk_w2D"),
         jet_antikt6topoem_flavor_component_ipplus_trk_w3D(&fDirector,"jet_antikt6topoem_flavor_component_ipplus_trk_w3D"),
         jet_antikt6topoem_flavor_component_ipplus_trk_pJP(&fDirector,"jet_antikt6topoem_flavor_component_ipplus_trk_pJP"),
         jet_antikt6topoem_flavor_component_ipplus_trk_pJPneg(&fDirector,"jet_antikt6topoem_flavor_component_ipplus_trk_pJPneg"),
         jet_antikt6topoem_flavor_component_ipplus_trk_grade(&fDirector,"jet_antikt6topoem_flavor_component_ipplus_trk_grade"),
         jet_antikt6topoem_flavor_component_ipplus_trk_isFromV0(&fDirector,"jet_antikt6topoem_flavor_component_ipplus_trk_isFromV0"),
         jet_antikt6topoem_flavor_component_svp_isValid(&fDirector,"jet_antikt6topoem_flavor_component_svp_isValid"),
         jet_antikt6topoem_flavor_component_svp_ntrkv(&fDirector,"jet_antikt6topoem_flavor_component_svp_ntrkv"),
         jet_antikt6topoem_flavor_component_svp_ntrkj(&fDirector,"jet_antikt6topoem_flavor_component_svp_ntrkj"),
         jet_antikt6topoem_flavor_component_svp_n2t(&fDirector,"jet_antikt6topoem_flavor_component_svp_n2t"),
         jet_antikt6topoem_flavor_component_svp_mass(&fDirector,"jet_antikt6topoem_flavor_component_svp_mass"),
         jet_antikt6topoem_flavor_component_svp_efrc(&fDirector,"jet_antikt6topoem_flavor_component_svp_efrc"),
         jet_antikt6topoem_flavor_component_svp_x  (&fDirector,"jet_antikt6topoem_flavor_component_svp_x"),
         jet_antikt6topoem_flavor_component_svp_y  (&fDirector,"jet_antikt6topoem_flavor_component_svp_y"),
         jet_antikt6topoem_flavor_component_svp_z  (&fDirector,"jet_antikt6topoem_flavor_component_svp_z"),
         jet_antikt6topoem_flavor_component_svp_err_x(&fDirector,"jet_antikt6topoem_flavor_component_svp_err_x"),
         jet_antikt6topoem_flavor_component_svp_err_y(&fDirector,"jet_antikt6topoem_flavor_component_svp_err_y"),
         jet_antikt6topoem_flavor_component_svp_err_z(&fDirector,"jet_antikt6topoem_flavor_component_svp_err_z"),
         jet_antikt6topoem_flavor_component_svp_cov_xy(&fDirector,"jet_antikt6topoem_flavor_component_svp_cov_xy"),
         jet_antikt6topoem_flavor_component_svp_cov_xz(&fDirector,"jet_antikt6topoem_flavor_component_svp_cov_xz"),
         jet_antikt6topoem_flavor_component_svp_cov_yz(&fDirector,"jet_antikt6topoem_flavor_component_svp_cov_yz"),
         jet_antikt6topoem_flavor_component_svp_chi2(&fDirector,"jet_antikt6topoem_flavor_component_svp_chi2"),
         jet_antikt6topoem_flavor_component_svp_ndof(&fDirector,"jet_antikt6topoem_flavor_component_svp_ndof"),
         jet_antikt6topoem_flavor_component_svp_ntrk(&fDirector,"jet_antikt6topoem_flavor_component_svp_ntrk"),
         jet_antikt6topoem_flavor_component_svp_trk_n(&fDirector,"jet_antikt6topoem_flavor_component_svp_trk_n"),
         jet_antikt6topoem_flavor_component_svp_trk_index(&fDirector,"jet_antikt6topoem_flavor_component_svp_trk_index"),
         jet_antikt6topoem_flavor_component_sv0p_isValid(&fDirector,"jet_antikt6topoem_flavor_component_sv0p_isValid"),
         jet_antikt6topoem_flavor_component_sv0p_ntrkv(&fDirector,"jet_antikt6topoem_flavor_component_sv0p_ntrkv"),
         jet_antikt6topoem_flavor_component_sv0p_ntrkj(&fDirector,"jet_antikt6topoem_flavor_component_sv0p_ntrkj"),
         jet_antikt6topoem_flavor_component_sv0p_n2t(&fDirector,"jet_antikt6topoem_flavor_component_sv0p_n2t"),
         jet_antikt6topoem_flavor_component_sv0p_mass(&fDirector,"jet_antikt6topoem_flavor_component_sv0p_mass"),
         jet_antikt6topoem_flavor_component_sv0p_efrc(&fDirector,"jet_antikt6topoem_flavor_component_sv0p_efrc"),
         jet_antikt6topoem_flavor_component_sv0p_x (&fDirector,"jet_antikt6topoem_flavor_component_sv0p_x"),
         jet_antikt6topoem_flavor_component_sv0p_y (&fDirector,"jet_antikt6topoem_flavor_component_sv0p_y"),
         jet_antikt6topoem_flavor_component_sv0p_z (&fDirector,"jet_antikt6topoem_flavor_component_sv0p_z"),
         jet_antikt6topoem_flavor_component_sv0p_err_x(&fDirector,"jet_antikt6topoem_flavor_component_sv0p_err_x"),
         jet_antikt6topoem_flavor_component_sv0p_err_y(&fDirector,"jet_antikt6topoem_flavor_component_sv0p_err_y"),
         jet_antikt6topoem_flavor_component_sv0p_err_z(&fDirector,"jet_antikt6topoem_flavor_component_sv0p_err_z"),
         jet_antikt6topoem_flavor_component_sv0p_cov_xy(&fDirector,"jet_antikt6topoem_flavor_component_sv0p_cov_xy"),
         jet_antikt6topoem_flavor_component_sv0p_cov_xz(&fDirector,"jet_antikt6topoem_flavor_component_sv0p_cov_xz"),
         jet_antikt6topoem_flavor_component_sv0p_cov_yz(&fDirector,"jet_antikt6topoem_flavor_component_sv0p_cov_yz"),
         jet_antikt6topoem_flavor_component_sv0p_chi2(&fDirector,"jet_antikt6topoem_flavor_component_sv0p_chi2"),
         jet_antikt6topoem_flavor_component_sv0p_ndof(&fDirector,"jet_antikt6topoem_flavor_component_sv0p_ndof"),
         jet_antikt6topoem_flavor_component_sv0p_ntrk(&fDirector,"jet_antikt6topoem_flavor_component_sv0p_ntrk"),
         jet_antikt6topoem_flavor_component_sv0p_trk_n(&fDirector,"jet_antikt6topoem_flavor_component_sv0p_trk_n"),
         jet_antikt6topoem_flavor_component_sv0p_trk_index(&fDirector,"jet_antikt6topoem_flavor_component_sv0p_trk_index"),
         jet_antikt6topoem_flavor_component_softmuoninfo_muon_n(&fDirector,"jet_antikt6topoem_flavor_component_softmuoninfo_muon_n"),
         jet_antikt6topoem_flavor_component_softmuoninfo_muon_index(&fDirector,"jet_antikt6topoem_flavor_component_softmuoninfo_muon_index"),
         jet_antikt6topoem_flavor_component_softmuoninfo_muon_w(&fDirector,"jet_antikt6topoem_flavor_component_softmuoninfo_muon_w"),
         jet_antikt6topoem_flavor_component_softmuoninfo_muon_pTRel(&fDirector,"jet_antikt6topoem_flavor_component_softmuoninfo_muon_pTRel"),
         jet_antikt6topoem_flavor_component_msvp_isValid(&fDirector,"jet_antikt6topoem_flavor_component_msvp_isValid"),
         jet_antikt6topoem_flavor_component_msvp_ntrkv(&fDirector,"jet_antikt6topoem_flavor_component_msvp_ntrkv"),
         jet_antikt6topoem_flavor_component_msvp_ntrkj(&fDirector,"jet_antikt6topoem_flavor_component_msvp_ntrkj"),
         jet_antikt6topoem_flavor_component_msvp_n2t(&fDirector,"jet_antikt6topoem_flavor_component_msvp_n2t"),
         jet_antikt6topoem_flavor_component_msvp_nVtx(&fDirector,"jet_antikt6topoem_flavor_component_msvp_nVtx"),
         jet_antikt6topoem_flavor_component_msvp_normWeightedDist(&fDirector,"jet_antikt6topoem_flavor_component_msvp_normWeightedDist"),
         jet_antikt6topoem_flavor_component_msvp_msvinjet_n(&fDirector,"jet_antikt6topoem_flavor_component_msvp_msvinjet_n"),
         jet_antikt6topoem_flavor_component_msvp_msvinjet_index(&fDirector,"jet_antikt6topoem_flavor_component_msvp_msvinjet_index"),
         jet_antikt6topoem_flavor_component_VKalbadtrack_n(&fDirector,"jet_antikt6topoem_flavor_component_VKalbadtrack_n"),
         jet_antikt6topoem_flavor_component_VKalbadtrack_index(&fDirector,"jet_antikt6topoem_flavor_component_VKalbadtrack_index"),
         jet_antikt6topoem_flavor_component_jfitvx_theta(&fDirector,"jet_antikt6topoem_flavor_component_jfitvx_theta"),
         jet_antikt6topoem_flavor_component_jfitvx_phi(&fDirector,"jet_antikt6topoem_flavor_component_jfitvx_phi"),
         jet_antikt6topoem_flavor_component_jfitvx_errtheta(&fDirector,"jet_antikt6topoem_flavor_component_jfitvx_errtheta"),
         jet_antikt6topoem_flavor_component_jfitvx_errphi(&fDirector,"jet_antikt6topoem_flavor_component_jfitvx_errphi"),
         jet_antikt6topoem_flavor_component_jfitvx_chi2(&fDirector,"jet_antikt6topoem_flavor_component_jfitvx_chi2"),
         jet_antikt6topoem_flavor_component_jfitvx_ndof(&fDirector,"jet_antikt6topoem_flavor_component_jfitvx_ndof"),
         jet_antikt6topoem_flavor_component_jfvxonjetaxis_n(&fDirector,"jet_antikt6topoem_flavor_component_jfvxonjetaxis_n"),
         jet_antikt6topoem_flavor_component_jfvxonjetaxis_index(&fDirector,"jet_antikt6topoem_flavor_component_jfvxonjetaxis_index"),
         jet_antikt6topoem_flavor_component_jftwotrackvertex_n(&fDirector,"jet_antikt6topoem_flavor_component_jftwotrackvertex_n"),
         jet_antikt6topoem_flavor_component_jftwotrackvertex_index(&fDirector,"jet_antikt6topoem_flavor_component_jftwotrackvertex_index"),
         jet_antikt6topoem_el_dr                   (&fDirector,"jet_antikt6topoem_el_dr"),
         jet_antikt6topoem_el_matched              (&fDirector,"jet_antikt6topoem_el_matched"),
         jet_antikt6topoem_mu_dr                   (&fDirector,"jet_antikt6topoem_mu_dr"),
         jet_antikt6topoem_mu_matched              (&fDirector,"jet_antikt6topoem_mu_matched"),
         jet_antikt6topoem_L1_dr                   (&fDirector,"jet_antikt6topoem_L1_dr"),
         jet_antikt6topoem_L1_matched              (&fDirector,"jet_antikt6topoem_L1_matched"),
         jet_antikt6topoem_L2_dr                   (&fDirector,"jet_antikt6topoem_L2_dr"),
         jet_antikt6topoem_L2_matched              (&fDirector,"jet_antikt6topoem_L2_matched"),
         jet_antikt6topoem_EF_dr                   (&fDirector,"jet_antikt6topoem_EF_dr"),
         jet_antikt6topoem_EF_matched              (&fDirector,"jet_antikt6topoem_EF_matched"),
         jet_antikt4truth_n                        (&fDirector,"jet_antikt4truth_n"),
         jet_antikt4truth_E                        (&fDirector,"jet_antikt4truth_E"),
         jet_antikt4truth_pt                       (&fDirector,"jet_antikt4truth_pt"),
         jet_antikt4truth_m                        (&fDirector,"jet_antikt4truth_m"),
         jet_antikt4truth_eta                      (&fDirector,"jet_antikt4truth_eta"),
         jet_antikt4truth_phi                      (&fDirector,"jet_antikt4truth_phi"),
         jet_antikt4truth_flavor_weight_Comb       (&fDirector,"jet_antikt4truth_flavor_weight_Comb"),
         jet_antikt4truth_flavor_weight_IP2D       (&fDirector,"jet_antikt4truth_flavor_weight_IP2D"),
         jet_antikt4truth_flavor_weight_IP3D       (&fDirector,"jet_antikt4truth_flavor_weight_IP3D"),
         jet_antikt4truth_flavor_weight_SV0        (&fDirector,"jet_antikt4truth_flavor_weight_SV0"),
         jet_antikt4truth_flavor_weight_SV1        (&fDirector,"jet_antikt4truth_flavor_weight_SV1"),
         jet_antikt4truth_flavor_weight_SV2        (&fDirector,"jet_antikt4truth_flavor_weight_SV2"),
         jet_antikt4truth_flavor_weight_JetProb    (&fDirector,"jet_antikt4truth_flavor_weight_JetProb"),
         jet_antikt4truth_flavor_weight_SoftMuonTag(&fDirector,"jet_antikt4truth_flavor_weight_SoftMuonTag"),
         jet_antikt4truth_flavor_weight_JetFitterTagNN(&fDirector,"jet_antikt4truth_flavor_weight_JetFitterTagNN"),
         jet_antikt4truth_flavor_weight_JetFitterCOMBNN(&fDirector,"jet_antikt4truth_flavor_weight_JetFitterCOMBNN"),
         jet_antikt4truth_flavor_weight_GbbNN      (&fDirector,"jet_antikt4truth_flavor_weight_GbbNN"),
         jet_antikt4truth_flavor_truth_label       (&fDirector,"jet_antikt4truth_flavor_truth_label"),
         jet_antikt4truth_flavor_truth_dRminToB    (&fDirector,"jet_antikt4truth_flavor_truth_dRminToB"),
         jet_antikt4truth_flavor_truth_dRminToC    (&fDirector,"jet_antikt4truth_flavor_truth_dRminToC"),
         jet_antikt4truth_flavor_truth_dRminToT    (&fDirector,"jet_antikt4truth_flavor_truth_dRminToT"),
         jet_antikt4truth_flavor_truth_BHadronpdg  (&fDirector,"jet_antikt4truth_flavor_truth_BHadronpdg"),
         jet_antikt4truth_flavor_truth_vx_x        (&fDirector,"jet_antikt4truth_flavor_truth_vx_x"),
         jet_antikt4truth_flavor_truth_vx_y        (&fDirector,"jet_antikt4truth_flavor_truth_vx_y"),
         jet_antikt4truth_flavor_truth_vx_z        (&fDirector,"jet_antikt4truth_flavor_truth_vx_z"),
         jet_antikt4truth_flavor_putruth_label     (&fDirector,"jet_antikt4truth_flavor_putruth_label"),
         jet_antikt4truth_flavor_putruth_dRminToB  (&fDirector,"jet_antikt4truth_flavor_putruth_dRminToB"),
         jet_antikt4truth_flavor_putruth_dRminToC  (&fDirector,"jet_antikt4truth_flavor_putruth_dRminToC"),
         jet_antikt4truth_flavor_putruth_dRminToT  (&fDirector,"jet_antikt4truth_flavor_putruth_dRminToT"),
         jet_antikt4truth_flavor_putruth_BHadronpdg(&fDirector,"jet_antikt4truth_flavor_putruth_BHadronpdg"),
         jet_antikt4truth_flavor_putruth_vx_x      (&fDirector,"jet_antikt4truth_flavor_putruth_vx_x"),
         jet_antikt4truth_flavor_putruth_vx_y      (&fDirector,"jet_antikt4truth_flavor_putruth_vx_y"),
         jet_antikt4truth_flavor_putruth_vx_z      (&fDirector,"jet_antikt4truth_flavor_putruth_vx_z"),
         jet_antikt4truth_flavor_component_assoctrk_n(&fDirector,"jet_antikt4truth_flavor_component_assoctrk_n"),
         jet_antikt4truth_flavor_component_assoctrk_index(&fDirector,"jet_antikt4truth_flavor_component_assoctrk_index"),
         jet_antikt4truth_flavor_component_assoctrk_signOfIP(&fDirector,"jet_antikt4truth_flavor_component_assoctrk_signOfIP"),
         jet_antikt4truth_flavor_component_assoctrk_signOfZIP(&fDirector,"jet_antikt4truth_flavor_component_assoctrk_signOfZIP"),
         jet_antikt4truth_flavor_component_assoctrk_ud0wrtPriVtx(&fDirector,"jet_antikt4truth_flavor_component_assoctrk_ud0wrtPriVtx"),
         jet_antikt4truth_flavor_component_assoctrk_ud0ErrwrtPriVtx(&fDirector,"jet_antikt4truth_flavor_component_assoctrk_ud0ErrwrtPriVtx"),
         jet_antikt4truth_flavor_component_assoctrk_uz0wrtPriVtx(&fDirector,"jet_antikt4truth_flavor_component_assoctrk_uz0wrtPriVtx"),
         jet_antikt4truth_flavor_component_assoctrk_uz0ErrwrtPriVtx(&fDirector,"jet_antikt4truth_flavor_component_assoctrk_uz0ErrwrtPriVtx"),
         jet_antikt4truth_flavor_component_assocmuon_n(&fDirector,"jet_antikt4truth_flavor_component_assocmuon_n"),
         jet_antikt4truth_flavor_component_assocmuon_index(&fDirector,"jet_antikt4truth_flavor_component_assocmuon_index"),
         jet_antikt4truth_flavor_component_gentruthlepton_n(&fDirector,"jet_antikt4truth_flavor_component_gentruthlepton_n"),
         jet_antikt4truth_flavor_component_gentruthlepton_index(&fDirector,"jet_antikt4truth_flavor_component_gentruthlepton_index"),
         jet_antikt4truth_flavor_component_gentruthlepton_origin(&fDirector,"jet_antikt4truth_flavor_component_gentruthlepton_origin"),
         jet_antikt4truth_flavor_component_gentruthlepton_slbarcode(&fDirector,"jet_antikt4truth_flavor_component_gentruthlepton_slbarcode"),
         jet_antikt4truth_flavor_component_ip2d_pu (&fDirector,"jet_antikt4truth_flavor_component_ip2d_pu"),
         jet_antikt4truth_flavor_component_ip2d_pb (&fDirector,"jet_antikt4truth_flavor_component_ip2d_pb"),
         jet_antikt4truth_flavor_component_ip2d_isValid(&fDirector,"jet_antikt4truth_flavor_component_ip2d_isValid"),
         jet_antikt4truth_flavor_component_ip2d_ntrk(&fDirector,"jet_antikt4truth_flavor_component_ip2d_ntrk"),
         jet_antikt4truth_flavor_component_ip3d_pu (&fDirector,"jet_antikt4truth_flavor_component_ip3d_pu"),
         jet_antikt4truth_flavor_component_ip3d_pb (&fDirector,"jet_antikt4truth_flavor_component_ip3d_pb"),
         jet_antikt4truth_flavor_component_ip3d_isValid(&fDirector,"jet_antikt4truth_flavor_component_ip3d_isValid"),
         jet_antikt4truth_flavor_component_ip3d_ntrk(&fDirector,"jet_antikt4truth_flavor_component_ip3d_ntrk"),
         jet_antikt4truth_flavor_component_sv1_pu  (&fDirector,"jet_antikt4truth_flavor_component_sv1_pu"),
         jet_antikt4truth_flavor_component_sv1_pb  (&fDirector,"jet_antikt4truth_flavor_component_sv1_pb"),
         jet_antikt4truth_flavor_component_sv1_isValid(&fDirector,"jet_antikt4truth_flavor_component_sv1_isValid"),
         jet_antikt4truth_flavor_component_sv2_pu  (&fDirector,"jet_antikt4truth_flavor_component_sv2_pu"),
         jet_antikt4truth_flavor_component_sv2_pb  (&fDirector,"jet_antikt4truth_flavor_component_sv2_pb"),
         jet_antikt4truth_flavor_component_sv2_isValid(&fDirector,"jet_antikt4truth_flavor_component_sv2_isValid"),
         jet_antikt4truth_flavor_component_jfit_pu (&fDirector,"jet_antikt4truth_flavor_component_jfit_pu"),
         jet_antikt4truth_flavor_component_jfit_pb (&fDirector,"jet_antikt4truth_flavor_component_jfit_pb"),
         jet_antikt4truth_flavor_component_jfit_pc (&fDirector,"jet_antikt4truth_flavor_component_jfit_pc"),
         jet_antikt4truth_flavor_component_jfit_isValid(&fDirector,"jet_antikt4truth_flavor_component_jfit_isValid"),
         jet_antikt4truth_flavor_component_jfitcomb_pu(&fDirector,"jet_antikt4truth_flavor_component_jfitcomb_pu"),
         jet_antikt4truth_flavor_component_jfitcomb_pb(&fDirector,"jet_antikt4truth_flavor_component_jfitcomb_pb"),
         jet_antikt4truth_flavor_component_jfitcomb_pc(&fDirector,"jet_antikt4truth_flavor_component_jfitcomb_pc"),
         jet_antikt4truth_flavor_component_jfitcomb_isValid(&fDirector,"jet_antikt4truth_flavor_component_jfitcomb_isValid"),
         jet_antikt4truth_flavor_component_gbbnn_nMatchingTracks(&fDirector,"jet_antikt4truth_flavor_component_gbbnn_nMatchingTracks"),
         jet_antikt4truth_flavor_component_gbbnn_trkJetWidth(&fDirector,"jet_antikt4truth_flavor_component_gbbnn_trkJetWidth"),
         jet_antikt4truth_flavor_component_gbbnn_trkJetMaxDeltaR(&fDirector,"jet_antikt4truth_flavor_component_gbbnn_trkJetMaxDeltaR"),
         jet_antikt4truth_flavor_component_jfit_nvtx(&fDirector,"jet_antikt4truth_flavor_component_jfit_nvtx"),
         jet_antikt4truth_flavor_component_jfit_nvtx1t(&fDirector,"jet_antikt4truth_flavor_component_jfit_nvtx1t"),
         jet_antikt4truth_flavor_component_jfit_ntrkAtVx(&fDirector,"jet_antikt4truth_flavor_component_jfit_ntrkAtVx"),
         jet_antikt4truth_flavor_component_jfit_efrc(&fDirector,"jet_antikt4truth_flavor_component_jfit_efrc"),
         jet_antikt4truth_flavor_component_jfit_mass(&fDirector,"jet_antikt4truth_flavor_component_jfit_mass"),
         jet_antikt4truth_flavor_component_jfit_sig3d(&fDirector,"jet_antikt4truth_flavor_component_jfit_sig3d"),
         jet_antikt4truth_flavor_component_jfit_deltaPhi(&fDirector,"jet_antikt4truth_flavor_component_jfit_deltaPhi"),
         jet_antikt4truth_flavor_component_jfit_deltaEta(&fDirector,"jet_antikt4truth_flavor_component_jfit_deltaEta"),
         jet_antikt4truth_flavor_component_ipplus_trk_n(&fDirector,"jet_antikt4truth_flavor_component_ipplus_trk_n"),
         jet_antikt4truth_flavor_component_ipplus_trk_index(&fDirector,"jet_antikt4truth_flavor_component_ipplus_trk_index"),
         jet_antikt4truth_flavor_component_ipplus_trk_d0val(&fDirector,"jet_antikt4truth_flavor_component_ipplus_trk_d0val"),
         jet_antikt4truth_flavor_component_ipplus_trk_d0sig(&fDirector,"jet_antikt4truth_flavor_component_ipplus_trk_d0sig"),
         jet_antikt4truth_flavor_component_ipplus_trk_z0val(&fDirector,"jet_antikt4truth_flavor_component_ipplus_trk_z0val"),
         jet_antikt4truth_flavor_component_ipplus_trk_z0sig(&fDirector,"jet_antikt4truth_flavor_component_ipplus_trk_z0sig"),
         jet_antikt4truth_flavor_component_ipplus_trk_w2D(&fDirector,"jet_antikt4truth_flavor_component_ipplus_trk_w2D"),
         jet_antikt4truth_flavor_component_ipplus_trk_w3D(&fDirector,"jet_antikt4truth_flavor_component_ipplus_trk_w3D"),
         jet_antikt4truth_flavor_component_ipplus_trk_pJP(&fDirector,"jet_antikt4truth_flavor_component_ipplus_trk_pJP"),
         jet_antikt4truth_flavor_component_ipplus_trk_pJPneg(&fDirector,"jet_antikt4truth_flavor_component_ipplus_trk_pJPneg"),
         jet_antikt4truth_flavor_component_ipplus_trk_grade(&fDirector,"jet_antikt4truth_flavor_component_ipplus_trk_grade"),
         jet_antikt4truth_flavor_component_ipplus_trk_isFromV0(&fDirector,"jet_antikt4truth_flavor_component_ipplus_trk_isFromV0"),
         jet_antikt4truth_flavor_component_svp_isValid(&fDirector,"jet_antikt4truth_flavor_component_svp_isValid"),
         jet_antikt4truth_flavor_component_svp_ntrkv(&fDirector,"jet_antikt4truth_flavor_component_svp_ntrkv"),
         jet_antikt4truth_flavor_component_svp_ntrkj(&fDirector,"jet_antikt4truth_flavor_component_svp_ntrkj"),
         jet_antikt4truth_flavor_component_svp_n2t (&fDirector,"jet_antikt4truth_flavor_component_svp_n2t"),
         jet_antikt4truth_flavor_component_svp_mass(&fDirector,"jet_antikt4truth_flavor_component_svp_mass"),
         jet_antikt4truth_flavor_component_svp_efrc(&fDirector,"jet_antikt4truth_flavor_component_svp_efrc"),
         jet_antikt4truth_flavor_component_svp_x   (&fDirector,"jet_antikt4truth_flavor_component_svp_x"),
         jet_antikt4truth_flavor_component_svp_y   (&fDirector,"jet_antikt4truth_flavor_component_svp_y"),
         jet_antikt4truth_flavor_component_svp_z   (&fDirector,"jet_antikt4truth_flavor_component_svp_z"),
         jet_antikt4truth_flavor_component_svp_err_x(&fDirector,"jet_antikt4truth_flavor_component_svp_err_x"),
         jet_antikt4truth_flavor_component_svp_err_y(&fDirector,"jet_antikt4truth_flavor_component_svp_err_y"),
         jet_antikt4truth_flavor_component_svp_err_z(&fDirector,"jet_antikt4truth_flavor_component_svp_err_z"),
         jet_antikt4truth_flavor_component_svp_cov_xy(&fDirector,"jet_antikt4truth_flavor_component_svp_cov_xy"),
         jet_antikt4truth_flavor_component_svp_cov_xz(&fDirector,"jet_antikt4truth_flavor_component_svp_cov_xz"),
         jet_antikt4truth_flavor_component_svp_cov_yz(&fDirector,"jet_antikt4truth_flavor_component_svp_cov_yz"),
         jet_antikt4truth_flavor_component_svp_chi2(&fDirector,"jet_antikt4truth_flavor_component_svp_chi2"),
         jet_antikt4truth_flavor_component_svp_ndof(&fDirector,"jet_antikt4truth_flavor_component_svp_ndof"),
         jet_antikt4truth_flavor_component_svp_ntrk(&fDirector,"jet_antikt4truth_flavor_component_svp_ntrk"),
         jet_antikt4truth_flavor_component_svp_trk_n(&fDirector,"jet_antikt4truth_flavor_component_svp_trk_n"),
         jet_antikt4truth_flavor_component_svp_trk_index(&fDirector,"jet_antikt4truth_flavor_component_svp_trk_index"),
         jet_antikt4truth_flavor_component_sv0p_isValid(&fDirector,"jet_antikt4truth_flavor_component_sv0p_isValid"),
         jet_antikt4truth_flavor_component_sv0p_ntrkv(&fDirector,"jet_antikt4truth_flavor_component_sv0p_ntrkv"),
         jet_antikt4truth_flavor_component_sv0p_ntrkj(&fDirector,"jet_antikt4truth_flavor_component_sv0p_ntrkj"),
         jet_antikt4truth_flavor_component_sv0p_n2t(&fDirector,"jet_antikt4truth_flavor_component_sv0p_n2t"),
         jet_antikt4truth_flavor_component_sv0p_mass(&fDirector,"jet_antikt4truth_flavor_component_sv0p_mass"),
         jet_antikt4truth_flavor_component_sv0p_efrc(&fDirector,"jet_antikt4truth_flavor_component_sv0p_efrc"),
         jet_antikt4truth_flavor_component_sv0p_x  (&fDirector,"jet_antikt4truth_flavor_component_sv0p_x"),
         jet_antikt4truth_flavor_component_sv0p_y  (&fDirector,"jet_antikt4truth_flavor_component_sv0p_y"),
         jet_antikt4truth_flavor_component_sv0p_z  (&fDirector,"jet_antikt4truth_flavor_component_sv0p_z"),
         jet_antikt4truth_flavor_component_sv0p_err_x(&fDirector,"jet_antikt4truth_flavor_component_sv0p_err_x"),
         jet_antikt4truth_flavor_component_sv0p_err_y(&fDirector,"jet_antikt4truth_flavor_component_sv0p_err_y"),
         jet_antikt4truth_flavor_component_sv0p_err_z(&fDirector,"jet_antikt4truth_flavor_component_sv0p_err_z"),
         jet_antikt4truth_flavor_component_sv0p_cov_xy(&fDirector,"jet_antikt4truth_flavor_component_sv0p_cov_xy"),
         jet_antikt4truth_flavor_component_sv0p_cov_xz(&fDirector,"jet_antikt4truth_flavor_component_sv0p_cov_xz"),
         jet_antikt4truth_flavor_component_sv0p_cov_yz(&fDirector,"jet_antikt4truth_flavor_component_sv0p_cov_yz"),
         jet_antikt4truth_flavor_component_sv0p_chi2(&fDirector,"jet_antikt4truth_flavor_component_sv0p_chi2"),
         jet_antikt4truth_flavor_component_sv0p_ndof(&fDirector,"jet_antikt4truth_flavor_component_sv0p_ndof"),
         jet_antikt4truth_flavor_component_sv0p_ntrk(&fDirector,"jet_antikt4truth_flavor_component_sv0p_ntrk"),
         jet_antikt4truth_flavor_component_sv0p_trk_n(&fDirector,"jet_antikt4truth_flavor_component_sv0p_trk_n"),
         jet_antikt4truth_flavor_component_sv0p_trk_index(&fDirector,"jet_antikt4truth_flavor_component_sv0p_trk_index"),
         jet_antikt4truth_flavor_component_softmuoninfo_muon_n(&fDirector,"jet_antikt4truth_flavor_component_softmuoninfo_muon_n"),
         jet_antikt4truth_flavor_component_softmuoninfo_muon_index(&fDirector,"jet_antikt4truth_flavor_component_softmuoninfo_muon_index"),
         jet_antikt4truth_flavor_component_softmuoninfo_muon_w(&fDirector,"jet_antikt4truth_flavor_component_softmuoninfo_muon_w"),
         jet_antikt4truth_flavor_component_softmuoninfo_muon_pTRel(&fDirector,"jet_antikt4truth_flavor_component_softmuoninfo_muon_pTRel"),
         jet_antikt4truth_flavor_component_msvp_isValid(&fDirector,"jet_antikt4truth_flavor_component_msvp_isValid"),
         jet_antikt4truth_flavor_component_msvp_ntrkv(&fDirector,"jet_antikt4truth_flavor_component_msvp_ntrkv"),
         jet_antikt4truth_flavor_component_msvp_ntrkj(&fDirector,"jet_antikt4truth_flavor_component_msvp_ntrkj"),
         jet_antikt4truth_flavor_component_msvp_n2t(&fDirector,"jet_antikt4truth_flavor_component_msvp_n2t"),
         jet_antikt4truth_flavor_component_msvp_nVtx(&fDirector,"jet_antikt4truth_flavor_component_msvp_nVtx"),
         jet_antikt4truth_flavor_component_msvp_normWeightedDist(&fDirector,"jet_antikt4truth_flavor_component_msvp_normWeightedDist"),
         jet_antikt4truth_flavor_component_msvp_msvinjet_n(&fDirector,"jet_antikt4truth_flavor_component_msvp_msvinjet_n"),
         jet_antikt4truth_flavor_component_msvp_msvinjet_index(&fDirector,"jet_antikt4truth_flavor_component_msvp_msvinjet_index"),
         jet_antikt4truth_flavor_component_VKalbadtrack_n(&fDirector,"jet_antikt4truth_flavor_component_VKalbadtrack_n"),
         jet_antikt4truth_flavor_component_VKalbadtrack_index(&fDirector,"jet_antikt4truth_flavor_component_VKalbadtrack_index"),
         jet_antikt4truth_flavor_component_jfitvx_theta(&fDirector,"jet_antikt4truth_flavor_component_jfitvx_theta"),
         jet_antikt4truth_flavor_component_jfitvx_phi(&fDirector,"jet_antikt4truth_flavor_component_jfitvx_phi"),
         jet_antikt4truth_flavor_component_jfitvx_errtheta(&fDirector,"jet_antikt4truth_flavor_component_jfitvx_errtheta"),
         jet_antikt4truth_flavor_component_jfitvx_errphi(&fDirector,"jet_antikt4truth_flavor_component_jfitvx_errphi"),
         jet_antikt4truth_flavor_component_jfitvx_chi2(&fDirector,"jet_antikt4truth_flavor_component_jfitvx_chi2"),
         jet_antikt4truth_flavor_component_jfitvx_ndof(&fDirector,"jet_antikt4truth_flavor_component_jfitvx_ndof"),
         jet_antikt4truth_flavor_component_jfvxonjetaxis_n(&fDirector,"jet_antikt4truth_flavor_component_jfvxonjetaxis_n"),
         jet_antikt4truth_flavor_component_jfvxonjetaxis_index(&fDirector,"jet_antikt4truth_flavor_component_jfvxonjetaxis_index"),
         jet_antikt4truth_flavor_component_jftwotrackvertex_n(&fDirector,"jet_antikt4truth_flavor_component_jftwotrackvertex_n"),
         jet_antikt4truth_flavor_component_jftwotrackvertex_index(&fDirector,"jet_antikt4truth_flavor_component_jftwotrackvertex_index"),
         jet_antikt4truth_el_dr                    (&fDirector,"jet_antikt4truth_el_dr"),
         jet_antikt4truth_el_matched               (&fDirector,"jet_antikt4truth_el_matched"),
         jet_antikt4truth_mu_dr                    (&fDirector,"jet_antikt4truth_mu_dr"),
         jet_antikt4truth_mu_matched               (&fDirector,"jet_antikt4truth_mu_matched"),
         jet_antikt4truth_L1_dr                    (&fDirector,"jet_antikt4truth_L1_dr"),
         jet_antikt4truth_L1_matched               (&fDirector,"jet_antikt4truth_L1_matched"),
         jet_antikt4truth_L2_dr                    (&fDirector,"jet_antikt4truth_L2_dr"),
         jet_antikt4truth_L2_matched               (&fDirector,"jet_antikt4truth_L2_matched"),
         jet_antikt4truth_EF_dr                    (&fDirector,"jet_antikt4truth_EF_dr"),
         jet_antikt4truth_EF_matched               (&fDirector,"jet_antikt4truth_EF_matched"),
         muoninjet_n                               (&fDirector,"muoninjet_n"),
         muoninjet_E                               (&fDirector,"muoninjet_E"),
         muoninjet_pt                              (&fDirector,"muoninjet_pt"),
         muoninjet_m                               (&fDirector,"muoninjet_m"),
         muoninjet_eta                             (&fDirector,"muoninjet_eta"),
         muoninjet_phi                             (&fDirector,"muoninjet_phi"),
         muoninjet_px                              (&fDirector,"muoninjet_px"),
         muoninjet_py                              (&fDirector,"muoninjet_py"),
         muoninjet_pz                              (&fDirector,"muoninjet_pz"),
         muoninjet_author                          (&fDirector,"muoninjet_author"),
         muoninjet_matchchi2                       (&fDirector,"muoninjet_matchchi2"),
         muoninjet_matchndof                       (&fDirector,"muoninjet_matchndof"),
         muoninjet_etcone20                        (&fDirector,"muoninjet_etcone20"),
         muoninjet_etcone30                        (&fDirector,"muoninjet_etcone30"),
         muoninjet_etcone40                        (&fDirector,"muoninjet_etcone40"),
         muoninjet_energyLossPar                   (&fDirector,"muoninjet_energyLossPar"),
         muoninjet_energyLossErr                   (&fDirector,"muoninjet_energyLossErr"),
         muoninjet_etCore                          (&fDirector,"muoninjet_etCore"),
         muoninjet_energyLossType                  (&fDirector,"muoninjet_energyLossType"),
         muoninjet_caloMuonIdTag                   (&fDirector,"muoninjet_caloMuonIdTag"),
         muoninjet_caloLRLikelihood                (&fDirector,"muoninjet_caloLRLikelihood"),
         muoninjet_nOutliersOnTrack                (&fDirector,"muoninjet_nOutliersOnTrack"),
         muoninjet_nMDTHits                        (&fDirector,"muoninjet_nMDTHits"),
         muoninjet_nMDTHoles                       (&fDirector,"muoninjet_nMDTHoles"),
         muoninjet_nCSCEtaHits                     (&fDirector,"muoninjet_nCSCEtaHits"),
         muoninjet_nCSCEtaHoles                    (&fDirector,"muoninjet_nCSCEtaHoles"),
         muoninjet_nCSCPhiHits                     (&fDirector,"muoninjet_nCSCPhiHits"),
         muoninjet_nCSCPhiHoles                    (&fDirector,"muoninjet_nCSCPhiHoles"),
         muoninjet_nRPCEtaHits                     (&fDirector,"muoninjet_nRPCEtaHits"),
         muoninjet_nRPCEtaHoles                    (&fDirector,"muoninjet_nRPCEtaHoles"),
         muoninjet_nRPCPhiHits                     (&fDirector,"muoninjet_nRPCPhiHits"),
         muoninjet_nRPCPhiHoles                    (&fDirector,"muoninjet_nRPCPhiHoles"),
         muoninjet_nTGCEtaHits                     (&fDirector,"muoninjet_nTGCEtaHits"),
         muoninjet_nTGCEtaHoles                    (&fDirector,"muoninjet_nTGCEtaHoles"),
         muoninjet_nTGCPhiHits                     (&fDirector,"muoninjet_nTGCPhiHits"),
         muoninjet_nTGCPhiHoles                    (&fDirector,"muoninjet_nTGCPhiHoles"),
         muoninjet_primtrk_chi2                    (&fDirector,"muoninjet_primtrk_chi2"),
         muoninjet_primtrk_ndof                    (&fDirector,"muoninjet_primtrk_ndof"),
         muoninjet_primtrk_hastrack                (&fDirector,"muoninjet_primtrk_hastrack"),
         muoninjet_trk_index                       (&fDirector,"muoninjet_trk_index"),
         jfvxonjetaxis_n                           (&fDirector,"jfvxonjetaxis_n"),
         jfvxonjetaxis_vtxPos                      (&fDirector,"jfvxonjetaxis_vtxPos"),
         jfvxonjetaxis_vtxErr                      (&fDirector,"jfvxonjetaxis_vtxErr"),
         jfvxonjetaxis_trk_n                       (&fDirector,"jfvxonjetaxis_trk_n"),
         jfvxonjetaxis_trk_phiAtVx                 (&fDirector,"jfvxonjetaxis_trk_phiAtVx"),
         jfvxonjetaxis_trk_thetaAtVx               (&fDirector,"jfvxonjetaxis_trk_thetaAtVx"),
         jfvxonjetaxis_trk_ptAtVx                  (&fDirector,"jfvxonjetaxis_trk_ptAtVx"),
         jfvxonjetaxis_trk_index                   (&fDirector,"jfvxonjetaxis_trk_index"),
         jftwotrkvertex_n                          (&fDirector,"jftwotrkvertex_n"),
         jftwotrkvertex_isNeutral                  (&fDirector,"jftwotrkvertex_isNeutral"),
         jftwotrkvertex_chi2                       (&fDirector,"jftwotrkvertex_chi2"),
         jftwotrkvertex_ndof                       (&fDirector,"jftwotrkvertex_ndof"),
         jftwotrkvertex_x                          (&fDirector,"jftwotrkvertex_x"),
         jftwotrkvertex_y                          (&fDirector,"jftwotrkvertex_y"),
         jftwotrkvertex_z                          (&fDirector,"jftwotrkvertex_z"),
         jftwotrkvertex_errx                       (&fDirector,"jftwotrkvertex_errx"),
         jftwotrkvertex_erry                       (&fDirector,"jftwotrkvertex_erry"),
         jftwotrkvertex_errz                       (&fDirector,"jftwotrkvertex_errz"),
         jftwotrkvertex_mass                       (&fDirector,"jftwotrkvertex_mass"),
         jftwotrkvertex_trk_n                      (&fDirector,"jftwotrkvertex_trk_n"),
         jftwotrkvertex_trk_index                  (&fDirector,"jftwotrkvertex_trk_index"),
         msvinjet_n                                (&fDirector,"msvinjet_n"),
         msvinjet_mass                             (&fDirector,"msvinjet_mass"),
         msvinjet_pt                               (&fDirector,"msvinjet_pt"),
         msvinjet_eta                              (&fDirector,"msvinjet_eta"),
         msvinjet_phi                              (&fDirector,"msvinjet_phi"),
         msvinjet_efrc                             (&fDirector,"msvinjet_efrc"),
         msvinjet_x                                (&fDirector,"msvinjet_x"),
         msvinjet_y                                (&fDirector,"msvinjet_y"),
         msvinjet_z                                (&fDirector,"msvinjet_z"),
         msvinjet_err_x                            (&fDirector,"msvinjet_err_x"),
         msvinjet_err_y                            (&fDirector,"msvinjet_err_y"),
         msvinjet_err_z                            (&fDirector,"msvinjet_err_z"),
         msvinjet_cov_xy                           (&fDirector,"msvinjet_cov_xy"),
         msvinjet_cov_xz                           (&fDirector,"msvinjet_cov_xz"),
         msvinjet_cov_yz                           (&fDirector,"msvinjet_cov_yz"),
         msvinjet_chi2                             (&fDirector,"msvinjet_chi2"),
         msvinjet_ndof                             (&fDirector,"msvinjet_ndof"),
         msvinjet_ntrk                             (&fDirector,"msvinjet_ntrk"),
         msvinjet_normDist                         (&fDirector,"msvinjet_normDist"),
         msvinjet_trk_n                            (&fDirector,"msvinjet_trk_n"),
         msvinjet_trk_index                        (&fDirector,"msvinjet_trk_index"),
         RunNumber                                 (&fDirector,"RunNumber"),
         EventNumber                               (&fDirector,"EventNumber"),
         timestamp                                 (&fDirector,"timestamp"),
         timestamp_ns                              (&fDirector,"timestamp_ns"),
         lbn                                       (&fDirector,"lbn"),
         bcid                                      (&fDirector,"bcid"),
         detmask0                                  (&fDirector,"detmask0"),
         detmask1                                  (&fDirector,"detmask1"),
         actualIntPerXing                          (&fDirector,"actualIntPerXing"),
         averageIntPerXing                         (&fDirector,"averageIntPerXing"),
         mc_channel_number                         (&fDirector,"mc_channel_number"),
         mc_event_number                           (&fDirector,"mc_event_number"),
         mc_event_weight                           (&fDirector,"mc_event_weight"),
         pixelFlags                                (&fDirector,"pixelFlags"),
         sctFlags                                  (&fDirector,"sctFlags"),
         trtFlags                                  (&fDirector,"trtFlags"),
         larFlags                                  (&fDirector,"larFlags"),
         tileFlags                                 (&fDirector,"tileFlags"),
         muonFlags                                 (&fDirector,"muonFlags"),
         fwdFlags                                  (&fDirector,"fwdFlags"),
         coreFlags                                 (&fDirector,"coreFlags"),
         pixelError                                (&fDirector,"pixelError"),
         sctError                                  (&fDirector,"sctError"),
         trtError                                  (&fDirector,"trtError"),
         larError                                  (&fDirector,"larError"),
         tileError                                 (&fDirector,"tileError"),
         muonError                                 (&fDirector,"muonError"),
         fwdError                                  (&fDirector,"fwdError"),
         coreError                                 (&fDirector,"coreError"),
         pileupinfo_n                              (&fDirector,"pileupinfo_n"),
         pileupinfo_time                           (&fDirector,"pileupinfo_time"),
         pileupinfo_index                          (&fDirector,"pileupinfo_index"),
         pileupinfo_type                           (&fDirector,"pileupinfo_type"),
         pileupinfo_runNumber                      (&fDirector,"pileupinfo_runNumber"),
         pileupinfo_EventNumber                    (&fDirector,"pileupinfo_EventNumber"),
         trk_n                                     (&fDirector,"trk_n"),
         trk_d0                                    (&fDirector,"trk_d0"),
         trk_z0                                    (&fDirector,"trk_z0"),
         trk_phi                                   (&fDirector,"trk_phi"),
         trk_theta                                 (&fDirector,"trk_theta"),
         trk_qoverp                                (&fDirector,"trk_qoverp"),
         trk_pt                                    (&fDirector,"trk_pt"),
         trk_eta                                   (&fDirector,"trk_eta"),
         trk_err_d0                                (&fDirector,"trk_err_d0"),
         trk_err_z0                                (&fDirector,"trk_err_z0"),
         trk_err_phi                               (&fDirector,"trk_err_phi"),
         trk_err_theta                             (&fDirector,"trk_err_theta"),
         trk_err_qoverp                            (&fDirector,"trk_err_qoverp"),
         trk_cov_d0_z0                             (&fDirector,"trk_cov_d0_z0"),
         trk_cov_d0_phi                            (&fDirector,"trk_cov_d0_phi"),
         trk_cov_d0_theta                          (&fDirector,"trk_cov_d0_theta"),
         trk_cov_d0_qoverp                         (&fDirector,"trk_cov_d0_qoverp"),
         trk_cov_z0_phi                            (&fDirector,"trk_cov_z0_phi"),
         trk_cov_z0_theta                          (&fDirector,"trk_cov_z0_theta"),
         trk_cov_z0_qoverp                         (&fDirector,"trk_cov_z0_qoverp"),
         trk_cov_phi_theta                         (&fDirector,"trk_cov_phi_theta"),
         trk_cov_phi_qoverp                        (&fDirector,"trk_cov_phi_qoverp"),
         trk_cov_theta_qoverp                      (&fDirector,"trk_cov_theta_qoverp"),
         trk_IPEstimate_d0_biased_wrtPV            (&fDirector,"trk_IPEstimate_d0_biased_wrtPV"),
         trk_IPEstimate_z0_biased_wrtPV            (&fDirector,"trk_IPEstimate_z0_biased_wrtPV"),
         trk_IPEstimate_d0_unbiased_wrtPV          (&fDirector,"trk_IPEstimate_d0_unbiased_wrtPV"),
         trk_IPEstimate_z0_unbiased_wrtPV          (&fDirector,"trk_IPEstimate_z0_unbiased_wrtPV"),
         trk_IPEstimate_err_d0_biased_wrtPV        (&fDirector,"trk_IPEstimate_err_d0_biased_wrtPV"),
         trk_IPEstimate_err_z0_biased_wrtPV        (&fDirector,"trk_IPEstimate_err_z0_biased_wrtPV"),
         trk_IPEstimate_err_d0_unbiased_wrtPV      (&fDirector,"trk_IPEstimate_err_d0_unbiased_wrtPV"),
         trk_IPEstimate_err_z0_unbiased_wrtPV      (&fDirector,"trk_IPEstimate_err_z0_unbiased_wrtPV"),
         trk_IPEstimate_errPV_d0_biased_wrtPV      (&fDirector,"trk_IPEstimate_errPV_d0_biased_wrtPV"),
         trk_IPEstimate_errPV_z0_biased_wrtPV      (&fDirector,"trk_IPEstimate_errPV_z0_biased_wrtPV"),
         trk_IPEstimate_errPV_d0_unbiased_wrtPV    (&fDirector,"trk_IPEstimate_errPV_d0_unbiased_wrtPV"),
         trk_IPEstimate_errPV_z0_unbiased_wrtPV    (&fDirector,"trk_IPEstimate_errPV_z0_unbiased_wrtPV"),
         trk_d0_wrtPV                              (&fDirector,"trk_d0_wrtPV"),
         trk_z0_wrtPV                              (&fDirector,"trk_z0_wrtPV"),
         trk_phi_wrtPV                             (&fDirector,"trk_phi_wrtPV"),
         trk_err_d0_wrtPV                          (&fDirector,"trk_err_d0_wrtPV"),
         trk_err_z0_wrtPV                          (&fDirector,"trk_err_z0_wrtPV"),
         trk_err_phi_wrtPV                         (&fDirector,"trk_err_phi_wrtPV"),
         trk_err_theta_wrtPV                       (&fDirector,"trk_err_theta_wrtPV"),
         trk_err_qoverp_wrtPV                      (&fDirector,"trk_err_qoverp_wrtPV"),
         trk_cov_d0_z0_wrtPV                       (&fDirector,"trk_cov_d0_z0_wrtPV"),
         trk_cov_d0_phi_wrtPV                      (&fDirector,"trk_cov_d0_phi_wrtPV"),
         trk_cov_d0_theta_wrtPV                    (&fDirector,"trk_cov_d0_theta_wrtPV"),
         trk_cov_d0_qoverp_wrtPV                   (&fDirector,"trk_cov_d0_qoverp_wrtPV"),
         trk_cov_z0_phi_wrtPV                      (&fDirector,"trk_cov_z0_phi_wrtPV"),
         trk_cov_z0_theta_wrtPV                    (&fDirector,"trk_cov_z0_theta_wrtPV"),
         trk_cov_z0_qoverp_wrtPV                   (&fDirector,"trk_cov_z0_qoverp_wrtPV"),
         trk_cov_phi_theta_wrtPV                   (&fDirector,"trk_cov_phi_theta_wrtPV"),
         trk_cov_phi_qoverp_wrtPV                  (&fDirector,"trk_cov_phi_qoverp_wrtPV"),
         trk_cov_theta_qoverp_wrtPV                (&fDirector,"trk_cov_theta_qoverp_wrtPV"),
         trk_d0_wrtBS                              (&fDirector,"trk_d0_wrtBS"),
         trk_z0_wrtBS                              (&fDirector,"trk_z0_wrtBS"),
         trk_phi_wrtBS                             (&fDirector,"trk_phi_wrtBS"),
         trk_err_d0_wrtBS                          (&fDirector,"trk_err_d0_wrtBS"),
         trk_err_z0_wrtBS                          (&fDirector,"trk_err_z0_wrtBS"),
         trk_err_phi_wrtBS                         (&fDirector,"trk_err_phi_wrtBS"),
         trk_err_theta_wrtBS                       (&fDirector,"trk_err_theta_wrtBS"),
         trk_err_qoverp_wrtBS                      (&fDirector,"trk_err_qoverp_wrtBS"),
         trk_chi2                                  (&fDirector,"trk_chi2"),
         trk_ndof                                  (&fDirector,"trk_ndof"),
         trk_nBLHits                               (&fDirector,"trk_nBLHits"),
         trk_nPixHits                              (&fDirector,"trk_nPixHits"),
         trk_nSCTHits                              (&fDirector,"trk_nSCTHits"),
         trk_nTRTHits                              (&fDirector,"trk_nTRTHits"),
         trk_nTRTHighTHits                         (&fDirector,"trk_nTRTHighTHits"),
         trk_nPixHoles                             (&fDirector,"trk_nPixHoles"),
         trk_nSCTHoles                             (&fDirector,"trk_nSCTHoles"),
         trk_nTRTHoles                             (&fDirector,"trk_nTRTHoles"),
         trk_nBLSharedHits                         (&fDirector,"trk_nBLSharedHits"),
         trk_nPixSharedHits                        (&fDirector,"trk_nPixSharedHits"),
         trk_nSCTSharedHits                        (&fDirector,"trk_nSCTSharedHits"),
         trk_nBLayerOutliers                       (&fDirector,"trk_nBLayerOutliers"),
         trk_nPixelOutliers                        (&fDirector,"trk_nPixelOutliers"),
         trk_nSCTOutliers                          (&fDirector,"trk_nSCTOutliers"),
         trk_nTRTOutliers                          (&fDirector,"trk_nTRTOutliers"),
         trk_nTRTHighTOutliers                     (&fDirector,"trk_nTRTHighTOutliers"),
         trk_nContribPixelLayers                   (&fDirector,"trk_nContribPixelLayers"),
         trk_nGangedPixels                         (&fDirector,"trk_nGangedPixels"),
         trk_nGangedFlaggedFakes                   (&fDirector,"trk_nGangedFlaggedFakes"),
         trk_nPixelDeadSensors                     (&fDirector,"trk_nPixelDeadSensors"),
         trk_nPixelSpoiltHits                      (&fDirector,"trk_nPixelSpoiltHits"),
         trk_nSCTDoubleHoles                       (&fDirector,"trk_nSCTDoubleHoles"),
         trk_nSCTDeadSensors                       (&fDirector,"trk_nSCTDeadSensors"),
         trk_nSCTSpoiltHits                        (&fDirector,"trk_nSCTSpoiltHits"),
         trk_expectBLayerHit                       (&fDirector,"trk_expectBLayerHit"),
         trk_hitPattern                            (&fDirector,"trk_hitPattern"),
         trk_nSiHits                               (&fDirector,"trk_nSiHits"),
         trk_fitter                                (&fDirector,"trk_fitter"),
         trk_patternReco1                          (&fDirector,"trk_patternReco1"),
         trk_patternReco2                          (&fDirector,"trk_patternReco2"),
         trk_seedFinder                            (&fDirector,"trk_seedFinder"),
         trk_blayerPrediction_x                    (&fDirector,"trk_blayerPrediction_x"),
         trk_blayerPrediction_y                    (&fDirector,"trk_blayerPrediction_y"),
         trk_blayerPrediction_z                    (&fDirector,"trk_blayerPrediction_z"),
         trk_blayerPrediction_locX                 (&fDirector,"trk_blayerPrediction_locX"),
         trk_blayerPrediction_locY                 (&fDirector,"trk_blayerPrediction_locY"),
         trk_blayerPrediction_err_locX             (&fDirector,"trk_blayerPrediction_err_locX"),
         trk_blayerPrediction_err_locY             (&fDirector,"trk_blayerPrediction_err_locY"),
         trk_blayerPrediction_etaDistToEdge        (&fDirector,"trk_blayerPrediction_etaDistToEdge"),
         trk_blayerPrediction_phiDistToEdge        (&fDirector,"trk_blayerPrediction_phiDistToEdge"),
         trk_blayerPrediction_detElementId         (&fDirector,"trk_blayerPrediction_detElementId"),
         trk_blayerPrediction_row                  (&fDirector,"trk_blayerPrediction_row"),
         trk_blayerPrediction_col                  (&fDirector,"trk_blayerPrediction_col"),
         trk_blayerPrediction_type                 (&fDirector,"trk_blayerPrediction_type"),
         trk_BLayer_hit_n                          (&fDirector,"trk_BLayer_hit_n"),
         trk_BLayer_hit_id                         (&fDirector,"trk_BLayer_hit_id"),
         trk_BLayer_hit_detElementId               (&fDirector,"trk_BLayer_hit_detElementId"),
         trk_BLayer_hit_bec                        (&fDirector,"trk_BLayer_hit_bec"),
         trk_BLayer_hit_layer                      (&fDirector,"trk_BLayer_hit_layer"),
         trk_BLayer_hit_charge                     (&fDirector,"trk_BLayer_hit_charge"),
         trk_BLayer_hit_sizePhi                    (&fDirector,"trk_BLayer_hit_sizePhi"),
         trk_BLayer_hit_sizeZ                      (&fDirector,"trk_BLayer_hit_sizeZ"),
         trk_BLayer_hit_size                       (&fDirector,"trk_BLayer_hit_size"),
         trk_BLayer_hit_isFake                     (&fDirector,"trk_BLayer_hit_isFake"),
         trk_BLayer_hit_isGanged                   (&fDirector,"trk_BLayer_hit_isGanged"),
         trk_BLayer_hit_isSplit                    (&fDirector,"trk_BLayer_hit_isSplit"),
         trk_BLayer_hit_splitProb1                 (&fDirector,"trk_BLayer_hit_splitProb1"),
         trk_BLayer_hit_splitProb2                 (&fDirector,"trk_BLayer_hit_splitProb2"),
         trk_BLayer_hit_isCompetingRIO             (&fDirector,"trk_BLayer_hit_isCompetingRIO"),
         trk_BLayer_hit_locX                       (&fDirector,"trk_BLayer_hit_locX"),
         trk_BLayer_hit_locY                       (&fDirector,"trk_BLayer_hit_locY"),
         trk_BLayer_hit_incidencePhi               (&fDirector,"trk_BLayer_hit_incidencePhi"),
         trk_BLayer_hit_incidenceTheta             (&fDirector,"trk_BLayer_hit_incidenceTheta"),
         trk_BLayer_hit_err_locX                   (&fDirector,"trk_BLayer_hit_err_locX"),
         trk_BLayer_hit_err_locY                   (&fDirector,"trk_BLayer_hit_err_locY"),
         trk_BLayer_hit_cov_locXY                  (&fDirector,"trk_BLayer_hit_cov_locXY"),
         trk_BLayer_hit_x                          (&fDirector,"trk_BLayer_hit_x"),
         trk_BLayer_hit_y                          (&fDirector,"trk_BLayer_hit_y"),
         trk_BLayer_hit_z                          (&fDirector,"trk_BLayer_hit_z"),
         trk_BLayer_hit_trkLocX                    (&fDirector,"trk_BLayer_hit_trkLocX"),
         trk_BLayer_hit_trkLocY                    (&fDirector,"trk_BLayer_hit_trkLocY"),
         trk_BLayer_hit_err_trkLocX                (&fDirector,"trk_BLayer_hit_err_trkLocX"),
         trk_BLayer_hit_err_trkLocY                (&fDirector,"trk_BLayer_hit_err_trkLocY"),
         trk_BLayer_hit_cov_trkLocXY               (&fDirector,"trk_BLayer_hit_cov_trkLocXY"),
         trk_BLayer_hit_locBiasedResidualX         (&fDirector,"trk_BLayer_hit_locBiasedResidualX"),
         trk_BLayer_hit_locBiasedResidualY         (&fDirector,"trk_BLayer_hit_locBiasedResidualY"),
         trk_BLayer_hit_locBiasedPullX             (&fDirector,"trk_BLayer_hit_locBiasedPullX"),
         trk_BLayer_hit_locBiasedPullY             (&fDirector,"trk_BLayer_hit_locBiasedPullY"),
         trk_BLayer_hit_locUnbiasedResidualX       (&fDirector,"trk_BLayer_hit_locUnbiasedResidualX"),
         trk_BLayer_hit_locUnbiasedResidualY       (&fDirector,"trk_BLayer_hit_locUnbiasedResidualY"),
         trk_BLayer_hit_locUnbiasedPullX           (&fDirector,"trk_BLayer_hit_locUnbiasedPullX"),
         trk_BLayer_hit_locUnbiasedPullY           (&fDirector,"trk_BLayer_hit_locUnbiasedPullY"),
         trk_BLayer_hit_chi2                       (&fDirector,"trk_BLayer_hit_chi2"),
         trk_BLayer_hit_ndof                       (&fDirector,"trk_BLayer_hit_ndof"),
         trk_Pixel_hit_n                           (&fDirector,"trk_Pixel_hit_n"),
         trk_Pixel_hit_id                          (&fDirector,"trk_Pixel_hit_id"),
         trk_Pixel_hit_detElementId                (&fDirector,"trk_Pixel_hit_detElementId"),
         trk_Pixel_hit_bec                         (&fDirector,"trk_Pixel_hit_bec"),
         trk_Pixel_hit_layer                       (&fDirector,"trk_Pixel_hit_layer"),
         trk_Pixel_hit_charge                      (&fDirector,"trk_Pixel_hit_charge"),
         trk_Pixel_hit_sizePhi                     (&fDirector,"trk_Pixel_hit_sizePhi"),
         trk_Pixel_hit_sizeZ                       (&fDirector,"trk_Pixel_hit_sizeZ"),
         trk_Pixel_hit_size                        (&fDirector,"trk_Pixel_hit_size"),
         trk_Pixel_hit_isFake                      (&fDirector,"trk_Pixel_hit_isFake"),
         trk_Pixel_hit_isGanged                    (&fDirector,"trk_Pixel_hit_isGanged"),
         trk_Pixel_hit_isSplit                     (&fDirector,"trk_Pixel_hit_isSplit"),
         trk_Pixel_hit_splitProb1                  (&fDirector,"trk_Pixel_hit_splitProb1"),
         trk_Pixel_hit_splitProb2                  (&fDirector,"trk_Pixel_hit_splitProb2"),
         trk_Pixel_hit_isCompetingRIO              (&fDirector,"trk_Pixel_hit_isCompetingRIO"),
         trk_Pixel_hit_locX                        (&fDirector,"trk_Pixel_hit_locX"),
         trk_Pixel_hit_locY                        (&fDirector,"trk_Pixel_hit_locY"),
         trk_Pixel_hit_incidencePhi                (&fDirector,"trk_Pixel_hit_incidencePhi"),
         trk_Pixel_hit_incidenceTheta              (&fDirector,"trk_Pixel_hit_incidenceTheta"),
         trk_Pixel_hit_err_locX                    (&fDirector,"trk_Pixel_hit_err_locX"),
         trk_Pixel_hit_err_locY                    (&fDirector,"trk_Pixel_hit_err_locY"),
         trk_Pixel_hit_cov_locXY                   (&fDirector,"trk_Pixel_hit_cov_locXY"),
         trk_Pixel_hit_x                           (&fDirector,"trk_Pixel_hit_x"),
         trk_Pixel_hit_y                           (&fDirector,"trk_Pixel_hit_y"),
         trk_Pixel_hit_z                           (&fDirector,"trk_Pixel_hit_z"),
         trk_Pixel_hit_trkLocX                     (&fDirector,"trk_Pixel_hit_trkLocX"),
         trk_Pixel_hit_trkLocY                     (&fDirector,"trk_Pixel_hit_trkLocY"),
         trk_Pixel_hit_err_trkLocX                 (&fDirector,"trk_Pixel_hit_err_trkLocX"),
         trk_Pixel_hit_err_trkLocY                 (&fDirector,"trk_Pixel_hit_err_trkLocY"),
         trk_Pixel_hit_cov_trkLocXY                (&fDirector,"trk_Pixel_hit_cov_trkLocXY"),
         trk_Pixel_hit_locBiasedResidualX          (&fDirector,"trk_Pixel_hit_locBiasedResidualX"),
         trk_Pixel_hit_locBiasedResidualY          (&fDirector,"trk_Pixel_hit_locBiasedResidualY"),
         trk_Pixel_hit_locBiasedPullX              (&fDirector,"trk_Pixel_hit_locBiasedPullX"),
         trk_Pixel_hit_locBiasedPullY              (&fDirector,"trk_Pixel_hit_locBiasedPullY"),
         trk_Pixel_hit_locUnbiasedResidualX        (&fDirector,"trk_Pixel_hit_locUnbiasedResidualX"),
         trk_Pixel_hit_locUnbiasedResidualY        (&fDirector,"trk_Pixel_hit_locUnbiasedResidualY"),
         trk_Pixel_hit_locUnbiasedPullX            (&fDirector,"trk_Pixel_hit_locUnbiasedPullX"),
         trk_Pixel_hit_locUnbiasedPullY            (&fDirector,"trk_Pixel_hit_locUnbiasedPullY"),
         trk_Pixel_hit_chi2                        (&fDirector,"trk_Pixel_hit_chi2"),
         trk_Pixel_hit_ndof                        (&fDirector,"trk_Pixel_hit_ndof"),
         trk_SCT_hit_n                             (&fDirector,"trk_SCT_hit_n"),
         trk_SCT_hit_id                            (&fDirector,"trk_SCT_hit_id"),
         trk_SCT_hit_detElementId                  (&fDirector,"trk_SCT_hit_detElementId"),
         trk_SCT_hit_bec                           (&fDirector,"trk_SCT_hit_bec"),
         trk_SCT_hit_layer                         (&fDirector,"trk_SCT_hit_layer"),
         trk_SCT_hit_sizePhi                       (&fDirector,"trk_SCT_hit_sizePhi"),
         trk_SCT_hit_isCompetingRIO                (&fDirector,"trk_SCT_hit_isCompetingRIO"),
         trk_SCT_hit_locX                          (&fDirector,"trk_SCT_hit_locX"),
         trk_SCT_hit_locY                          (&fDirector,"trk_SCT_hit_locY"),
         trk_SCT_hit_incidencePhi                  (&fDirector,"trk_SCT_hit_incidencePhi"),
         trk_SCT_hit_incidenceTheta                (&fDirector,"trk_SCT_hit_incidenceTheta"),
         trk_SCT_hit_err_locX                      (&fDirector,"trk_SCT_hit_err_locX"),
         trk_SCT_hit_err_locY                      (&fDirector,"trk_SCT_hit_err_locY"),
         trk_SCT_hit_cov_locXY                     (&fDirector,"trk_SCT_hit_cov_locXY"),
         trk_SCT_hit_x                             (&fDirector,"trk_SCT_hit_x"),
         trk_SCT_hit_y                             (&fDirector,"trk_SCT_hit_y"),
         trk_SCT_hit_z                             (&fDirector,"trk_SCT_hit_z"),
         trk_SCT_hit_trkLocX                       (&fDirector,"trk_SCT_hit_trkLocX"),
         trk_SCT_hit_trkLocY                       (&fDirector,"trk_SCT_hit_trkLocY"),
         trk_SCT_hit_err_trkLocX                   (&fDirector,"trk_SCT_hit_err_trkLocX"),
         trk_SCT_hit_err_trkLocY                   (&fDirector,"trk_SCT_hit_err_trkLocY"),
         trk_SCT_hit_cov_trkLocXY                  (&fDirector,"trk_SCT_hit_cov_trkLocXY"),
         trk_SCT_hit_locBiasedResidualX            (&fDirector,"trk_SCT_hit_locBiasedResidualX"),
         trk_SCT_hit_locBiasedResidualY            (&fDirector,"trk_SCT_hit_locBiasedResidualY"),
         trk_SCT_hit_locBiasedPullX                (&fDirector,"trk_SCT_hit_locBiasedPullX"),
         trk_SCT_hit_locBiasedPullY                (&fDirector,"trk_SCT_hit_locBiasedPullY"),
         trk_SCT_hit_locUnbiasedResidualX          (&fDirector,"trk_SCT_hit_locUnbiasedResidualX"),
         trk_SCT_hit_locUnbiasedResidualY          (&fDirector,"trk_SCT_hit_locUnbiasedResidualY"),
         trk_SCT_hit_locUnbiasedPullX              (&fDirector,"trk_SCT_hit_locUnbiasedPullX"),
         trk_SCT_hit_locUnbiasedPullY              (&fDirector,"trk_SCT_hit_locUnbiasedPullY"),
         trk_SCT_hit_chi2                          (&fDirector,"trk_SCT_hit_chi2"),
         trk_SCT_hit_ndof                          (&fDirector,"trk_SCT_hit_ndof"),
         trk_BLayer_outlier_n                      (&fDirector,"trk_BLayer_outlier_n"),
         trk_BLayer_outlier_id                     (&fDirector,"trk_BLayer_outlier_id"),
         trk_BLayer_outlier_detElementId           (&fDirector,"trk_BLayer_outlier_detElementId"),
         trk_BLayer_outlier_bec                    (&fDirector,"trk_BLayer_outlier_bec"),
         trk_BLayer_outlier_layer                  (&fDirector,"trk_BLayer_outlier_layer"),
         trk_BLayer_outlier_charge                 (&fDirector,"trk_BLayer_outlier_charge"),
         trk_BLayer_outlier_sizePhi                (&fDirector,"trk_BLayer_outlier_sizePhi"),
         trk_BLayer_outlier_sizeZ                  (&fDirector,"trk_BLayer_outlier_sizeZ"),
         trk_BLayer_outlier_size                   (&fDirector,"trk_BLayer_outlier_size"),
         trk_BLayer_outlier_isFake                 (&fDirector,"trk_BLayer_outlier_isFake"),
         trk_BLayer_outlier_isGanged               (&fDirector,"trk_BLayer_outlier_isGanged"),
         trk_BLayer_outlier_isSplit                (&fDirector,"trk_BLayer_outlier_isSplit"),
         trk_BLayer_outlier_splitProb1             (&fDirector,"trk_BLayer_outlier_splitProb1"),
         trk_BLayer_outlier_splitProb2             (&fDirector,"trk_BLayer_outlier_splitProb2"),
         trk_BLayer_outlier_isCompetingRIO         (&fDirector,"trk_BLayer_outlier_isCompetingRIO"),
         trk_BLayer_outlier_locX                   (&fDirector,"trk_BLayer_outlier_locX"),
         trk_BLayer_outlier_locY                   (&fDirector,"trk_BLayer_outlier_locY"),
         trk_BLayer_outlier_incidencePhi           (&fDirector,"trk_BLayer_outlier_incidencePhi"),
         trk_BLayer_outlier_incidenceTheta         (&fDirector,"trk_BLayer_outlier_incidenceTheta"),
         trk_BLayer_outlier_err_locX               (&fDirector,"trk_BLayer_outlier_err_locX"),
         trk_BLayer_outlier_err_locY               (&fDirector,"trk_BLayer_outlier_err_locY"),
         trk_BLayer_outlier_cov_locXY              (&fDirector,"trk_BLayer_outlier_cov_locXY"),
         trk_BLayer_outlier_x                      (&fDirector,"trk_BLayer_outlier_x"),
         trk_BLayer_outlier_y                      (&fDirector,"trk_BLayer_outlier_y"),
         trk_BLayer_outlier_z                      (&fDirector,"trk_BLayer_outlier_z"),
         trk_BLayer_outlier_trkLocX                (&fDirector,"trk_BLayer_outlier_trkLocX"),
         trk_BLayer_outlier_trkLocY                (&fDirector,"trk_BLayer_outlier_trkLocY"),
         trk_BLayer_outlier_err_trkLocX            (&fDirector,"trk_BLayer_outlier_err_trkLocX"),
         trk_BLayer_outlier_err_trkLocY            (&fDirector,"trk_BLayer_outlier_err_trkLocY"),
         trk_BLayer_outlier_cov_trkLocXY           (&fDirector,"trk_BLayer_outlier_cov_trkLocXY"),
         trk_BLayer_outlier_locBiasedResidualX     (&fDirector,"trk_BLayer_outlier_locBiasedResidualX"),
         trk_BLayer_outlier_locBiasedResidualY     (&fDirector,"trk_BLayer_outlier_locBiasedResidualY"),
         trk_BLayer_outlier_locBiasedPullX         (&fDirector,"trk_BLayer_outlier_locBiasedPullX"),
         trk_BLayer_outlier_locBiasedPullY         (&fDirector,"trk_BLayer_outlier_locBiasedPullY"),
         trk_BLayer_outlier_locUnbiasedResidualX   (&fDirector,"trk_BLayer_outlier_locUnbiasedResidualX"),
         trk_BLayer_outlier_locUnbiasedResidualY   (&fDirector,"trk_BLayer_outlier_locUnbiasedResidualY"),
         trk_BLayer_outlier_locUnbiasedPullX       (&fDirector,"trk_BLayer_outlier_locUnbiasedPullX"),
         trk_BLayer_outlier_locUnbiasedPullY       (&fDirector,"trk_BLayer_outlier_locUnbiasedPullY"),
         trk_BLayer_outlier_chi2                   (&fDirector,"trk_BLayer_outlier_chi2"),
         trk_BLayer_outlier_ndof                   (&fDirector,"trk_BLayer_outlier_ndof"),
         trk_Pixel_outlier_n                       (&fDirector,"trk_Pixel_outlier_n"),
         trk_Pixel_outlier_id                      (&fDirector,"trk_Pixel_outlier_id"),
         trk_Pixel_outlier_detElementId            (&fDirector,"trk_Pixel_outlier_detElementId"),
         trk_Pixel_outlier_bec                     (&fDirector,"trk_Pixel_outlier_bec"),
         trk_Pixel_outlier_layer                   (&fDirector,"trk_Pixel_outlier_layer"),
         trk_Pixel_outlier_charge                  (&fDirector,"trk_Pixel_outlier_charge"),
         trk_Pixel_outlier_sizePhi                 (&fDirector,"trk_Pixel_outlier_sizePhi"),
         trk_Pixel_outlier_sizeZ                   (&fDirector,"trk_Pixel_outlier_sizeZ"),
         trk_Pixel_outlier_size                    (&fDirector,"trk_Pixel_outlier_size"),
         trk_Pixel_outlier_isFake                  (&fDirector,"trk_Pixel_outlier_isFake"),
         trk_Pixel_outlier_isGanged                (&fDirector,"trk_Pixel_outlier_isGanged"),
         trk_Pixel_outlier_isSplit                 (&fDirector,"trk_Pixel_outlier_isSplit"),
         trk_Pixel_outlier_splitProb1              (&fDirector,"trk_Pixel_outlier_splitProb1"),
         trk_Pixel_outlier_splitProb2              (&fDirector,"trk_Pixel_outlier_splitProb2"),
         trk_Pixel_outlier_isCompetingRIO          (&fDirector,"trk_Pixel_outlier_isCompetingRIO"),
         trk_Pixel_outlier_locX                    (&fDirector,"trk_Pixel_outlier_locX"),
         trk_Pixel_outlier_locY                    (&fDirector,"trk_Pixel_outlier_locY"),
         trk_Pixel_outlier_incidencePhi            (&fDirector,"trk_Pixel_outlier_incidencePhi"),
         trk_Pixel_outlier_incidenceTheta          (&fDirector,"trk_Pixel_outlier_incidenceTheta"),
         trk_Pixel_outlier_err_locX                (&fDirector,"trk_Pixel_outlier_err_locX"),
         trk_Pixel_outlier_err_locY                (&fDirector,"trk_Pixel_outlier_err_locY"),
         trk_Pixel_outlier_cov_locXY               (&fDirector,"trk_Pixel_outlier_cov_locXY"),
         trk_Pixel_outlier_x                       (&fDirector,"trk_Pixel_outlier_x"),
         trk_Pixel_outlier_y                       (&fDirector,"trk_Pixel_outlier_y"),
         trk_Pixel_outlier_z                       (&fDirector,"trk_Pixel_outlier_z"),
         trk_Pixel_outlier_trkLocX                 (&fDirector,"trk_Pixel_outlier_trkLocX"),
         trk_Pixel_outlier_trkLocY                 (&fDirector,"trk_Pixel_outlier_trkLocY"),
         trk_Pixel_outlier_err_trkLocX             (&fDirector,"trk_Pixel_outlier_err_trkLocX"),
         trk_Pixel_outlier_err_trkLocY             (&fDirector,"trk_Pixel_outlier_err_trkLocY"),
         trk_Pixel_outlier_cov_trkLocXY            (&fDirector,"trk_Pixel_outlier_cov_trkLocXY"),
         trk_Pixel_outlier_locBiasedResidualX      (&fDirector,"trk_Pixel_outlier_locBiasedResidualX"),
         trk_Pixel_outlier_locBiasedResidualY      (&fDirector,"trk_Pixel_outlier_locBiasedResidualY"),
         trk_Pixel_outlier_locBiasedPullX          (&fDirector,"trk_Pixel_outlier_locBiasedPullX"),
         trk_Pixel_outlier_locBiasedPullY          (&fDirector,"trk_Pixel_outlier_locBiasedPullY"),
         trk_Pixel_outlier_locUnbiasedResidualX    (&fDirector,"trk_Pixel_outlier_locUnbiasedResidualX"),
         trk_Pixel_outlier_locUnbiasedResidualY    (&fDirector,"trk_Pixel_outlier_locUnbiasedResidualY"),
         trk_Pixel_outlier_locUnbiasedPullX        (&fDirector,"trk_Pixel_outlier_locUnbiasedPullX"),
         trk_Pixel_outlier_locUnbiasedPullY        (&fDirector,"trk_Pixel_outlier_locUnbiasedPullY"),
         trk_Pixel_outlier_chi2                    (&fDirector,"trk_Pixel_outlier_chi2"),
         trk_Pixel_outlier_ndof                    (&fDirector,"trk_Pixel_outlier_ndof"),
         trk_BLayer_hole_n                         (&fDirector,"trk_BLayer_hole_n"),
         trk_BLayer_hole_detElementId              (&fDirector,"trk_BLayer_hole_detElementId"),
         trk_BLayer_hole_bec                       (&fDirector,"trk_BLayer_hole_bec"),
         trk_BLayer_hole_layer                     (&fDirector,"trk_BLayer_hole_layer"),
         trk_BLayer_hole_trkLocX                   (&fDirector,"trk_BLayer_hole_trkLocX"),
         trk_BLayer_hole_trkLocY                   (&fDirector,"trk_BLayer_hole_trkLocY"),
         trk_BLayer_hole_err_trkLocX               (&fDirector,"trk_BLayer_hole_err_trkLocX"),
         trk_BLayer_hole_err_trkLocY               (&fDirector,"trk_BLayer_hole_err_trkLocY"),
         trk_BLayer_hole_cov_trkLocXY              (&fDirector,"trk_BLayer_hole_cov_trkLocXY"),
         trk_Pixel_hole_n                          (&fDirector,"trk_Pixel_hole_n"),
         trk_Pixel_hole_detElementId               (&fDirector,"trk_Pixel_hole_detElementId"),
         trk_Pixel_hole_bec                        (&fDirector,"trk_Pixel_hole_bec"),
         trk_Pixel_hole_layer                      (&fDirector,"trk_Pixel_hole_layer"),
         trk_Pixel_hole_trkLocX                    (&fDirector,"trk_Pixel_hole_trkLocX"),
         trk_Pixel_hole_trkLocY                    (&fDirector,"trk_Pixel_hole_trkLocY"),
         trk_Pixel_hole_err_trkLocX                (&fDirector,"trk_Pixel_hole_err_trkLocX"),
         trk_Pixel_hole_err_trkLocY                (&fDirector,"trk_Pixel_hole_err_trkLocY"),
         trk_Pixel_hole_cov_trkLocXY               (&fDirector,"trk_Pixel_hole_cov_trkLocXY"),
         trk_primvx_weight                         (&fDirector,"trk_primvx_weight"),
         trk_primvx_index                          (&fDirector,"trk_primvx_index"),
         trk_mcpart_probability                    (&fDirector,"trk_mcpart_probability"),
         trk_mcpart_barcode                        (&fDirector,"trk_mcpart_barcode"),
         trk_mcpart_index                          (&fDirector,"trk_mcpart_index"),
         trk_detailed_mc_n                         (&fDirector,"trk_detailed_mc_n"),
         trk_detailed_mc_nCommonPixHits            (&fDirector,"trk_detailed_mc_nCommonPixHits"),
         trk_detailed_mc_nCommonSCTHits            (&fDirector,"trk_detailed_mc_nCommonSCTHits"),
         trk_detailed_mc_nCommonTRTHits            (&fDirector,"trk_detailed_mc_nCommonTRTHits"),
         trk_detailed_mc_nRecoPixHits              (&fDirector,"trk_detailed_mc_nRecoPixHits"),
         trk_detailed_mc_nRecoSCTHits              (&fDirector,"trk_detailed_mc_nRecoSCTHits"),
         trk_detailed_mc_nRecoTRTHits              (&fDirector,"trk_detailed_mc_nRecoTRTHits"),
         trk_detailed_mc_nTruthPixHits             (&fDirector,"trk_detailed_mc_nTruthPixHits"),
         trk_detailed_mc_nTruthSCTHits             (&fDirector,"trk_detailed_mc_nTruthSCTHits"),
         trk_detailed_mc_nTruthTRTHits             (&fDirector,"trk_detailed_mc_nTruthTRTHits"),
         trk_detailed_mc_begVtx_barcode            (&fDirector,"trk_detailed_mc_begVtx_barcode"),
         trk_detailed_mc_endVtx_barcode            (&fDirector,"trk_detailed_mc_endVtx_barcode"),
         trk_detailed_mc_barcode                   (&fDirector,"trk_detailed_mc_barcode"),
         trk_detailed_mc_index                     (&fDirector,"trk_detailed_mc_index"),
         metmuonboyetx                             (&fDirector,"metmuonboyetx"),
         metmuonboyety                             (&fDirector,"metmuonboyety"),
         metmuonboyphi                             (&fDirector,"metmuonboyphi"),
         metmuonboyet                              (&fDirector,"metmuonboyet"),
         metmuonboysumet                           (&fDirector,"metmuonboysumet"),
         metreffinaletx                            (&fDirector,"metreffinaletx"),
         metreffinalety                            (&fDirector,"metreffinalety"),
         metreffinalphi                            (&fDirector,"metreffinalphi"),
         metreffinalet                             (&fDirector,"metreffinalet"),
         metreffinalsumet                          (&fDirector,"metreffinalsumet"),
         pixClus_n                                 (&fDirector,"pixClus_n"),
         pixClus_id                                (&fDirector,"pixClus_id"),
         pixClus_bec                               (&fDirector,"pixClus_bec"),
         pixClus_layer                             (&fDirector,"pixClus_layer"),
         pixClus_detElementId                      (&fDirector,"pixClus_detElementId"),
         pixClus_phi_module                        (&fDirector,"pixClus_phi_module"),
         pixClus_eta_module                        (&fDirector,"pixClus_eta_module"),
         pixClus_col                               (&fDirector,"pixClus_col"),
         pixClus_row                               (&fDirector,"pixClus_row"),
         pixClus_charge                            (&fDirector,"pixClus_charge"),
         pixClus_LVL1A                             (&fDirector,"pixClus_LVL1A"),
         pixClus_sizePhi                           (&fDirector,"pixClus_sizePhi"),
         pixClus_sizeZ                             (&fDirector,"pixClus_sizeZ"),
         pixClus_size                              (&fDirector,"pixClus_size"),
         pixClus_locX                              (&fDirector,"pixClus_locX"),
         pixClus_locY                              (&fDirector,"pixClus_locY"),
         pixClus_x                                 (&fDirector,"pixClus_x"),
         pixClus_y                                 (&fDirector,"pixClus_y"),
         pixClus_z                                 (&fDirector,"pixClus_z"),
         pixClus_isFake                            (&fDirector,"pixClus_isFake"),
         pixClus_isGanged                          (&fDirector,"pixClus_isGanged"),
         pixClus_isSplit                           (&fDirector,"pixClus_isSplit"),
         pixClus_splitProb1                        (&fDirector,"pixClus_splitProb1"),
         pixClus_splitProb2                        (&fDirector,"pixClus_splitProb2"),
         pixClus_mc_barcode                        (&fDirector,"pixClus_mc_barcode"),
         primvx_n                                  (&fDirector,"primvx_n"),
         primvx_x                                  (&fDirector,"primvx_x"),
         primvx_y                                  (&fDirector,"primvx_y"),
         primvx_z                                  (&fDirector,"primvx_z"),
         primvx_err_x                              (&fDirector,"primvx_err_x"),
         primvx_err_y                              (&fDirector,"primvx_err_y"),
         primvx_err_z                              (&fDirector,"primvx_err_z"),
         primvx_cov_xy                             (&fDirector,"primvx_cov_xy"),
         primvx_cov_xz                             (&fDirector,"primvx_cov_xz"),
         primvx_cov_yz                             (&fDirector,"primvx_cov_yz"),
         primvx_type                               (&fDirector,"primvx_type"),
         primvx_chi2                               (&fDirector,"primvx_chi2"),
         primvx_ndof                               (&fDirector,"primvx_ndof"),
         primvx_px                                 (&fDirector,"primvx_px"),
         primvx_py                                 (&fDirector,"primvx_py"),
         primvx_pz                                 (&fDirector,"primvx_pz"),
         primvx_E                                  (&fDirector,"primvx_E"),
         primvx_m                                  (&fDirector,"primvx_m"),
         primvx_nTracks                            (&fDirector,"primvx_nTracks"),
         primvx_sumPt                              (&fDirector,"primvx_sumPt"),
         primvx_trk_n                              (&fDirector,"primvx_trk_n"),
         primvx_trk_weight                         (&fDirector,"primvx_trk_weight"),
         primvx_trk_unbiased_d0                    (&fDirector,"primvx_trk_unbiased_d0"),
         primvx_trk_unbiased_z0                    (&fDirector,"primvx_trk_unbiased_z0"),
         primvx_trk_err_unbiased_d0                (&fDirector,"primvx_trk_err_unbiased_d0"),
         primvx_trk_err_unbiased_z0                (&fDirector,"primvx_trk_err_unbiased_z0"),
         primvx_trk_chi2                           (&fDirector,"primvx_trk_chi2"),
         primvx_trk_d0                             (&fDirector,"primvx_trk_d0"),
         primvx_trk_z0                             (&fDirector,"primvx_trk_z0"),
         primvx_trk_phi                            (&fDirector,"primvx_trk_phi"),
         primvx_trk_theta                          (&fDirector,"primvx_trk_theta"),
         primvx_trk_index                          (&fDirector,"primvx_trk_index"),
         beamSpot_x                                (&fDirector,"beamSpot_x"),
         beamSpot_y                                (&fDirector,"beamSpot_y"),
         beamSpot_z                                (&fDirector,"beamSpot_z"),
         beamSpot_sigma_x                          (&fDirector,"beamSpot_sigma_x"),
         beamSpot_sigma_y                          (&fDirector,"beamSpot_sigma_y"),
         beamSpot_sigma_z                          (&fDirector,"beamSpot_sigma_z"),
         mcevt_n                                   (&fDirector,"mcevt_n"),
         mcevt_signal_process_id                   (&fDirector,"mcevt_signal_process_id"),
         mcevt_event_number                        (&fDirector,"mcevt_event_number"),
         mcevt_event_scale                         (&fDirector,"mcevt_event_scale"),
         mcevt_alphaQCD                            (&fDirector,"mcevt_alphaQCD"),
         mcevt_alphaQED                            (&fDirector,"mcevt_alphaQED"),
         mcevt_pdf_id1                             (&fDirector,"mcevt_pdf_id1"),
         mcevt_pdf_id2                             (&fDirector,"mcevt_pdf_id2"),
         mcevt_pdf_x1                              (&fDirector,"mcevt_pdf_x1"),
         mcevt_pdf_x2                              (&fDirector,"mcevt_pdf_x2"),
         mcevt_pdf_scale                           (&fDirector,"mcevt_pdf_scale"),
         mcevt_pdf1                                (&fDirector,"mcevt_pdf1"),
         mcevt_pdf2                                (&fDirector,"mcevt_pdf2"),
         mcevt_weight                              (&fDirector,"mcevt_weight"),
         mcevt_nparticle                           (&fDirector,"mcevt_nparticle"),
         mcevt_pileUpType                          (&fDirector,"mcevt_pileUpType"),
         mcvtx_n                                   (&fDirector,"mcvtx_n"),
         mcvtx_x                                   (&fDirector,"mcvtx_x"),
         mcvtx_y                                   (&fDirector,"mcvtx_y"),
         mcvtx_z                                   (&fDirector,"mcvtx_z"),
         mcvtx_barcode                             (&fDirector,"mcvtx_barcode"),
         mcvtx_mcevt_index                         (&fDirector,"mcvtx_mcevt_index"),
         mcpart_n                                  (&fDirector,"mcpart_n"),
         mcpart_pt                                 (&fDirector,"mcpart_pt"),
         mcpart_m                                  (&fDirector,"mcpart_m"),
         mcpart_eta                                (&fDirector,"mcpart_eta"),
         mcpart_phi                                (&fDirector,"mcpart_phi"),
         mcpart_type                               (&fDirector,"mcpart_type"),
         mcpart_status                             (&fDirector,"mcpart_status"),
         mcpart_barcode                            (&fDirector,"mcpart_barcode"),
         mcpart_mothertype                         (&fDirector,"mcpart_mothertype"),
         mcpart_motherbarcode                      (&fDirector,"mcpart_motherbarcode"),
         mcpart_mcevt_index                        (&fDirector,"mcpart_mcevt_index"),
         mcpart_mcprodvtx_index                    (&fDirector,"mcpart_mcprodvtx_index"),
         mcpart_mother_n                           (&fDirector,"mcpart_mother_n"),
         mcpart_mother_index                       (&fDirector,"mcpart_mother_index"),
         mcpart_mcdecayvtx_index                   (&fDirector,"mcpart_mcdecayvtx_index"),
         mcpart_child_n                            (&fDirector,"mcpart_child_n"),
         mcpart_child_index                        (&fDirector,"mcpart_child_index"),
         mcpart_truthtracks_index                  (&fDirector,"mcpart_truthtracks_index"),
         truthtrack_n                              (&fDirector,"truthtrack_n"),
         truthtrack_ok                             (&fDirector,"truthtrack_ok"),
         truthtrack_d0                             (&fDirector,"truthtrack_d0"),
         truthtrack_z0                             (&fDirector,"truthtrack_z0"),
         truthtrack_phi                            (&fDirector,"truthtrack_phi"),
         truthtrack_theta                          (&fDirector,"truthtrack_theta"),
         truthtrack_qoverp                         (&fDirector,"truthtrack_qoverp"),
         trig_L1_TAV                               (&fDirector,"trig_L1_TAV"),
         trig_L2_passedPhysics                     (&fDirector,"trig_L2_passedPhysics"),
         trig_EF_passedPhysics                     (&fDirector,"trig_EF_passedPhysics"),
         trig_L1_TBP                               (&fDirector,"trig_L1_TBP"),
         trig_L1_TAP                               (&fDirector,"trig_L1_TAP"),
         trig_L2_passedRaw                         (&fDirector,"trig_L2_passedRaw"),
         trig_EF_passedRaw                         (&fDirector,"trig_EF_passedRaw"),
         trig_L2_truncated                         (&fDirector,"trig_L2_truncated"),
         trig_EF_truncated                         (&fDirector,"trig_EF_truncated"),
         trig_L2_resurrected                       (&fDirector,"trig_L2_resurrected"),
         trig_EF_resurrected                       (&fDirector,"trig_EF_resurrected"),
         trig_L2_passedThrough                     (&fDirector,"trig_L2_passedThrough"),
         trig_EF_passedThrough                     (&fDirector,"trig_EF_passedThrough"),
         trig_DB_SMK                               (&fDirector,"trig_DB_SMK"),
         trig_DB_L1PSK                             (&fDirector,"trig_DB_L1PSK"),
         trig_DB_HLTPSK                            (&fDirector,"trig_DB_HLTPSK"),
         EF_2b10_medium_3L1J10                     (&fDirector,"EF_2b10_medium_3L1J10"),
         EF_2b10_medium_4L1J10                     (&fDirector,"EF_2b10_medium_4L1J10"),
         EF_2b10_medium_4j30_a4tc_EFFS             (&fDirector,"EF_2b10_medium_4j30_a4tc_EFFS"),
         EF_2b10_medium_L1JE100                    (&fDirector,"EF_2b10_medium_L1JE100"),
         EF_2b10_medium_L1JE140                    (&fDirector,"EF_2b10_medium_L1JE140"),
         EF_2b10_medium_L1_2J10J50                 (&fDirector,"EF_2b10_medium_L1_2J10J50"),
         EF_2b10_medium_j100_j30_a4tc_EFFS         (&fDirector,"EF_2b10_medium_j100_j30_a4tc_EFFS"),
         EF_2b10_medium_j75_j30_a4tc_EFFS          (&fDirector,"EF_2b10_medium_j75_j30_a4tc_EFFS"),
         EF_2b10_tight_4j30_a4tc_EFFS              (&fDirector,"EF_2b10_tight_4j30_a4tc_EFFS"),
         EF_2b15_medium_3L1J15                     (&fDirector,"EF_2b15_medium_3L1J15"),
         EF_2b15_medium_3j40_a4tc_EFFS             (&fDirector,"EF_2b15_medium_3j40_a4tc_EFFS"),
         EF_2b15_medium_j75_j40_a4tc_EFFS          (&fDirector,"EF_2b15_medium_j75_j40_a4tc_EFFS"),
         EF_2b20_medium_3L1J20                     (&fDirector,"EF_2b20_medium_3L1J20"),
         EF_2b20_medium_3j45_a4tc_EFFS             (&fDirector,"EF_2b20_medium_3j45_a4tc_EFFS"),
         EF_2b20_medium_j75_j45_a4tc_EFFS          (&fDirector,"EF_2b20_medium_j75_j45_a4tc_EFFS"),
         EF_2fj100_a4tc_EFFS_deta50_FB             (&fDirector,"EF_2fj100_a4tc_EFFS_deta50_FB"),
         EF_2fj30_a4tc_EFFS_deta50_FB              (&fDirector,"EF_2fj30_a4tc_EFFS_deta50_FB"),
         EF_2fj30_a4tc_EFFS_deta50_FC              (&fDirector,"EF_2fj30_a4tc_EFFS_deta50_FC"),
         EF_2fj55_a4tc_EFFS_deta50_FB              (&fDirector,"EF_2fj55_a4tc_EFFS_deta50_FB"),
         EF_2fj55_a4tc_EFFS_deta50_FC              (&fDirector,"EF_2fj55_a4tc_EFFS_deta50_FC"),
         EF_2fj75_a4tc_EFFS_deta50_FB              (&fDirector,"EF_2fj75_a4tc_EFFS_deta50_FB"),
         EF_2fj75_a4tc_EFFS_deta50_FC              (&fDirector,"EF_2fj75_a4tc_EFFS_deta50_FC"),
         EF_2j100_a4tc_EFFS_deta35_FC              (&fDirector,"EF_2j100_a4tc_EFFS_deta35_FC"),
         EF_2j135_a4tc_EFFS_deta35_FC              (&fDirector,"EF_2j135_a4tc_EFFS_deta35_FC"),
         EF_2j180_a4tc_EFFS_deta35_FC              (&fDirector,"EF_2j180_a4tc_EFFS_deta35_FC"),
         EF_2j240_a4tc_EFFS_deta35_FC              (&fDirector,"EF_2j240_a4tc_EFFS_deta35_FC"),
         EF_2j45_a4tc_EFFS_leadingmct100_xe40_medium_noMu(&fDirector,"EF_2j45_a4tc_EFFS_leadingmct100_xe40_medium_noMu"),
         EF_2j55_a4tc_EFFS_leadingmct100_xe40_medium_noMu(&fDirector,"EF_2j55_a4tc_EFFS_leadingmct100_xe40_medium_noMu"),
         EF_3b10_loose_4L1J10                      (&fDirector,"EF_3b10_loose_4L1J10"),
         EF_3b10_medium_4j30_a4tc_EFFS             (&fDirector,"EF_3b10_medium_4j30_a4tc_EFFS"),
         EF_3b15_loose_4L1J15                      (&fDirector,"EF_3b15_loose_4L1J15"),
         EF_3b15_medium_4j40_a4tc_EFFS             (&fDirector,"EF_3b15_medium_4j40_a4tc_EFFS"),
         EF_3j100_a4tc_EFFS                        (&fDirector,"EF_3j100_a4tc_EFFS"),
         EF_3j100_a4tc_EFFS_L1J75                  (&fDirector,"EF_3j100_a4tc_EFFS_L1J75"),
         EF_3j30_a4tc_EFFS                         (&fDirector,"EF_3j30_a4tc_EFFS"),
         EF_3j40_a4tc_EFFS                         (&fDirector,"EF_3j40_a4tc_EFFS"),
         EF_3j45_a4tc_EFFS                         (&fDirector,"EF_3j45_a4tc_EFFS"),
         EF_3j75_a4tc_EFFS                         (&fDirector,"EF_3j75_a4tc_EFFS"),
         EF_4j30_a4tc_EFFS                         (&fDirector,"EF_4j30_a4tc_EFFS"),
         EF_4j40_a4tc_EFFS                         (&fDirector,"EF_4j40_a4tc_EFFS"),
         EF_4j40_a4tc_EFFS_ht350                   (&fDirector,"EF_4j40_a4tc_EFFS_ht350"),
         EF_4j40_a4tc_EFFS_ht400                   (&fDirector,"EF_4j40_a4tc_EFFS_ht400"),
         EF_4j45_a4tc_EFFS                         (&fDirector,"EF_4j45_a4tc_EFFS"),
         EF_4j55_a4tc_EFFS                         (&fDirector,"EF_4j55_a4tc_EFFS"),
         EF_4j60_a4tc_EFFS                         (&fDirector,"EF_4j60_a4tc_EFFS"),
         EF_5j30_a4tc_EFFS                         (&fDirector,"EF_5j30_a4tc_EFFS"),
         EF_5j40_a4tc_EFFS                         (&fDirector,"EF_5j40_a4tc_EFFS"),
         EF_5j45_a4tc_EFFS                         (&fDirector,"EF_5j45_a4tc_EFFS"),
         EF_6j30_a4tc_EFFS                         (&fDirector,"EF_6j30_a4tc_EFFS"),
         EF_6j30_a4tc_EFFS_L15J10                  (&fDirector,"EF_6j30_a4tc_EFFS_L15J10"),
         EF_6j40_a4tc_EFFS                         (&fDirector,"EF_6j40_a4tc_EFFS"),
         EF_6j45_a4tc_EFFS                         (&fDirector,"EF_6j45_a4tc_EFFS"),
         EF_7j30_a4tc_EFFS_L15J10                  (&fDirector,"EF_7j30_a4tc_EFFS_L15J10"),
         EF_7j30_a4tc_EFFS_L16J10                  (&fDirector,"EF_7j30_a4tc_EFFS_L16J10"),
         EF_b10_EFj10_a4tc_EFFS_IDTrkNoCut         (&fDirector,"EF_b10_EFj10_a4tc_EFFS_IDTrkNoCut"),
         EF_b10_IDTrkNoCut                         (&fDirector,"EF_b10_IDTrkNoCut"),
         EF_b10_L2Star_IDTrkNoCut                  (&fDirector,"EF_b10_L2Star_IDTrkNoCut"),
         EF_b10_medium_4L1J10                      (&fDirector,"EF_b10_medium_4L1J10"),
         EF_b10_medium_4j30_a4tc_EFFS              (&fDirector,"EF_b10_medium_4j30_a4tc_EFFS"),
         EF_b10_medium_EFxe25_noMu_L1JE100         (&fDirector,"EF_b10_medium_EFxe25_noMu_L1JE100"),
         EF_b10_medium_EFxe25_noMu_L1JE140         (&fDirector,"EF_b10_medium_EFxe25_noMu_L1JE140"),
         EF_b10_medium_EFxe25_noMu_L1_2J10J50      (&fDirector,"EF_b10_medium_EFxe25_noMu_L1_2J10J50"),
         EF_b10_medium_j75_j55_2j30_a4tc_EFFS      (&fDirector,"EF_b10_medium_j75_j55_2j30_a4tc_EFFS"),
         EF_b10_tight_4L1J10                       (&fDirector,"EF_b10_tight_4L1J10"),
         EF_b10_tight_4j30_a4tc_EFFS               (&fDirector,"EF_b10_tight_4j30_a4tc_EFFS"),
         EF_b10_tight_L1JE100                      (&fDirector,"EF_b10_tight_L1JE100"),
         EF_b10_tight_L1JE140                      (&fDirector,"EF_b10_tight_L1JE140"),
         EF_b10_tight_j75_j55_2j30_a4tc_EFFS       (&fDirector,"EF_b10_tight_j75_j55_2j30_a4tc_EFFS"),
         EF_b15_IDTrkNoCut                         (&fDirector,"EF_b15_IDTrkNoCut"),
         EF_b20_IDTrkNoCut                         (&fDirector,"EF_b20_IDTrkNoCut"),
         EF_fj100_a4tc_EFFS                        (&fDirector,"EF_fj100_a4tc_EFFS"),
         EF_fj10_a4tc_EFFS                         (&fDirector,"EF_fj10_a4tc_EFFS"),
         EF_fj10_a4tc_EFFS_1vx                     (&fDirector,"EF_fj10_a4tc_EFFS_1vx"),
         EF_fj135_a4tc_EFFS                        (&fDirector,"EF_fj135_a4tc_EFFS"),
         EF_fj15_a4tc_EFFS                         (&fDirector,"EF_fj15_a4tc_EFFS"),
         EF_fj20_a4tc_EFFS                         (&fDirector,"EF_fj20_a4tc_EFFS"),
         EF_fj30_a4tc_EFFS                         (&fDirector,"EF_fj30_a4tc_EFFS"),
         EF_fj30_a4tc_EFFS_l2cleanph               (&fDirector,"EF_fj30_a4tc_EFFS_l2cleanph"),
         EF_fj55_a4tc_EFFS                         (&fDirector,"EF_fj55_a4tc_EFFS"),
         EF_fj75_a4tc_EFFS                         (&fDirector,"EF_fj75_a4tc_EFFS"),
         EF_j100_a4tc_EFFS                         (&fDirector,"EF_j100_a4tc_EFFS"),
         EF_j100_a4tc_EFFS_ht350                   (&fDirector,"EF_j100_a4tc_EFFS_ht350"),
         EF_j100_a4tc_EFFS_ht400                   (&fDirector,"EF_j100_a4tc_EFFS_ht400"),
         EF_j100_a4tc_EFFS_ht500                   (&fDirector,"EF_j100_a4tc_EFFS_ht500"),
         EF_j100_j30_a4tc_EFFS_L2dphi04            (&fDirector,"EF_j100_j30_a4tc_EFFS_L2dphi04"),
         EF_j10_a4tc_EFFS                          (&fDirector,"EF_j10_a4tc_EFFS"),
         EF_j10_a4tc_EFFS_1vx                      (&fDirector,"EF_j10_a4tc_EFFS_1vx"),
         EF_j135_a4tc_EFFS                         (&fDirector,"EF_j135_a4tc_EFFS"),
         EF_j135_a4tc_EFFS_ht500                   (&fDirector,"EF_j135_a4tc_EFFS_ht500"),
         EF_j135_j30_a4tc_EFFS_L2dphi04            (&fDirector,"EF_j135_j30_a4tc_EFFS_L2dphi04"),
         EF_j135_j30_a4tc_EFFS_dphi04              (&fDirector,"EF_j135_j30_a4tc_EFFS_dphi04"),
         EF_j15_a4tc_EFFS                          (&fDirector,"EF_j15_a4tc_EFFS"),
         EF_j180_a4tc_EFFS                         (&fDirector,"EF_j180_a4tc_EFFS"),
         EF_j180_j30_a4tc_EFFS_dphi04              (&fDirector,"EF_j180_j30_a4tc_EFFS_dphi04"),
         EF_j20_a4tc_EFFS                          (&fDirector,"EF_j20_a4tc_EFFS"),
         EF_j240_a10tc_EFFS                        (&fDirector,"EF_j240_a10tc_EFFS"),
         EF_j240_a4tc_EFFS                         (&fDirector,"EF_j240_a4tc_EFFS"),
         EF_j240_a4tc_EFFS_l2cleanph               (&fDirector,"EF_j240_a4tc_EFFS_l2cleanph"),
         EF_j30_a4tc_EFFS                          (&fDirector,"EF_j30_a4tc_EFFS"),
         EF_j30_a4tc_EFFS_l2cleanph                (&fDirector,"EF_j30_a4tc_EFFS_l2cleanph"),
         EF_j30_cosmic                             (&fDirector,"EF_j30_cosmic"),
         EF_j30_eta13_a4tc_EFFS_EFxe30_noMu_empty  (&fDirector,"EF_j30_eta13_a4tc_EFFS_EFxe30_noMu_empty"),
         EF_j30_eta13_a4tc_EFFS_EFxe30_noMu_firstempty(&fDirector,"EF_j30_eta13_a4tc_EFFS_EFxe30_noMu_firstempty"),
         EF_j30_firstempty                         (&fDirector,"EF_j30_firstempty"),
         EF_j30_fj30_a4tc_EFFS                     (&fDirector,"EF_j30_fj30_a4tc_EFFS"),
         EF_j320_a10tc_EFFS                        (&fDirector,"EF_j320_a10tc_EFFS"),
         EF_j320_a4tc_EFFS                         (&fDirector,"EF_j320_a4tc_EFFS"),
         EF_j35_a4tc_EFFS                          (&fDirector,"EF_j35_a4tc_EFFS"),
         EF_j35_a4tc_EFFS_L1TAU_HV                 (&fDirector,"EF_j35_a4tc_EFFS_L1TAU_HV"),
         EF_j35_a4tc_EFFS_L1TAU_HV_cosmic          (&fDirector,"EF_j35_a4tc_EFFS_L1TAU_HV_cosmic"),
         EF_j35_a4tc_EFFS_L1TAU_HV_firstempty      (&fDirector,"EF_j35_a4tc_EFFS_L1TAU_HV_firstempty"),
         EF_j35_a4tc_EFFS_L1TAU_HV_unpaired_iso    (&fDirector,"EF_j35_a4tc_EFFS_L1TAU_HV_unpaired_iso"),
         EF_j35_a4tc_EFFS_L1TAU_HV_unpaired_noniso (&fDirector,"EF_j35_a4tc_EFFS_L1TAU_HV_unpaired_noniso"),
         EF_j35_a4tc_EFFS_L1TAU_HVtrk              (&fDirector,"EF_j35_a4tc_EFFS_L1TAU_HVtrk"),
         EF_j35_a4tc_EFFS_L1TAU_HVtrk_cosmic       (&fDirector,"EF_j35_a4tc_EFFS_L1TAU_HVtrk_cosmic"),
         EF_j35_a4tc_EFFS_L1TAU_HVtrk_firstempty   (&fDirector,"EF_j35_a4tc_EFFS_L1TAU_HVtrk_firstempty"),
         EF_j35_a4tc_EFFS_L1TAU_HVtrk_unpaired_iso (&fDirector,"EF_j35_a4tc_EFFS_L1TAU_HVtrk_unpaired_iso"),
         EF_j35_a4tc_EFFS_L1TAU_HVtrk_unpaired_noniso(&fDirector,"EF_j35_a4tc_EFFS_L1TAU_HVtrk_unpaired_noniso"),
         EF_j40_a4tc_EFFS                          (&fDirector,"EF_j40_a4tc_EFFS"),
         EF_j40_fj40_a4tc_EFFS                     (&fDirector,"EF_j40_fj40_a4tc_EFFS"),
         EF_j425_a10tc_EFFS                        (&fDirector,"EF_j425_a10tc_EFFS"),
         EF_j425_a4tc_EFFS                         (&fDirector,"EF_j425_a4tc_EFFS"),
         EF_j45_a4tc_EFFS                          (&fDirector,"EF_j45_a4tc_EFFS"),
         EF_j50_a4tc_EFFS                          (&fDirector,"EF_j50_a4tc_EFFS"),
         EF_j50_cosmic                             (&fDirector,"EF_j50_cosmic"),
         EF_j50_eta13_a4tc_EFFS_EFxe50_noMu_empty  (&fDirector,"EF_j50_eta13_a4tc_EFFS_EFxe50_noMu_empty"),
         EF_j50_eta13_a4tc_EFFS_EFxe50_noMu_firstempty(&fDirector,"EF_j50_eta13_a4tc_EFFS_EFxe50_noMu_firstempty"),
         EF_j50_eta25_a4tc_EFFS_EFxe50_noMu_empty  (&fDirector,"EF_j50_eta25_a4tc_EFFS_EFxe50_noMu_empty"),
         EF_j50_eta25_a4tc_EFFS_EFxe50_noMu_firstempty(&fDirector,"EF_j50_eta25_a4tc_EFFS_EFxe50_noMu_firstempty"),
         EF_j50_firstempty                         (&fDirector,"EF_j50_firstempty"),
         EF_j55_a4tc_EFFS                          (&fDirector,"EF_j55_a4tc_EFFS"),
         EF_j55_a4tc_EFFS_xe55_medium_noMu_dphi2j30xe10(&fDirector,"EF_j55_a4tc_EFFS_xe55_medium_noMu_dphi2j30xe10"),
         EF_j55_a4tc_EFFS_xe55_medium_noMu_dphi2j30xe10_l2cleancons(&fDirector,"EF_j55_a4tc_EFFS_xe55_medium_noMu_dphi2j30xe10_l2cleancons"),
         EF_j55_fj55_a4tc_EFFS                     (&fDirector,"EF_j55_fj55_a4tc_EFFS"),
         EF_j65_a4tc_EFFS_xe65_noMu_dphi2j30xe10   (&fDirector,"EF_j65_a4tc_EFFS_xe65_noMu_dphi2j30xe10"),
         EF_j70_j25_dphi03_NoEF                    (&fDirector,"EF_j70_j25_dphi03_NoEF"),
         EF_j75_2j30_a4tc_EFFS_ht350               (&fDirector,"EF_j75_2j30_a4tc_EFFS_ht350"),
         EF_j75_a4tc_EFFS                          (&fDirector,"EF_j75_a4tc_EFFS"),
         EF_j75_a4tc_EFFS_xe40_loose_noMu          (&fDirector,"EF_j75_a4tc_EFFS_xe40_loose_noMu"),
         EF_j75_a4tc_EFFS_xe40_loose_noMu_dphijxe03(&fDirector,"EF_j75_a4tc_EFFS_xe40_loose_noMu_dphijxe03"),
         EF_j75_a4tc_EFFS_xe45_loose_noMu          (&fDirector,"EF_j75_a4tc_EFFS_xe45_loose_noMu"),
         EF_j75_a4tc_EFFS_xe55_loose_noMu          (&fDirector,"EF_j75_a4tc_EFFS_xe55_loose_noMu"),
         EF_j75_a4tc_EFFS_xe55_noMu                (&fDirector,"EF_j75_a4tc_EFFS_xe55_noMu"),
         EF_j75_a4tc_EFFS_xe55_noMu_l2cleancons    (&fDirector,"EF_j75_a4tc_EFFS_xe55_noMu_l2cleancons"),
         EF_j75_a4tc_EFFS_xs35_noMu                (&fDirector,"EF_j75_a4tc_EFFS_xs35_noMu"),
         EF_j75_fj75_a4tc_EFFS                     (&fDirector,"EF_j75_fj75_a4tc_EFFS"),
         EF_j75_j30_a4tc_EFFS                      (&fDirector,"EF_j75_j30_a4tc_EFFS"),
         EF_j75_j30_a4tc_EFFS_L2anymct100          (&fDirector,"EF_j75_j30_a4tc_EFFS_L2anymct100"),
         EF_j75_j30_a4tc_EFFS_L2anymct150          (&fDirector,"EF_j75_j30_a4tc_EFFS_L2anymct150"),
         EF_j75_j30_a4tc_EFFS_L2anymct175          (&fDirector,"EF_j75_j30_a4tc_EFFS_L2anymct175"),
         EF_j75_j30_a4tc_EFFS_L2dphi04             (&fDirector,"EF_j75_j30_a4tc_EFFS_L2dphi04"),
         EF_j75_j30_a4tc_EFFS_anymct150            (&fDirector,"EF_j75_j30_a4tc_EFFS_anymct150"),
         EF_j75_j30_a4tc_EFFS_anymct175            (&fDirector,"EF_j75_j30_a4tc_EFFS_anymct175"),
         EF_j75_j30_a4tc_EFFS_leadingmct150        (&fDirector,"EF_j75_j30_a4tc_EFFS_leadingmct150"),
         EF_j80_a4tc_EFFS_xe60_noMu                (&fDirector,"EF_j80_a4tc_EFFS_xe60_noMu"),
         EF_je195_NoEF                             (&fDirector,"EF_je195_NoEF"),
         EF_je255_NoEF                             (&fDirector,"EF_je255_NoEF"),
         EF_je300_NoEF                             (&fDirector,"EF_je300_NoEF"),
         EF_je350_NoEF                             (&fDirector,"EF_je350_NoEF"),
         EF_je420_NoEF                             (&fDirector,"EF_je420_NoEF"),
         EF_je500_NoEF                             (&fDirector,"EF_je500_NoEF"),
         EF_mu4_L1J10_matched                      (&fDirector,"EF_mu4_L1J10_matched"),
         EF_mu4_L1J20_matched                      (&fDirector,"EF_mu4_L1J20_matched"),
         EF_mu4_L1J30_matched                      (&fDirector,"EF_mu4_L1J30_matched"),
         EF_mu4_L1J50_matched                      (&fDirector,"EF_mu4_L1J50_matched"),
         EF_mu4_L1J75_matched                      (&fDirector,"EF_mu4_L1J75_matched"),
         EF_mu4_j10_a4tc_EFFS                      (&fDirector,"EF_mu4_j10_a4tc_EFFS"),
         L1_2FJ10                                  (&fDirector,"L1_2FJ10"),
         L1_2FJ30                                  (&fDirector,"L1_2FJ30"),
         L1_2FJ50                                  (&fDirector,"L1_2FJ50"),
         L1_2J10_J30_JE140                         (&fDirector,"L1_2J10_J30_JE140"),
         L1_2J10_J50                               (&fDirector,"L1_2J10_J50"),
         L1_2J10_J75                               (&fDirector,"L1_2J10_J75"),
         L1_2J15_J50                               (&fDirector,"L1_2J15_J50"),
         L1_2J20_XE20                              (&fDirector,"L1_2J20_XE20"),
         L1_2J30_XE20                              (&fDirector,"L1_2J30_XE20"),
         L1_3J10                                   (&fDirector,"L1_3J10"),
         L1_3J10_J50                               (&fDirector,"L1_3J10_J50"),
         L1_3J15                                   (&fDirector,"L1_3J15"),
         L1_3J20                                   (&fDirector,"L1_3J20"),
         L1_3J50                                   (&fDirector,"L1_3J50"),
         L1_3J75                                   (&fDirector,"L1_3J75"),
         L1_4J10                                   (&fDirector,"L1_4J10"),
         L1_4J15                                   (&fDirector,"L1_4J15"),
         L1_4J20                                   (&fDirector,"L1_4J20"),
         L1_4J30                                   (&fDirector,"L1_4J30"),
         L1_5J10                                   (&fDirector,"L1_5J10"),
         L1_5J20                                   (&fDirector,"L1_5J20"),
         L1_6J10                                   (&fDirector,"L1_6J10"),
         L1_FJ10                                   (&fDirector,"L1_FJ10"),
         L1_FJ10_EMPTY                             (&fDirector,"L1_FJ10_EMPTY"),
         L1_FJ10_FIRSTEMPTY                        (&fDirector,"L1_FJ10_FIRSTEMPTY"),
         L1_FJ10_UNPAIRED_ISO                      (&fDirector,"L1_FJ10_UNPAIRED_ISO"),
         L1_FJ30                                   (&fDirector,"L1_FJ30"),
         L1_FJ50                                   (&fDirector,"L1_FJ50"),
         L1_FJ75                                   (&fDirector,"L1_FJ75"),
         L1_J10                                    (&fDirector,"L1_J10"),
         L1_J10_EMPTY                              (&fDirector,"L1_J10_EMPTY"),
         L1_J10_FIRSTEMPTY                         (&fDirector,"L1_J10_FIRSTEMPTY"),
         L1_J10_FJ10                               (&fDirector,"L1_J10_FJ10"),
         L1_J10_UNPAIRED_ISO                       (&fDirector,"L1_J10_UNPAIRED_ISO"),
         L1_J10_UNPAIRED_NONISO                    (&fDirector,"L1_J10_UNPAIRED_NONISO"),
         L1_J15                                    (&fDirector,"L1_J15"),
         L1_J175                                   (&fDirector,"L1_J175"),
         L1_J20                                    (&fDirector,"L1_J20"),
         L1_J250                                   (&fDirector,"L1_J250"),
         L1_J30                                    (&fDirector,"L1_J30"),
         L1_J30_EMPTY                              (&fDirector,"L1_J30_EMPTY"),
         L1_J30_FIRSTEMPTY                         (&fDirector,"L1_J30_FIRSTEMPTY"),
         L1_J30_FJ30                               (&fDirector,"L1_J30_FJ30"),
         L1_J30_UNPAIRED_ISO                       (&fDirector,"L1_J30_UNPAIRED_ISO"),
         L1_J30_UNPAIRED_NONISO                    (&fDirector,"L1_J30_UNPAIRED_NONISO"),
         L1_J30_XE35                               (&fDirector,"L1_J30_XE35"),
         L1_J30_XE40                               (&fDirector,"L1_J30_XE40"),
         L1_J50                                    (&fDirector,"L1_J50"),
         L1_J50_FJ50                               (&fDirector,"L1_J50_FJ50"),
         L1_J50_XE20                               (&fDirector,"L1_J50_XE20"),
         L1_J50_XE30                               (&fDirector,"L1_J50_XE30"),
         L1_J50_XE35                               (&fDirector,"L1_J50_XE35"),
         L1_J50_XE40                               (&fDirector,"L1_J50_XE40"),
         L1_J50_XS25                               (&fDirector,"L1_J50_XS25"),
         L1_J75                                    (&fDirector,"L1_J75"),
         L1_JE100                                  (&fDirector,"L1_JE100"),
         L1_JE140                                  (&fDirector,"L1_JE140"),
         L1_JE200                                  (&fDirector,"L1_JE200"),
         L2_2b10_medium_3L1J10                     (&fDirector,"L2_2b10_medium_3L1J10"),
         L2_2b10_medium_4L1J10                     (&fDirector,"L2_2b10_medium_4L1J10"),
         L2_2b10_medium_4j25                       (&fDirector,"L2_2b10_medium_4j25"),
         L2_2b10_medium_L1JE100                    (&fDirector,"L2_2b10_medium_L1JE100"),
         L2_2b10_medium_L1JE140                    (&fDirector,"L2_2b10_medium_L1JE140"),
         L2_2b10_medium_L1_2J10J50                 (&fDirector,"L2_2b10_medium_L1_2J10J50"),
         L2_2b10_medium_j70_j25                    (&fDirector,"L2_2b10_medium_j70_j25"),
         L2_2b10_medium_j95_j25                    (&fDirector,"L2_2b10_medium_j95_j25"),
         L2_2b10_tight_4j25                        (&fDirector,"L2_2b10_tight_4j25"),
         L2_2b15_medium_3L1J15                     (&fDirector,"L2_2b15_medium_3L1J15"),
         L2_2b15_medium_3j35                       (&fDirector,"L2_2b15_medium_3j35"),
         L2_2b15_medium_j70_j35                    (&fDirector,"L2_2b15_medium_j70_j35"),
         L2_2b20_medium_3L1J20                     (&fDirector,"L2_2b20_medium_3L1J20"),
         L2_2b20_medium_3j40                       (&fDirector,"L2_2b20_medium_3j40"),
         L2_2b20_medium_j70_j40                    (&fDirector,"L2_2b20_medium_j70_j40"),
         L2_2fj25                                  (&fDirector,"L2_2fj25"),
         L2_2fj50                                  (&fDirector,"L2_2fj50"),
         L2_2fj70                                  (&fDirector,"L2_2fj70"),
         L2_2j25_j70_dphi03                        (&fDirector,"L2_2j25_j70_dphi03"),
         L2_2j40_anymct100_xe20_medium_noMu        (&fDirector,"L2_2j40_anymct100_xe20_medium_noMu"),
         L2_2j50_anymct100_xe20_medium_noMu        (&fDirector,"L2_2j50_anymct100_xe20_medium_noMu"),
         L2_3b10_loose_4L1J10                      (&fDirector,"L2_3b10_loose_4L1J10"),
         L2_3b10_medium_4j25                       (&fDirector,"L2_3b10_medium_4j25"),
         L2_3b15_loose_4L1J15                      (&fDirector,"L2_3b15_loose_4L1J15"),
         L2_3b15_medium_4j35                       (&fDirector,"L2_3b15_medium_4j35"),
         L2_3j25                                   (&fDirector,"L2_3j25"),
         L2_3j35                                   (&fDirector,"L2_3j35"),
         L2_3j40                                   (&fDirector,"L2_3j40"),
         L2_3j70                                   (&fDirector,"L2_3j70"),
         L2_3j95                                   (&fDirector,"L2_3j95"),
         L2_4j25                                   (&fDirector,"L2_4j25"),
         L2_4j35                                   (&fDirector,"L2_4j35"),
         L2_4j40                                   (&fDirector,"L2_4j40"),
         L2_4j50                                   (&fDirector,"L2_4j50"),
         L2_5j25                                   (&fDirector,"L2_5j25"),
         L2_5j35                                   (&fDirector,"L2_5j35"),
         L2_5j40                                   (&fDirector,"L2_5j40"),
         L2_6j25                                   (&fDirector,"L2_6j25"),
         L2_b10_IDTrkNoCut                         (&fDirector,"L2_b10_IDTrkNoCut"),
         L2_b10_L2Star_IDTrkNoCut                  (&fDirector,"L2_b10_L2Star_IDTrkNoCut"),
         L2_b10_medium_4L1J10                      (&fDirector,"L2_b10_medium_4L1J10"),
         L2_b10_medium_4j25                        (&fDirector,"L2_b10_medium_4j25"),
         L2_b10_medium_EFxe25_noMu_L1JE100         (&fDirector,"L2_b10_medium_EFxe25_noMu_L1JE100"),
         L2_b10_medium_EFxe25_noMu_L1JE140         (&fDirector,"L2_b10_medium_EFxe25_noMu_L1JE140"),
         L2_b10_medium_EFxe25_noMu_L1_2J10J50      (&fDirector,"L2_b10_medium_EFxe25_noMu_L1_2J10J50"),
         L2_b10_medium_j70_2j50_4j25               (&fDirector,"L2_b10_medium_j70_2j50_4j25"),
         L2_b10_tight_4L1J10                       (&fDirector,"L2_b10_tight_4L1J10"),
         L2_b10_tight_4j25                         (&fDirector,"L2_b10_tight_4j25"),
         L2_b10_tight_L1JE100                      (&fDirector,"L2_b10_tight_L1JE100"),
         L2_b10_tight_L1JE140                      (&fDirector,"L2_b10_tight_L1JE140"),
         L2_b10_tight_j70_2j50_4j25                (&fDirector,"L2_b10_tight_j70_2j50_4j25"),
         L2_b15_IDTrkNoCut                         (&fDirector,"L2_b15_IDTrkNoCut"),
         L2_b20_IDTrkNoCut                         (&fDirector,"L2_b20_IDTrkNoCut"),
         L2_fj10_empty_larcalib                    (&fDirector,"L2_fj10_empty_larcalib"),
         L2_fj25                                   (&fDirector,"L2_fj25"),
         L2_fj25_l2cleanph                         (&fDirector,"L2_fj25_l2cleanph"),
         L2_fj25_larcalib                          (&fDirector,"L2_fj25_larcalib"),
         L2_fj50                                   (&fDirector,"L2_fj50"),
         L2_fj50_larcalib                          (&fDirector,"L2_fj50_larcalib"),
         L2_fj70                                   (&fDirector,"L2_fj70"),
         L2_fj95                                   (&fDirector,"L2_fj95"),
         L2_j10_empty_larcalib                     (&fDirector,"L2_j10_empty_larcalib"),
         L2_j25                                    (&fDirector,"L2_j25"),
         L2_j25_cosmic                             (&fDirector,"L2_j25_cosmic"),
         L2_j25_firstempty                         (&fDirector,"L2_j25_firstempty"),
         L2_j25_fj25                               (&fDirector,"L2_j25_fj25"),
         L2_j25_l2cleanph                          (&fDirector,"L2_j25_l2cleanph"),
         L2_j25_larcalib                           (&fDirector,"L2_j25_larcalib"),
         L2_j30                                    (&fDirector,"L2_j30"),
         L2_j30_L1TAU_HV                           (&fDirector,"L2_j30_L1TAU_HV"),
         L2_j30_L1TAU_HV_cosmic                    (&fDirector,"L2_j30_L1TAU_HV_cosmic"),
         L2_j30_L1TAU_HV_firstempty                (&fDirector,"L2_j30_L1TAU_HV_firstempty"),
         L2_j30_L1TAU_HV_unpaired_iso              (&fDirector,"L2_j30_L1TAU_HV_unpaired_iso"),
         L2_j30_L1TAU_HV_unpaired_noniso           (&fDirector,"L2_j30_L1TAU_HV_unpaired_noniso"),
         L2_j30_L1TAU_HVtrk                        (&fDirector,"L2_j30_L1TAU_HVtrk"),
         L2_j30_L1TAU_HVtrk_cosmic                 (&fDirector,"L2_j30_L1TAU_HVtrk_cosmic"),
         L2_j30_L1TAU_HVtrk_firstempty             (&fDirector,"L2_j30_L1TAU_HVtrk_firstempty"),
         L2_j30_L1TAU_HVtrk_unpaired_iso           (&fDirector,"L2_j30_L1TAU_HVtrk_unpaired_iso"),
         L2_j30_L1TAU_HVtrk_unpaired_noniso        (&fDirector,"L2_j30_L1TAU_HVtrk_unpaired_noniso"),
         L2_j30_Trackless_HV                       (&fDirector,"L2_j30_Trackless_HV"),
         L2_j30_Trackless_HV_L1MU10                (&fDirector,"L2_j30_Trackless_HV_L1MU10"),
         L2_j30_Trackless_HV_cosmic                (&fDirector,"L2_j30_Trackless_HV_cosmic"),
         L2_j30_Trackless_HV_firstempty            (&fDirector,"L2_j30_Trackless_HV_firstempty"),
         L2_j30_Trackless_HV_unpaired_iso          (&fDirector,"L2_j30_Trackless_HV_unpaired_iso"),
         L2_j30_Trackless_HV_unpaired_noniso       (&fDirector,"L2_j30_Trackless_HV_unpaired_noniso"),
         L2_j30_eta13_a4tc_EFFS_EFxe30_noMu_empty  (&fDirector,"L2_j30_eta13_a4tc_EFFS_EFxe30_noMu_empty"),
         L2_j30_eta13_a4tc_EFFS_EFxe30_noMu_firstempty(&fDirector,"L2_j30_eta13_a4tc_EFFS_EFxe30_noMu_firstempty"),
         L2_j35                                    (&fDirector,"L2_j35"),
         L2_j35_fj35                               (&fDirector,"L2_j35_fj35"),
         L2_j40                                    (&fDirector,"L2_j40"),
         L2_j45                                    (&fDirector,"L2_j45"),
         L2_j45_cosmic                             (&fDirector,"L2_j45_cosmic"),
         L2_j45_firstempty                         (&fDirector,"L2_j45_firstempty"),
         L2_j50                                    (&fDirector,"L2_j50"),
         L2_j50_eta13_a4tc_EFFS_EFxe50_noMu_empty  (&fDirector,"L2_j50_eta13_a4tc_EFFS_EFxe50_noMu_empty"),
         L2_j50_eta13_a4tc_EFFS_EFxe50_noMu_firstempty(&fDirector,"L2_j50_eta13_a4tc_EFFS_EFxe50_noMu_firstempty"),
         L2_j50_eta25_a4tc_EFFS_EFxe50_noMu_empty  (&fDirector,"L2_j50_eta25_a4tc_EFFS_EFxe50_noMu_empty"),
         L2_j50_eta25_a4tc_EFFS_EFxe50_noMu_firstempty(&fDirector,"L2_j50_eta25_a4tc_EFFS_EFxe50_noMu_firstempty"),
         L2_j50_fj50                               (&fDirector,"L2_j50_fj50"),
         L2_j50_larcalib                           (&fDirector,"L2_j50_larcalib"),
         L2_j50_xe35_medium_noMu                   (&fDirector,"L2_j50_xe35_medium_noMu"),
         L2_j50_xe35_medium_noMu_l2cleancons       (&fDirector,"L2_j50_xe35_medium_noMu_l2cleancons"),
         L2_j60_xe45_noMu                          (&fDirector,"L2_j60_xe45_noMu"),
         L2_j70                                    (&fDirector,"L2_j70"),
         L2_j70_2j25                               (&fDirector,"L2_j70_2j25"),
         L2_j70_2j25_L2anymct100                   (&fDirector,"L2_j70_2j25_L2anymct100"),
         L2_j70_2j25_anymct100                     (&fDirector,"L2_j70_2j25_anymct100"),
         L2_j70_2j25_anymct150                     (&fDirector,"L2_j70_2j25_anymct150"),
         L2_j70_2j25_anymct175                     (&fDirector,"L2_j70_2j25_anymct175"),
         L2_j70_2j25_dphi04                        (&fDirector,"L2_j70_2j25_dphi04"),
         L2_j70_3j25                               (&fDirector,"L2_j70_3j25"),
         L2_j70_fj70                               (&fDirector,"L2_j70_fj70"),
         L2_j70_xe20_loose_noMu                    (&fDirector,"L2_j70_xe20_loose_noMu"),
         L2_j70_xe25_loose_noMu                    (&fDirector,"L2_j70_xe25_loose_noMu"),
         L2_j70_xe35_noMu                          (&fDirector,"L2_j70_xe35_noMu"),
         L2_j70_xe35_noMu_l2cleancons              (&fDirector,"L2_j70_xe35_noMu_l2cleancons"),
         L2_j70_xs25_noMu                          (&fDirector,"L2_j70_xs25_noMu"),
         L2_j75_xe40_noMu                          (&fDirector,"L2_j75_xe40_noMu"),
         L2_j95                                    (&fDirector,"L2_j95"),
         L2_j95_2j25_dphi04                        (&fDirector,"L2_j95_2j25_dphi04"),
         L2_j95_l2cleanph                          (&fDirector,"L2_j95_l2cleanph"),
         L2_j95_larcalib                           (&fDirector,"L2_j95_larcalib"),
         L2_je195                                  (&fDirector,"L2_je195"),
         L2_je255                                  (&fDirector,"L2_je255"),
         L2_je300                                  (&fDirector,"L2_je300"),
         L2_je350                                  (&fDirector,"L2_je350"),
         L2_je420                                  (&fDirector,"L2_je420"),
         L2_je500                                  (&fDirector,"L2_je500"),
         L2_mu4_L1J10_matched                      (&fDirector,"L2_mu4_L1J10_matched"),
         L2_mu4_L1J20_matched                      (&fDirector,"L2_mu4_L1J20_matched"),
         L2_mu4_L1J30_matched                      (&fDirector,"L2_mu4_L1J30_matched"),
         L2_mu4_L1J50_matched                      (&fDirector,"L2_mu4_L1J50_matched"),
         L2_mu4_L1J75_matched                      (&fDirector,"L2_mu4_L1J75_matched"),
         L2_mu4_j10_a4tc_EFFS                      (&fDirector,"L2_mu4_j10_a4tc_EFFS"),
         trig_Nav_n                                (&fDirector,"trig_Nav_n"),
         trig_Nav_chain_ChainId                    (&fDirector,"trig_Nav_chain_ChainId"),
         trig_Nav_chain_RoIType                    (&fDirector,"trig_Nav_chain_RoIType"),
         trig_Nav_chain_RoIIndex                   (&fDirector,"trig_Nav_chain_RoIIndex"),
         trig_RoI_L2_b_n                           (&fDirector,"trig_RoI_L2_b_n"),
         trig_RoI_L2_b_type                        (&fDirector,"trig_RoI_L2_b_type"),
         trig_RoI_L2_b_active                      (&fDirector,"trig_RoI_L2_b_active"),
         trig_RoI_L2_b_lastStep                    (&fDirector,"trig_RoI_L2_b_lastStep"),
         trig_RoI_L2_b_TENumber                    (&fDirector,"trig_RoI_L2_b_TENumber"),
         trig_RoI_L2_b_roiNumber                   (&fDirector,"trig_RoI_L2_b_roiNumber"),
         trig_RoI_L2_b_Jet_ROI                     (&fDirector,"trig_RoI_L2_b_Jet_ROI"),
         trig_RoI_L2_b_Jet_ROIStatus               (&fDirector,"trig_RoI_L2_b_Jet_ROIStatus"),
         trig_RoI_L2_b_Muon_ROI                    (&fDirector,"trig_RoI_L2_b_Muon_ROI"),
         trig_RoI_L2_b_Muon_ROIStatus              (&fDirector,"trig_RoI_L2_b_Muon_ROIStatus"),
         trig_RoI_L2_b_TrigL2BjetContainer         (&fDirector,"trig_RoI_L2_b_TrigL2BjetContainer"),
         trig_RoI_L2_b_TrigL2BjetContainerStatus   (&fDirector,"trig_RoI_L2_b_TrigL2BjetContainerStatus"),
         trig_RoI_L2_b_TrigInDetTrackCollection_TrigSiTrack_Jet(&fDirector,"trig_RoI_L2_b_TrigInDetTrackCollection_TrigSiTrack_Jet"),
         trig_RoI_L2_b_TrigInDetTrackCollection_TrigSiTrack_JetStatus(&fDirector,"trig_RoI_L2_b_TrigInDetTrackCollection_TrigSiTrack_JetStatus"),
         trig_RoI_L2_b_TrigInDetTrackCollection_TrigIDSCAN_Jet(&fDirector,"trig_RoI_L2_b_TrigInDetTrackCollection_TrigIDSCAN_Jet"),
         trig_RoI_L2_b_TrigInDetTrackCollection_TrigIDSCAN_JetStatus(&fDirector,"trig_RoI_L2_b_TrigInDetTrackCollection_TrigIDSCAN_JetStatus"),
         trig_RoI_EF_b_n                           (&fDirector,"trig_RoI_EF_b_n"),
         trig_RoI_EF_b_type                        (&fDirector,"trig_RoI_EF_b_type"),
         trig_RoI_EF_b_active                      (&fDirector,"trig_RoI_EF_b_active"),
         trig_RoI_EF_b_lastStep                    (&fDirector,"trig_RoI_EF_b_lastStep"),
         trig_RoI_EF_b_TENumber                    (&fDirector,"trig_RoI_EF_b_TENumber"),
         trig_RoI_EF_b_roiNumber                   (&fDirector,"trig_RoI_EF_b_roiNumber"),
         trig_RoI_EF_b_Jet_ROI                     (&fDirector,"trig_RoI_EF_b_Jet_ROI"),
         trig_RoI_EF_b_Jet_ROIStatus               (&fDirector,"trig_RoI_EF_b_Jet_ROIStatus"),
         trig_RoI_EF_b_Muon_ROI                    (&fDirector,"trig_RoI_EF_b_Muon_ROI"),
         trig_RoI_EF_b_Muon_ROIStatus              (&fDirector,"trig_RoI_EF_b_Muon_ROIStatus"),
         trig_RoI_EF_b_TrigEFBjetContainer         (&fDirector,"trig_RoI_EF_b_TrigEFBjetContainer"),
         trig_RoI_EF_b_TrigEFBjetContainerStatus   (&fDirector,"trig_RoI_EF_b_TrigEFBjetContainerStatus"),
         trig_RoI_EF_b_Rec__TrackParticleContainer (&fDirector,"trig_RoI_EF_b_Rec::TrackParticleContainer"),
         trig_RoI_EF_b_Rec__TrackParticleContainerStatus(&fDirector,"trig_RoI_EF_b_Rec::TrackParticleContainerStatus"),
         trig_L1_jet_n                             (&fDirector,"trig_L1_jet_n"),
         trig_L1_jet_eta                           (&fDirector,"trig_L1_jet_eta"),
         trig_L1_jet_phi                           (&fDirector,"trig_L1_jet_phi"),
         trig_L1_jet_thrNames                      (&fDirector,"trig_L1_jet_thrNames"),
         trig_L1_jet_thrValues                     (&fDirector,"trig_L1_jet_thrValues"),
         trig_L1_jet_thrPattern                    (&fDirector,"trig_L1_jet_thrPattern"),
         trig_L1_jet_et4x4                         (&fDirector,"trig_L1_jet_et4x4"),
         trig_L1_jet_et6x6                         (&fDirector,"trig_L1_jet_et6x6"),
         trig_L1_jet_et8x8                         (&fDirector,"trig_L1_jet_et8x8"),
         trig_L1_jet_RoIWord                       (&fDirector,"trig_L1_jet_RoIWord"),
         trig_L2_bjet_n                            (&fDirector,"trig_L2_bjet_n"),
         trig_L2_bjet_roiId                        (&fDirector,"trig_L2_bjet_roiId"),
         trig_L2_bjet_valid                        (&fDirector,"trig_L2_bjet_valid"),
         trig_L2_bjet_prmVtx                       (&fDirector,"trig_L2_bjet_prmVtx"),
         trig_L2_bjet_pt                           (&fDirector,"trig_L2_bjet_pt"),
         trig_L2_bjet_eta                          (&fDirector,"trig_L2_bjet_eta"),
         trig_L2_bjet_phi                          (&fDirector,"trig_L2_bjet_phi"),
         trig_L2_bjet_xComb                        (&fDirector,"trig_L2_bjet_xComb"),
         trig_L2_bjet_xIP1D                        (&fDirector,"trig_L2_bjet_xIP1D"),
         trig_L2_bjet_xIP2D                        (&fDirector,"trig_L2_bjet_xIP2D"),
         trig_L2_bjet_xIP3D                        (&fDirector,"trig_L2_bjet_xIP3D"),
         trig_L2_bjet_xCHI2                        (&fDirector,"trig_L2_bjet_xCHI2"),
         trig_L2_bjet_xSV                          (&fDirector,"trig_L2_bjet_xSV"),
         trig_L2_bjet_xMVtx                        (&fDirector,"trig_L2_bjet_xMVtx"),
         trig_L2_bjet_xEVtx                        (&fDirector,"trig_L2_bjet_xEVtx"),
         trig_L2_bjet_xNVtx                        (&fDirector,"trig_L2_bjet_xNVtx"),
         trig_L2_bjet_BSx                          (&fDirector,"trig_L2_bjet_BSx"),
         trig_L2_bjet_BSy                          (&fDirector,"trig_L2_bjet_BSy"),
         trig_L2_bjet_BSz                          (&fDirector,"trig_L2_bjet_BSz"),
         trig_L2_bjet_sBSx                         (&fDirector,"trig_L2_bjet_sBSx"),
         trig_L2_bjet_sBSy                         (&fDirector,"trig_L2_bjet_sBSy"),
         trig_L2_bjet_sBSz                         (&fDirector,"trig_L2_bjet_sBSz"),
         trig_L2_bjet_sBSxy                        (&fDirector,"trig_L2_bjet_sBSxy"),
         trig_L2_bjet_BTiltXZ                      (&fDirector,"trig_L2_bjet_BTiltXZ"),
         trig_L2_bjet_BTiltYZ                      (&fDirector,"trig_L2_bjet_BTiltYZ"),
         trig_L2_bjet_BSstatus                     (&fDirector,"trig_L2_bjet_BSstatus"),
         trig_EF_bjet_n                            (&fDirector,"trig_EF_bjet_n"),
         trig_EF_bjet_roiId                        (&fDirector,"trig_EF_bjet_roiId"),
         trig_EF_bjet_valid                        (&fDirector,"trig_EF_bjet_valid"),
         trig_EF_bjet_prmVtx                       (&fDirector,"trig_EF_bjet_prmVtx"),
         trig_EF_bjet_pt                           (&fDirector,"trig_EF_bjet_pt"),
         trig_EF_bjet_eta                          (&fDirector,"trig_EF_bjet_eta"),
         trig_EF_bjet_phi                          (&fDirector,"trig_EF_bjet_phi"),
         trig_EF_bjet_xComb                        (&fDirector,"trig_EF_bjet_xComb"),
         trig_EF_bjet_xIP1D                        (&fDirector,"trig_EF_bjet_xIP1D"),
         trig_EF_bjet_xIP2D                        (&fDirector,"trig_EF_bjet_xIP2D"),
         trig_EF_bjet_xIP3D                        (&fDirector,"trig_EF_bjet_xIP3D"),
         trig_EF_bjet_xCHI2                        (&fDirector,"trig_EF_bjet_xCHI2"),
         trig_EF_bjet_xSV                          (&fDirector,"trig_EF_bjet_xSV"),
         trig_EF_bjet_xMVtx                        (&fDirector,"trig_EF_bjet_xMVtx"),
         trig_EF_bjet_xEVtx                        (&fDirector,"trig_EF_bjet_xEVtx"),
         trig_EF_bjet_xNVtx                        (&fDirector,"trig_EF_bjet_xNVtx"),
         trig_EF_pv_n                              (&fDirector,"trig_EF_pv_n"),
         trig_EF_pv_x                              (&fDirector,"trig_EF_pv_x"),
         trig_EF_pv_y                              (&fDirector,"trig_EF_pv_y"),
         trig_EF_pv_z                              (&fDirector,"trig_EF_pv_z"),
         trig_EF_pv_type                           (&fDirector,"trig_EF_pv_type"),
         trig_EF_pv_err_x                          (&fDirector,"trig_EF_pv_err_x"),
         trig_EF_pv_err_y                          (&fDirector,"trig_EF_pv_err_y"),
         trig_EF_pv_err_z                          (&fDirector,"trig_EF_pv_err_z"),
         trig_L2_jet_n                             (&fDirector,"trig_L2_jet_n"),
         trig_L2_jet_E                             (&fDirector,"trig_L2_jet_E"),
         trig_L2_jet_eta                           (&fDirector,"trig_L2_jet_eta"),
         trig_L2_jet_phi                           (&fDirector,"trig_L2_jet_phi"),
         trig_L2_jet_RoIWord                       (&fDirector,"trig_L2_jet_RoIWord"),
         trig_L2_jet_ehad0                         (&fDirector,"trig_L2_jet_ehad0"),
         trig_L2_jet_eem0                          (&fDirector,"trig_L2_jet_eem0"),
         trig_L2_jet_nLeadingCells                 (&fDirector,"trig_L2_jet_nLeadingCells"),
         trig_L2_jet_hecf                          (&fDirector,"trig_L2_jet_hecf"),
         trig_L2_jet_jetQuality                    (&fDirector,"trig_L2_jet_jetQuality"),
         trig_L2_jet_emf                           (&fDirector,"trig_L2_jet_emf"),
         trig_L2_jet_jetTimeCells                  (&fDirector,"trig_L2_jet_jetTimeCells"),
         trig_L2_jet_L2_2fj25                      (&fDirector,"trig_L2_jet_L2_2fj25"),
         trig_L2_jet_L2_2fj50                      (&fDirector,"trig_L2_jet_L2_2fj50"),
         trig_L2_jet_L2_2fj70                      (&fDirector,"trig_L2_jet_L2_2fj70"),
         trig_L2_jet_L2_2j25_j70_dphi03            (&fDirector,"trig_L2_jet_L2_2j25_j70_dphi03"),
         trig_L2_jet_L2_2j40_anymct100_xe20_medium_noMu(&fDirector,"trig_L2_jet_L2_2j40_anymct100_xe20_medium_noMu"),
         trig_L2_jet_L2_2j50_anymct100_xe20_medium_noMu(&fDirector,"trig_L2_jet_L2_2j50_anymct100_xe20_medium_noMu"),
         trig_L2_jet_L2_3j25                       (&fDirector,"trig_L2_jet_L2_3j25"),
         trig_L2_jet_L2_3j35                       (&fDirector,"trig_L2_jet_L2_3j35"),
         trig_L2_jet_L2_3j40                       (&fDirector,"trig_L2_jet_L2_3j40"),
         trig_L2_jet_L2_3j70                       (&fDirector,"trig_L2_jet_L2_3j70"),
         trig_L2_jet_L2_3j95                       (&fDirector,"trig_L2_jet_L2_3j95"),
         trig_L2_jet_L2_4j25                       (&fDirector,"trig_L2_jet_L2_4j25"),
         trig_L2_jet_L2_4j35                       (&fDirector,"trig_L2_jet_L2_4j35"),
         trig_L2_jet_L2_4j40                       (&fDirector,"trig_L2_jet_L2_4j40"),
         trig_L2_jet_L2_4j50                       (&fDirector,"trig_L2_jet_L2_4j50"),
         trig_L2_jet_L2_5j25                       (&fDirector,"trig_L2_jet_L2_5j25"),
         trig_L2_jet_L2_5j35                       (&fDirector,"trig_L2_jet_L2_5j35"),
         trig_L2_jet_L2_5j40                       (&fDirector,"trig_L2_jet_L2_5j40"),
         trig_L2_jet_L2_6j25                       (&fDirector,"trig_L2_jet_L2_6j25"),
         trig_L2_jet_L2_fj10_empty_larcalib        (&fDirector,"trig_L2_jet_L2_fj10_empty_larcalib"),
         trig_L2_jet_L2_fj25                       (&fDirector,"trig_L2_jet_L2_fj25"),
         trig_L2_jet_L2_fj25_l2cleanph             (&fDirector,"trig_L2_jet_L2_fj25_l2cleanph"),
         trig_L2_jet_L2_fj25_larcalib              (&fDirector,"trig_L2_jet_L2_fj25_larcalib"),
         trig_L2_jet_L2_fj50                       (&fDirector,"trig_L2_jet_L2_fj50"),
         trig_L2_jet_L2_fj50_larcalib              (&fDirector,"trig_L2_jet_L2_fj50_larcalib"),
         trig_L2_jet_L2_fj70                       (&fDirector,"trig_L2_jet_L2_fj70"),
         trig_L2_jet_L2_fj95                       (&fDirector,"trig_L2_jet_L2_fj95"),
         trig_L2_jet_L2_j10_empty_larcalib         (&fDirector,"trig_L2_jet_L2_j10_empty_larcalib"),
         trig_L2_jet_L2_j25                        (&fDirector,"trig_L2_jet_L2_j25"),
         trig_L2_jet_L2_j25_cosmic                 (&fDirector,"trig_L2_jet_L2_j25_cosmic"),
         trig_L2_jet_L2_j25_firstempty             (&fDirector,"trig_L2_jet_L2_j25_firstempty"),
         trig_L2_jet_L2_j25_fj25                   (&fDirector,"trig_L2_jet_L2_j25_fj25"),
         trig_L2_jet_L2_j25_l2cleanph              (&fDirector,"trig_L2_jet_L2_j25_l2cleanph"),
         trig_L2_jet_L2_j25_larcalib               (&fDirector,"trig_L2_jet_L2_j25_larcalib"),
         trig_L2_jet_L2_j30                        (&fDirector,"trig_L2_jet_L2_j30"),
         trig_L2_jet_L2_j30_L1TAU_HV               (&fDirector,"trig_L2_jet_L2_j30_L1TAU_HV"),
         trig_L2_jet_L2_j30_L1TAU_HV_cosmic        (&fDirector,"trig_L2_jet_L2_j30_L1TAU_HV_cosmic"),
         trig_L2_jet_L2_j30_L1TAU_HV_firstempty    (&fDirector,"trig_L2_jet_L2_j30_L1TAU_HV_firstempty"),
         trig_L2_jet_L2_j30_L1TAU_HV_unpaired_iso  (&fDirector,"trig_L2_jet_L2_j30_L1TAU_HV_unpaired_iso"),
         trig_L2_jet_L2_j30_L1TAU_HV_unpaired_noniso(&fDirector,"trig_L2_jet_L2_j30_L1TAU_HV_unpaired_noniso"),
         trig_L2_jet_L2_j30_L1TAU_HVtrk            (&fDirector,"trig_L2_jet_L2_j30_L1TAU_HVtrk"),
         trig_L2_jet_L2_j30_L1TAU_HVtrk_cosmic     (&fDirector,"trig_L2_jet_L2_j30_L1TAU_HVtrk_cosmic"),
         trig_L2_jet_L2_j30_L1TAU_HVtrk_firstempty (&fDirector,"trig_L2_jet_L2_j30_L1TAU_HVtrk_firstempty"),
         trig_L2_jet_L2_j30_L1TAU_HVtrk_unpaired_iso(&fDirector,"trig_L2_jet_L2_j30_L1TAU_HVtrk_unpaired_iso"),
         trig_L2_jet_L2_j30_L1TAU_HVtrk_unpaired_noniso(&fDirector,"trig_L2_jet_L2_j30_L1TAU_HVtrk_unpaired_noniso"),
         trig_L2_jet_L2_j30_Trackless_HV           (&fDirector,"trig_L2_jet_L2_j30_Trackless_HV"),
         trig_L2_jet_L2_j30_Trackless_HV_L1MU10    (&fDirector,"trig_L2_jet_L2_j30_Trackless_HV_L1MU10"),
         trig_L2_jet_L2_j30_Trackless_HV_cosmic    (&fDirector,"trig_L2_jet_L2_j30_Trackless_HV_cosmic"),
         trig_L2_jet_L2_j30_Trackless_HV_firstempty(&fDirector,"trig_L2_jet_L2_j30_Trackless_HV_firstempty"),
         trig_L2_jet_L2_j30_Trackless_HV_unpaired_iso(&fDirector,"trig_L2_jet_L2_j30_Trackless_HV_unpaired_iso"),
         trig_L2_jet_L2_j30_Trackless_HV_unpaired_noniso(&fDirector,"trig_L2_jet_L2_j30_Trackless_HV_unpaired_noniso"),
         trig_L2_jet_L2_j30_eta13_a4tc_EFFS_EFxe30_noMu_empty(&fDirector,"trig_L2_jet_L2_j30_eta13_a4tc_EFFS_EFxe30_noMu_empty"),
         trig_L2_jet_L2_j30_eta13_a4tc_EFFS_EFxe30_noMu_firstempty(&fDirector,"trig_L2_jet_L2_j30_eta13_a4tc_EFFS_EFxe30_noMu_firstempty"),
         trig_L2_jet_L2_j35                        (&fDirector,"trig_L2_jet_L2_j35"),
         trig_L2_jet_L2_j35_fj35                   (&fDirector,"trig_L2_jet_L2_j35_fj35"),
         trig_L2_jet_L2_j40                        (&fDirector,"trig_L2_jet_L2_j40"),
         trig_L2_jet_L2_j45                        (&fDirector,"trig_L2_jet_L2_j45"),
         trig_L2_jet_L2_j45_cosmic                 (&fDirector,"trig_L2_jet_L2_j45_cosmic"),
         trig_L2_jet_L2_j45_firstempty             (&fDirector,"trig_L2_jet_L2_j45_firstempty"),
         trig_L2_jet_L2_j50                        (&fDirector,"trig_L2_jet_L2_j50"),
         trig_L2_jet_L2_j50_eta13_a4tc_EFFS_EFxe50_noMu_empty(&fDirector,"trig_L2_jet_L2_j50_eta13_a4tc_EFFS_EFxe50_noMu_empty"),
         trig_L2_jet_L2_j50_eta13_a4tc_EFFS_EFxe50_noMu_firstempty(&fDirector,"trig_L2_jet_L2_j50_eta13_a4tc_EFFS_EFxe50_noMu_firstempty"),
         trig_L2_jet_L2_j50_eta25_a4tc_EFFS_EFxe50_noMu_empty(&fDirector,"trig_L2_jet_L2_j50_eta25_a4tc_EFFS_EFxe50_noMu_empty"),
         trig_L2_jet_L2_j50_eta25_a4tc_EFFS_EFxe50_noMu_firstempty(&fDirector,"trig_L2_jet_L2_j50_eta25_a4tc_EFFS_EFxe50_noMu_firstempty"),
         trig_L2_jet_L2_j50_fj50                   (&fDirector,"trig_L2_jet_L2_j50_fj50"),
         trig_L2_jet_L2_j50_larcalib               (&fDirector,"trig_L2_jet_L2_j50_larcalib"),
         trig_L2_jet_L2_j50_xe35_medium_noMu       (&fDirector,"trig_L2_jet_L2_j50_xe35_medium_noMu"),
         trig_L2_jet_L2_j50_xe35_medium_noMu_l2cleancons(&fDirector,"trig_L2_jet_L2_j50_xe35_medium_noMu_l2cleancons"),
         trig_L2_jet_L2_j60_xe45_noMu              (&fDirector,"trig_L2_jet_L2_j60_xe45_noMu"),
         trig_L2_jet_L2_j70                        (&fDirector,"trig_L2_jet_L2_j70"),
         trig_L2_jet_L2_j70_2j25                   (&fDirector,"trig_L2_jet_L2_j70_2j25"),
         trig_L2_jet_L2_j70_2j25_L2anymct100       (&fDirector,"trig_L2_jet_L2_j70_2j25_L2anymct100"),
         trig_L2_jet_L2_j70_2j25_anymct100         (&fDirector,"trig_L2_jet_L2_j70_2j25_anymct100"),
         trig_L2_jet_L2_j70_2j25_anymct150         (&fDirector,"trig_L2_jet_L2_j70_2j25_anymct150"),
         trig_L2_jet_L2_j70_2j25_anymct175         (&fDirector,"trig_L2_jet_L2_j70_2j25_anymct175"),
         trig_L2_jet_L2_j70_2j25_dphi04            (&fDirector,"trig_L2_jet_L2_j70_2j25_dphi04"),
         trig_L2_jet_L2_j70_3j25                   (&fDirector,"trig_L2_jet_L2_j70_3j25"),
         trig_L2_jet_L2_j70_fj70                   (&fDirector,"trig_L2_jet_L2_j70_fj70"),
         trig_L2_jet_L2_j70_xe20_loose_noMu        (&fDirector,"trig_L2_jet_L2_j70_xe20_loose_noMu"),
         trig_L2_jet_L2_j70_xe25_loose_noMu        (&fDirector,"trig_L2_jet_L2_j70_xe25_loose_noMu"),
         trig_L2_jet_L2_j70_xe35_noMu              (&fDirector,"trig_L2_jet_L2_j70_xe35_noMu"),
         trig_L2_jet_L2_j70_xe35_noMu_l2cleancons  (&fDirector,"trig_L2_jet_L2_j70_xe35_noMu_l2cleancons"),
         trig_L2_jet_L2_j70_xs25_noMu              (&fDirector,"trig_L2_jet_L2_j70_xs25_noMu"),
         trig_L2_jet_L2_j75_xe40_noMu              (&fDirector,"trig_L2_jet_L2_j75_xe40_noMu"),
         trig_L2_jet_L2_j95                        (&fDirector,"trig_L2_jet_L2_j95"),
         trig_L2_jet_L2_j95_2j25_dphi04            (&fDirector,"trig_L2_jet_L2_j95_2j25_dphi04"),
         trig_L2_jet_L2_j95_l2cleanph              (&fDirector,"trig_L2_jet_L2_j95_l2cleanph"),
         trig_L2_jet_L2_j95_larcalib               (&fDirector,"trig_L2_jet_L2_j95_larcalib"),
         trig_L2_jet_L2_je195                      (&fDirector,"trig_L2_jet_L2_je195"),
         trig_L2_jet_L2_je255                      (&fDirector,"trig_L2_jet_L2_je255"),
         trig_L2_jet_L2_je300                      (&fDirector,"trig_L2_jet_L2_je300"),
         trig_L2_jet_L2_je350                      (&fDirector,"trig_L2_jet_L2_je350"),
         trig_L2_jet_L2_je420                      (&fDirector,"trig_L2_jet_L2_je420"),
         trig_L2_jet_L2_je500                      (&fDirector,"trig_L2_jet_L2_je500"),
         trig_EF_jet_n                             (&fDirector,"trig_EF_jet_n"),
         trig_EF_jet_emscale_E                     (&fDirector,"trig_EF_jet_emscale_E"),
         trig_EF_jet_emscale_pt                    (&fDirector,"trig_EF_jet_emscale_pt"),
         trig_EF_jet_emscale_m                     (&fDirector,"trig_EF_jet_emscale_m"),
         trig_EF_jet_emscale_eta                   (&fDirector,"trig_EF_jet_emscale_eta"),
         trig_EF_jet_emscale_phi                   (&fDirector,"trig_EF_jet_emscale_phi"),
         trig_EF_jet_a4                            (&fDirector,"trig_EF_jet_a4"),
         trig_EF_jet_a4tc                          (&fDirector,"trig_EF_jet_a4tc"),
         trig_EF_jet_a10tc                         (&fDirector,"trig_EF_jet_a10tc"),
         trig_EF_jet_a6                            (&fDirector,"trig_EF_jet_a6"),
         trig_EF_jet_a6tc                          (&fDirector,"trig_EF_jet_a6tc"),
         trig_EF_jet_RoIword                       (&fDirector,"trig_EF_jet_RoIword"),
         trig_EF_jet_EF_2fj100_a4tc_EFFS_deta50_FB (&fDirector,"trig_EF_jet_EF_2fj100_a4tc_EFFS_deta50_FB"),
         trig_EF_jet_EF_2fj30_a4tc_EFFS_deta50_FB  (&fDirector,"trig_EF_jet_EF_2fj30_a4tc_EFFS_deta50_FB"),
         trig_EF_jet_EF_2fj30_a4tc_EFFS_deta50_FC  (&fDirector,"trig_EF_jet_EF_2fj30_a4tc_EFFS_deta50_FC"),
         trig_EF_jet_EF_2fj55_a4tc_EFFS_deta50_FB  (&fDirector,"trig_EF_jet_EF_2fj55_a4tc_EFFS_deta50_FB"),
         trig_EF_jet_EF_2fj55_a4tc_EFFS_deta50_FC  (&fDirector,"trig_EF_jet_EF_2fj55_a4tc_EFFS_deta50_FC"),
         trig_EF_jet_EF_2fj75_a4tc_EFFS_deta50_FB  (&fDirector,"trig_EF_jet_EF_2fj75_a4tc_EFFS_deta50_FB"),
         trig_EF_jet_EF_2fj75_a4tc_EFFS_deta50_FC  (&fDirector,"trig_EF_jet_EF_2fj75_a4tc_EFFS_deta50_FC"),
         trig_EF_jet_EF_2j100_a4tc_EFFS_deta35_FC  (&fDirector,"trig_EF_jet_EF_2j100_a4tc_EFFS_deta35_FC"),
         trig_EF_jet_EF_2j135_a4tc_EFFS_deta35_FC  (&fDirector,"trig_EF_jet_EF_2j135_a4tc_EFFS_deta35_FC"),
         trig_EF_jet_EF_2j180_a4tc_EFFS_deta35_FC  (&fDirector,"trig_EF_jet_EF_2j180_a4tc_EFFS_deta35_FC"),
         trig_EF_jet_EF_2j240_a4tc_EFFS_deta35_FC  (&fDirector,"trig_EF_jet_EF_2j240_a4tc_EFFS_deta35_FC"),
         trig_EF_jet_EF_2j45_a4tc_EFFS_leadingmct100_xe40_medium_noMu(&fDirector,"trig_EF_jet_EF_2j45_a4tc_EFFS_leadingmct100_xe40_medium_noMu"),
         trig_EF_jet_EF_2j55_a4tc_EFFS_leadingmct100_xe40_medium_noMu(&fDirector,"trig_EF_jet_EF_2j55_a4tc_EFFS_leadingmct100_xe40_medium_noMu"),
         trig_EF_jet_EF_3j100_a4tc_EFFS            (&fDirector,"trig_EF_jet_EF_3j100_a4tc_EFFS"),
         trig_EF_jet_EF_3j100_a4tc_EFFS_L1J75      (&fDirector,"trig_EF_jet_EF_3j100_a4tc_EFFS_L1J75"),
         trig_EF_jet_EF_3j30_a4tc_EFFS             (&fDirector,"trig_EF_jet_EF_3j30_a4tc_EFFS"),
         trig_EF_jet_EF_3j40_a4tc_EFFS             (&fDirector,"trig_EF_jet_EF_3j40_a4tc_EFFS"),
         trig_EF_jet_EF_3j45_a4tc_EFFS             (&fDirector,"trig_EF_jet_EF_3j45_a4tc_EFFS"),
         trig_EF_jet_EF_3j75_a4tc_EFFS             (&fDirector,"trig_EF_jet_EF_3j75_a4tc_EFFS"),
         trig_EF_jet_EF_4j30_a4tc_EFFS             (&fDirector,"trig_EF_jet_EF_4j30_a4tc_EFFS"),
         trig_EF_jet_EF_4j40_a4tc_EFFS             (&fDirector,"trig_EF_jet_EF_4j40_a4tc_EFFS"),
         trig_EF_jet_EF_4j40_a4tc_EFFS_ht350       (&fDirector,"trig_EF_jet_EF_4j40_a4tc_EFFS_ht350"),
         trig_EF_jet_EF_4j40_a4tc_EFFS_ht400       (&fDirector,"trig_EF_jet_EF_4j40_a4tc_EFFS_ht400"),
         trig_EF_jet_EF_4j45_a4tc_EFFS             (&fDirector,"trig_EF_jet_EF_4j45_a4tc_EFFS"),
         trig_EF_jet_EF_4j55_a4tc_EFFS             (&fDirector,"trig_EF_jet_EF_4j55_a4tc_EFFS"),
         trig_EF_jet_EF_4j60_a4tc_EFFS             (&fDirector,"trig_EF_jet_EF_4j60_a4tc_EFFS"),
         trig_EF_jet_EF_5j30_a4tc_EFFS             (&fDirector,"trig_EF_jet_EF_5j30_a4tc_EFFS"),
         trig_EF_jet_EF_5j40_a4tc_EFFS             (&fDirector,"trig_EF_jet_EF_5j40_a4tc_EFFS"),
         trig_EF_jet_EF_5j45_a4tc_EFFS             (&fDirector,"trig_EF_jet_EF_5j45_a4tc_EFFS"),
         trig_EF_jet_EF_6j30_a4tc_EFFS             (&fDirector,"trig_EF_jet_EF_6j30_a4tc_EFFS"),
         trig_EF_jet_EF_6j30_a4tc_EFFS_L15J10      (&fDirector,"trig_EF_jet_EF_6j30_a4tc_EFFS_L15J10"),
         trig_EF_jet_EF_6j40_a4tc_EFFS             (&fDirector,"trig_EF_jet_EF_6j40_a4tc_EFFS"),
         trig_EF_jet_EF_6j45_a4tc_EFFS             (&fDirector,"trig_EF_jet_EF_6j45_a4tc_EFFS"),
         trig_EF_jet_EF_7j30_a4tc_EFFS_L15J10      (&fDirector,"trig_EF_jet_EF_7j30_a4tc_EFFS_L15J10"),
         trig_EF_jet_EF_7j30_a4tc_EFFS_L16J10      (&fDirector,"trig_EF_jet_EF_7j30_a4tc_EFFS_L16J10"),
         trig_EF_jet_EF_fj100_a4tc_EFFS            (&fDirector,"trig_EF_jet_EF_fj100_a4tc_EFFS"),
         trig_EF_jet_EF_fj10_a4tc_EFFS             (&fDirector,"trig_EF_jet_EF_fj10_a4tc_EFFS"),
         trig_EF_jet_EF_fj10_a4tc_EFFS_1vx         (&fDirector,"trig_EF_jet_EF_fj10_a4tc_EFFS_1vx"),
         trig_EF_jet_EF_fj135_a4tc_EFFS            (&fDirector,"trig_EF_jet_EF_fj135_a4tc_EFFS"),
         trig_EF_jet_EF_fj15_a4tc_EFFS             (&fDirector,"trig_EF_jet_EF_fj15_a4tc_EFFS"),
         trig_EF_jet_EF_fj20_a4tc_EFFS             (&fDirector,"trig_EF_jet_EF_fj20_a4tc_EFFS"),
         trig_EF_jet_EF_fj30_a4tc_EFFS             (&fDirector,"trig_EF_jet_EF_fj30_a4tc_EFFS"),
         trig_EF_jet_EF_fj30_a4tc_EFFS_l2cleanph   (&fDirector,"trig_EF_jet_EF_fj30_a4tc_EFFS_l2cleanph"),
         trig_EF_jet_EF_fj55_a4tc_EFFS             (&fDirector,"trig_EF_jet_EF_fj55_a4tc_EFFS"),
         trig_EF_jet_EF_fj75_a4tc_EFFS             (&fDirector,"trig_EF_jet_EF_fj75_a4tc_EFFS"),
         trig_EF_jet_EF_j100_a4tc_EFFS             (&fDirector,"trig_EF_jet_EF_j100_a4tc_EFFS"),
         trig_EF_jet_EF_j100_a4tc_EFFS_ht350       (&fDirector,"trig_EF_jet_EF_j100_a4tc_EFFS_ht350"),
         trig_EF_jet_EF_j100_a4tc_EFFS_ht400       (&fDirector,"trig_EF_jet_EF_j100_a4tc_EFFS_ht400"),
         trig_EF_jet_EF_j100_a4tc_EFFS_ht500       (&fDirector,"trig_EF_jet_EF_j100_a4tc_EFFS_ht500"),
         trig_EF_jet_EF_j100_j30_a4tc_EFFS_L2dphi04(&fDirector,"trig_EF_jet_EF_j100_j30_a4tc_EFFS_L2dphi04"),
         trig_EF_jet_EF_j10_a4tc_EFFS              (&fDirector,"trig_EF_jet_EF_j10_a4tc_EFFS"),
         trig_EF_jet_EF_j10_a4tc_EFFS_1vx          (&fDirector,"trig_EF_jet_EF_j10_a4tc_EFFS_1vx"),
         trig_EF_jet_EF_j135_a4tc_EFFS             (&fDirector,"trig_EF_jet_EF_j135_a4tc_EFFS"),
         trig_EF_jet_EF_j135_a4tc_EFFS_ht500       (&fDirector,"trig_EF_jet_EF_j135_a4tc_EFFS_ht500"),
         trig_EF_jet_EF_j135_j30_a4tc_EFFS_L2dphi04(&fDirector,"trig_EF_jet_EF_j135_j30_a4tc_EFFS_L2dphi04"),
         trig_EF_jet_EF_j135_j30_a4tc_EFFS_dphi04  (&fDirector,"trig_EF_jet_EF_j135_j30_a4tc_EFFS_dphi04"),
         trig_EF_jet_EF_j15_a4tc_EFFS              (&fDirector,"trig_EF_jet_EF_j15_a4tc_EFFS"),
         trig_EF_jet_EF_j180_a4tc_EFFS             (&fDirector,"trig_EF_jet_EF_j180_a4tc_EFFS"),
         trig_EF_jet_EF_j180_j30_a4tc_EFFS_dphi04  (&fDirector,"trig_EF_jet_EF_j180_j30_a4tc_EFFS_dphi04"),
         trig_EF_jet_EF_j20_a4tc_EFFS              (&fDirector,"trig_EF_jet_EF_j20_a4tc_EFFS"),
         trig_EF_jet_EF_j240_a10tc_EFFS            (&fDirector,"trig_EF_jet_EF_j240_a10tc_EFFS"),
         trig_EF_jet_EF_j240_a4tc_EFFS             (&fDirector,"trig_EF_jet_EF_j240_a4tc_EFFS"),
         trig_EF_jet_EF_j240_a4tc_EFFS_l2cleanph   (&fDirector,"trig_EF_jet_EF_j240_a4tc_EFFS_l2cleanph"),
         trig_EF_jet_EF_j30_a4tc_EFFS              (&fDirector,"trig_EF_jet_EF_j30_a4tc_EFFS"),
         trig_EF_jet_EF_j30_a4tc_EFFS_l2cleanph    (&fDirector,"trig_EF_jet_EF_j30_a4tc_EFFS_l2cleanph"),
         trig_EF_jet_EF_j30_cosmic                 (&fDirector,"trig_EF_jet_EF_j30_cosmic"),
         trig_EF_jet_EF_j30_eta13_a4tc_EFFS_EFxe30_noMu_empty(&fDirector,"trig_EF_jet_EF_j30_eta13_a4tc_EFFS_EFxe30_noMu_empty"),
         trig_EF_jet_EF_j30_eta13_a4tc_EFFS_EFxe30_noMu_firstempty(&fDirector,"trig_EF_jet_EF_j30_eta13_a4tc_EFFS_EFxe30_noMu_firstempty"),
         trig_EF_jet_EF_j30_firstempty             (&fDirector,"trig_EF_jet_EF_j30_firstempty"),
         trig_EF_jet_EF_j30_fj30_a4tc_EFFS         (&fDirector,"trig_EF_jet_EF_j30_fj30_a4tc_EFFS"),
         trig_EF_jet_EF_j320_a10tc_EFFS            (&fDirector,"trig_EF_jet_EF_j320_a10tc_EFFS"),
         trig_EF_jet_EF_j320_a4tc_EFFS             (&fDirector,"trig_EF_jet_EF_j320_a4tc_EFFS"),
         trig_EF_jet_EF_j35_a4tc_EFFS              (&fDirector,"trig_EF_jet_EF_j35_a4tc_EFFS"),
         trig_EF_jet_EF_j35_a4tc_EFFS_L1TAU_HV     (&fDirector,"trig_EF_jet_EF_j35_a4tc_EFFS_L1TAU_HV"),
         trig_EF_jet_EF_j35_a4tc_EFFS_L1TAU_HV_cosmic(&fDirector,"trig_EF_jet_EF_j35_a4tc_EFFS_L1TAU_HV_cosmic"),
         trig_EF_jet_EF_j35_a4tc_EFFS_L1TAU_HV_firstempty(&fDirector,"trig_EF_jet_EF_j35_a4tc_EFFS_L1TAU_HV_firstempty"),
         trig_EF_jet_EF_j35_a4tc_EFFS_L1TAU_HV_unpaired_iso(&fDirector,"trig_EF_jet_EF_j35_a4tc_EFFS_L1TAU_HV_unpaired_iso"),
         trig_EF_jet_EF_j35_a4tc_EFFS_L1TAU_HV_unpaired_noniso(&fDirector,"trig_EF_jet_EF_j35_a4tc_EFFS_L1TAU_HV_unpaired_noniso"),
         trig_EF_jet_EF_j35_a4tc_EFFS_L1TAU_HVtrk  (&fDirector,"trig_EF_jet_EF_j35_a4tc_EFFS_L1TAU_HVtrk"),
         trig_EF_jet_EF_j35_a4tc_EFFS_L1TAU_HVtrk_cosmic(&fDirector,"trig_EF_jet_EF_j35_a4tc_EFFS_L1TAU_HVtrk_cosmic"),
         trig_EF_jet_EF_j35_a4tc_EFFS_L1TAU_HVtrk_firstempty(&fDirector,"trig_EF_jet_EF_j35_a4tc_EFFS_L1TAU_HVtrk_firstempty"),
         trig_EF_jet_EF_j35_a4tc_EFFS_L1TAU_HVtrk_unpaired_iso(&fDirector,"trig_EF_jet_EF_j35_a4tc_EFFS_L1TAU_HVtrk_unpaired_iso"),
         trig_EF_jet_EF_j35_a4tc_EFFS_L1TAU_HVtrk_unpaired_noniso(&fDirector,"trig_EF_jet_EF_j35_a4tc_EFFS_L1TAU_HVtrk_unpaired_noniso"),
         trig_EF_jet_EF_j40_a4tc_EFFS              (&fDirector,"trig_EF_jet_EF_j40_a4tc_EFFS"),
         trig_EF_jet_EF_j40_fj40_a4tc_EFFS         (&fDirector,"trig_EF_jet_EF_j40_fj40_a4tc_EFFS"),
         trig_EF_jet_EF_j425_a10tc_EFFS            (&fDirector,"trig_EF_jet_EF_j425_a10tc_EFFS"),
         trig_EF_jet_EF_j425_a4tc_EFFS             (&fDirector,"trig_EF_jet_EF_j425_a4tc_EFFS"),
         trig_EF_jet_EF_j45_a4tc_EFFS              (&fDirector,"trig_EF_jet_EF_j45_a4tc_EFFS"),
         trig_EF_jet_EF_j50_a4tc_EFFS              (&fDirector,"trig_EF_jet_EF_j50_a4tc_EFFS"),
         trig_EF_jet_EF_j50_cosmic                 (&fDirector,"trig_EF_jet_EF_j50_cosmic"),
         trig_EF_jet_EF_j50_eta13_a4tc_EFFS_EFxe50_noMu_empty(&fDirector,"trig_EF_jet_EF_j50_eta13_a4tc_EFFS_EFxe50_noMu_empty"),
         trig_EF_jet_EF_j50_eta13_a4tc_EFFS_EFxe50_noMu_firstempty(&fDirector,"trig_EF_jet_EF_j50_eta13_a4tc_EFFS_EFxe50_noMu_firstempty"),
         trig_EF_jet_EF_j50_eta25_a4tc_EFFS_EFxe50_noMu_empty(&fDirector,"trig_EF_jet_EF_j50_eta25_a4tc_EFFS_EFxe50_noMu_empty"),
         trig_EF_jet_EF_j50_eta25_a4tc_EFFS_EFxe50_noMu_firstempty(&fDirector,"trig_EF_jet_EF_j50_eta25_a4tc_EFFS_EFxe50_noMu_firstempty"),
         trig_EF_jet_EF_j50_firstempty             (&fDirector,"trig_EF_jet_EF_j50_firstempty"),
         trig_EF_jet_EF_j55_a4tc_EFFS              (&fDirector,"trig_EF_jet_EF_j55_a4tc_EFFS"),
         trig_EF_jet_EF_j55_a4tc_EFFS_xe55_medium_noMu_dphi2j30xe10(&fDirector,"trig_EF_jet_EF_j55_a4tc_EFFS_xe55_medium_noMu_dphi2j30xe10"),
         trig_EF_jet_EF_j55_a4tc_EFFS_xe55_medium_noMu_dphi2j30xe10_l2cleancons(&fDirector,"trig_EF_jet_EF_j55_a4tc_EFFS_xe55_medium_noMu_dphi2j30xe10_l2cleancons"),
         trig_EF_jet_EF_j55_fj55_a4tc_EFFS         (&fDirector,"trig_EF_jet_EF_j55_fj55_a4tc_EFFS"),
         trig_EF_jet_EF_j65_a4tc_EFFS_xe65_noMu_dphi2j30xe10(&fDirector,"trig_EF_jet_EF_j65_a4tc_EFFS_xe65_noMu_dphi2j30xe10"),
         trig_EF_jet_EF_j70_j25_dphi03_NoEF        (&fDirector,"trig_EF_jet_EF_j70_j25_dphi03_NoEF"),
         trig_EF_jet_EF_j75_2j30_a4tc_EFFS_ht350   (&fDirector,"trig_EF_jet_EF_j75_2j30_a4tc_EFFS_ht350"),
         trig_EF_jet_EF_j75_a4tc_EFFS              (&fDirector,"trig_EF_jet_EF_j75_a4tc_EFFS"),
         trig_EF_jet_EF_j75_a4tc_EFFS_xe40_loose_noMu(&fDirector,"trig_EF_jet_EF_j75_a4tc_EFFS_xe40_loose_noMu"),
         trig_EF_jet_EF_j75_a4tc_EFFS_xe40_loose_noMu_dphijxe03(&fDirector,"trig_EF_jet_EF_j75_a4tc_EFFS_xe40_loose_noMu_dphijxe03"),
         trig_EF_jet_EF_j75_a4tc_EFFS_xe45_loose_noMu(&fDirector,"trig_EF_jet_EF_j75_a4tc_EFFS_xe45_loose_noMu"),
         trig_EF_jet_EF_j75_a4tc_EFFS_xe55_loose_noMu(&fDirector,"trig_EF_jet_EF_j75_a4tc_EFFS_xe55_loose_noMu"),
         trig_EF_jet_EF_j75_a4tc_EFFS_xe55_noMu    (&fDirector,"trig_EF_jet_EF_j75_a4tc_EFFS_xe55_noMu"),
         trig_EF_jet_EF_j75_a4tc_EFFS_xe55_noMu_l2cleancons(&fDirector,"trig_EF_jet_EF_j75_a4tc_EFFS_xe55_noMu_l2cleancons"),
         trig_EF_jet_EF_j75_a4tc_EFFS_xs35_noMu    (&fDirector,"trig_EF_jet_EF_j75_a4tc_EFFS_xs35_noMu"),
         trig_EF_jet_EF_j75_fj75_a4tc_EFFS         (&fDirector,"trig_EF_jet_EF_j75_fj75_a4tc_EFFS"),
         trig_EF_jet_EF_j75_j30_a4tc_EFFS          (&fDirector,"trig_EF_jet_EF_j75_j30_a4tc_EFFS"),
         trig_EF_jet_EF_j75_j30_a4tc_EFFS_L2anymct100(&fDirector,"trig_EF_jet_EF_j75_j30_a4tc_EFFS_L2anymct100"),
         trig_EF_jet_EF_j75_j30_a4tc_EFFS_L2anymct150(&fDirector,"trig_EF_jet_EF_j75_j30_a4tc_EFFS_L2anymct150"),
         trig_EF_jet_EF_j75_j30_a4tc_EFFS_L2anymct175(&fDirector,"trig_EF_jet_EF_j75_j30_a4tc_EFFS_L2anymct175"),
         trig_EF_jet_EF_j75_j30_a4tc_EFFS_L2dphi04 (&fDirector,"trig_EF_jet_EF_j75_j30_a4tc_EFFS_L2dphi04"),
         trig_EF_jet_EF_j75_j30_a4tc_EFFS_anymct150(&fDirector,"trig_EF_jet_EF_j75_j30_a4tc_EFFS_anymct150"),
         trig_EF_jet_EF_j75_j30_a4tc_EFFS_anymct175(&fDirector,"trig_EF_jet_EF_j75_j30_a4tc_EFFS_anymct175"),
         trig_EF_jet_EF_j75_j30_a4tc_EFFS_leadingmct150(&fDirector,"trig_EF_jet_EF_j75_j30_a4tc_EFFS_leadingmct150"),
         trig_EF_jet_EF_j80_a4tc_EFFS_xe60_noMu    (&fDirector,"trig_EF_jet_EF_j80_a4tc_EFFS_xe60_noMu"),
         trig_EF_jet_EF_je195_NoEF                 (&fDirector,"trig_EF_jet_EF_je195_NoEF"),
         trig_EF_jet_EF_je255_NoEF                 (&fDirector,"trig_EF_jet_EF_je255_NoEF"),
         trig_EF_jet_EF_je300_NoEF                 (&fDirector,"trig_EF_jet_EF_je300_NoEF"),
         trig_EF_jet_EF_je350_NoEF                 (&fDirector,"trig_EF_jet_EF_je350_NoEF"),
         trig_EF_jet_EF_je420_NoEF                 (&fDirector,"trig_EF_jet_EF_je420_NoEF"),
         trig_EF_jet_EF_je500_NoEF                 (&fDirector,"trig_EF_jet_EF_je500_NoEF"),
         trig_RoI_L2_j_n                           (&fDirector,"trig_RoI_L2_j_n"),
         trig_RoI_L2_j_type                        (&fDirector,"trig_RoI_L2_j_type"),
         trig_RoI_L2_j_active                      (&fDirector,"trig_RoI_L2_j_active"),
         trig_RoI_L2_j_lastStep                    (&fDirector,"trig_RoI_L2_j_lastStep"),
         trig_RoI_L2_j_TENumber                    (&fDirector,"trig_RoI_L2_j_TENumber"),
         trig_RoI_L2_j_roiNumber                   (&fDirector,"trig_RoI_L2_j_roiNumber"),
         trig_RoI_L2_j_TrigT2Jet                   (&fDirector,"trig_RoI_L2_j_TrigT2Jet"),
         trig_RoI_L2_j_TrigT2JetStatus             (&fDirector,"trig_RoI_L2_j_TrigT2JetStatus"),
         trig_RoI_L2_j_Jet_ROI                     (&fDirector,"trig_RoI_L2_j_Jet_ROI"),
         trig_RoI_L2_j_Jet_ROIStatus               (&fDirector,"trig_RoI_L2_j_Jet_ROIStatus"),
         trig_RoI_EF_j_n                           (&fDirector,"trig_RoI_EF_j_n"),
         trig_RoI_EF_j_type                        (&fDirector,"trig_RoI_EF_j_type"),
         trig_RoI_EF_j_active                      (&fDirector,"trig_RoI_EF_j_active"),
         trig_RoI_EF_j_lastStep                    (&fDirector,"trig_RoI_EF_j_lastStep"),
         trig_RoI_EF_j_TENumber                    (&fDirector,"trig_RoI_EF_j_TENumber"),
         trig_RoI_EF_j_roiNumber                   (&fDirector,"trig_RoI_EF_j_roiNumber"),
         trig_RoI_EF_j_JetCollection               (&fDirector,"trig_RoI_EF_j_JetCollection"),
         trig_RoI_EF_j_JetCollectionStatus         (&fDirector,"trig_RoI_EF_j_JetCollectionStatus"),
         deadPixMod_idHash                         (&fDirector,"deadPixMod_idHash"),
         deadPixMod_nDead                          (&fDirector,"deadPixMod_nDead")
      { }

      // Proxy for each of the branches and leaves of the tree
      TIntProxy                                  jet_antikt4topoem_n;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_E;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_pt;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_m;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_eta;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_phi;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_EtaOrigin;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_PhiOrigin;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_MOrigin;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_EtaOriginEM;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_PhiOriginEM;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_MOriginEM;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_WIDTH;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_n90;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_Timing;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_LArQuality;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_nTrk;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_sumPtTrk;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_OriginIndex;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_HECQuality;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_NegativeE;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_AverageLArQF;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_YFlip12;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_YFlip23;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_BCH_CORR_CELL;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_BCH_CORR_DOTX;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_BCH_CORR_JET;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_BCH_CORR_JET_FORCELL;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_ENG_BAD_CELLS;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_N_BAD_CELLS;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_N_BAD_CELLS_CORR;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_BAD_CELLS_CORR_E;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_NumTowers;
      TStlSimpleProxy<vector<int> >              jet_antikt4topoem_SamplingMax;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_fracSamplingMax;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_hecf;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_tgap3f;
      TStlSimpleProxy<vector<int> >              jet_antikt4topoem_isUgly;
      TStlSimpleProxy<vector<int> >              jet_antikt4topoem_isBadLoose;
      TStlSimpleProxy<vector<int> >              jet_antikt4topoem_isBadMedium;
      TStlSimpleProxy<vector<int> >              jet_antikt4topoem_isBadTight;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_emfrac;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_Offset;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_EMJES;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_EMJES_EtaCorr;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_EMJESnooffset;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_GCWJES;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_GCWJES_EtaCorr;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_CB;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_emscale_E;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_emscale_pt;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_emscale_m;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_emscale_eta;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_emscale_phi;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_jvtx_x;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_jvtx_y;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_jvtx_z;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_jvtxf;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_GSCFactorF;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_WidthFraction;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_weight_Comb;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_weight_IP2D;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_weight_IP3D;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_weight_SV0;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_weight_SV1;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_weight_SV2;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_weight_JetProb;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_weight_SoftMuonTag;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_weight_JetFitterTagNN;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_weight_JetFitterCOMBNN;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_weight_GbbNN;
      TStlSimpleProxy<vector<int> >              jet_antikt4topoem_flavor_truth_label;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_truth_dRminToB;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_truth_dRminToC;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_truth_dRminToT;
      TStlSimpleProxy<vector<int> >              jet_antikt4topoem_flavor_truth_BHadronpdg;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_truth_vx_x;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_truth_vx_y;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_truth_vx_z;
      TStlSimpleProxy<vector<int> >              jet_antikt4topoem_flavor_putruth_label;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_putruth_dRminToB;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_putruth_dRminToC;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_putruth_dRminToT;
      TStlSimpleProxy<vector<int> >              jet_antikt4topoem_flavor_putruth_BHadronpdg;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_putruth_vx_x;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_putruth_vx_y;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_putruth_vx_z;
      TStlSimpleProxy<vector<int> >              jet_antikt4topoem_flavor_component_assoctrk_n;
      TStlPx_vector_int_                         jet_antikt4topoem_flavor_component_assoctrk_index;
      TStlPx_vector_int_                         jet_antikt4topoem_flavor_component_assoctrk_signOfIP;
      TStlPx_vector_int_                         jet_antikt4topoem_flavor_component_assoctrk_signOfZIP;
      TStlPx_vector_float_                       jet_antikt4topoem_flavor_component_assoctrk_ud0wrtPriVtx;
      TStlPx_vector_float_                       jet_antikt4topoem_flavor_component_assoctrk_ud0ErrwrtPriVtx;
      TStlPx_vector_float_                       jet_antikt4topoem_flavor_component_assoctrk_uz0wrtPriVtx;
      TStlPx_vector_float_                       jet_antikt4topoem_flavor_component_assoctrk_uz0ErrwrtPriVtx;
      TStlSimpleProxy<vector<int> >              jet_antikt4topoem_flavor_component_assocmuon_n;
      TStlPx_vector_int_                         jet_antikt4topoem_flavor_component_assocmuon_index;
      TStlSimpleProxy<vector<int> >              jet_antikt4topoem_flavor_component_gentruthlepton_n;
      TStlPx_vector_int_                         jet_antikt4topoem_flavor_component_gentruthlepton_index;
      TStlPx_vector_short_                       jet_antikt4topoem_flavor_component_gentruthlepton_origin;
      TStlPx_vector_int_                         jet_antikt4topoem_flavor_component_gentruthlepton_slbarcode;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_component_ip2d_pu;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_component_ip2d_pb;
      TStlSimpleProxy<vector<int> >              jet_antikt4topoem_flavor_component_ip2d_isValid;
      TStlSimpleProxy<vector<int> >              jet_antikt4topoem_flavor_component_ip2d_ntrk;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_component_ip3d_pu;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_component_ip3d_pb;
      TStlSimpleProxy<vector<int> >              jet_antikt4topoem_flavor_component_ip3d_isValid;
      TStlSimpleProxy<vector<int> >              jet_antikt4topoem_flavor_component_ip3d_ntrk;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_component_sv1_pu;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_component_sv1_pb;
      TStlSimpleProxy<vector<int> >              jet_antikt4topoem_flavor_component_sv1_isValid;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_component_sv2_pu;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_component_sv2_pb;
      TStlSimpleProxy<vector<int> >              jet_antikt4topoem_flavor_component_sv2_isValid;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_component_jfit_pu;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_component_jfit_pb;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_component_jfit_pc;
      TStlSimpleProxy<vector<int> >              jet_antikt4topoem_flavor_component_jfit_isValid;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_component_jfitcomb_pu;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_component_jfitcomb_pb;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_component_jfitcomb_pc;
      TStlSimpleProxy<vector<int> >              jet_antikt4topoem_flavor_component_jfitcomb_isValid;
      TStlSimpleProxy<vector<int> >              jet_antikt4topoem_flavor_component_gbbnn_nMatchingTracks;
      TStlSimpleProxy<vector<double> >           jet_antikt4topoem_flavor_component_gbbnn_trkJetWidth;
      TStlSimpleProxy<vector<double> >           jet_antikt4topoem_flavor_component_gbbnn_trkJetMaxDeltaR;
      TStlSimpleProxy<vector<int> >              jet_antikt4topoem_flavor_component_jfit_nvtx;
      TStlSimpleProxy<vector<int> >              jet_antikt4topoem_flavor_component_jfit_nvtx1t;
      TStlSimpleProxy<vector<int> >              jet_antikt4topoem_flavor_component_jfit_ntrkAtVx;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_component_jfit_efrc;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_component_jfit_mass;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_component_jfit_sig3d;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_component_jfit_deltaPhi;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_component_jfit_deltaEta;
      TStlSimpleProxy<vector<int> >              jet_antikt4topoem_flavor_component_ipplus_trk_n;
      TStlPx_vector_int_                         jet_antikt4topoem_flavor_component_ipplus_trk_index;
      TStlPx_vector_float_                       jet_antikt4topoem_flavor_component_ipplus_trk_d0val;
      TStlPx_vector_float_                       jet_antikt4topoem_flavor_component_ipplus_trk_d0sig;
      TStlPx_vector_float_                       jet_antikt4topoem_flavor_component_ipplus_trk_z0val;
      TStlPx_vector_float_                       jet_antikt4topoem_flavor_component_ipplus_trk_z0sig;
      TStlPx_vector_float_                       jet_antikt4topoem_flavor_component_ipplus_trk_w2D;
      TStlPx_vector_float_                       jet_antikt4topoem_flavor_component_ipplus_trk_w3D;
      TStlPx_vector_float_                       jet_antikt4topoem_flavor_component_ipplus_trk_pJP;
      TStlPx_vector_float_                       jet_antikt4topoem_flavor_component_ipplus_trk_pJPneg;
      TStlPx_vector_int_                         jet_antikt4topoem_flavor_component_ipplus_trk_grade;
      TStlPx_vector_int_                         jet_antikt4topoem_flavor_component_ipplus_trk_isFromV0;
      TStlSimpleProxy<vector<int> >              jet_antikt4topoem_flavor_component_svp_isValid;
      TStlSimpleProxy<vector<int> >              jet_antikt4topoem_flavor_component_svp_ntrkv;
      TStlSimpleProxy<vector<int> >              jet_antikt4topoem_flavor_component_svp_ntrkj;
      TStlSimpleProxy<vector<int> >              jet_antikt4topoem_flavor_component_svp_n2t;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_component_svp_mass;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_component_svp_efrc;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_component_svp_x;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_component_svp_y;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_component_svp_z;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_component_svp_err_x;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_component_svp_err_y;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_component_svp_err_z;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_component_svp_cov_xy;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_component_svp_cov_xz;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_component_svp_cov_yz;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_component_svp_chi2;
      TStlSimpleProxy<vector<int> >              jet_antikt4topoem_flavor_component_svp_ndof;
      TStlSimpleProxy<vector<int> >              jet_antikt4topoem_flavor_component_svp_ntrk;
      TStlSimpleProxy<vector<int> >              jet_antikt4topoem_flavor_component_svp_trk_n;
      TStlPx_vector_int_                         jet_antikt4topoem_flavor_component_svp_trk_index;
      TStlSimpleProxy<vector<int> >              jet_antikt4topoem_flavor_component_sv0p_isValid;
      TStlSimpleProxy<vector<int> >              jet_antikt4topoem_flavor_component_sv0p_ntrkv;
      TStlSimpleProxy<vector<int> >              jet_antikt4topoem_flavor_component_sv0p_ntrkj;
      TStlSimpleProxy<vector<int> >              jet_antikt4topoem_flavor_component_sv0p_n2t;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_component_sv0p_mass;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_component_sv0p_efrc;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_component_sv0p_x;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_component_sv0p_y;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_component_sv0p_z;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_component_sv0p_err_x;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_component_sv0p_err_y;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_component_sv0p_err_z;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_component_sv0p_cov_xy;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_component_sv0p_cov_xz;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_component_sv0p_cov_yz;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_component_sv0p_chi2;
      TStlSimpleProxy<vector<int> >              jet_antikt4topoem_flavor_component_sv0p_ndof;
      TStlSimpleProxy<vector<int> >              jet_antikt4topoem_flavor_component_sv0p_ntrk;
      TStlSimpleProxy<vector<int> >              jet_antikt4topoem_flavor_component_sv0p_trk_n;
      TStlPx_vector_int_                         jet_antikt4topoem_flavor_component_sv0p_trk_index;
      TStlSimpleProxy<vector<int> >              jet_antikt4topoem_flavor_component_softmuoninfo_muon_n;
      TStlPx_vector_int_                         jet_antikt4topoem_flavor_component_softmuoninfo_muon_index;
      TStlPx_vector_float_                       jet_antikt4topoem_flavor_component_softmuoninfo_muon_w;
      TStlPx_vector_float_                       jet_antikt4topoem_flavor_component_softmuoninfo_muon_pTRel;
      TStlSimpleProxy<vector<int> >              jet_antikt4topoem_flavor_component_msvp_isValid;
      TStlSimpleProxy<vector<int> >              jet_antikt4topoem_flavor_component_msvp_ntrkv;
      TStlSimpleProxy<vector<int> >              jet_antikt4topoem_flavor_component_msvp_ntrkj;
      TStlSimpleProxy<vector<int> >              jet_antikt4topoem_flavor_component_msvp_n2t;
      TStlSimpleProxy<vector<int> >              jet_antikt4topoem_flavor_component_msvp_nVtx;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_component_msvp_normWeightedDist;
      TStlSimpleProxy<vector<int> >              jet_antikt4topoem_flavor_component_msvp_msvinjet_n;
      TStlPx_vector_int_                         jet_antikt4topoem_flavor_component_msvp_msvinjet_index;
      TStlSimpleProxy<vector<int> >              jet_antikt4topoem_flavor_component_VKalbadtrack_n;
      TStlPx_vector_int_                         jet_antikt4topoem_flavor_component_VKalbadtrack_index;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_component_jfitvx_theta;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_component_jfitvx_phi;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_component_jfitvx_errtheta;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_component_jfitvx_errphi;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_component_jfitvx_chi2;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_flavor_component_jfitvx_ndof;
      TStlSimpleProxy<vector<int> >              jet_antikt4topoem_flavor_component_jfvxonjetaxis_n;
      TStlPx_vector_int_                         jet_antikt4topoem_flavor_component_jfvxonjetaxis_index;
      TStlSimpleProxy<vector<int> >              jet_antikt4topoem_flavor_component_jftwotrackvertex_n;
      TStlPx_vector_int_                         jet_antikt4topoem_flavor_component_jftwotrackvertex_index;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_el_dr;
      TStlSimpleProxy<vector<int> >              jet_antikt4topoem_el_matched;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_mu_dr;
      TStlSimpleProxy<vector<int> >              jet_antikt4topoem_mu_matched;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_L1_dr;
      TStlSimpleProxy<vector<int> >              jet_antikt4topoem_L1_matched;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_L2_dr;
      TStlSimpleProxy<vector<int> >              jet_antikt4topoem_L2_matched;
      TStlSimpleProxy<vector<float> >            jet_antikt4topoem_EF_dr;
      TStlSimpleProxy<vector<int> >              jet_antikt4topoem_EF_matched;
      TIntProxy                                  jet_antikt6topoem_n;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_E;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_pt;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_m;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_eta;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_phi;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_EtaOrigin;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_PhiOrigin;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_MOrigin;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_EtaOriginEM;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_PhiOriginEM;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_MOriginEM;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_WIDTH;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_n90;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_Timing;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_LArQuality;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_nTrk;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_sumPtTrk;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_OriginIndex;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_HECQuality;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_NegativeE;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_AverageLArQF;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_YFlip12;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_YFlip23;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_BCH_CORR_CELL;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_BCH_CORR_DOTX;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_BCH_CORR_JET;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_BCH_CORR_JET_FORCELL;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_ENG_BAD_CELLS;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_N_BAD_CELLS;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_N_BAD_CELLS_CORR;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_BAD_CELLS_CORR_E;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_NumTowers;
      TStlSimpleProxy<vector<int> >              jet_antikt6topoem_SamplingMax;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_fracSamplingMax;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_hecf;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_tgap3f;
      TStlSimpleProxy<vector<int> >              jet_antikt6topoem_isUgly;
      TStlSimpleProxy<vector<int> >              jet_antikt6topoem_isBadLoose;
      TStlSimpleProxy<vector<int> >              jet_antikt6topoem_isBadMedium;
      TStlSimpleProxy<vector<int> >              jet_antikt6topoem_isBadTight;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_emfrac;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_Offset;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_EMJES;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_EMJES_EtaCorr;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_EMJESnooffset;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_GCWJES;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_GCWJES_EtaCorr;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_CB;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_emscale_E;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_emscale_pt;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_emscale_m;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_emscale_eta;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_emscale_phi;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_jvtx_x;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_jvtx_y;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_jvtx_z;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_jvtxf;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_GSCFactorF;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_WidthFraction;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_weight_Comb;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_weight_IP2D;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_weight_IP3D;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_weight_SV0;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_weight_SV1;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_weight_SV2;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_weight_JetProb;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_weight_SoftMuonTag;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_weight_JetFitterTagNN;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_weight_JetFitterCOMBNN;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_weight_GbbNN;
      TStlSimpleProxy<vector<int> >              jet_antikt6topoem_flavor_truth_label;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_truth_dRminToB;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_truth_dRminToC;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_truth_dRminToT;
      TStlSimpleProxy<vector<int> >              jet_antikt6topoem_flavor_truth_BHadronpdg;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_truth_vx_x;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_truth_vx_y;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_truth_vx_z;
      TStlSimpleProxy<vector<int> >              jet_antikt6topoem_flavor_putruth_label;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_putruth_dRminToB;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_putruth_dRminToC;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_putruth_dRminToT;
      TStlSimpleProxy<vector<int> >              jet_antikt6topoem_flavor_putruth_BHadronpdg;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_putruth_vx_x;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_putruth_vx_y;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_putruth_vx_z;
      TStlSimpleProxy<vector<int> >              jet_antikt6topoem_flavor_component_assoctrk_n;
      TStlPx_vector_int_                         jet_antikt6topoem_flavor_component_assoctrk_index;
      TStlPx_vector_int_                         jet_antikt6topoem_flavor_component_assoctrk_signOfIP;
      TStlPx_vector_int_                         jet_antikt6topoem_flavor_component_assoctrk_signOfZIP;
      TStlPx_vector_float_                       jet_antikt6topoem_flavor_component_assoctrk_ud0wrtPriVtx;
      TStlPx_vector_float_                       jet_antikt6topoem_flavor_component_assoctrk_ud0ErrwrtPriVtx;
      TStlPx_vector_float_                       jet_antikt6topoem_flavor_component_assoctrk_uz0wrtPriVtx;
      TStlPx_vector_float_                       jet_antikt6topoem_flavor_component_assoctrk_uz0ErrwrtPriVtx;
      TStlSimpleProxy<vector<int> >              jet_antikt6topoem_flavor_component_assocmuon_n;
      TStlPx_vector_int_                         jet_antikt6topoem_flavor_component_assocmuon_index;
      TStlSimpleProxy<vector<int> >              jet_antikt6topoem_flavor_component_gentruthlepton_n;
      TStlPx_vector_int_                         jet_antikt6topoem_flavor_component_gentruthlepton_index;
      TStlPx_vector_short_                       jet_antikt6topoem_flavor_component_gentruthlepton_origin;
      TStlPx_vector_int_                         jet_antikt6topoem_flavor_component_gentruthlepton_slbarcode;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_component_ip2d_pu;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_component_ip2d_pb;
      TStlSimpleProxy<vector<int> >              jet_antikt6topoem_flavor_component_ip2d_isValid;
      TStlSimpleProxy<vector<int> >              jet_antikt6topoem_flavor_component_ip2d_ntrk;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_component_ip3d_pu;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_component_ip3d_pb;
      TStlSimpleProxy<vector<int> >              jet_antikt6topoem_flavor_component_ip3d_isValid;
      TStlSimpleProxy<vector<int> >              jet_antikt6topoem_flavor_component_ip3d_ntrk;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_component_sv1_pu;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_component_sv1_pb;
      TStlSimpleProxy<vector<int> >              jet_antikt6topoem_flavor_component_sv1_isValid;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_component_sv2_pu;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_component_sv2_pb;
      TStlSimpleProxy<vector<int> >              jet_antikt6topoem_flavor_component_sv2_isValid;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_component_jfit_pu;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_component_jfit_pb;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_component_jfit_pc;
      TStlSimpleProxy<vector<int> >              jet_antikt6topoem_flavor_component_jfit_isValid;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_component_jfitcomb_pu;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_component_jfitcomb_pb;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_component_jfitcomb_pc;
      TStlSimpleProxy<vector<int> >              jet_antikt6topoem_flavor_component_jfitcomb_isValid;
      TStlSimpleProxy<vector<int> >              jet_antikt6topoem_flavor_component_gbbnn_nMatchingTracks;
      TStlSimpleProxy<vector<double> >           jet_antikt6topoem_flavor_component_gbbnn_trkJetWidth;
      TStlSimpleProxy<vector<double> >           jet_antikt6topoem_flavor_component_gbbnn_trkJetMaxDeltaR;
      TStlSimpleProxy<vector<int> >              jet_antikt6topoem_flavor_component_jfit_nvtx;
      TStlSimpleProxy<vector<int> >              jet_antikt6topoem_flavor_component_jfit_nvtx1t;
      TStlSimpleProxy<vector<int> >              jet_antikt6topoem_flavor_component_jfit_ntrkAtVx;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_component_jfit_efrc;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_component_jfit_mass;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_component_jfit_sig3d;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_component_jfit_deltaPhi;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_component_jfit_deltaEta;
      TStlSimpleProxy<vector<int> >              jet_antikt6topoem_flavor_component_ipplus_trk_n;
      TStlPx_vector_int_                         jet_antikt6topoem_flavor_component_ipplus_trk_index;
      TStlPx_vector_float_                       jet_antikt6topoem_flavor_component_ipplus_trk_d0val;
      TStlPx_vector_float_                       jet_antikt6topoem_flavor_component_ipplus_trk_d0sig;
      TStlPx_vector_float_                       jet_antikt6topoem_flavor_component_ipplus_trk_z0val;
      TStlPx_vector_float_                       jet_antikt6topoem_flavor_component_ipplus_trk_z0sig;
      TStlPx_vector_float_                       jet_antikt6topoem_flavor_component_ipplus_trk_w2D;
      TStlPx_vector_float_                       jet_antikt6topoem_flavor_component_ipplus_trk_w3D;
      TStlPx_vector_float_                       jet_antikt6topoem_flavor_component_ipplus_trk_pJP;
      TStlPx_vector_float_                       jet_antikt6topoem_flavor_component_ipplus_trk_pJPneg;
      TStlPx_vector_int_                         jet_antikt6topoem_flavor_component_ipplus_trk_grade;
      TStlPx_vector_int_                         jet_antikt6topoem_flavor_component_ipplus_trk_isFromV0;
      TStlSimpleProxy<vector<int> >              jet_antikt6topoem_flavor_component_svp_isValid;
      TStlSimpleProxy<vector<int> >              jet_antikt6topoem_flavor_component_svp_ntrkv;
      TStlSimpleProxy<vector<int> >              jet_antikt6topoem_flavor_component_svp_ntrkj;
      TStlSimpleProxy<vector<int> >              jet_antikt6topoem_flavor_component_svp_n2t;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_component_svp_mass;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_component_svp_efrc;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_component_svp_x;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_component_svp_y;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_component_svp_z;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_component_svp_err_x;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_component_svp_err_y;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_component_svp_err_z;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_component_svp_cov_xy;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_component_svp_cov_xz;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_component_svp_cov_yz;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_component_svp_chi2;
      TStlSimpleProxy<vector<int> >              jet_antikt6topoem_flavor_component_svp_ndof;
      TStlSimpleProxy<vector<int> >              jet_antikt6topoem_flavor_component_svp_ntrk;
      TStlSimpleProxy<vector<int> >              jet_antikt6topoem_flavor_component_svp_trk_n;
      TStlPx_vector_int_                         jet_antikt6topoem_flavor_component_svp_trk_index;
      TStlSimpleProxy<vector<int> >              jet_antikt6topoem_flavor_component_sv0p_isValid;
      TStlSimpleProxy<vector<int> >              jet_antikt6topoem_flavor_component_sv0p_ntrkv;
      TStlSimpleProxy<vector<int> >              jet_antikt6topoem_flavor_component_sv0p_ntrkj;
      TStlSimpleProxy<vector<int> >              jet_antikt6topoem_flavor_component_sv0p_n2t;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_component_sv0p_mass;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_component_sv0p_efrc;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_component_sv0p_x;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_component_sv0p_y;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_component_sv0p_z;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_component_sv0p_err_x;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_component_sv0p_err_y;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_component_sv0p_err_z;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_component_sv0p_cov_xy;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_component_sv0p_cov_xz;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_component_sv0p_cov_yz;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_component_sv0p_chi2;
      TStlSimpleProxy<vector<int> >              jet_antikt6topoem_flavor_component_sv0p_ndof;
      TStlSimpleProxy<vector<int> >              jet_antikt6topoem_flavor_component_sv0p_ntrk;
      TStlSimpleProxy<vector<int> >              jet_antikt6topoem_flavor_component_sv0p_trk_n;
      TStlPx_vector_int_                         jet_antikt6topoem_flavor_component_sv0p_trk_index;
      TStlSimpleProxy<vector<int> >              jet_antikt6topoem_flavor_component_softmuoninfo_muon_n;
      TStlPx_vector_int_                         jet_antikt6topoem_flavor_component_softmuoninfo_muon_index;
      TStlPx_vector_float_                       jet_antikt6topoem_flavor_component_softmuoninfo_muon_w;
      TStlPx_vector_float_                       jet_antikt6topoem_flavor_component_softmuoninfo_muon_pTRel;
      TStlSimpleProxy<vector<int> >              jet_antikt6topoem_flavor_component_msvp_isValid;
      TStlSimpleProxy<vector<int> >              jet_antikt6topoem_flavor_component_msvp_ntrkv;
      TStlSimpleProxy<vector<int> >              jet_antikt6topoem_flavor_component_msvp_ntrkj;
      TStlSimpleProxy<vector<int> >              jet_antikt6topoem_flavor_component_msvp_n2t;
      TStlSimpleProxy<vector<int> >              jet_antikt6topoem_flavor_component_msvp_nVtx;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_component_msvp_normWeightedDist;
      TStlSimpleProxy<vector<int> >              jet_antikt6topoem_flavor_component_msvp_msvinjet_n;
      TStlPx_vector_int_                         jet_antikt6topoem_flavor_component_msvp_msvinjet_index;
      TStlSimpleProxy<vector<int> >              jet_antikt6topoem_flavor_component_VKalbadtrack_n;
      TStlPx_vector_int_                         jet_antikt6topoem_flavor_component_VKalbadtrack_index;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_component_jfitvx_theta;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_component_jfitvx_phi;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_component_jfitvx_errtheta;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_component_jfitvx_errphi;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_component_jfitvx_chi2;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_flavor_component_jfitvx_ndof;
      TStlSimpleProxy<vector<int> >              jet_antikt6topoem_flavor_component_jfvxonjetaxis_n;
      TStlPx_vector_int_                         jet_antikt6topoem_flavor_component_jfvxonjetaxis_index;
      TStlSimpleProxy<vector<int> >              jet_antikt6topoem_flavor_component_jftwotrackvertex_n;
      TStlPx_vector_int_                         jet_antikt6topoem_flavor_component_jftwotrackvertex_index;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_el_dr;
      TStlSimpleProxy<vector<int> >              jet_antikt6topoem_el_matched;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_mu_dr;
      TStlSimpleProxy<vector<int> >              jet_antikt6topoem_mu_matched;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_L1_dr;
      TStlSimpleProxy<vector<int> >              jet_antikt6topoem_L1_matched;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_L2_dr;
      TStlSimpleProxy<vector<int> >              jet_antikt6topoem_L2_matched;
      TStlSimpleProxy<vector<float> >            jet_antikt6topoem_EF_dr;
      TStlSimpleProxy<vector<int> >              jet_antikt6topoem_EF_matched;
      TIntProxy                                  jet_antikt4truth_n;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_E;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_pt;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_m;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_eta;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_phi;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_weight_Comb;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_weight_IP2D;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_weight_IP3D;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_weight_SV0;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_weight_SV1;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_weight_SV2;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_weight_JetProb;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_weight_SoftMuonTag;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_weight_JetFitterTagNN;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_weight_JetFitterCOMBNN;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_weight_GbbNN;
      TStlSimpleProxy<vector<int> >              jet_antikt4truth_flavor_truth_label;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_truth_dRminToB;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_truth_dRminToC;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_truth_dRminToT;
      TStlSimpleProxy<vector<int> >              jet_antikt4truth_flavor_truth_BHadronpdg;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_truth_vx_x;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_truth_vx_y;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_truth_vx_z;
      TStlSimpleProxy<vector<int> >              jet_antikt4truth_flavor_putruth_label;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_putruth_dRminToB;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_putruth_dRminToC;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_putruth_dRminToT;
      TStlSimpleProxy<vector<int> >              jet_antikt4truth_flavor_putruth_BHadronpdg;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_putruth_vx_x;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_putruth_vx_y;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_putruth_vx_z;
      TStlSimpleProxy<vector<int> >              jet_antikt4truth_flavor_component_assoctrk_n;
      TStlPx_vector_int_                         jet_antikt4truth_flavor_component_assoctrk_index;
      TStlPx_vector_int_                         jet_antikt4truth_flavor_component_assoctrk_signOfIP;
      TStlPx_vector_int_                         jet_antikt4truth_flavor_component_assoctrk_signOfZIP;
      TStlPx_vector_float_                       jet_antikt4truth_flavor_component_assoctrk_ud0wrtPriVtx;
      TStlPx_vector_float_                       jet_antikt4truth_flavor_component_assoctrk_ud0ErrwrtPriVtx;
      TStlPx_vector_float_                       jet_antikt4truth_flavor_component_assoctrk_uz0wrtPriVtx;
      TStlPx_vector_float_                       jet_antikt4truth_flavor_component_assoctrk_uz0ErrwrtPriVtx;
      TStlSimpleProxy<vector<int> >              jet_antikt4truth_flavor_component_assocmuon_n;
      TStlPx_vector_int_                         jet_antikt4truth_flavor_component_assocmuon_index;
      TStlSimpleProxy<vector<int> >              jet_antikt4truth_flavor_component_gentruthlepton_n;
      TStlPx_vector_int_                         jet_antikt4truth_flavor_component_gentruthlepton_index;
      TStlPx_vector_short_                       jet_antikt4truth_flavor_component_gentruthlepton_origin;
      TStlPx_vector_int_                         jet_antikt4truth_flavor_component_gentruthlepton_slbarcode;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_component_ip2d_pu;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_component_ip2d_pb;
      TStlSimpleProxy<vector<int> >              jet_antikt4truth_flavor_component_ip2d_isValid;
      TStlSimpleProxy<vector<int> >              jet_antikt4truth_flavor_component_ip2d_ntrk;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_component_ip3d_pu;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_component_ip3d_pb;
      TStlSimpleProxy<vector<int> >              jet_antikt4truth_flavor_component_ip3d_isValid;
      TStlSimpleProxy<vector<int> >              jet_antikt4truth_flavor_component_ip3d_ntrk;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_component_sv1_pu;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_component_sv1_pb;
      TStlSimpleProxy<vector<int> >              jet_antikt4truth_flavor_component_sv1_isValid;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_component_sv2_pu;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_component_sv2_pb;
      TStlSimpleProxy<vector<int> >              jet_antikt4truth_flavor_component_sv2_isValid;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_component_jfit_pu;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_component_jfit_pb;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_component_jfit_pc;
      TStlSimpleProxy<vector<int> >              jet_antikt4truth_flavor_component_jfit_isValid;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_component_jfitcomb_pu;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_component_jfitcomb_pb;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_component_jfitcomb_pc;
      TStlSimpleProxy<vector<int> >              jet_antikt4truth_flavor_component_jfitcomb_isValid;
      TStlSimpleProxy<vector<int> >              jet_antikt4truth_flavor_component_gbbnn_nMatchingTracks;
      TStlSimpleProxy<vector<double> >           jet_antikt4truth_flavor_component_gbbnn_trkJetWidth;
      TStlSimpleProxy<vector<double> >           jet_antikt4truth_flavor_component_gbbnn_trkJetMaxDeltaR;
      TStlSimpleProxy<vector<int> >              jet_antikt4truth_flavor_component_jfit_nvtx;
      TStlSimpleProxy<vector<int> >              jet_antikt4truth_flavor_component_jfit_nvtx1t;
      TStlSimpleProxy<vector<int> >              jet_antikt4truth_flavor_component_jfit_ntrkAtVx;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_component_jfit_efrc;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_component_jfit_mass;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_component_jfit_sig3d;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_component_jfit_deltaPhi;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_component_jfit_deltaEta;
      TStlSimpleProxy<vector<int> >              jet_antikt4truth_flavor_component_ipplus_trk_n;
      TStlPx_vector_int_                         jet_antikt4truth_flavor_component_ipplus_trk_index;
      TStlPx_vector_float_                       jet_antikt4truth_flavor_component_ipplus_trk_d0val;
      TStlPx_vector_float_                       jet_antikt4truth_flavor_component_ipplus_trk_d0sig;
      TStlPx_vector_float_                       jet_antikt4truth_flavor_component_ipplus_trk_z0val;
      TStlPx_vector_float_                       jet_antikt4truth_flavor_component_ipplus_trk_z0sig;
      TStlPx_vector_float_                       jet_antikt4truth_flavor_component_ipplus_trk_w2D;
      TStlPx_vector_float_                       jet_antikt4truth_flavor_component_ipplus_trk_w3D;
      TStlPx_vector_float_                       jet_antikt4truth_flavor_component_ipplus_trk_pJP;
      TStlPx_vector_float_                       jet_antikt4truth_flavor_component_ipplus_trk_pJPneg;
      TStlPx_vector_int_                         jet_antikt4truth_flavor_component_ipplus_trk_grade;
      TStlPx_vector_int_                         jet_antikt4truth_flavor_component_ipplus_trk_isFromV0;
      TStlSimpleProxy<vector<int> >              jet_antikt4truth_flavor_component_svp_isValid;
      TStlSimpleProxy<vector<int> >              jet_antikt4truth_flavor_component_svp_ntrkv;
      TStlSimpleProxy<vector<int> >              jet_antikt4truth_flavor_component_svp_ntrkj;
      TStlSimpleProxy<vector<int> >              jet_antikt4truth_flavor_component_svp_n2t;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_component_svp_mass;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_component_svp_efrc;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_component_svp_x;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_component_svp_y;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_component_svp_z;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_component_svp_err_x;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_component_svp_err_y;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_component_svp_err_z;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_component_svp_cov_xy;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_component_svp_cov_xz;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_component_svp_cov_yz;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_component_svp_chi2;
      TStlSimpleProxy<vector<int> >              jet_antikt4truth_flavor_component_svp_ndof;
      TStlSimpleProxy<vector<int> >              jet_antikt4truth_flavor_component_svp_ntrk;
      TStlSimpleProxy<vector<int> >              jet_antikt4truth_flavor_component_svp_trk_n;
      TStlPx_vector_int_                         jet_antikt4truth_flavor_component_svp_trk_index;
      TStlSimpleProxy<vector<int> >              jet_antikt4truth_flavor_component_sv0p_isValid;
      TStlSimpleProxy<vector<int> >              jet_antikt4truth_flavor_component_sv0p_ntrkv;
      TStlSimpleProxy<vector<int> >              jet_antikt4truth_flavor_component_sv0p_ntrkj;
      TStlSimpleProxy<vector<int> >              jet_antikt4truth_flavor_component_sv0p_n2t;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_component_sv0p_mass;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_component_sv0p_efrc;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_component_sv0p_x;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_component_sv0p_y;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_component_sv0p_z;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_component_sv0p_err_x;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_component_sv0p_err_y;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_component_sv0p_err_z;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_component_sv0p_cov_xy;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_component_sv0p_cov_xz;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_component_sv0p_cov_yz;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_component_sv0p_chi2;
      TStlSimpleProxy<vector<int> >              jet_antikt4truth_flavor_component_sv0p_ndof;
      TStlSimpleProxy<vector<int> >              jet_antikt4truth_flavor_component_sv0p_ntrk;
      TStlSimpleProxy<vector<int> >              jet_antikt4truth_flavor_component_sv0p_trk_n;
      TStlPx_vector_int_                         jet_antikt4truth_flavor_component_sv0p_trk_index;
      TStlSimpleProxy<vector<int> >              jet_antikt4truth_flavor_component_softmuoninfo_muon_n;
      TStlPx_vector_int_                         jet_antikt4truth_flavor_component_softmuoninfo_muon_index;
      TStlPx_vector_float_                       jet_antikt4truth_flavor_component_softmuoninfo_muon_w;
      TStlPx_vector_float_                       jet_antikt4truth_flavor_component_softmuoninfo_muon_pTRel;
      TStlSimpleProxy<vector<int> >              jet_antikt4truth_flavor_component_msvp_isValid;
      TStlSimpleProxy<vector<int> >              jet_antikt4truth_flavor_component_msvp_ntrkv;
      TStlSimpleProxy<vector<int> >              jet_antikt4truth_flavor_component_msvp_ntrkj;
      TStlSimpleProxy<vector<int> >              jet_antikt4truth_flavor_component_msvp_n2t;
      TStlSimpleProxy<vector<int> >              jet_antikt4truth_flavor_component_msvp_nVtx;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_component_msvp_normWeightedDist;
      TStlSimpleProxy<vector<int> >              jet_antikt4truth_flavor_component_msvp_msvinjet_n;
      TStlPx_vector_int_                         jet_antikt4truth_flavor_component_msvp_msvinjet_index;
      TStlSimpleProxy<vector<int> >              jet_antikt4truth_flavor_component_VKalbadtrack_n;
      TStlPx_vector_int_                         jet_antikt4truth_flavor_component_VKalbadtrack_index;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_component_jfitvx_theta;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_component_jfitvx_phi;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_component_jfitvx_errtheta;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_component_jfitvx_errphi;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_component_jfitvx_chi2;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_flavor_component_jfitvx_ndof;
      TStlSimpleProxy<vector<int> >              jet_antikt4truth_flavor_component_jfvxonjetaxis_n;
      TStlPx_vector_int_                         jet_antikt4truth_flavor_component_jfvxonjetaxis_index;
      TStlSimpleProxy<vector<int> >              jet_antikt4truth_flavor_component_jftwotrackvertex_n;
      TStlPx_vector_int_                         jet_antikt4truth_flavor_component_jftwotrackvertex_index;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_el_dr;
      TStlSimpleProxy<vector<int> >              jet_antikt4truth_el_matched;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_mu_dr;
      TStlSimpleProxy<vector<int> >              jet_antikt4truth_mu_matched;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_L1_dr;
      TStlSimpleProxy<vector<int> >              jet_antikt4truth_L1_matched;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_L2_dr;
      TStlSimpleProxy<vector<int> >              jet_antikt4truth_L2_matched;
      TStlSimpleProxy<vector<float> >            jet_antikt4truth_EF_dr;
      TStlSimpleProxy<vector<int> >              jet_antikt4truth_EF_matched;
      TIntProxy                                  muoninjet_n;
      TStlSimpleProxy<vector<float> >            muoninjet_E;
      TStlSimpleProxy<vector<float> >            muoninjet_pt;
      TStlSimpleProxy<vector<float> >            muoninjet_m;
      TStlSimpleProxy<vector<float> >            muoninjet_eta;
      TStlSimpleProxy<vector<float> >            muoninjet_phi;
      TStlSimpleProxy<vector<float> >            muoninjet_px;
      TStlSimpleProxy<vector<float> >            muoninjet_py;
      TStlSimpleProxy<vector<float> >            muoninjet_pz;
      TStlSimpleProxy<vector<int> >              muoninjet_author;
      TStlSimpleProxy<vector<float> >            muoninjet_matchchi2;
      TStlSimpleProxy<vector<int> >              muoninjet_matchndof;
      TStlSimpleProxy<vector<float> >            muoninjet_etcone20;
      TStlSimpleProxy<vector<float> >            muoninjet_etcone30;
      TStlSimpleProxy<vector<float> >            muoninjet_etcone40;
      TStlSimpleProxy<vector<float> >            muoninjet_energyLossPar;
      TStlSimpleProxy<vector<float> >            muoninjet_energyLossErr;
      TStlSimpleProxy<vector<float> >            muoninjet_etCore;
      TStlSimpleProxy<vector<float> >            muoninjet_energyLossType;
      TStlSimpleProxy<vector<unsigned short> >   muoninjet_caloMuonIdTag;
      TStlSimpleProxy<vector<double> >           muoninjet_caloLRLikelihood;
      TStlSimpleProxy<vector<int> >              muoninjet_nOutliersOnTrack;
      TStlSimpleProxy<vector<int> >              muoninjet_nMDTHits;
      TStlSimpleProxy<vector<int> >              muoninjet_nMDTHoles;
      TStlSimpleProxy<vector<int> >              muoninjet_nCSCEtaHits;
      TStlSimpleProxy<vector<int> >              muoninjet_nCSCEtaHoles;
      TStlSimpleProxy<vector<int> >              muoninjet_nCSCPhiHits;
      TStlSimpleProxy<vector<int> >              muoninjet_nCSCPhiHoles;
      TStlSimpleProxy<vector<int> >              muoninjet_nRPCEtaHits;
      TStlSimpleProxy<vector<int> >              muoninjet_nRPCEtaHoles;
      TStlSimpleProxy<vector<int> >              muoninjet_nRPCPhiHits;
      TStlSimpleProxy<vector<int> >              muoninjet_nRPCPhiHoles;
      TStlSimpleProxy<vector<int> >              muoninjet_nTGCEtaHits;
      TStlSimpleProxy<vector<int> >              muoninjet_nTGCEtaHoles;
      TStlSimpleProxy<vector<int> >              muoninjet_nTGCPhiHits;
      TStlSimpleProxy<vector<int> >              muoninjet_nTGCPhiHoles;
      TStlSimpleProxy<vector<float> >            muoninjet_primtrk_chi2;
      TStlSimpleProxy<vector<int> >              muoninjet_primtrk_ndof;
      TStlSimpleProxy<vector<int> >              muoninjet_primtrk_hastrack;
      TStlSimpleProxy<vector<int> >              muoninjet_trk_index;
      TIntProxy                                  jfvxonjetaxis_n;
      TStlSimpleProxy<vector<float> >            jfvxonjetaxis_vtxPos;
      TStlSimpleProxy<vector<float> >            jfvxonjetaxis_vtxErr;
      TStlSimpleProxy<vector<int> >              jfvxonjetaxis_trk_n;
      TStlPx_vector_float_                       jfvxonjetaxis_trk_phiAtVx;
      TStlPx_vector_float_                       jfvxonjetaxis_trk_thetaAtVx;
      TStlPx_vector_float_                       jfvxonjetaxis_trk_ptAtVx;
      TStlPx_vector_int_                         jfvxonjetaxis_trk_index;
      TIntProxy                                  jftwotrkvertex_n;
      TStlSimpleProxy<vector<float> >            jftwotrkvertex_isNeutral;
      TStlSimpleProxy<vector<float> >            jftwotrkvertex_chi2;
      TStlSimpleProxy<vector<float> >            jftwotrkvertex_ndof;
      TStlSimpleProxy<vector<float> >            jftwotrkvertex_x;
      TStlSimpleProxy<vector<float> >            jftwotrkvertex_y;
      TStlSimpleProxy<vector<float> >            jftwotrkvertex_z;
      TStlSimpleProxy<vector<float> >            jftwotrkvertex_errx;
      TStlSimpleProxy<vector<float> >            jftwotrkvertex_erry;
      TStlSimpleProxy<vector<float> >            jftwotrkvertex_errz;
      TStlSimpleProxy<vector<float> >            jftwotrkvertex_mass;
      TStlSimpleProxy<vector<int> >              jftwotrkvertex_trk_n;
      TStlPx_vector_int_                         jftwotrkvertex_trk_index;
      TIntProxy                                  msvinjet_n;
      TStlSimpleProxy<vector<float> >            msvinjet_mass;
      TStlSimpleProxy<vector<float> >            msvinjet_pt;
      TStlSimpleProxy<vector<float> >            msvinjet_eta;
      TStlSimpleProxy<vector<float> >            msvinjet_phi;
      TStlSimpleProxy<vector<float> >            msvinjet_efrc;
      TStlSimpleProxy<vector<float> >            msvinjet_x;
      TStlSimpleProxy<vector<float> >            msvinjet_y;
      TStlSimpleProxy<vector<float> >            msvinjet_z;
      TStlSimpleProxy<vector<float> >            msvinjet_err_x;
      TStlSimpleProxy<vector<float> >            msvinjet_err_y;
      TStlSimpleProxy<vector<float> >            msvinjet_err_z;
      TStlSimpleProxy<vector<float> >            msvinjet_cov_xy;
      TStlSimpleProxy<vector<float> >            msvinjet_cov_xz;
      TStlSimpleProxy<vector<float> >            msvinjet_cov_yz;
      TStlSimpleProxy<vector<float> >            msvinjet_chi2;
      TStlSimpleProxy<vector<int> >              msvinjet_ndof;
      TStlSimpleProxy<vector<int> >              msvinjet_ntrk;
      TStlSimpleProxy<vector<float> >            msvinjet_normDist;
      TStlSimpleProxy<vector<int> >              msvinjet_trk_n;
      TStlPx_vector_int_                         msvinjet_trk_index;
      TUIntProxy                                 RunNumber;
      TUIntProxy                                 EventNumber;
      TUIntProxy                                 timestamp;
      TUIntProxy                                 timestamp_ns;
      TUIntProxy                                 lbn;
      TUIntProxy                                 bcid;
      TUIntProxy                                 detmask0;
      TUIntProxy                                 detmask1;
      TFloatProxy                                actualIntPerXing;
      TFloatProxy                                averageIntPerXing;
      TUIntProxy                                 mc_channel_number;
      TUIntProxy                                 mc_event_number;
      TDoubleProxy                               mc_event_weight;
      TUIntProxy                                 pixelFlags;
      TUIntProxy                                 sctFlags;
      TUIntProxy                                 trtFlags;
      TUIntProxy                                 larFlags;
      TUIntProxy                                 tileFlags;
      TUIntProxy                                 muonFlags;
      TUIntProxy                                 fwdFlags;
      TUIntProxy                                 coreFlags;
      TUIntProxy                                 pixelError;
      TUIntProxy                                 sctError;
      TUIntProxy                                 trtError;
      TUIntProxy                                 larError;
      TUIntProxy                                 tileError;
      TUIntProxy                                 muonError;
      TUIntProxy                                 fwdError;
      TUIntProxy                                 coreError;
      TIntProxy                                  pileupinfo_n;
      TStlSimpleProxy<vector<int> >              pileupinfo_time;
      TStlSimpleProxy<vector<int> >              pileupinfo_index;
      TStlSimpleProxy<vector<int> >              pileupinfo_type;
      TStlSimpleProxy<vector<int> >              pileupinfo_runNumber;
      TStlSimpleProxy<vector<int> >              pileupinfo_EventNumber;
      TIntProxy                                  trk_n;
      TStlSimpleProxy<vector<float> >            trk_d0;
      TStlSimpleProxy<vector<float> >            trk_z0;
      TStlSimpleProxy<vector<float> >            trk_phi;
      TStlSimpleProxy<vector<float> >            trk_theta;
      TStlSimpleProxy<vector<float> >            trk_qoverp;
      TStlSimpleProxy<vector<float> >            trk_pt;
      TStlSimpleProxy<vector<float> >            trk_eta;
      TStlSimpleProxy<vector<float> >            trk_err_d0;
      TStlSimpleProxy<vector<float> >            trk_err_z0;
      TStlSimpleProxy<vector<float> >            trk_err_phi;
      TStlSimpleProxy<vector<float> >            trk_err_theta;
      TStlSimpleProxy<vector<float> >            trk_err_qoverp;
      TStlSimpleProxy<vector<float> >            trk_cov_d0_z0;
      TStlSimpleProxy<vector<float> >            trk_cov_d0_phi;
      TStlSimpleProxy<vector<float> >            trk_cov_d0_theta;
      TStlSimpleProxy<vector<float> >            trk_cov_d0_qoverp;
      TStlSimpleProxy<vector<float> >            trk_cov_z0_phi;
      TStlSimpleProxy<vector<float> >            trk_cov_z0_theta;
      TStlSimpleProxy<vector<float> >            trk_cov_z0_qoverp;
      TStlSimpleProxy<vector<float> >            trk_cov_phi_theta;
      TStlSimpleProxy<vector<float> >            trk_cov_phi_qoverp;
      TStlSimpleProxy<vector<float> >            trk_cov_theta_qoverp;
      TStlSimpleProxy<vector<float> >            trk_IPEstimate_d0_biased_wrtPV;
      TStlSimpleProxy<vector<float> >            trk_IPEstimate_z0_biased_wrtPV;
      TStlSimpleProxy<vector<float> >            trk_IPEstimate_d0_unbiased_wrtPV;
      TStlSimpleProxy<vector<float> >            trk_IPEstimate_z0_unbiased_wrtPV;
      TStlSimpleProxy<vector<float> >            trk_IPEstimate_err_d0_biased_wrtPV;
      TStlSimpleProxy<vector<float> >            trk_IPEstimate_err_z0_biased_wrtPV;
      TStlSimpleProxy<vector<float> >            trk_IPEstimate_err_d0_unbiased_wrtPV;
      TStlSimpleProxy<vector<float> >            trk_IPEstimate_err_z0_unbiased_wrtPV;
      TStlSimpleProxy<vector<float> >            trk_IPEstimate_errPV_d0_biased_wrtPV;
      TStlSimpleProxy<vector<float> >            trk_IPEstimate_errPV_z0_biased_wrtPV;
      TStlSimpleProxy<vector<float> >            trk_IPEstimate_errPV_d0_unbiased_wrtPV;
      TStlSimpleProxy<vector<float> >            trk_IPEstimate_errPV_z0_unbiased_wrtPV;
      TStlSimpleProxy<vector<float> >            trk_d0_wrtPV;
      TStlSimpleProxy<vector<float> >            trk_z0_wrtPV;
      TStlSimpleProxy<vector<float> >            trk_phi_wrtPV;
      TStlSimpleProxy<vector<float> >            trk_err_d0_wrtPV;
      TStlSimpleProxy<vector<float> >            trk_err_z0_wrtPV;
      TStlSimpleProxy<vector<float> >            trk_err_phi_wrtPV;
      TStlSimpleProxy<vector<float> >            trk_err_theta_wrtPV;
      TStlSimpleProxy<vector<float> >            trk_err_qoverp_wrtPV;
      TStlSimpleProxy<vector<float> >            trk_cov_d0_z0_wrtPV;
      TStlSimpleProxy<vector<float> >            trk_cov_d0_phi_wrtPV;
      TStlSimpleProxy<vector<float> >            trk_cov_d0_theta_wrtPV;
      TStlSimpleProxy<vector<float> >            trk_cov_d0_qoverp_wrtPV;
      TStlSimpleProxy<vector<float> >            trk_cov_z0_phi_wrtPV;
      TStlSimpleProxy<vector<float> >            trk_cov_z0_theta_wrtPV;
      TStlSimpleProxy<vector<float> >            trk_cov_z0_qoverp_wrtPV;
      TStlSimpleProxy<vector<float> >            trk_cov_phi_theta_wrtPV;
      TStlSimpleProxy<vector<float> >            trk_cov_phi_qoverp_wrtPV;
      TStlSimpleProxy<vector<float> >            trk_cov_theta_qoverp_wrtPV;
      TStlSimpleProxy<vector<float> >            trk_d0_wrtBS;
      TStlSimpleProxy<vector<float> >            trk_z0_wrtBS;
      TStlSimpleProxy<vector<float> >            trk_phi_wrtBS;
      TStlSimpleProxy<vector<float> >            trk_err_d0_wrtBS;
      TStlSimpleProxy<vector<float> >            trk_err_z0_wrtBS;
      TStlSimpleProxy<vector<float> >            trk_err_phi_wrtBS;
      TStlSimpleProxy<vector<float> >            trk_err_theta_wrtBS;
      TStlSimpleProxy<vector<float> >            trk_err_qoverp_wrtBS;
      TStlSimpleProxy<vector<float> >            trk_chi2;
      TStlSimpleProxy<vector<int> >              trk_ndof;
      TStlSimpleProxy<vector<int> >              trk_nBLHits;
      TStlSimpleProxy<vector<int> >              trk_nPixHits;
      TStlSimpleProxy<vector<int> >              trk_nSCTHits;
      TStlSimpleProxy<vector<int> >              trk_nTRTHits;
      TStlSimpleProxy<vector<int> >              trk_nTRTHighTHits;
      TStlSimpleProxy<vector<int> >              trk_nPixHoles;
      TStlSimpleProxy<vector<int> >              trk_nSCTHoles;
      TStlSimpleProxy<vector<int> >              trk_nTRTHoles;
      TStlSimpleProxy<vector<int> >              trk_nBLSharedHits;
      TStlSimpleProxy<vector<int> >              trk_nPixSharedHits;
      TStlSimpleProxy<vector<int> >              trk_nSCTSharedHits;
      TStlSimpleProxy<vector<int> >              trk_nBLayerOutliers;
      TStlSimpleProxy<vector<int> >              trk_nPixelOutliers;
      TStlSimpleProxy<vector<int> >              trk_nSCTOutliers;
      TStlSimpleProxy<vector<int> >              trk_nTRTOutliers;
      TStlSimpleProxy<vector<int> >              trk_nTRTHighTOutliers;
      TStlSimpleProxy<vector<int> >              trk_nContribPixelLayers;
      TStlSimpleProxy<vector<int> >              trk_nGangedPixels;
      TStlSimpleProxy<vector<int> >              trk_nGangedFlaggedFakes;
      TStlSimpleProxy<vector<int> >              trk_nPixelDeadSensors;
      TStlSimpleProxy<vector<int> >              trk_nPixelSpoiltHits;
      TStlSimpleProxy<vector<int> >              trk_nSCTDoubleHoles;
      TStlSimpleProxy<vector<int> >              trk_nSCTDeadSensors;
      TStlSimpleProxy<vector<int> >              trk_nSCTSpoiltHits;
      TStlSimpleProxy<vector<int> >              trk_expectBLayerHit;
      TStlSimpleProxy<vector<int> >              trk_hitPattern;
      TStlSimpleProxy<vector<int> >              trk_nSiHits;
      TStlSimpleProxy<vector<int> >              trk_fitter;
      TStlSimpleProxy<vector<int> >              trk_patternReco1;
      TStlSimpleProxy<vector<int> >              trk_patternReco2;
      TStlSimpleProxy<vector<int> >              trk_seedFinder;
      TStlPx_vector_float_                       trk_blayerPrediction_x;
      TStlPx_vector_float_                       trk_blayerPrediction_y;
      TStlPx_vector_float_                       trk_blayerPrediction_z;
      TStlPx_vector_float_                       trk_blayerPrediction_locX;
      TStlPx_vector_float_                       trk_blayerPrediction_locY;
      TStlPx_vector_float_                       trk_blayerPrediction_err_locX;
      TStlPx_vector_float_                       trk_blayerPrediction_err_locY;
      TStlPx_vector_float_                       trk_blayerPrediction_etaDistToEdge;
      TStlPx_vector_float_                       trk_blayerPrediction_phiDistToEdge;
      TStlPx_vector_unsignedint_                 trk_blayerPrediction_detElementId;
      TStlPx_vector_int_                         trk_blayerPrediction_row;
      TStlPx_vector_int_                         trk_blayerPrediction_col;
      TStlPx_vector_int_                         trk_blayerPrediction_type;
      TStlSimpleProxy<vector<int> >              trk_BLayer_hit_n;
      TStlPx_vector_unsignedint_                 trk_BLayer_hit_id;
      TStlPx_vector_unsignedint_                 trk_BLayer_hit_detElementId;
      TStlPx_vector_int_                         trk_BLayer_hit_bec;
      TStlPx_vector_int_                         trk_BLayer_hit_layer;
      TStlPx_vector_float_                       trk_BLayer_hit_charge;
      TStlPx_vector_int_                         trk_BLayer_hit_sizePhi;
      TStlPx_vector_int_                         trk_BLayer_hit_sizeZ;
      TStlPx_vector_int_                         trk_BLayer_hit_size;
      TStlPx_vector_int_                         trk_BLayer_hit_isFake;
      TStlPx_vector_int_                         trk_BLayer_hit_isGanged;
      TStlPx_vector_int_                         trk_BLayer_hit_isSplit;
      TStlPx_vector_int_                         trk_BLayer_hit_splitProb1;
      TStlPx_vector_int_                         trk_BLayer_hit_splitProb2;
      TStlPx_vector_int_                         trk_BLayer_hit_isCompetingRIO;
      TStlPx_vector_float_                       trk_BLayer_hit_locX;
      TStlPx_vector_float_                       trk_BLayer_hit_locY;
      TStlPx_vector_float_                       trk_BLayer_hit_incidencePhi;
      TStlPx_vector_float_                       trk_BLayer_hit_incidenceTheta;
      TStlPx_vector_float_                       trk_BLayer_hit_err_locX;
      TStlPx_vector_float_                       trk_BLayer_hit_err_locY;
      TStlPx_vector_float_                       trk_BLayer_hit_cov_locXY;
      TStlPx_vector_float_                       trk_BLayer_hit_x;
      TStlPx_vector_float_                       trk_BLayer_hit_y;
      TStlPx_vector_float_                       trk_BLayer_hit_z;
      TStlPx_vector_float_                       trk_BLayer_hit_trkLocX;
      TStlPx_vector_float_                       trk_BLayer_hit_trkLocY;
      TStlPx_vector_float_                       trk_BLayer_hit_err_trkLocX;
      TStlPx_vector_float_                       trk_BLayer_hit_err_trkLocY;
      TStlPx_vector_float_                       trk_BLayer_hit_cov_trkLocXY;
      TStlPx_vector_float_                       trk_BLayer_hit_locBiasedResidualX;
      TStlPx_vector_float_                       trk_BLayer_hit_locBiasedResidualY;
      TStlPx_vector_float_                       trk_BLayer_hit_locBiasedPullX;
      TStlPx_vector_float_                       trk_BLayer_hit_locBiasedPullY;
      TStlPx_vector_float_                       trk_BLayer_hit_locUnbiasedResidualX;
      TStlPx_vector_float_                       trk_BLayer_hit_locUnbiasedResidualY;
      TStlPx_vector_float_                       trk_BLayer_hit_locUnbiasedPullX;
      TStlPx_vector_float_                       trk_BLayer_hit_locUnbiasedPullY;
      TStlPx_vector_float_                       trk_BLayer_hit_chi2;
      TStlPx_vector_int_                         trk_BLayer_hit_ndof;
      TStlSimpleProxy<vector<int> >              trk_Pixel_hit_n;
      TStlPx_vector_unsignedint_                 trk_Pixel_hit_id;
      TStlPx_vector_unsignedint_                 trk_Pixel_hit_detElementId;
      TStlPx_vector_int_                         trk_Pixel_hit_bec;
      TStlPx_vector_int_                         trk_Pixel_hit_layer;
      TStlPx_vector_float_                       trk_Pixel_hit_charge;
      TStlPx_vector_int_                         trk_Pixel_hit_sizePhi;
      TStlPx_vector_int_                         trk_Pixel_hit_sizeZ;
      TStlPx_vector_int_                         trk_Pixel_hit_size;
      TStlPx_vector_int_                         trk_Pixel_hit_isFake;
      TStlPx_vector_int_                         trk_Pixel_hit_isGanged;
      TStlPx_vector_int_                         trk_Pixel_hit_isSplit;
      TStlPx_vector_int_                         trk_Pixel_hit_splitProb1;
      TStlPx_vector_int_                         trk_Pixel_hit_splitProb2;
      TStlPx_vector_int_                         trk_Pixel_hit_isCompetingRIO;
      TStlPx_vector_float_                       trk_Pixel_hit_locX;
      TStlPx_vector_float_                       trk_Pixel_hit_locY;
      TStlPx_vector_float_                       trk_Pixel_hit_incidencePhi;
      TStlPx_vector_float_                       trk_Pixel_hit_incidenceTheta;
      TStlPx_vector_float_                       trk_Pixel_hit_err_locX;
      TStlPx_vector_float_                       trk_Pixel_hit_err_locY;
      TStlPx_vector_float_                       trk_Pixel_hit_cov_locXY;
      TStlPx_vector_float_                       trk_Pixel_hit_x;
      TStlPx_vector_float_                       trk_Pixel_hit_y;
      TStlPx_vector_float_                       trk_Pixel_hit_z;
      TStlPx_vector_float_                       trk_Pixel_hit_trkLocX;
      TStlPx_vector_float_                       trk_Pixel_hit_trkLocY;
      TStlPx_vector_float_                       trk_Pixel_hit_err_trkLocX;
      TStlPx_vector_float_                       trk_Pixel_hit_err_trkLocY;
      TStlPx_vector_float_                       trk_Pixel_hit_cov_trkLocXY;
      TStlPx_vector_float_                       trk_Pixel_hit_locBiasedResidualX;
      TStlPx_vector_float_                       trk_Pixel_hit_locBiasedResidualY;
      TStlPx_vector_float_                       trk_Pixel_hit_locBiasedPullX;
      TStlPx_vector_float_                       trk_Pixel_hit_locBiasedPullY;
      TStlPx_vector_float_                       trk_Pixel_hit_locUnbiasedResidualX;
      TStlPx_vector_float_                       trk_Pixel_hit_locUnbiasedResidualY;
      TStlPx_vector_float_                       trk_Pixel_hit_locUnbiasedPullX;
      TStlPx_vector_float_                       trk_Pixel_hit_locUnbiasedPullY;
      TStlPx_vector_float_                       trk_Pixel_hit_chi2;
      TStlPx_vector_int_                         trk_Pixel_hit_ndof;
      TStlSimpleProxy<vector<int> >              trk_SCT_hit_n;
      TStlPx_vector_unsignedint_                 trk_SCT_hit_id;
      TStlPx_vector_unsignedint_                 trk_SCT_hit_detElementId;
      TStlPx_vector_int_                         trk_SCT_hit_bec;
      TStlPx_vector_int_                         trk_SCT_hit_layer;
      TStlPx_vector_int_                         trk_SCT_hit_sizePhi;
      TStlPx_vector_int_                         trk_SCT_hit_isCompetingRIO;
      TStlPx_vector_float_                       trk_SCT_hit_locX;
      TStlPx_vector_float_                       trk_SCT_hit_locY;
      TStlPx_vector_float_                       trk_SCT_hit_incidencePhi;
      TStlPx_vector_float_                       trk_SCT_hit_incidenceTheta;
      TStlPx_vector_float_                       trk_SCT_hit_err_locX;
      TStlPx_vector_float_                       trk_SCT_hit_err_locY;
      TStlPx_vector_float_                       trk_SCT_hit_cov_locXY;
      TStlPx_vector_float_                       trk_SCT_hit_x;
      TStlPx_vector_float_                       trk_SCT_hit_y;
      TStlPx_vector_float_                       trk_SCT_hit_z;
      TStlPx_vector_float_                       trk_SCT_hit_trkLocX;
      TStlPx_vector_float_                       trk_SCT_hit_trkLocY;
      TStlPx_vector_float_                       trk_SCT_hit_err_trkLocX;
      TStlPx_vector_float_                       trk_SCT_hit_err_trkLocY;
      TStlPx_vector_float_                       trk_SCT_hit_cov_trkLocXY;
      TStlPx_vector_float_                       trk_SCT_hit_locBiasedResidualX;
      TStlPx_vector_float_                       trk_SCT_hit_locBiasedResidualY;
      TStlPx_vector_float_                       trk_SCT_hit_locBiasedPullX;
      TStlPx_vector_float_                       trk_SCT_hit_locBiasedPullY;
      TStlPx_vector_float_                       trk_SCT_hit_locUnbiasedResidualX;
      TStlPx_vector_float_                       trk_SCT_hit_locUnbiasedResidualY;
      TStlPx_vector_float_                       trk_SCT_hit_locUnbiasedPullX;
      TStlPx_vector_float_                       trk_SCT_hit_locUnbiasedPullY;
      TStlPx_vector_float_                       trk_SCT_hit_chi2;
      TStlPx_vector_int_                         trk_SCT_hit_ndof;
      TStlSimpleProxy<vector<int> >              trk_BLayer_outlier_n;
      TStlPx_vector_unsignedint_                 trk_BLayer_outlier_id;
      TStlPx_vector_unsignedint_                 trk_BLayer_outlier_detElementId;
      TStlPx_vector_int_                         trk_BLayer_outlier_bec;
      TStlPx_vector_int_                         trk_BLayer_outlier_layer;
      TStlPx_vector_float_                       trk_BLayer_outlier_charge;
      TStlPx_vector_int_                         trk_BLayer_outlier_sizePhi;
      TStlPx_vector_int_                         trk_BLayer_outlier_sizeZ;
      TStlPx_vector_int_                         trk_BLayer_outlier_size;
      TStlPx_vector_int_                         trk_BLayer_outlier_isFake;
      TStlPx_vector_int_                         trk_BLayer_outlier_isGanged;
      TStlPx_vector_int_                         trk_BLayer_outlier_isSplit;
      TStlPx_vector_int_                         trk_BLayer_outlier_splitProb1;
      TStlPx_vector_int_                         trk_BLayer_outlier_splitProb2;
      TStlPx_vector_int_                         trk_BLayer_outlier_isCompetingRIO;
      TStlPx_vector_float_                       trk_BLayer_outlier_locX;
      TStlPx_vector_float_                       trk_BLayer_outlier_locY;
      TStlPx_vector_float_                       trk_BLayer_outlier_incidencePhi;
      TStlPx_vector_float_                       trk_BLayer_outlier_incidenceTheta;
      TStlPx_vector_float_                       trk_BLayer_outlier_err_locX;
      TStlPx_vector_float_                       trk_BLayer_outlier_err_locY;
      TStlPx_vector_float_                       trk_BLayer_outlier_cov_locXY;
      TStlPx_vector_float_                       trk_BLayer_outlier_x;
      TStlPx_vector_float_                       trk_BLayer_outlier_y;
      TStlPx_vector_float_                       trk_BLayer_outlier_z;
      TStlPx_vector_float_                       trk_BLayer_outlier_trkLocX;
      TStlPx_vector_float_                       trk_BLayer_outlier_trkLocY;
      TStlPx_vector_float_                       trk_BLayer_outlier_err_trkLocX;
      TStlPx_vector_float_                       trk_BLayer_outlier_err_trkLocY;
      TStlPx_vector_float_                       trk_BLayer_outlier_cov_trkLocXY;
      TStlPx_vector_float_                       trk_BLayer_outlier_locBiasedResidualX;
      TStlPx_vector_float_                       trk_BLayer_outlier_locBiasedResidualY;
      TStlPx_vector_float_                       trk_BLayer_outlier_locBiasedPullX;
      TStlPx_vector_float_                       trk_BLayer_outlier_locBiasedPullY;
      TStlPx_vector_float_                       trk_BLayer_outlier_locUnbiasedResidualX;
      TStlPx_vector_float_                       trk_BLayer_outlier_locUnbiasedResidualY;
      TStlPx_vector_float_                       trk_BLayer_outlier_locUnbiasedPullX;
      TStlPx_vector_float_                       trk_BLayer_outlier_locUnbiasedPullY;
      TStlPx_vector_float_                       trk_BLayer_outlier_chi2;
      TStlPx_vector_int_                         trk_BLayer_outlier_ndof;
      TStlSimpleProxy<vector<int> >              trk_Pixel_outlier_n;
      TStlPx_vector_unsignedint_                 trk_Pixel_outlier_id;
      TStlPx_vector_unsignedint_                 trk_Pixel_outlier_detElementId;
      TStlPx_vector_int_                         trk_Pixel_outlier_bec;
      TStlPx_vector_int_                         trk_Pixel_outlier_layer;
      TStlPx_vector_float_                       trk_Pixel_outlier_charge;
      TStlPx_vector_int_                         trk_Pixel_outlier_sizePhi;
      TStlPx_vector_int_                         trk_Pixel_outlier_sizeZ;
      TStlPx_vector_int_                         trk_Pixel_outlier_size;
      TStlPx_vector_int_                         trk_Pixel_outlier_isFake;
      TStlPx_vector_int_                         trk_Pixel_outlier_isGanged;
      TStlPx_vector_int_                         trk_Pixel_outlier_isSplit;
      TStlPx_vector_int_                         trk_Pixel_outlier_splitProb1;
      TStlPx_vector_int_                         trk_Pixel_outlier_splitProb2;
      TStlPx_vector_int_                         trk_Pixel_outlier_isCompetingRIO;
      TStlPx_vector_float_                       trk_Pixel_outlier_locX;
      TStlPx_vector_float_                       trk_Pixel_outlier_locY;
      TStlPx_vector_float_                       trk_Pixel_outlier_incidencePhi;
      TStlPx_vector_float_                       trk_Pixel_outlier_incidenceTheta;
      TStlPx_vector_float_                       trk_Pixel_outlier_err_locX;
      TStlPx_vector_float_                       trk_Pixel_outlier_err_locY;
      TStlPx_vector_float_                       trk_Pixel_outlier_cov_locXY;
      TStlPx_vector_float_                       trk_Pixel_outlier_x;
      TStlPx_vector_float_                       trk_Pixel_outlier_y;
      TStlPx_vector_float_                       trk_Pixel_outlier_z;
      TStlPx_vector_float_                       trk_Pixel_outlier_trkLocX;
      TStlPx_vector_float_                       trk_Pixel_outlier_trkLocY;
      TStlPx_vector_float_                       trk_Pixel_outlier_err_trkLocX;
      TStlPx_vector_float_                       trk_Pixel_outlier_err_trkLocY;
      TStlPx_vector_float_                       trk_Pixel_outlier_cov_trkLocXY;
      TStlPx_vector_float_                       trk_Pixel_outlier_locBiasedResidualX;
      TStlPx_vector_float_                       trk_Pixel_outlier_locBiasedResidualY;
      TStlPx_vector_float_                       trk_Pixel_outlier_locBiasedPullX;
      TStlPx_vector_float_                       trk_Pixel_outlier_locBiasedPullY;
      TStlPx_vector_float_                       trk_Pixel_outlier_locUnbiasedResidualX;
      TStlPx_vector_float_                       trk_Pixel_outlier_locUnbiasedResidualY;
      TStlPx_vector_float_                       trk_Pixel_outlier_locUnbiasedPullX;
      TStlPx_vector_float_                       trk_Pixel_outlier_locUnbiasedPullY;
      TStlPx_vector_float_                       trk_Pixel_outlier_chi2;
      TStlPx_vector_int_                         trk_Pixel_outlier_ndof;
      TStlSimpleProxy<vector<int> >              trk_BLayer_hole_n;
      TStlPx_vector_unsignedint_                 trk_BLayer_hole_detElementId;
      TStlPx_vector_int_                         trk_BLayer_hole_bec;
      TStlPx_vector_int_                         trk_BLayer_hole_layer;
      TStlPx_vector_float_                       trk_BLayer_hole_trkLocX;
      TStlPx_vector_float_                       trk_BLayer_hole_trkLocY;
      TStlPx_vector_float_                       trk_BLayer_hole_err_trkLocX;
      TStlPx_vector_float_                       trk_BLayer_hole_err_trkLocY;
      TStlPx_vector_float_                       trk_BLayer_hole_cov_trkLocXY;
      TStlSimpleProxy<vector<int> >              trk_Pixel_hole_n;
      TStlPx_vector_unsignedint_                 trk_Pixel_hole_detElementId;
      TStlPx_vector_int_                         trk_Pixel_hole_bec;
      TStlPx_vector_int_                         trk_Pixel_hole_layer;
      TStlPx_vector_float_                       trk_Pixel_hole_trkLocX;
      TStlPx_vector_float_                       trk_Pixel_hole_trkLocY;
      TStlPx_vector_float_                       trk_Pixel_hole_err_trkLocX;
      TStlPx_vector_float_                       trk_Pixel_hole_err_trkLocY;
      TStlPx_vector_float_                       trk_Pixel_hole_cov_trkLocXY;
      TStlSimpleProxy<vector<float> >            trk_primvx_weight;
      TStlSimpleProxy<vector<int> >              trk_primvx_index;
      TStlSimpleProxy<vector<float> >            trk_mcpart_probability;
      TStlSimpleProxy<vector<int> >              trk_mcpart_barcode;
      TStlSimpleProxy<vector<int> >              trk_mcpart_index;
      TStlSimpleProxy<vector<int> >              trk_detailed_mc_n;
      TStlPx_vector_int_                         trk_detailed_mc_nCommonPixHits;
      TStlPx_vector_int_                         trk_detailed_mc_nCommonSCTHits;
      TStlPx_vector_int_                         trk_detailed_mc_nCommonTRTHits;
      TStlPx_vector_int_                         trk_detailed_mc_nRecoPixHits;
      TStlPx_vector_int_                         trk_detailed_mc_nRecoSCTHits;
      TStlPx_vector_int_                         trk_detailed_mc_nRecoTRTHits;
      TStlPx_vector_int_                         trk_detailed_mc_nTruthPixHits;
      TStlPx_vector_int_                         trk_detailed_mc_nTruthSCTHits;
      TStlPx_vector_int_                         trk_detailed_mc_nTruthTRTHits;
      TStlPx_vector_int_                         trk_detailed_mc_begVtx_barcode;
      TStlPx_vector_int_                         trk_detailed_mc_endVtx_barcode;
      TStlPx_vector_int_                         trk_detailed_mc_barcode;
      TStlPx_vector_int_                         trk_detailed_mc_index;
      TFloatProxy                                metmuonboyetx;
      TFloatProxy                                metmuonboyety;
      TFloatProxy                                metmuonboyphi;
      TFloatProxy                                metmuonboyet;
      TFloatProxy                                metmuonboysumet;
      TFloatProxy                                metreffinaletx;
      TFloatProxy                                metreffinalety;
      TFloatProxy                                metreffinalphi;
      TFloatProxy                                metreffinalet;
      TFloatProxy                                metreffinalsumet;
      TIntProxy                                  pixClus_n;
      TStlSimpleProxy<vector<unsigned int> >     pixClus_id;
      TStlSimpleProxy<vector<char> >             pixClus_bec;
      TStlSimpleProxy<vector<char> >             pixClus_layer;
      TStlSimpleProxy<vector<unsigned int> >     pixClus_detElementId;
      TStlSimpleProxy<vector<char> >             pixClus_phi_module;
      TStlSimpleProxy<vector<char> >             pixClus_eta_module;
      TStlSimpleProxy<vector<short> >            pixClus_col;
      TStlSimpleProxy<vector<short> >            pixClus_row;
      TStlSimpleProxy<vector<float> >            pixClus_charge;
      TStlSimpleProxy<vector<short> >            pixClus_LVL1A;
      TStlSimpleProxy<vector<int> >              pixClus_sizePhi;
      TStlSimpleProxy<vector<int> >              pixClus_sizeZ;
      TStlSimpleProxy<vector<int> >              pixClus_size;
      TStlSimpleProxy<vector<float> >            pixClus_locX;
      TStlSimpleProxy<vector<float> >            pixClus_locY;
      TStlSimpleProxy<vector<float> >            pixClus_x;
      TStlSimpleProxy<vector<float> >            pixClus_y;
      TStlSimpleProxy<vector<float> >            pixClus_z;
      TStlSimpleProxy<vector<char> >             pixClus_isFake;
      TStlSimpleProxy<vector<char> >             pixClus_isGanged;
      TStlSimpleProxy<vector<int> >              pixClus_isSplit;
      TStlSimpleProxy<vector<int> >              pixClus_splitProb1;
      TStlSimpleProxy<vector<int> >              pixClus_splitProb2;
      TStlPx_vector_int_                         pixClus_mc_barcode;
      TIntProxy                                  primvx_n;
      TStlSimpleProxy<vector<float> >            primvx_x;
      TStlSimpleProxy<vector<float> >            primvx_y;
      TStlSimpleProxy<vector<float> >            primvx_z;
      TStlSimpleProxy<vector<float> >            primvx_err_x;
      TStlSimpleProxy<vector<float> >            primvx_err_y;
      TStlSimpleProxy<vector<float> >            primvx_err_z;
      TStlSimpleProxy<vector<float> >            primvx_cov_xy;
      TStlSimpleProxy<vector<float> >            primvx_cov_xz;
      TStlSimpleProxy<vector<float> >            primvx_cov_yz;
      TStlSimpleProxy<vector<int> >              primvx_type;
      TStlSimpleProxy<vector<float> >            primvx_chi2;
      TStlSimpleProxy<vector<int> >              primvx_ndof;
      TStlSimpleProxy<vector<float> >            primvx_px;
      TStlSimpleProxy<vector<float> >            primvx_py;
      TStlSimpleProxy<vector<float> >            primvx_pz;
      TStlSimpleProxy<vector<float> >            primvx_E;
      TStlSimpleProxy<vector<float> >            primvx_m;
      TStlSimpleProxy<vector<int> >              primvx_nTracks;
      TStlSimpleProxy<vector<float> >            primvx_sumPt;
      TStlSimpleProxy<vector<int> >              primvx_trk_n;
      TStlPx_vector_float_                       primvx_trk_weight;
      TStlPx_vector_float_                       primvx_trk_unbiased_d0;
      TStlPx_vector_float_                       primvx_trk_unbiased_z0;
      TStlPx_vector_float_                       primvx_trk_err_unbiased_d0;
      TStlPx_vector_float_                       primvx_trk_err_unbiased_z0;
      TStlPx_vector_float_                       primvx_trk_chi2;
      TStlPx_vector_float_                       primvx_trk_d0;
      TStlPx_vector_float_                       primvx_trk_z0;
      TStlPx_vector_float_                       primvx_trk_phi;
      TStlPx_vector_float_                       primvx_trk_theta;
      TStlPx_vector_int_                         primvx_trk_index;
      TFloatProxy                                beamSpot_x;
      TFloatProxy                                beamSpot_y;
      TFloatProxy                                beamSpot_z;
      TFloatProxy                                beamSpot_sigma_x;
      TFloatProxy                                beamSpot_sigma_y;
      TFloatProxy                                beamSpot_sigma_z;
      TIntProxy                                  mcevt_n;
      TStlSimpleProxy<vector<int> >              mcevt_signal_process_id;
      TStlSimpleProxy<vector<int> >              mcevt_event_number;
      TStlSimpleProxy<vector<double> >           mcevt_event_scale;
      TStlSimpleProxy<vector<double> >           mcevt_alphaQCD;
      TStlSimpleProxy<vector<double> >           mcevt_alphaQED;
      TStlSimpleProxy<vector<int> >              mcevt_pdf_id1;
      TStlSimpleProxy<vector<int> >              mcevt_pdf_id2;
      TStlSimpleProxy<vector<double> >           mcevt_pdf_x1;
      TStlSimpleProxy<vector<double> >           mcevt_pdf_x2;
      TStlSimpleProxy<vector<double> >           mcevt_pdf_scale;
      TStlSimpleProxy<vector<double> >           mcevt_pdf1;
      TStlSimpleProxy<vector<double> >           mcevt_pdf2;
      TStlPx_vector_double_                      mcevt_weight;
      TStlSimpleProxy<vector<int> >              mcevt_nparticle;
      TStlSimpleProxy<vector<short> >            mcevt_pileUpType;
      TIntProxy                                  mcvtx_n;
      TStlSimpleProxy<vector<float> >            mcvtx_x;
      TStlSimpleProxy<vector<float> >            mcvtx_y;
      TStlSimpleProxy<vector<float> >            mcvtx_z;
      TStlSimpleProxy<vector<int> >              mcvtx_barcode;
      TStlSimpleProxy<vector<int> >              mcvtx_mcevt_index;
      TIntProxy                                  mcpart_n;
      TStlSimpleProxy<vector<float> >            mcpart_pt;
      TStlSimpleProxy<vector<float> >            mcpart_m;
      TStlSimpleProxy<vector<float> >            mcpart_eta;
      TStlSimpleProxy<vector<float> >            mcpart_phi;
      TStlSimpleProxy<vector<int> >              mcpart_type;
      TStlSimpleProxy<vector<int> >              mcpart_status;
      TStlSimpleProxy<vector<int> >              mcpart_barcode;
      TStlSimpleProxy<vector<int> >              mcpart_mothertype;
      TStlSimpleProxy<vector<int> >              mcpart_motherbarcode;
      TStlSimpleProxy<vector<int> >              mcpart_mcevt_index;
      TStlSimpleProxy<vector<int> >              mcpart_mcprodvtx_index;
      TStlSimpleProxy<vector<int> >              mcpart_mother_n;
      TStlPx_vector_int_                         mcpart_mother_index;
      TStlSimpleProxy<vector<int> >              mcpart_mcdecayvtx_index;
      TStlSimpleProxy<vector<int> >              mcpart_child_n;
      TStlPx_vector_int_                         mcpart_child_index;
      TStlSimpleProxy<vector<int> >              mcpart_truthtracks_index;
      TIntProxy                                  truthtrack_n;
      TStlSimpleProxy<vector<int> >              truthtrack_ok;
      TStlSimpleProxy<vector<float> >            truthtrack_d0;
      TStlSimpleProxy<vector<float> >            truthtrack_z0;
      TStlSimpleProxy<vector<float> >            truthtrack_phi;
      TStlSimpleProxy<vector<float> >            truthtrack_theta;
      TStlSimpleProxy<vector<float> >            truthtrack_qoverp;
      TStlSimpleProxy<vector<unsigned int> >     trig_L1_TAV;
      TStlSimpleProxy<vector<short> >            trig_L2_passedPhysics;
      TStlSimpleProxy<vector<short> >            trig_EF_passedPhysics;
      TStlSimpleProxy<vector<unsigned int> >     trig_L1_TBP;
      TStlSimpleProxy<vector<unsigned int> >     trig_L1_TAP;
      TStlSimpleProxy<vector<short> >            trig_L2_passedRaw;
      TStlSimpleProxy<vector<short> >            trig_EF_passedRaw;
      TBoolProxy                                 trig_L2_truncated;
      TBoolProxy                                 trig_EF_truncated;
      TStlSimpleProxy<vector<short> >            trig_L2_resurrected;
      TStlSimpleProxy<vector<short> >            trig_EF_resurrected;
      TStlSimpleProxy<vector<short> >            trig_L2_passedThrough;
      TStlSimpleProxy<vector<short> >            trig_EF_passedThrough;
      TUIntProxy                                 trig_DB_SMK;
      TUIntProxy                                 trig_DB_L1PSK;
      TUIntProxy                                 trig_DB_HLTPSK;
      TBoolProxy                                 EF_2b10_medium_3L1J10;
      TBoolProxy                                 EF_2b10_medium_4L1J10;
      TBoolProxy                                 EF_2b10_medium_4j30_a4tc_EFFS;
      TBoolProxy                                 EF_2b10_medium_L1JE100;
      TBoolProxy                                 EF_2b10_medium_L1JE140;
      TBoolProxy                                 EF_2b10_medium_L1_2J10J50;
      TBoolProxy                                 EF_2b10_medium_j100_j30_a4tc_EFFS;
      TBoolProxy                                 EF_2b10_medium_j75_j30_a4tc_EFFS;
      TBoolProxy                                 EF_2b10_tight_4j30_a4tc_EFFS;
      TBoolProxy                                 EF_2b15_medium_3L1J15;
      TBoolProxy                                 EF_2b15_medium_3j40_a4tc_EFFS;
      TBoolProxy                                 EF_2b15_medium_j75_j40_a4tc_EFFS;
      TBoolProxy                                 EF_2b20_medium_3L1J20;
      TBoolProxy                                 EF_2b20_medium_3j45_a4tc_EFFS;
      TBoolProxy                                 EF_2b20_medium_j75_j45_a4tc_EFFS;
      TBoolProxy                                 EF_2fj100_a4tc_EFFS_deta50_FB;
      TBoolProxy                                 EF_2fj30_a4tc_EFFS_deta50_FB;
      TBoolProxy                                 EF_2fj30_a4tc_EFFS_deta50_FC;
      TBoolProxy                                 EF_2fj55_a4tc_EFFS_deta50_FB;
      TBoolProxy                                 EF_2fj55_a4tc_EFFS_deta50_FC;
      TBoolProxy                                 EF_2fj75_a4tc_EFFS_deta50_FB;
      TBoolProxy                                 EF_2fj75_a4tc_EFFS_deta50_FC;
      TBoolProxy                                 EF_2j100_a4tc_EFFS_deta35_FC;
      TBoolProxy                                 EF_2j135_a4tc_EFFS_deta35_FC;
      TBoolProxy                                 EF_2j180_a4tc_EFFS_deta35_FC;
      TBoolProxy                                 EF_2j240_a4tc_EFFS_deta35_FC;
      TBoolProxy                                 EF_2j45_a4tc_EFFS_leadingmct100_xe40_medium_noMu;
      TBoolProxy                                 EF_2j55_a4tc_EFFS_leadingmct100_xe40_medium_noMu;
      TBoolProxy                                 EF_3b10_loose_4L1J10;
      TBoolProxy                                 EF_3b10_medium_4j30_a4tc_EFFS;
      TBoolProxy                                 EF_3b15_loose_4L1J15;
      TBoolProxy                                 EF_3b15_medium_4j40_a4tc_EFFS;
      TBoolProxy                                 EF_3j100_a4tc_EFFS;
      TBoolProxy                                 EF_3j100_a4tc_EFFS_L1J75;
      TBoolProxy                                 EF_3j30_a4tc_EFFS;
      TBoolProxy                                 EF_3j40_a4tc_EFFS;
      TBoolProxy                                 EF_3j45_a4tc_EFFS;
      TBoolProxy                                 EF_3j75_a4tc_EFFS;
      TBoolProxy                                 EF_4j30_a4tc_EFFS;
      TBoolProxy                                 EF_4j40_a4tc_EFFS;
      TBoolProxy                                 EF_4j40_a4tc_EFFS_ht350;
      TBoolProxy                                 EF_4j40_a4tc_EFFS_ht400;
      TBoolProxy                                 EF_4j45_a4tc_EFFS;
      TBoolProxy                                 EF_4j55_a4tc_EFFS;
      TBoolProxy                                 EF_4j60_a4tc_EFFS;
      TBoolProxy                                 EF_5j30_a4tc_EFFS;
      TBoolProxy                                 EF_5j40_a4tc_EFFS;
      TBoolProxy                                 EF_5j45_a4tc_EFFS;
      TBoolProxy                                 EF_6j30_a4tc_EFFS;
      TBoolProxy                                 EF_6j30_a4tc_EFFS_L15J10;
      TBoolProxy                                 EF_6j40_a4tc_EFFS;
      TBoolProxy                                 EF_6j45_a4tc_EFFS;
      TBoolProxy                                 EF_7j30_a4tc_EFFS_L15J10;
      TBoolProxy                                 EF_7j30_a4tc_EFFS_L16J10;
      TBoolProxy                                 EF_b10_EFj10_a4tc_EFFS_IDTrkNoCut;
      TBoolProxy                                 EF_b10_IDTrkNoCut;
      TBoolProxy                                 EF_b10_L2Star_IDTrkNoCut;
      TBoolProxy                                 EF_b10_medium_4L1J10;
      TBoolProxy                                 EF_b10_medium_4j30_a4tc_EFFS;
      TBoolProxy                                 EF_b10_medium_EFxe25_noMu_L1JE100;
      TBoolProxy                                 EF_b10_medium_EFxe25_noMu_L1JE140;
      TBoolProxy                                 EF_b10_medium_EFxe25_noMu_L1_2J10J50;
      TBoolProxy                                 EF_b10_medium_j75_j55_2j30_a4tc_EFFS;
      TBoolProxy                                 EF_b10_tight_4L1J10;
      TBoolProxy                                 EF_b10_tight_4j30_a4tc_EFFS;
      TBoolProxy                                 EF_b10_tight_L1JE100;
      TBoolProxy                                 EF_b10_tight_L1JE140;
      TBoolProxy                                 EF_b10_tight_j75_j55_2j30_a4tc_EFFS;
      TBoolProxy                                 EF_b15_IDTrkNoCut;
      TBoolProxy                                 EF_b20_IDTrkNoCut;
      TBoolProxy                                 EF_fj100_a4tc_EFFS;
      TBoolProxy                                 EF_fj10_a4tc_EFFS;
      TBoolProxy                                 EF_fj10_a4tc_EFFS_1vx;
      TBoolProxy                                 EF_fj135_a4tc_EFFS;
      TBoolProxy                                 EF_fj15_a4tc_EFFS;
      TBoolProxy                                 EF_fj20_a4tc_EFFS;
      TBoolProxy                                 EF_fj30_a4tc_EFFS;
      TBoolProxy                                 EF_fj30_a4tc_EFFS_l2cleanph;
      TBoolProxy                                 EF_fj55_a4tc_EFFS;
      TBoolProxy                                 EF_fj75_a4tc_EFFS;
      TBoolProxy                                 EF_j100_a4tc_EFFS;
      TBoolProxy                                 EF_j100_a4tc_EFFS_ht350;
      TBoolProxy                                 EF_j100_a4tc_EFFS_ht400;
      TBoolProxy                                 EF_j100_a4tc_EFFS_ht500;
      TBoolProxy                                 EF_j100_j30_a4tc_EFFS_L2dphi04;
      TBoolProxy                                 EF_j10_a4tc_EFFS;
      TBoolProxy                                 EF_j10_a4tc_EFFS_1vx;
      TBoolProxy                                 EF_j135_a4tc_EFFS;
      TBoolProxy                                 EF_j135_a4tc_EFFS_ht500;
      TBoolProxy                                 EF_j135_j30_a4tc_EFFS_L2dphi04;
      TBoolProxy                                 EF_j135_j30_a4tc_EFFS_dphi04;
      TBoolProxy                                 EF_j15_a4tc_EFFS;
      TBoolProxy                                 EF_j180_a4tc_EFFS;
      TBoolProxy                                 EF_j180_j30_a4tc_EFFS_dphi04;
      TBoolProxy                                 EF_j20_a4tc_EFFS;
      TBoolProxy                                 EF_j240_a10tc_EFFS;
      TBoolProxy                                 EF_j240_a4tc_EFFS;
      TBoolProxy                                 EF_j240_a4tc_EFFS_l2cleanph;
      TBoolProxy                                 EF_j30_a4tc_EFFS;
      TBoolProxy                                 EF_j30_a4tc_EFFS_l2cleanph;
      TBoolProxy                                 EF_j30_cosmic;
      TBoolProxy                                 EF_j30_eta13_a4tc_EFFS_EFxe30_noMu_empty;
      TBoolProxy                                 EF_j30_eta13_a4tc_EFFS_EFxe30_noMu_firstempty;
      TBoolProxy                                 EF_j30_firstempty;
      TBoolProxy                                 EF_j30_fj30_a4tc_EFFS;
      TBoolProxy                                 EF_j320_a10tc_EFFS;
      TBoolProxy                                 EF_j320_a4tc_EFFS;
      TBoolProxy                                 EF_j35_a4tc_EFFS;
      TBoolProxy                                 EF_j35_a4tc_EFFS_L1TAU_HV;
      TBoolProxy                                 EF_j35_a4tc_EFFS_L1TAU_HV_cosmic;
      TBoolProxy                                 EF_j35_a4tc_EFFS_L1TAU_HV_firstempty;
      TBoolProxy                                 EF_j35_a4tc_EFFS_L1TAU_HV_unpaired_iso;
      TBoolProxy                                 EF_j35_a4tc_EFFS_L1TAU_HV_unpaired_noniso;
      TBoolProxy                                 EF_j35_a4tc_EFFS_L1TAU_HVtrk;
      TBoolProxy                                 EF_j35_a4tc_EFFS_L1TAU_HVtrk_cosmic;
      TBoolProxy                                 EF_j35_a4tc_EFFS_L1TAU_HVtrk_firstempty;
      TBoolProxy                                 EF_j35_a4tc_EFFS_L1TAU_HVtrk_unpaired_iso;
      TBoolProxy                                 EF_j35_a4tc_EFFS_L1TAU_HVtrk_unpaired_noniso;
      TBoolProxy                                 EF_j40_a4tc_EFFS;
      TBoolProxy                                 EF_j40_fj40_a4tc_EFFS;
      TBoolProxy                                 EF_j425_a10tc_EFFS;
      TBoolProxy                                 EF_j425_a4tc_EFFS;
      TBoolProxy                                 EF_j45_a4tc_EFFS;
      TBoolProxy                                 EF_j50_a4tc_EFFS;
      TBoolProxy                                 EF_j50_cosmic;
      TBoolProxy                                 EF_j50_eta13_a4tc_EFFS_EFxe50_noMu_empty;
      TBoolProxy                                 EF_j50_eta13_a4tc_EFFS_EFxe50_noMu_firstempty;
      TBoolProxy                                 EF_j50_eta25_a4tc_EFFS_EFxe50_noMu_empty;
      TBoolProxy                                 EF_j50_eta25_a4tc_EFFS_EFxe50_noMu_firstempty;
      TBoolProxy                                 EF_j50_firstempty;
      TBoolProxy                                 EF_j55_a4tc_EFFS;
      TBoolProxy                                 EF_j55_a4tc_EFFS_xe55_medium_noMu_dphi2j30xe10;
      TBoolProxy                                 EF_j55_a4tc_EFFS_xe55_medium_noMu_dphi2j30xe10_l2cleancons;
      TBoolProxy                                 EF_j55_fj55_a4tc_EFFS;
      TBoolProxy                                 EF_j65_a4tc_EFFS_xe65_noMu_dphi2j30xe10;
      TBoolProxy                                 EF_j70_j25_dphi03_NoEF;
      TBoolProxy                                 EF_j75_2j30_a4tc_EFFS_ht350;
      TBoolProxy                                 EF_j75_a4tc_EFFS;
      TBoolProxy                                 EF_j75_a4tc_EFFS_xe40_loose_noMu;
      TBoolProxy                                 EF_j75_a4tc_EFFS_xe40_loose_noMu_dphijxe03;
      TBoolProxy                                 EF_j75_a4tc_EFFS_xe45_loose_noMu;
      TBoolProxy                                 EF_j75_a4tc_EFFS_xe55_loose_noMu;
      TBoolProxy                                 EF_j75_a4tc_EFFS_xe55_noMu;
      TBoolProxy                                 EF_j75_a4tc_EFFS_xe55_noMu_l2cleancons;
      TBoolProxy                                 EF_j75_a4tc_EFFS_xs35_noMu;
      TBoolProxy                                 EF_j75_fj75_a4tc_EFFS;
      TBoolProxy                                 EF_j75_j30_a4tc_EFFS;
      TBoolProxy                                 EF_j75_j30_a4tc_EFFS_L2anymct100;
      TBoolProxy                                 EF_j75_j30_a4tc_EFFS_L2anymct150;
      TBoolProxy                                 EF_j75_j30_a4tc_EFFS_L2anymct175;
      TBoolProxy                                 EF_j75_j30_a4tc_EFFS_L2dphi04;
      TBoolProxy                                 EF_j75_j30_a4tc_EFFS_anymct150;
      TBoolProxy                                 EF_j75_j30_a4tc_EFFS_anymct175;
      TBoolProxy                                 EF_j75_j30_a4tc_EFFS_leadingmct150;
      TBoolProxy                                 EF_j80_a4tc_EFFS_xe60_noMu;
      TBoolProxy                                 EF_je195_NoEF;
      TBoolProxy                                 EF_je255_NoEF;
      TBoolProxy                                 EF_je300_NoEF;
      TBoolProxy                                 EF_je350_NoEF;
      TBoolProxy                                 EF_je420_NoEF;
      TBoolProxy                                 EF_je500_NoEF;
      TBoolProxy                                 EF_mu4_L1J10_matched;
      TBoolProxy                                 EF_mu4_L1J20_matched;
      TBoolProxy                                 EF_mu4_L1J30_matched;
      TBoolProxy                                 EF_mu4_L1J50_matched;
      TBoolProxy                                 EF_mu4_L1J75_matched;
      TBoolProxy                                 EF_mu4_j10_a4tc_EFFS;
      TBoolProxy                                 L1_2FJ10;
      TBoolProxy                                 L1_2FJ30;
      TBoolProxy                                 L1_2FJ50;
      TBoolProxy                                 L1_2J10_J30_JE140;
      TBoolProxy                                 L1_2J10_J50;
      TBoolProxy                                 L1_2J10_J75;
      TBoolProxy                                 L1_2J15_J50;
      TBoolProxy                                 L1_2J20_XE20;
      TBoolProxy                                 L1_2J30_XE20;
      TBoolProxy                                 L1_3J10;
      TBoolProxy                                 L1_3J10_J50;
      TBoolProxy                                 L1_3J15;
      TBoolProxy                                 L1_3J20;
      TBoolProxy                                 L1_3J50;
      TBoolProxy                                 L1_3J75;
      TBoolProxy                                 L1_4J10;
      TBoolProxy                                 L1_4J15;
      TBoolProxy                                 L1_4J20;
      TBoolProxy                                 L1_4J30;
      TBoolProxy                                 L1_5J10;
      TBoolProxy                                 L1_5J20;
      TBoolProxy                                 L1_6J10;
      TBoolProxy                                 L1_FJ10;
      TBoolProxy                                 L1_FJ10_EMPTY;
      TBoolProxy                                 L1_FJ10_FIRSTEMPTY;
      TBoolProxy                                 L1_FJ10_UNPAIRED_ISO;
      TBoolProxy                                 L1_FJ30;
      TBoolProxy                                 L1_FJ50;
      TBoolProxy                                 L1_FJ75;
      TBoolProxy                                 L1_J10;
      TBoolProxy                                 L1_J10_EMPTY;
      TBoolProxy                                 L1_J10_FIRSTEMPTY;
      TBoolProxy                                 L1_J10_FJ10;
      TBoolProxy                                 L1_J10_UNPAIRED_ISO;
      TBoolProxy                                 L1_J10_UNPAIRED_NONISO;
      TBoolProxy                                 L1_J15;
      TBoolProxy                                 L1_J175;
      TBoolProxy                                 L1_J20;
      TBoolProxy                                 L1_J250;
      TBoolProxy                                 L1_J30;
      TBoolProxy                                 L1_J30_EMPTY;
      TBoolProxy                                 L1_J30_FIRSTEMPTY;
      TBoolProxy                                 L1_J30_FJ30;
      TBoolProxy                                 L1_J30_UNPAIRED_ISO;
      TBoolProxy                                 L1_J30_UNPAIRED_NONISO;
      TBoolProxy                                 L1_J30_XE35;
      TBoolProxy                                 L1_J30_XE40;
      TBoolProxy                                 L1_J50;
      TBoolProxy                                 L1_J50_FJ50;
      TBoolProxy                                 L1_J50_XE20;
      TBoolProxy                                 L1_J50_XE30;
      TBoolProxy                                 L1_J50_XE35;
      TBoolProxy                                 L1_J50_XE40;
      TBoolProxy                                 L1_J50_XS25;
      TBoolProxy                                 L1_J75;
      TBoolProxy                                 L1_JE100;
      TBoolProxy                                 L1_JE140;
      TBoolProxy                                 L1_JE200;
      TBoolProxy                                 L2_2b10_medium_3L1J10;
      TBoolProxy                                 L2_2b10_medium_4L1J10;
      TBoolProxy                                 L2_2b10_medium_4j25;
      TBoolProxy                                 L2_2b10_medium_L1JE100;
      TBoolProxy                                 L2_2b10_medium_L1JE140;
      TBoolProxy                                 L2_2b10_medium_L1_2J10J50;
      TBoolProxy                                 L2_2b10_medium_j70_j25;
      TBoolProxy                                 L2_2b10_medium_j95_j25;
      TBoolProxy                                 L2_2b10_tight_4j25;
      TBoolProxy                                 L2_2b15_medium_3L1J15;
      TBoolProxy                                 L2_2b15_medium_3j35;
      TBoolProxy                                 L2_2b15_medium_j70_j35;
      TBoolProxy                                 L2_2b20_medium_3L1J20;
      TBoolProxy                                 L2_2b20_medium_3j40;
      TBoolProxy                                 L2_2b20_medium_j70_j40;
      TBoolProxy                                 L2_2fj25;
      TBoolProxy                                 L2_2fj50;
      TBoolProxy                                 L2_2fj70;
      TBoolProxy                                 L2_2j25_j70_dphi03;
      TBoolProxy                                 L2_2j40_anymct100_xe20_medium_noMu;
      TBoolProxy                                 L2_2j50_anymct100_xe20_medium_noMu;
      TBoolProxy                                 L2_3b10_loose_4L1J10;
      TBoolProxy                                 L2_3b10_medium_4j25;
      TBoolProxy                                 L2_3b15_loose_4L1J15;
      TBoolProxy                                 L2_3b15_medium_4j35;
      TBoolProxy                                 L2_3j25;
      TBoolProxy                                 L2_3j35;
      TBoolProxy                                 L2_3j40;
      TBoolProxy                                 L2_3j70;
      TBoolProxy                                 L2_3j95;
      TBoolProxy                                 L2_4j25;
      TBoolProxy                                 L2_4j35;
      TBoolProxy                                 L2_4j40;
      TBoolProxy                                 L2_4j50;
      TBoolProxy                                 L2_5j25;
      TBoolProxy                                 L2_5j35;
      TBoolProxy                                 L2_5j40;
      TBoolProxy                                 L2_6j25;
      TBoolProxy                                 L2_b10_IDTrkNoCut;
      TBoolProxy                                 L2_b10_L2Star_IDTrkNoCut;
      TBoolProxy                                 L2_b10_medium_4L1J10;
      TBoolProxy                                 L2_b10_medium_4j25;
      TBoolProxy                                 L2_b10_medium_EFxe25_noMu_L1JE100;
      TBoolProxy                                 L2_b10_medium_EFxe25_noMu_L1JE140;
      TBoolProxy                                 L2_b10_medium_EFxe25_noMu_L1_2J10J50;
      TBoolProxy                                 L2_b10_medium_j70_2j50_4j25;
      TBoolProxy                                 L2_b10_tight_4L1J10;
      TBoolProxy                                 L2_b10_tight_4j25;
      TBoolProxy                                 L2_b10_tight_L1JE100;
      TBoolProxy                                 L2_b10_tight_L1JE140;
      TBoolProxy                                 L2_b10_tight_j70_2j50_4j25;
      TBoolProxy                                 L2_b15_IDTrkNoCut;
      TBoolProxy                                 L2_b20_IDTrkNoCut;
      TBoolProxy                                 L2_fj10_empty_larcalib;
      TBoolProxy                                 L2_fj25;
      TBoolProxy                                 L2_fj25_l2cleanph;
      TBoolProxy                                 L2_fj25_larcalib;
      TBoolProxy                                 L2_fj50;
      TBoolProxy                                 L2_fj50_larcalib;
      TBoolProxy                                 L2_fj70;
      TBoolProxy                                 L2_fj95;
      TBoolProxy                                 L2_j10_empty_larcalib;
      TBoolProxy                                 L2_j25;
      TBoolProxy                                 L2_j25_cosmic;
      TBoolProxy                                 L2_j25_firstempty;
      TBoolProxy                                 L2_j25_fj25;
      TBoolProxy                                 L2_j25_l2cleanph;
      TBoolProxy                                 L2_j25_larcalib;
      TBoolProxy                                 L2_j30;
      TBoolProxy                                 L2_j30_L1TAU_HV;
      TBoolProxy                                 L2_j30_L1TAU_HV_cosmic;
      TBoolProxy                                 L2_j30_L1TAU_HV_firstempty;
      TBoolProxy                                 L2_j30_L1TAU_HV_unpaired_iso;
      TBoolProxy                                 L2_j30_L1TAU_HV_unpaired_noniso;
      TBoolProxy                                 L2_j30_L1TAU_HVtrk;
      TBoolProxy                                 L2_j30_L1TAU_HVtrk_cosmic;
      TBoolProxy                                 L2_j30_L1TAU_HVtrk_firstempty;
      TBoolProxy                                 L2_j30_L1TAU_HVtrk_unpaired_iso;
      TBoolProxy                                 L2_j30_L1TAU_HVtrk_unpaired_noniso;
      TBoolProxy                                 L2_j30_Trackless_HV;
      TBoolProxy                                 L2_j30_Trackless_HV_L1MU10;
      TBoolProxy                                 L2_j30_Trackless_HV_cosmic;
      TBoolProxy                                 L2_j30_Trackless_HV_firstempty;
      TBoolProxy                                 L2_j30_Trackless_HV_unpaired_iso;
      TBoolProxy                                 L2_j30_Trackless_HV_unpaired_noniso;
      TBoolProxy                                 L2_j30_eta13_a4tc_EFFS_EFxe30_noMu_empty;
      TBoolProxy                                 L2_j30_eta13_a4tc_EFFS_EFxe30_noMu_firstempty;
      TBoolProxy                                 L2_j35;
      TBoolProxy                                 L2_j35_fj35;
      TBoolProxy                                 L2_j40;
      TBoolProxy                                 L2_j45;
      TBoolProxy                                 L2_j45_cosmic;
      TBoolProxy                                 L2_j45_firstempty;
      TBoolProxy                                 L2_j50;
      TBoolProxy                                 L2_j50_eta13_a4tc_EFFS_EFxe50_noMu_empty;
      TBoolProxy                                 L2_j50_eta13_a4tc_EFFS_EFxe50_noMu_firstempty;
      TBoolProxy                                 L2_j50_eta25_a4tc_EFFS_EFxe50_noMu_empty;
      TBoolProxy                                 L2_j50_eta25_a4tc_EFFS_EFxe50_noMu_firstempty;
      TBoolProxy                                 L2_j50_fj50;
      TBoolProxy                                 L2_j50_larcalib;
      TBoolProxy                                 L2_j50_xe35_medium_noMu;
      TBoolProxy                                 L2_j50_xe35_medium_noMu_l2cleancons;
      TBoolProxy                                 L2_j60_xe45_noMu;
      TBoolProxy                                 L2_j70;
      TBoolProxy                                 L2_j70_2j25;
      TBoolProxy                                 L2_j70_2j25_L2anymct100;
      TBoolProxy                                 L2_j70_2j25_anymct100;
      TBoolProxy                                 L2_j70_2j25_anymct150;
      TBoolProxy                                 L2_j70_2j25_anymct175;
      TBoolProxy                                 L2_j70_2j25_dphi04;
      TBoolProxy                                 L2_j70_3j25;
      TBoolProxy                                 L2_j70_fj70;
      TBoolProxy                                 L2_j70_xe20_loose_noMu;
      TBoolProxy                                 L2_j70_xe25_loose_noMu;
      TBoolProxy                                 L2_j70_xe35_noMu;
      TBoolProxy                                 L2_j70_xe35_noMu_l2cleancons;
      TBoolProxy                                 L2_j70_xs25_noMu;
      TBoolProxy                                 L2_j75_xe40_noMu;
      TBoolProxy                                 L2_j95;
      TBoolProxy                                 L2_j95_2j25_dphi04;
      TBoolProxy                                 L2_j95_l2cleanph;
      TBoolProxy                                 L2_j95_larcalib;
      TBoolProxy                                 L2_je195;
      TBoolProxy                                 L2_je255;
      TBoolProxy                                 L2_je300;
      TBoolProxy                                 L2_je350;
      TBoolProxy                                 L2_je420;
      TBoolProxy                                 L2_je500;
      TBoolProxy                                 L2_mu4_L1J10_matched;
      TBoolProxy                                 L2_mu4_L1J20_matched;
      TBoolProxy                                 L2_mu4_L1J30_matched;
      TBoolProxy                                 L2_mu4_L1J50_matched;
      TBoolProxy                                 L2_mu4_L1J75_matched;
      TBoolProxy                                 L2_mu4_j10_a4tc_EFFS;
      TIntProxy                                  trig_Nav_n;
      TStlSimpleProxy<vector<short> >            trig_Nav_chain_ChainId;
      TStlPx_vector_int_                         trig_Nav_chain_RoIType;
      TStlPx_vector_int_                         trig_Nav_chain_RoIIndex;
      TIntProxy                                  trig_RoI_L2_b_n;
      TStlSimpleProxy<vector<short> >            trig_RoI_L2_b_type;
      TStlSimpleProxy<vector<short> >            trig_RoI_L2_b_active;
      TStlSimpleProxy<vector<short> >            trig_RoI_L2_b_lastStep;
      TStlSimpleProxy<vector<short> >            trig_RoI_L2_b_TENumber;
      TStlSimpleProxy<vector<short> >            trig_RoI_L2_b_roiNumber;
      TStlSimpleProxy<vector<int> >              trig_RoI_L2_b_Jet_ROI;
      TStlSimpleProxy<vector<int> >              trig_RoI_L2_b_Jet_ROIStatus;
      TStlSimpleProxy<vector<int> >              trig_RoI_L2_b_Muon_ROI;
      TStlSimpleProxy<vector<int> >              trig_RoI_L2_b_Muon_ROIStatus;
      TStlPx_vector_int_                         trig_RoI_L2_b_TrigL2BjetContainer;
      TStlPx_vector_int_                         trig_RoI_L2_b_TrigL2BjetContainerStatus;
      TStlPx_vector_int_                         trig_RoI_L2_b_TrigInDetTrackCollection_TrigSiTrack_Jet;
      TStlPx_vector_int_                         trig_RoI_L2_b_TrigInDetTrackCollection_TrigSiTrack_JetStatus;
      TStlPx_vector_int_                         trig_RoI_L2_b_TrigInDetTrackCollection_TrigIDSCAN_Jet;
      TStlPx_vector_int_                         trig_RoI_L2_b_TrigInDetTrackCollection_TrigIDSCAN_JetStatus;
      TIntProxy                                  trig_RoI_EF_b_n;
      TStlSimpleProxy<vector<short> >            trig_RoI_EF_b_type;
      TStlSimpleProxy<vector<short> >            trig_RoI_EF_b_active;
      TStlSimpleProxy<vector<short> >            trig_RoI_EF_b_lastStep;
      TStlSimpleProxy<vector<short> >            trig_RoI_EF_b_TENumber;
      TStlSimpleProxy<vector<short> >            trig_RoI_EF_b_roiNumber;
      TStlSimpleProxy<vector<int> >              trig_RoI_EF_b_Jet_ROI;
      TStlSimpleProxy<vector<int> >              trig_RoI_EF_b_Jet_ROIStatus;
      TStlSimpleProxy<vector<int> >              trig_RoI_EF_b_Muon_ROI;
      TStlSimpleProxy<vector<int> >              trig_RoI_EF_b_Muon_ROIStatus;
      TStlPx_vector_int_                         trig_RoI_EF_b_TrigEFBjetContainer;
      TStlPx_vector_int_                         trig_RoI_EF_b_TrigEFBjetContainerStatus;
      TStlPx_vector_int_                         trig_RoI_EF_b_Rec__TrackParticleContainer;
      TStlPx_vector_int_                         trig_RoI_EF_b_Rec__TrackParticleContainerStatus;
      TIntProxy                                  trig_L1_jet_n;
      TStlSimpleProxy<vector<float> >            trig_L1_jet_eta;
      TStlSimpleProxy<vector<float> >            trig_L1_jet_phi;
      TStlPx_vector_string_                      trig_L1_jet_thrNames;
      TStlPx_vector_float_                       trig_L1_jet_thrValues;
      TStlSimpleProxy<vector<unsigned int> >     trig_L1_jet_thrPattern;
      TStlSimpleProxy<vector<float> >            trig_L1_jet_et4x4;
      TStlSimpleProxy<vector<float> >            trig_L1_jet_et6x6;
      TStlSimpleProxy<vector<float> >            trig_L1_jet_et8x8;
      TStlSimpleProxy<vector<unsigned int> >     trig_L1_jet_RoIWord;
      TIntProxy                                  trig_L2_bjet_n;
      TStlSimpleProxy<vector<int> >              trig_L2_bjet_roiId;
      TStlSimpleProxy<vector<int> >              trig_L2_bjet_valid;
      TStlSimpleProxy<vector<float> >            trig_L2_bjet_prmVtx;
      TStlSimpleProxy<vector<float> >            trig_L2_bjet_pt;
      TStlSimpleProxy<vector<float> >            trig_L2_bjet_eta;
      TStlSimpleProxy<vector<float> >            trig_L2_bjet_phi;
      TStlSimpleProxy<vector<float> >            trig_L2_bjet_xComb;
      TStlSimpleProxy<vector<float> >            trig_L2_bjet_xIP1D;
      TStlSimpleProxy<vector<float> >            trig_L2_bjet_xIP2D;
      TStlSimpleProxy<vector<float> >            trig_L2_bjet_xIP3D;
      TStlSimpleProxy<vector<float> >            trig_L2_bjet_xCHI2;
      TStlSimpleProxy<vector<float> >            trig_L2_bjet_xSV;
      TStlSimpleProxy<vector<float> >            trig_L2_bjet_xMVtx;
      TStlSimpleProxy<vector<float> >            trig_L2_bjet_xEVtx;
      TStlSimpleProxy<vector<float> >            trig_L2_bjet_xNVtx;
      TStlSimpleProxy<vector<float> >            trig_L2_bjet_BSx;
      TStlSimpleProxy<vector<float> >            trig_L2_bjet_BSy;
      TStlSimpleProxy<vector<float> >            trig_L2_bjet_BSz;
      TStlSimpleProxy<vector<float> >            trig_L2_bjet_sBSx;
      TStlSimpleProxy<vector<float> >            trig_L2_bjet_sBSy;
      TStlSimpleProxy<vector<float> >            trig_L2_bjet_sBSz;
      TStlSimpleProxy<vector<float> >            trig_L2_bjet_sBSxy;
      TStlSimpleProxy<vector<float> >            trig_L2_bjet_BTiltXZ;
      TStlSimpleProxy<vector<float> >            trig_L2_bjet_BTiltYZ;
      TStlSimpleProxy<vector<int> >              trig_L2_bjet_BSstatus;
      TIntProxy                                  trig_EF_bjet_n;
      TStlSimpleProxy<vector<int> >              trig_EF_bjet_roiId;
      TStlSimpleProxy<vector<int> >              trig_EF_bjet_valid;
      TStlSimpleProxy<vector<float> >            trig_EF_bjet_prmVtx;
      TStlSimpleProxy<vector<float> >            trig_EF_bjet_pt;
      TStlSimpleProxy<vector<float> >            trig_EF_bjet_eta;
      TStlSimpleProxy<vector<float> >            trig_EF_bjet_phi;
      TStlSimpleProxy<vector<float> >            trig_EF_bjet_xComb;
      TStlSimpleProxy<vector<float> >            trig_EF_bjet_xIP1D;
      TStlSimpleProxy<vector<float> >            trig_EF_bjet_xIP2D;
      TStlSimpleProxy<vector<float> >            trig_EF_bjet_xIP3D;
      TStlSimpleProxy<vector<float> >            trig_EF_bjet_xCHI2;
      TStlSimpleProxy<vector<float> >            trig_EF_bjet_xSV;
      TStlSimpleProxy<vector<float> >            trig_EF_bjet_xMVtx;
      TStlSimpleProxy<vector<float> >            trig_EF_bjet_xEVtx;
      TStlSimpleProxy<vector<float> >            trig_EF_bjet_xNVtx;
      TIntProxy                                  trig_EF_pv_n;
      TStlSimpleProxy<vector<float> >            trig_EF_pv_x;
      TStlSimpleProxy<vector<float> >            trig_EF_pv_y;
      TStlSimpleProxy<vector<float> >            trig_EF_pv_z;
      TStlSimpleProxy<vector<int> >              trig_EF_pv_type;
      TStlSimpleProxy<vector<float> >            trig_EF_pv_err_x;
      TStlSimpleProxy<vector<float> >            trig_EF_pv_err_y;
      TStlSimpleProxy<vector<float> >            trig_EF_pv_err_z;
      TIntProxy                                  trig_L2_jet_n;
      TStlSimpleProxy<vector<float> >            trig_L2_jet_E;
      TStlSimpleProxy<vector<float> >            trig_L2_jet_eta;
      TStlSimpleProxy<vector<float> >            trig_L2_jet_phi;
      TStlSimpleProxy<vector<unsigned int> >     trig_L2_jet_RoIWord;
      TStlSimpleProxy<vector<double> >           trig_L2_jet_ehad0;
      TStlSimpleProxy<vector<double> >           trig_L2_jet_eem0;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_nLeadingCells;
      TStlSimpleProxy<vector<float> >            trig_L2_jet_hecf;
      TStlSimpleProxy<vector<float> >            trig_L2_jet_jetQuality;
      TStlSimpleProxy<vector<float> >            trig_L2_jet_emf;
      TStlSimpleProxy<vector<float> >            trig_L2_jet_jetTimeCells;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_2fj25;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_2fj50;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_2fj70;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_2j25_j70_dphi03;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_2j40_anymct100_xe20_medium_noMu;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_2j50_anymct100_xe20_medium_noMu;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_3j25;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_3j35;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_3j40;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_3j70;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_3j95;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_4j25;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_4j35;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_4j40;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_4j50;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_5j25;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_5j35;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_5j40;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_6j25;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_fj10_empty_larcalib;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_fj25;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_fj25_l2cleanph;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_fj25_larcalib;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_fj50;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_fj50_larcalib;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_fj70;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_fj95;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_j10_empty_larcalib;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_j25;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_j25_cosmic;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_j25_firstempty;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_j25_fj25;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_j25_l2cleanph;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_j25_larcalib;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_j30;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_j30_L1TAU_HV;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_j30_L1TAU_HV_cosmic;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_j30_L1TAU_HV_firstempty;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_j30_L1TAU_HV_unpaired_iso;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_j30_L1TAU_HV_unpaired_noniso;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_j30_L1TAU_HVtrk;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_j30_L1TAU_HVtrk_cosmic;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_j30_L1TAU_HVtrk_firstempty;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_j30_L1TAU_HVtrk_unpaired_iso;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_j30_L1TAU_HVtrk_unpaired_noniso;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_j30_Trackless_HV;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_j30_Trackless_HV_L1MU10;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_j30_Trackless_HV_cosmic;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_j30_Trackless_HV_firstempty;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_j30_Trackless_HV_unpaired_iso;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_j30_Trackless_HV_unpaired_noniso;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_j30_eta13_a4tc_EFFS_EFxe30_noMu_empty;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_j30_eta13_a4tc_EFFS_EFxe30_noMu_firstempty;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_j35;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_j35_fj35;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_j40;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_j45;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_j45_cosmic;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_j45_firstempty;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_j50;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_j50_eta13_a4tc_EFFS_EFxe50_noMu_empty;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_j50_eta13_a4tc_EFFS_EFxe50_noMu_firstempty;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_j50_eta25_a4tc_EFFS_EFxe50_noMu_empty;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_j50_eta25_a4tc_EFFS_EFxe50_noMu_firstempty;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_j50_fj50;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_j50_larcalib;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_j50_xe35_medium_noMu;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_j50_xe35_medium_noMu_l2cleancons;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_j60_xe45_noMu;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_j70;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_j70_2j25;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_j70_2j25_L2anymct100;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_j70_2j25_anymct100;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_j70_2j25_anymct150;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_j70_2j25_anymct175;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_j70_2j25_dphi04;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_j70_3j25;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_j70_fj70;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_j70_xe20_loose_noMu;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_j70_xe25_loose_noMu;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_j70_xe35_noMu;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_j70_xe35_noMu_l2cleancons;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_j70_xs25_noMu;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_j75_xe40_noMu;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_j95;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_j95_2j25_dphi04;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_j95_l2cleanph;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_j95_larcalib;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_je195;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_je255;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_je300;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_je350;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_je420;
      TStlSimpleProxy<vector<int> >              trig_L2_jet_L2_je500;
      TIntProxy                                  trig_EF_jet_n;
      TStlSimpleProxy<vector<float> >            trig_EF_jet_emscale_E;
      TStlSimpleProxy<vector<float> >            trig_EF_jet_emscale_pt;
      TStlSimpleProxy<vector<float> >            trig_EF_jet_emscale_m;
      TStlSimpleProxy<vector<float> >            trig_EF_jet_emscale_eta;
      TStlSimpleProxy<vector<float> >            trig_EF_jet_emscale_phi;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_a4;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_a4tc;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_a10tc;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_a6;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_a6tc;
      TStlSimpleProxy<vector<unsigned int> >     trig_EF_jet_RoIword;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_2fj100_a4tc_EFFS_deta50_FB;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_2fj30_a4tc_EFFS_deta50_FB;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_2fj30_a4tc_EFFS_deta50_FC;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_2fj55_a4tc_EFFS_deta50_FB;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_2fj55_a4tc_EFFS_deta50_FC;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_2fj75_a4tc_EFFS_deta50_FB;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_2fj75_a4tc_EFFS_deta50_FC;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_2j100_a4tc_EFFS_deta35_FC;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_2j135_a4tc_EFFS_deta35_FC;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_2j180_a4tc_EFFS_deta35_FC;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_2j240_a4tc_EFFS_deta35_FC;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_2j45_a4tc_EFFS_leadingmct100_xe40_medium_noMu;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_2j55_a4tc_EFFS_leadingmct100_xe40_medium_noMu;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_3j100_a4tc_EFFS;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_3j100_a4tc_EFFS_L1J75;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_3j30_a4tc_EFFS;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_3j40_a4tc_EFFS;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_3j45_a4tc_EFFS;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_3j75_a4tc_EFFS;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_4j30_a4tc_EFFS;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_4j40_a4tc_EFFS;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_4j40_a4tc_EFFS_ht350;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_4j40_a4tc_EFFS_ht400;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_4j45_a4tc_EFFS;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_4j55_a4tc_EFFS;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_4j60_a4tc_EFFS;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_5j30_a4tc_EFFS;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_5j40_a4tc_EFFS;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_5j45_a4tc_EFFS;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_6j30_a4tc_EFFS;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_6j30_a4tc_EFFS_L15J10;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_6j40_a4tc_EFFS;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_6j45_a4tc_EFFS;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_7j30_a4tc_EFFS_L15J10;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_7j30_a4tc_EFFS_L16J10;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_fj100_a4tc_EFFS;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_fj10_a4tc_EFFS;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_fj10_a4tc_EFFS_1vx;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_fj135_a4tc_EFFS;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_fj15_a4tc_EFFS;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_fj20_a4tc_EFFS;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_fj30_a4tc_EFFS;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_fj30_a4tc_EFFS_l2cleanph;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_fj55_a4tc_EFFS;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_fj75_a4tc_EFFS;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j100_a4tc_EFFS;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j100_a4tc_EFFS_ht350;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j100_a4tc_EFFS_ht400;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j100_a4tc_EFFS_ht500;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j100_j30_a4tc_EFFS_L2dphi04;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j10_a4tc_EFFS;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j10_a4tc_EFFS_1vx;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j135_a4tc_EFFS;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j135_a4tc_EFFS_ht500;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j135_j30_a4tc_EFFS_L2dphi04;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j135_j30_a4tc_EFFS_dphi04;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j15_a4tc_EFFS;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j180_a4tc_EFFS;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j180_j30_a4tc_EFFS_dphi04;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j20_a4tc_EFFS;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j240_a10tc_EFFS;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j240_a4tc_EFFS;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j240_a4tc_EFFS_l2cleanph;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j30_a4tc_EFFS;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j30_a4tc_EFFS_l2cleanph;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j30_cosmic;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j30_eta13_a4tc_EFFS_EFxe30_noMu_empty;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j30_eta13_a4tc_EFFS_EFxe30_noMu_firstempty;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j30_firstempty;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j30_fj30_a4tc_EFFS;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j320_a10tc_EFFS;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j320_a4tc_EFFS;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j35_a4tc_EFFS;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j35_a4tc_EFFS_L1TAU_HV;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j35_a4tc_EFFS_L1TAU_HV_cosmic;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j35_a4tc_EFFS_L1TAU_HV_firstempty;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j35_a4tc_EFFS_L1TAU_HV_unpaired_iso;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j35_a4tc_EFFS_L1TAU_HV_unpaired_noniso;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j35_a4tc_EFFS_L1TAU_HVtrk;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j35_a4tc_EFFS_L1TAU_HVtrk_cosmic;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j35_a4tc_EFFS_L1TAU_HVtrk_firstempty;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j35_a4tc_EFFS_L1TAU_HVtrk_unpaired_iso;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j35_a4tc_EFFS_L1TAU_HVtrk_unpaired_noniso;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j40_a4tc_EFFS;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j40_fj40_a4tc_EFFS;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j425_a10tc_EFFS;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j425_a4tc_EFFS;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j45_a4tc_EFFS;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j50_a4tc_EFFS;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j50_cosmic;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j50_eta13_a4tc_EFFS_EFxe50_noMu_empty;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j50_eta13_a4tc_EFFS_EFxe50_noMu_firstempty;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j50_eta25_a4tc_EFFS_EFxe50_noMu_empty;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j50_eta25_a4tc_EFFS_EFxe50_noMu_firstempty;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j50_firstempty;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j55_a4tc_EFFS;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j55_a4tc_EFFS_xe55_medium_noMu_dphi2j30xe10;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j55_a4tc_EFFS_xe55_medium_noMu_dphi2j30xe10_l2cleancons;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j55_fj55_a4tc_EFFS;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j65_a4tc_EFFS_xe65_noMu_dphi2j30xe10;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j70_j25_dphi03_NoEF;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j75_2j30_a4tc_EFFS_ht350;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j75_a4tc_EFFS;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j75_a4tc_EFFS_xe40_loose_noMu;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j75_a4tc_EFFS_xe40_loose_noMu_dphijxe03;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j75_a4tc_EFFS_xe45_loose_noMu;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j75_a4tc_EFFS_xe55_loose_noMu;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j75_a4tc_EFFS_xe55_noMu;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j75_a4tc_EFFS_xe55_noMu_l2cleancons;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j75_a4tc_EFFS_xs35_noMu;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j75_fj75_a4tc_EFFS;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j75_j30_a4tc_EFFS;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j75_j30_a4tc_EFFS_L2anymct100;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j75_j30_a4tc_EFFS_L2anymct150;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j75_j30_a4tc_EFFS_L2anymct175;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j75_j30_a4tc_EFFS_L2dphi04;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j75_j30_a4tc_EFFS_anymct150;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j75_j30_a4tc_EFFS_anymct175;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j75_j30_a4tc_EFFS_leadingmct150;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_j80_a4tc_EFFS_xe60_noMu;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_je195_NoEF;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_je255_NoEF;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_je300_NoEF;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_je350_NoEF;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_je420_NoEF;
      TStlSimpleProxy<vector<int> >              trig_EF_jet_EF_je500_NoEF;
      TIntProxy                                  trig_RoI_L2_j_n;
      TStlSimpleProxy<vector<short> >            trig_RoI_L2_j_type;
      TStlSimpleProxy<vector<short> >            trig_RoI_L2_j_active;
      TStlSimpleProxy<vector<short> >            trig_RoI_L2_j_lastStep;
      TStlSimpleProxy<vector<short> >            trig_RoI_L2_j_TENumber;
      TStlSimpleProxy<vector<short> >            trig_RoI_L2_j_roiNumber;
      TStlSimpleProxy<vector<int> >              trig_RoI_L2_j_TrigT2Jet;
      TStlSimpleProxy<vector<int> >              trig_RoI_L2_j_TrigT2JetStatus;
      TStlSimpleProxy<vector<int> >              trig_RoI_L2_j_Jet_ROI;
      TStlSimpleProxy<vector<int> >              trig_RoI_L2_j_Jet_ROIStatus;
      TIntProxy                                  trig_RoI_EF_j_n;
      TStlSimpleProxy<vector<short> >            trig_RoI_EF_j_type;
      TStlSimpleProxy<vector<short> >            trig_RoI_EF_j_active;
      TStlSimpleProxy<vector<short> >            trig_RoI_EF_j_lastStep;
      TStlSimpleProxy<vector<short> >            trig_RoI_EF_j_TENumber;
      TStlSimpleProxy<vector<short> >            trig_RoI_EF_j_roiNumber;
      TStlPx_vector_int_                         trig_RoI_EF_j_JetCollection;
      TStlPx_vector_int_                         trig_RoI_EF_j_JetCollectionStatus;
      TStlSimpleProxy<vector<short> >            deadPixMod_idHash;
      TIntProxy                                  deadPixMod_nDead;
   };

   // Proxy for each of the branches, leaves and friends of the tree
   TIntProxy                                  RunNumber;


   ntuple_CollectionTree(TTree *tree=0) : 
      fChain(0),
      htemp(0),
      fDirector(tree,-1),
      fClass                (TClass::GetClass("ntuple_CollectionTree")),
      RunNumber                                 (&fDirector,"RunNumber")
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
#pragma link C++ class ntuple_CollectionTree::TStlPx_vector_int_-;
#pragma link C++ class ntuple_CollectionTree::TStlPx_vector_float_-;
#pragma link C++ class ntuple_CollectionTree::TStlPx_vector_short_-;
#pragma link C++ class ntuple_CollectionTree::TStlPx_vector_unsignedint_-;
#pragma link C++ class ntuple_CollectionTree::TStlPx_vector_double_-;
#pragma link C++ class ntuple_CollectionTree::TStlPx_vector_string_-;
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
