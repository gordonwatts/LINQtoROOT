/////////////////////////////////////////////////////////////////////////
//   This class has been automatically generated 
//   (at Wed Apr 06 10:46:57 2011 by ROOT version 5.28/00)
//   from TTree vtuple/JetTagNtuple
//   found on file: C:\Users\gwatts\Documents\ATLAS\Projects\LINQToROOT\DemosAndTests\btag-slim.root
/////////////////////////////////////////////////////////////////////////


#ifndef ntuple_vtuple_h
#define ntuple_vtuple_h

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


class junk_macro_parsettree_vtuple_Interface {
   // This class defines the list of methods that are directly used by ntuple_vtuple,
   // and that can be overloaded in the user's script
public:
   void junk_macro_parsettree_vtuple_Begin(TTree*) {}
   void junk_macro_parsettree_vtuple_SlaveBegin(TTree*) {}
   Bool_t junk_macro_parsettree_vtuple_Notify() { return kTRUE; }
   Bool_t junk_macro_parsettree_vtuple_Process(Long64_t) { return kTRUE; }
   void junk_macro_parsettree_vtuple_SlaveTerminate() {}
   void junk_macro_parsettree_vtuple_Terminate() {}
};


class ntuple_vtuple : public TSelector, public junk_macro_parsettree_vtuple_Interface {
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
   struct TStlPx_vector_vector_int__
   {
      TStlPx_vector_vector_int__(TBranchProxyDirector* director,const char *top,const char *mid=0) :
         ffPrefix(top,mid),
         obj(director, top, mid)
      {};
      TStlPx_vector_vector_int__(TBranchProxyDirector* director, TBranchProxy *parent, const char *membername, const char *top=0, const char *mid=0) :
         ffPrefix(top,mid),
         obj(director, parent, membername)
      {};
      TBranchProxyHelper ffPrefix;
      InjecTBranchProxyInterface();
      const vector<vector<int> >& At(UInt_t i) {
         static vector<vector<int> > default_val;
         if (!obj.Read()) return default_val;
         vector<vector<int> > *temp = & obj.GetPtr()->at(i);
         if (temp) return *temp; else return default_val;
      }
      const vector<vector<int> >& operator[](Int_t i) { return At(i); }
      const vector<vector<int> >& operator[](UInt_t i) { return At(i); }
      Int_t GetEntries() { return obj.GetPtr()->size(); }
      const vector<vector<vector<int> > >* operator->() { return obj.GetPtr(); }
      operator vector<vector<vector<int> > >*() { return obj.GetPtr(); }
      TObjProxy<vector<vector<vector<int> > > > obj;

   };
   struct TStlPx_vector_vector_float__
   {
      TStlPx_vector_vector_float__(TBranchProxyDirector* director,const char *top,const char *mid=0) :
         ffPrefix(top,mid),
         obj(director, top, mid)
      {};
      TStlPx_vector_vector_float__(TBranchProxyDirector* director, TBranchProxy *parent, const char *membername, const char *top=0, const char *mid=0) :
         ffPrefix(top,mid),
         obj(director, parent, membername)
      {};
      TBranchProxyHelper ffPrefix;
      InjecTBranchProxyInterface();
      const vector<vector<float> >& At(UInt_t i) {
         static vector<vector<float> > default_val;
         if (!obj.Read()) return default_val;
         vector<vector<float> > *temp = & obj.GetPtr()->at(i);
         if (temp) return *temp; else return default_val;
      }
      const vector<vector<float> >& operator[](Int_t i) { return At(i); }
      const vector<vector<float> >& operator[](UInt_t i) { return At(i); }
      Int_t GetEntries() { return obj.GetPtr()->size(); }
      const vector<vector<vector<float> > >* operator->() { return obj.GetPtr(); }
      operator vector<vector<vector<float> > >*() { return obj.GetPtr(); }
      TObjProxy<vector<vector<vector<float> > > > obj;

   };
   struct TStlPx_vector_bool_
   {
      TStlPx_vector_bool_(TBranchProxyDirector* director,const char *top,const char *mid=0) :
         ffPrefix(top,mid),
         obj(director, top, mid)
      {};
      TStlPx_vector_bool_(TBranchProxyDirector* director, TBranchProxy *parent, const char *membername, const char *top=0, const char *mid=0) :
         ffPrefix(top,mid),
         obj(director, parent, membername)
      {};
      TBranchProxyHelper ffPrefix;
      InjecTBranchProxyInterface();
      const vector<bool>& At(UInt_t i) {
         static vector<bool> default_val;
         if (!obj.Read()) return default_val;
         vector<bool> *temp = & obj.GetPtr()->at(i);
         if (temp) return *temp; else return default_val;
      }
      const vector<bool>& operator[](Int_t i) { return At(i); }
      const vector<bool>& operator[](UInt_t i) { return At(i); }
      Int_t GetEntries() { return obj.GetPtr()->size(); }
      const vector<vector<bool> >* operator->() { return obj.GetPtr(); }
      operator vector<vector<bool> >*() { return obj.GetPtr(); }
      TObjProxy<vector<vector<bool> > > obj;

   };

   // Proxy for each of the branches, leaves and friends of the tree
   TUIntProxy                                 run;
   TUIntProxy                                 evt;
   TUIntProxy                                 lumiBlock;
   TUIntProxy                                 timeStamp;
   TUIntProxy                                 lhcBCID;
   TFloatProxy                                MBTStimeDiff;
   TFloatProxy                                LArECtimeDiff;
   TUIntProxy                                 syncevt;
   TUIntProxy                                 uidmc;
   TUIntProxy                                 uidtp;
   TUIntProxy                                 nbpvtrue;
   TUIntProxy                                 nbpv;
   TUIntProxy                                 ntrueparticles;
   TUIntProxy                                 ntruetracks;
   TUIntProxy                                 ntracks;
   TUIntProxy                                 nhits;
   TUIntProxy                                 npixclus;
   TUIntProxy                                 nblayerinfo;
   TUIntProxy                                 nmuons;
   TUIntProxy                                 nelectrons;
   TStlSimpleProxy<vector<unsigned int> >     pvtype;
   TStlSimpleProxy<vector<unsigned int> >     pvnbtk;
   TStlSimpleProxy<vector<float> >            pvsumpt;
   TStlSimpleProxy<vector<float> >            pvx;
   TStlSimpleProxy<vector<float> >            pvy;
   TStlSimpleProxy<vector<float> >            pvz;
   TStlSimpleProxy<vector<float> >            errpvx;
   TStlSimpleProxy<vector<float> >            errpvy;
   TStlSimpleProxy<vector<float> >            errpvz;
   TStlSimpleProxy<vector<float> >            covpvxpvy;
   TStlSimpleProxy<vector<float> >            covpvypvz;
   TStlSimpleProxy<vector<float> >            covpvzpvx;
   TStlSimpleProxy<vector<int> >              pvidx;
   TStlSimpleProxy<vector<float> >            d0;
   TStlSimpleProxy<vector<float> >            z0;
   TStlSimpleProxy<vector<float> >            phi;
   TStlSimpleProxy<vector<float> >            theta;
   TStlSimpleProxy<vector<float> >            qop;
   TStlSimpleProxy<vector<float> >            pt;
   TStlSimpleProxy<vector<float> >            eta;
   TStlSimpleProxy<vector<float> >            errd0;
   TStlSimpleProxy<vector<float> >            errz0;
   TStlSimpleProxy<vector<float> >            errphi;
   TStlSimpleProxy<vector<float> >            errtheta;
   TStlSimpleProxy<vector<float> >            errqop;
   TStlSimpleProxy<vector<Long64_t> >         trackPattInfo;
   TStlSimpleProxy<vector<unsigned long> >    trackPropInfo;
   TStlSimpleProxy<vector<int> >              trackFitInfo;
   TStlSimpleProxy<vector<float> >            chi2;
   TStlSimpleProxy<vector<int> >              ndof;
   TStlSimpleProxy<vector<unsigned long> >    hitpattern;
   TStlSimpleProxy<vector<int> >              nbla;
   TStlSimpleProxy<vector<int> >              nblaout;
   TStlSimpleProxy<vector<int> >              npix;
   TStlSimpleProxy<vector<int> >              nsct;
   TStlSimpleProxy<vector<int> >              ntrt;
   TStlSimpleProxy<vector<int> >              npixdead;
   TStlSimpleProxy<vector<int> >              xpectbla;
   TStlSimpleProxy<vector<int> >              nblash;
   TStlSimpleProxy<vector<int> >              npixsh;
   TStlSimpleProxy<vector<int> >              nsctsh;
   TStlSimpleProxy<vector<int> >              nambig;
   TStlSimpleProxy<vector<int> >              nganged;
   TStlSimpleProxy<vector<int> >              npixho;
   TStlSimpleProxy<vector<int> >              nsctho;
   TStlSimpleProxy<vector<float> >            blaresx;
   TStlSimpleProxy<vector<float> >            blaresy;
   TStlSimpleProxy<vector<float> >            blapulx;
   TStlSimpleProxy<vector<float> >            blapuly;
   TStlSimpleProxy<vector<float> >            d0wrtPVunb;
   TStlSimpleProxy<vector<float> >            z0wrtPVunb;
   TStlSimpleProxy<vector<float> >            errd0wrtPVunb;
   TStlSimpleProxy<vector<float> >            errz0wrtPVunb;
   TStlSimpleProxy<vector<float> >            d0wrtPVbias;
   TStlSimpleProxy<vector<float> >            z0wrtPVbias;
   TStlSimpleProxy<vector<float> >            errd0wrtPVbias;
   TStlSimpleProxy<vector<float> >            errz0wrtPVbias;
   TStlSimpleProxy<vector<float> >            d0wrtBSunb;
   TStlSimpleProxy<vector<float> >            z0wrtBSunb;
   TStlSimpleProxy<vector<float> >            errd0wrtBSunb;
   TStlSimpleProxy<vector<float> >            errz0wrtBSunb;
   TStlSimpleProxy<vector<float> >            z0SinThetawrtPVunb;
   TStlSimpleProxy<vector<float> >            errz0SinThetawrtPVunb;
   TStlSimpleProxy<vector<float> >            errPVunbd0;
   TStlSimpleProxy<vector<float> >            errPVunbz0;
   TStlSimpleProxy<vector<float> >            errPVunbz0SinTheta;
   TUIntProxy                                 njetsAK4TPEMJ;
   TIntProxy                                  nghostsAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            phiAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            etaAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            ptAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            eAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            ptCalibFAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            eCalibFAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            etaCalibFAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            phiCalibFAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            emfAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            hecfAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            timeAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            fbadQAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            tg3fAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            fmaxAK4TPEMJ;
   TStlSimpleProxy<vector<short> >            qualityAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            n90AK4TPEMJ;
   TStlSimpleProxy<vector<float> >            n90constituentsAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            BCH_CORR_CELLAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            SamplingMaxAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            JVFAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            nTrkAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            sumPtTrkAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            OriginIndexAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            HECQualityAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            NegativeEAK4TPEMJ;
   TStlSimpleProxy<vector<int> >              ntrakAK4TPEMJ;
   TStlPx_vector_int_                         trakassoc_trkindexAK4TPEMJ;
   TStlSimpleProxy<vector<int> >              nmuonAK4TPEMJ;
   TStlSimpleProxy<vector<int> >              nmuon2AK4TPEMJ;
   TStlPx_vector_int_                         muonassoc_muindexAK4TPEMJ;
   TStlPx_vector_float_                       muonassoc_ptrelAK4TPEMJ;
   TStlPx_vector_float_                       muonassoc_softmuwAK4TPEMJ;
   TStlSimpleProxy<vector<int> >              nelecAK4TPEMJ;
   TStlPx_vector_int_                         elecassoc_trkindexAK4TPEMJ;
   TStlSimpleProxy<vector<short> >            ntagElecAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            electag_ptrelAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            electag_d0AK4TPEMJ;
   TStlSimpleProxy<vector<short> >            electag_indexAK4TPEMJ;
   TStlSimpleProxy<vector<int> >              ntagsAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            ip2d_wAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            ip2dflip_wAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            ip2dpos_wAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            ip2dneg_wAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            ip2dspc_wAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            ip2dspcflip_wAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            ip2dspcpos_wAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            ip2dspcneg_wAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            ip3d_wAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            ip3dflip_wAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            ip3dpos_wAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            ip3dneg_wAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            ip3dspc_wAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            ip3dspcflip_wAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            ip3dspcpos_wAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            ip3dspcneg_wAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            sv0_wAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            sv1_wAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            sv1flip_wAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            sv2_wAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            sv2flip_wAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            cmb_wAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            jetp_wAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            jetpneg_wAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            trkc_wAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            trkcneg_wAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            softm_wAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            softm2_wAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            softmchi2_wAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            softe_wAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            jfitNN_wAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            jfitCOMBNN_wAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            jfitNNneg_wAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            jfitCOMBNNneg_wAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            jfitCOMBNNpos_wAK4TPEMJ;
   TStlSimpleProxy<vector<int> >              ipplus_ntrkAK4TPEMJ;
   TStlSimpleProxy<vector<int> >              svplus_ntrkAK4TPEMJ;
   TStlSimpleProxy<vector<int> >              sv0plus_ntrkAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            ip2d_pbAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            ip2d_puAK4TPEMJ;
   TStlSimpleProxy<vector<int> >              ip2d_ntrkAK4TPEMJ;
   TStlPx_vector_float_                       ip2d_trkwAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            ip3d_pbAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            ip3d_puAK4TPEMJ;
   TStlSimpleProxy<vector<int> >              ip3d_ntrkAK4TPEMJ;
   TStlPx_vector_float_                       ip3d_trkwAK4TPEMJ;
   TStlPx_vector_float_                       jetp_trkwAK4TPEMJ;
   TStlPx_vector_float_                       jetpneg_trkwAK4TPEMJ;
   TStlPx_vector_int_                         ip_trkindexAK4TPEMJ;
   TStlPx_vector_int_                         ip_trkgradeAK4TPEMJ;
   TStlPx_vector_int_                         ip_trkv0intAK4TPEMJ;
   TStlPx_vector_float_                       ip_trkd0valAK4TPEMJ;
   TStlPx_vector_float_                       ip_trkd0sigAK4TPEMJ;
   TStlPx_vector_float_                       ip_trkz0valAK4TPEMJ;
   TStlPx_vector_float_                       ip_trkz0sigAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            sv1_pbAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            sv1_puAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            sv2_pbAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            sv2_puAK4TPEMJ;
   TStlSimpleProxy<vector<int> >              sv_okAK4TPEMJ;
   TStlSimpleProxy<vector<int> >              sv_ntrkvAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            sv_massAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            sv_efrcAK4TPEMJ;
   TStlSimpleProxy<vector<int> >              sv_nv2tAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            sv_xAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            sv_yAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            sv_zAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            sv_err_xAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            sv_err_yAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            sv_err_zAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            sv_cov_xyAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            sv_cov_xzAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            sv_cov_yzAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            sv_chi2AK4TPEMJ;
   TStlSimpleProxy<vector<int> >              sv_ndofAK4TPEMJ;
   TStlPx_vector_int_                         sv_trkindexAK4TPEMJ;
   TStlPx_vector_int_                         sv_badtrkindexAK4TPEMJ;
   TStlSimpleProxy<vector<int> >              sv0_okAK4TPEMJ;
   TStlSimpleProxy<vector<int> >              sv0_ntrkvAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            sv0_massAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            sv0_efrcAK4TPEMJ;
   TStlSimpleProxy<vector<int> >              sv0_nv2tAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            sv0_xAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            sv0_yAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            sv0_zAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            sv0_err_xAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            sv0_err_yAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            sv0_err_zAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            sv0_cov_xyAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            sv0_cov_xzAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            sv0_cov_yzAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            sv0_chi2AK4TPEMJ;
   TStlSimpleProxy<vector<int> >              sv0_ndofAK4TPEMJ;
   TStlPx_vector_int_                         sv0_trkindexAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            jfit_pbAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            jfit_puAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            jfit_pcAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            jfitCOMBNN_pbAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            jfitCOMBNN_puAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            jfitCOMBNN_pcAK4TPEMJ;
   TStlSimpleProxy<vector<int> >              jfit_nvtxntAK4TPEMJ;
   TStlSimpleProxy<vector<int> >              jfit_nvtx1tAK4TPEMJ;
   TStlSimpleProxy<vector<int> >              jfit_ntrkAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            jfit_efrcAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            jfit_massAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            jfit_sig3dAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            jfit_dphiAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            jfit_detaAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            jfit_phiAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            jfit_thetaAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            jfit_phierrAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            jfit_thetaerrAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            jfitFlip_pbAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            jfitFlip_puAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            jfitFlip_pcAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            jfitFlipCOMBNN_pbAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            jfitFlipCOMBNN_puAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            jfitFlipCOMBNN_pcAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            jfitCOMBNNIP3DPos_pbAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            jfitCOMBNNIP3DPos_puAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            jfitCOMBNNIP3DPos_pcAK4TPEMJ;
   TStlSimpleProxy<vector<int> >              jfitFlip_nvtxntAK4TPEMJ;
   TStlSimpleProxy<vector<int> >              jfitFlip_nvtx1tAK4TPEMJ;
   TStlSimpleProxy<vector<int> >              jfitFlip_ntrkAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            jfitFlip_efrcAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            jfitFlip_massAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            jfitFlip_sig3dAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            jfitFlip_dphiAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            jfitFlip_detaAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            jfitFlip_phiAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            jfitFlip_thetaAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            jfitFlip_phierrAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            jfitFlip_thetaerrAK4TPEMJ;
   TStlSimpleProxy<vector<float> >            jfit_probchi2AK4TPEMJ;
   TStlPx_vector_float_                       jfit_vtxpositionAK4TPEMJ;
   TStlPx_vector_float_                       jfit_vtxerrorAK4TPEMJ;
   TStlPx_vector_vector_int__                 jfit_trackindexAtVtxAK4TPEMJ;
   TStlPx_vector_vector_float__               jfit_trackphiAtVtxAK4TPEMJ;
   TStlPx_vector_vector_float__               jfit_trackthetaAtVtxAK4TPEMJ;
   TStlPx_vector_vector_float__               jfit_trackptAtVtxAK4TPEMJ;
   TStlPx_vector_float_                       jfit_twotrkvtx_xAK4TPEMJ;
   TStlPx_vector_float_                       jfit_twotrkvtx_yAK4TPEMJ;
   TStlPx_vector_float_                       jfit_twotrkvtx_zAK4TPEMJ;
   TStlPx_vector_float_                       jfit_twotrkvtx_errxAK4TPEMJ;
   TStlPx_vector_float_                       jfit_twotrkvtx_erryAK4TPEMJ;
   TStlPx_vector_float_                       jfit_twotrkvtx_errzAK4TPEMJ;
   TStlPx_vector_float_                       jfit_twotrkvtx_massAK4TPEMJ;
   TStlPx_vector_float_                       jfit_twotrkvtx_probchi2AK4TPEMJ;
   TStlPx_vector_bool_                        jfit_twotrkvtx_isNeutralAK4TPEMJ;
   TStlPx_vector_int_                         jfit_twotrkvtx_trkindex1AK4TPEMJ;
   TStlPx_vector_int_                         jfit_twotrkvtx_trkindex2AK4TPEMJ;
   TStlSimpleProxy<vector<float> >            jfitFlip_probchi2AK4TPEMJ;
   TStlPx_vector_float_                       jfitFlip_vtxpositionAK4TPEMJ;
   TStlPx_vector_float_                       jfitFlip_vtxerrorAK4TPEMJ;
   TStlPx_vector_int_                         jfitFlip_ntracksAK4TPEMJ;
   TUIntProxy                                 njetsAK6TPEMJ;
   TIntProxy                                  nghostsAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            phiAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            etaAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            ptAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            eAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            ptCalibFAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            eCalibFAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            etaCalibFAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            phiCalibFAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            emfAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            hecfAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            timeAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            fbadQAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            tg3fAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            fmaxAK6TPEMJ;
   TStlSimpleProxy<vector<short> >            qualityAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            n90AK6TPEMJ;
   TStlSimpleProxy<vector<float> >            n90constituentsAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            BCH_CORR_CELLAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            SamplingMaxAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            JVFAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            nTrkAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            sumPtTrkAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            OriginIndexAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            HECQualityAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            NegativeEAK6TPEMJ;
   TStlSimpleProxy<vector<int> >              ntrakAK6TPEMJ;
   TStlPx_vector_int_                         trakassoc_trkindexAK6TPEMJ;
   TStlSimpleProxy<vector<int> >              nmuonAK6TPEMJ;
   TStlSimpleProxy<vector<int> >              nmuon2AK6TPEMJ;
   TStlPx_vector_int_                         muonassoc_muindexAK6TPEMJ;
   TStlPx_vector_float_                       muonassoc_ptrelAK6TPEMJ;
   TStlPx_vector_float_                       muonassoc_softmuwAK6TPEMJ;
   TStlSimpleProxy<vector<int> >              nelecAK6TPEMJ;
   TStlPx_vector_int_                         elecassoc_trkindexAK6TPEMJ;
   TStlSimpleProxy<vector<short> >            ntagElecAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            electag_ptrelAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            electag_d0AK6TPEMJ;
   TStlSimpleProxy<vector<short> >            electag_indexAK6TPEMJ;
   TStlSimpleProxy<vector<int> >              ntagsAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            ip2d_wAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            ip2dflip_wAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            ip2dpos_wAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            ip2dneg_wAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            ip2dspc_wAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            ip2dspcflip_wAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            ip2dspcpos_wAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            ip2dspcneg_wAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            ip3d_wAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            ip3dflip_wAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            ip3dpos_wAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            ip3dneg_wAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            ip3dspc_wAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            ip3dspcflip_wAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            ip3dspcpos_wAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            ip3dspcneg_wAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            sv0_wAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            sv1_wAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            sv1flip_wAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            sv2_wAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            sv2flip_wAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            cmb_wAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            jetp_wAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            jetpneg_wAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            trkc_wAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            trkcneg_wAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            softm_wAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            softm2_wAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            softmchi2_wAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            softe_wAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            jfitNN_wAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            jfitCOMBNN_wAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            jfitNNneg_wAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            jfitCOMBNNneg_wAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            jfitCOMBNNpos_wAK6TPEMJ;
   TStlSimpleProxy<vector<int> >              ipplus_ntrkAK6TPEMJ;
   TStlSimpleProxy<vector<int> >              svplus_ntrkAK6TPEMJ;
   TStlSimpleProxy<vector<int> >              sv0plus_ntrkAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            ip2d_pbAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            ip2d_puAK6TPEMJ;
   TStlSimpleProxy<vector<int> >              ip2d_ntrkAK6TPEMJ;
   TStlPx_vector_float_                       ip2d_trkwAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            ip3d_pbAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            ip3d_puAK6TPEMJ;
   TStlSimpleProxy<vector<int> >              ip3d_ntrkAK6TPEMJ;
   TStlPx_vector_float_                       ip3d_trkwAK6TPEMJ;
   TStlPx_vector_float_                       jetp_trkwAK6TPEMJ;
   TStlPx_vector_float_                       jetpneg_trkwAK6TPEMJ;
   TStlPx_vector_int_                         ip_trkindexAK6TPEMJ;
   TStlPx_vector_int_                         ip_trkgradeAK6TPEMJ;
   TStlPx_vector_int_                         ip_trkv0intAK6TPEMJ;
   TStlPx_vector_float_                       ip_trkd0valAK6TPEMJ;
   TStlPx_vector_float_                       ip_trkd0sigAK6TPEMJ;
   TStlPx_vector_float_                       ip_trkz0valAK6TPEMJ;
   TStlPx_vector_float_                       ip_trkz0sigAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            sv1_pbAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            sv1_puAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            sv2_pbAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            sv2_puAK6TPEMJ;
   TStlSimpleProxy<vector<int> >              sv_okAK6TPEMJ;
   TStlSimpleProxy<vector<int> >              sv_ntrkvAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            sv_massAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            sv_efrcAK6TPEMJ;
   TStlSimpleProxy<vector<int> >              sv_nv2tAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            sv_xAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            sv_yAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            sv_zAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            sv_err_xAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            sv_err_yAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            sv_err_zAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            sv_cov_xyAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            sv_cov_xzAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            sv_cov_yzAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            sv_chi2AK6TPEMJ;
   TStlSimpleProxy<vector<int> >              sv_ndofAK6TPEMJ;
   TStlPx_vector_int_                         sv_trkindexAK6TPEMJ;
   TStlPx_vector_int_                         sv_badtrkindexAK6TPEMJ;
   TStlSimpleProxy<vector<int> >              sv0_okAK6TPEMJ;
   TStlSimpleProxy<vector<int> >              sv0_ntrkvAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            sv0_massAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            sv0_efrcAK6TPEMJ;
   TStlSimpleProxy<vector<int> >              sv0_nv2tAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            sv0_xAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            sv0_yAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            sv0_zAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            sv0_err_xAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            sv0_err_yAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            sv0_err_zAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            sv0_cov_xyAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            sv0_cov_xzAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            sv0_cov_yzAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            sv0_chi2AK6TPEMJ;
   TStlSimpleProxy<vector<int> >              sv0_ndofAK6TPEMJ;
   TStlPx_vector_int_                         sv0_trkindexAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            jfit_pbAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            jfit_puAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            jfit_pcAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            jfitCOMBNN_pbAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            jfitCOMBNN_puAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            jfitCOMBNN_pcAK6TPEMJ;
   TStlSimpleProxy<vector<int> >              jfit_nvtxntAK6TPEMJ;
   TStlSimpleProxy<vector<int> >              jfit_nvtx1tAK6TPEMJ;
   TStlSimpleProxy<vector<int> >              jfit_ntrkAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            jfit_efrcAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            jfit_massAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            jfit_sig3dAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            jfit_dphiAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            jfit_detaAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            jfit_phiAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            jfit_thetaAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            jfit_phierrAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            jfit_thetaerrAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            jfitFlip_pbAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            jfitFlip_puAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            jfitFlip_pcAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            jfitFlipCOMBNN_pbAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            jfitFlipCOMBNN_puAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            jfitFlipCOMBNN_pcAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            jfitCOMBNNIP3DPos_pbAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            jfitCOMBNNIP3DPos_puAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            jfitCOMBNNIP3DPos_pcAK6TPEMJ;
   TStlSimpleProxy<vector<int> >              jfitFlip_nvtxntAK6TPEMJ;
   TStlSimpleProxy<vector<int> >              jfitFlip_nvtx1tAK6TPEMJ;
   TStlSimpleProxy<vector<int> >              jfitFlip_ntrkAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            jfitFlip_efrcAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            jfitFlip_massAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            jfitFlip_sig3dAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            jfitFlip_dphiAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            jfitFlip_detaAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            jfitFlip_phiAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            jfitFlip_thetaAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            jfitFlip_phierrAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            jfitFlip_thetaerrAK6TPEMJ;
   TStlSimpleProxy<vector<float> >            jfit_probchi2AK6TPEMJ;
   TStlPx_vector_float_                       jfit_vtxpositionAK6TPEMJ;
   TStlPx_vector_float_                       jfit_vtxerrorAK6TPEMJ;
   TStlPx_vector_vector_int__                 jfit_trackindexAtVtxAK6TPEMJ;
   TStlPx_vector_vector_float__               jfit_trackphiAtVtxAK6TPEMJ;
   TStlPx_vector_vector_float__               jfit_trackthetaAtVtxAK6TPEMJ;
   TStlPx_vector_vector_float__               jfit_trackptAtVtxAK6TPEMJ;
   TStlPx_vector_float_                       jfit_twotrkvtx_xAK6TPEMJ;
   TStlPx_vector_float_                       jfit_twotrkvtx_yAK6TPEMJ;
   TStlPx_vector_float_                       jfit_twotrkvtx_zAK6TPEMJ;
   TStlPx_vector_float_                       jfit_twotrkvtx_errxAK6TPEMJ;
   TStlPx_vector_float_                       jfit_twotrkvtx_erryAK6TPEMJ;
   TStlPx_vector_float_                       jfit_twotrkvtx_errzAK6TPEMJ;
   TStlPx_vector_float_                       jfit_twotrkvtx_massAK6TPEMJ;
   TStlPx_vector_float_                       jfit_twotrkvtx_probchi2AK6TPEMJ;
   TStlPx_vector_bool_                        jfit_twotrkvtx_isNeutralAK6TPEMJ;
   TStlPx_vector_int_                         jfit_twotrkvtx_trkindex1AK6TPEMJ;
   TStlPx_vector_int_                         jfit_twotrkvtx_trkindex2AK6TPEMJ;
   TStlSimpleProxy<vector<float> >            jfitFlip_probchi2AK6TPEMJ;
   TStlPx_vector_float_                       jfitFlip_vtxpositionAK6TPEMJ;
   TStlPx_vector_float_                       jfitFlip_vtxerrorAK6TPEMJ;
   TStlPx_vector_int_                         jfitFlip_ntracksAK6TPEMJ;
   TFloatProxy                                ExMissMETTopo;
   TFloatProxy                                EyMissMETTopo;
   TFloatProxy                                EtSumMETTopo;
   TFloatProxy                                ExMissMETMuon;
   TFloatProxy                                EyMissMETMuon;
   TFloatProxy                                EtSumMETMuon;
   TStlSimpleProxy<vector<int> >              mutype;
   TStlSimpleProxy<vector<float> >            muphi;
   TStlSimpleProxy<vector<float> >            mupt;
   TStlSimpleProxy<vector<float> >            mueta;
   TStlSimpleProxy<vector<int> >              muauthor;
   TStlSimpleProxy<vector<float> >            mumuChi2;
   TStlSimpleProxy<vector<float> >            mumatchChi2;
   TStlSimpleProxy<vector<float> >            muetCone20;
   TStlSimpleProxy<vector<float> >            muetCone30;
   TStlSimpleProxy<vector<float> >            muetCone40;
   TStlSimpleProxy<vector<float> >            mueLossPar;
   TStlSimpleProxy<vector<float> >            mueLossMeas;
   TStlSimpleProxy<vector<int> >              munMDTHits;
   TStlSimpleProxy<vector<int> >              munMDTHoles;
   TStlSimpleProxy<vector<int> >              munCSCEtaHits;
   TStlSimpleProxy<vector<int> >              munCSCEtaHoles;
   TStlSimpleProxy<vector<int> >              munCSCPhiHits;
   TStlSimpleProxy<vector<int> >              munCSCPhiHoles;
   TStlSimpleProxy<vector<int> >              munRPCEtaHits;
   TStlSimpleProxy<vector<int> >              munRPCEtaHoles;
   TStlSimpleProxy<vector<int> >              munRPCPhiHits;
   TStlSimpleProxy<vector<int> >              munRPCPhiHoles;
   TStlSimpleProxy<vector<int> >              munTGCEtaHits;
   TStlSimpleProxy<vector<int> >              munTGCEtaHoles;
   TStlSimpleProxy<vector<int> >              munTGCPhiHits;
   TStlSimpleProxy<vector<int> >              munTGCPhiHoles;
   TStlSimpleProxy<vector<int> >              mutrkindex;
   TStlSimpleProxy<vector<unsigned short> >   elecEorP;
   TStlSimpleProxy<vector<short> >            elecIdx;
   TStlSimpleProxy<vector<short> >            elecAut;
   TStlSimpleProxy<vector<float> >            elecE;
   TStlSimpleProxy<vector<float> >            eleceta;
   TStlSimpleProxy<vector<float> >            elecphi;
   TStlSimpleProxy<vector<unsigned short> >   elecType;


   ntuple_vtuple(TTree *tree=0) : 
      fChain(0),
      htemp(0),
      fDirector(tree,-1),
      fClass                (TClass::GetClass("ntuple_vtuple")),
      run                                       (&fDirector,"run"),
      evt                                       (&fDirector,"evt"),
      lumiBlock                                 (&fDirector,"lumiBlock"),
      timeStamp                                 (&fDirector,"timeStamp"),
      lhcBCID                                   (&fDirector,"lhcBCID"),
      MBTStimeDiff                              (&fDirector,"MBTStimeDiff"),
      LArECtimeDiff                             (&fDirector,"LArECtimeDiff"),
      syncevt                                   (&fDirector,"syncevt"),
      uidmc                                     (&fDirector,"uidmc"),
      uidtp                                     (&fDirector,"uidtp"),
      nbpvtrue                                  (&fDirector,"nbpvtrue"),
      nbpv                                      (&fDirector,"nbpv"),
      ntrueparticles                            (&fDirector,"ntrueparticles"),
      ntruetracks                               (&fDirector,"ntruetracks"),
      ntracks                                   (&fDirector,"ntracks"),
      nhits                                     (&fDirector,"nhits"),
      npixclus                                  (&fDirector,"npixclus"),
      nblayerinfo                               (&fDirector,"nblayerinfo"),
      nmuons                                    (&fDirector,"nmuons"),
      nelectrons                                (&fDirector,"nelectrons"),
      pvtype                                    (&fDirector,"pvtype"),
      pvnbtk                                    (&fDirector,"pvnbtk"),
      pvsumpt                                   (&fDirector,"pvsumpt"),
      pvx                                       (&fDirector,"pvx"),
      pvy                                       (&fDirector,"pvy"),
      pvz                                       (&fDirector,"pvz"),
      errpvx                                    (&fDirector,"errpvx"),
      errpvy                                    (&fDirector,"errpvy"),
      errpvz                                    (&fDirector,"errpvz"),
      covpvxpvy                                 (&fDirector,"covpvxpvy"),
      covpvypvz                                 (&fDirector,"covpvypvz"),
      covpvzpvx                                 (&fDirector,"covpvzpvx"),
      pvidx                                     (&fDirector,"pvidx"),
      d0                                        (&fDirector,"d0"),
      z0                                        (&fDirector,"z0"),
      phi                                       (&fDirector,"phi"),
      theta                                     (&fDirector,"theta"),
      qop                                       (&fDirector,"qop"),
      pt                                        (&fDirector,"pt"),
      eta                                       (&fDirector,"eta"),
      errd0                                     (&fDirector,"errd0"),
      errz0                                     (&fDirector,"errz0"),
      errphi                                    (&fDirector,"errphi"),
      errtheta                                  (&fDirector,"errtheta"),
      errqop                                    (&fDirector,"errqop"),
      trackPattInfo                             (&fDirector,"trackPattInfo"),
      trackPropInfo                             (&fDirector,"trackPropInfo"),
      trackFitInfo                              (&fDirector,"trackFitInfo"),
      chi2                                      (&fDirector,"chi2"),
      ndof                                      (&fDirector,"ndof"),
      hitpattern                                (&fDirector,"hitpattern"),
      nbla                                      (&fDirector,"nbla"),
      nblaout                                   (&fDirector,"nblaout"),
      npix                                      (&fDirector,"npix"),
      nsct                                      (&fDirector,"nsct"),
      ntrt                                      (&fDirector,"ntrt"),
      npixdead                                  (&fDirector,"npixdead"),
      xpectbla                                  (&fDirector,"xpectbla"),
      nblash                                    (&fDirector,"nblash"),
      npixsh                                    (&fDirector,"npixsh"),
      nsctsh                                    (&fDirector,"nsctsh"),
      nambig                                    (&fDirector,"nambig"),
      nganged                                   (&fDirector,"nganged"),
      npixho                                    (&fDirector,"npixho"),
      nsctho                                    (&fDirector,"nsctho"),
      blaresx                                   (&fDirector,"blaresx"),
      blaresy                                   (&fDirector,"blaresy"),
      blapulx                                   (&fDirector,"blapulx"),
      blapuly                                   (&fDirector,"blapuly"),
      d0wrtPVunb                                (&fDirector,"d0wrtPVunb"),
      z0wrtPVunb                                (&fDirector,"z0wrtPVunb"),
      errd0wrtPVunb                             (&fDirector,"errd0wrtPVunb"),
      errz0wrtPVunb                             (&fDirector,"errz0wrtPVunb"),
      d0wrtPVbias                               (&fDirector,"d0wrtPVbias"),
      z0wrtPVbias                               (&fDirector,"z0wrtPVbias"),
      errd0wrtPVbias                            (&fDirector,"errd0wrtPVbias"),
      errz0wrtPVbias                            (&fDirector,"errz0wrtPVbias"),
      d0wrtBSunb                                (&fDirector,"d0wrtBSunb"),
      z0wrtBSunb                                (&fDirector,"z0wrtBSunb"),
      errd0wrtBSunb                             (&fDirector,"errd0wrtBSunb"),
      errz0wrtBSunb                             (&fDirector,"errz0wrtBSunb"),
      z0SinThetawrtPVunb                        (&fDirector,"z0SinThetawrtPVunb"),
      errz0SinThetawrtPVunb                     (&fDirector,"errz0SinThetawrtPVunb"),
      errPVunbd0                                (&fDirector,"errPVunbd0"),
      errPVunbz0                                (&fDirector,"errPVunbz0"),
      errPVunbz0SinTheta                        (&fDirector,"errPVunbz0SinTheta"),
      njetsAK4TPEMJ                             (&fDirector,"njetsAK4TPEMJ"),
      nghostsAK4TPEMJ                           (&fDirector,"nghostsAK4TPEMJ"),
      phiAK4TPEMJ                               (&fDirector,"phiAK4TPEMJ"),
      etaAK4TPEMJ                               (&fDirector,"etaAK4TPEMJ"),
      ptAK4TPEMJ                                (&fDirector,"ptAK4TPEMJ"),
      eAK4TPEMJ                                 (&fDirector,"eAK4TPEMJ"),
      ptCalibFAK4TPEMJ                          (&fDirector,"ptCalibFAK4TPEMJ"),
      eCalibFAK4TPEMJ                           (&fDirector,"eCalibFAK4TPEMJ"),
      etaCalibFAK4TPEMJ                         (&fDirector,"etaCalibFAK4TPEMJ"),
      phiCalibFAK4TPEMJ                         (&fDirector,"phiCalibFAK4TPEMJ"),
      emfAK4TPEMJ                               (&fDirector,"emfAK4TPEMJ"),
      hecfAK4TPEMJ                              (&fDirector,"hecfAK4TPEMJ"),
      timeAK4TPEMJ                              (&fDirector,"timeAK4TPEMJ"),
      fbadQAK4TPEMJ                             (&fDirector,"fbadQAK4TPEMJ"),
      tg3fAK4TPEMJ                              (&fDirector,"tg3fAK4TPEMJ"),
      fmaxAK4TPEMJ                              (&fDirector,"fmaxAK4TPEMJ"),
      qualityAK4TPEMJ                           (&fDirector,"qualityAK4TPEMJ"),
      n90AK4TPEMJ                               (&fDirector,"n90AK4TPEMJ"),
      n90constituentsAK4TPEMJ                   (&fDirector,"n90constituentsAK4TPEMJ"),
      BCH_CORR_CELLAK4TPEMJ                     (&fDirector,"BCH_CORR_CELLAK4TPEMJ"),
      SamplingMaxAK4TPEMJ                       (&fDirector,"SamplingMaxAK4TPEMJ"),
      JVFAK4TPEMJ                               (&fDirector,"JVFAK4TPEMJ"),
      nTrkAK4TPEMJ                              (&fDirector,"nTrkAK4TPEMJ"),
      sumPtTrkAK4TPEMJ                          (&fDirector,"sumPtTrkAK4TPEMJ"),
      OriginIndexAK4TPEMJ                       (&fDirector,"OriginIndexAK4TPEMJ"),
      HECQualityAK4TPEMJ                        (&fDirector,"HECQualityAK4TPEMJ"),
      NegativeEAK4TPEMJ                         (&fDirector,"NegativeEAK4TPEMJ"),
      ntrakAK4TPEMJ                             (&fDirector,"ntrakAK4TPEMJ"),
      trakassoc_trkindexAK4TPEMJ                (&fDirector,"trakassoc_trkindexAK4TPEMJ"),
      nmuonAK4TPEMJ                             (&fDirector,"nmuonAK4TPEMJ"),
      nmuon2AK4TPEMJ                            (&fDirector,"nmuon2AK4TPEMJ"),
      muonassoc_muindexAK4TPEMJ                 (&fDirector,"muonassoc_muindexAK4TPEMJ"),
      muonassoc_ptrelAK4TPEMJ                   (&fDirector,"muonassoc_ptrelAK4TPEMJ"),
      muonassoc_softmuwAK4TPEMJ                 (&fDirector,"muonassoc_softmuwAK4TPEMJ"),
      nelecAK4TPEMJ                             (&fDirector,"nelecAK4TPEMJ"),
      elecassoc_trkindexAK4TPEMJ                (&fDirector,"elecassoc_trkindexAK4TPEMJ"),
      ntagElecAK4TPEMJ                          (&fDirector,"ntagElecAK4TPEMJ"),
      electag_ptrelAK4TPEMJ                     (&fDirector,"electag_ptrelAK4TPEMJ"),
      electag_d0AK4TPEMJ                        (&fDirector,"electag_d0AK4TPEMJ"),
      electag_indexAK4TPEMJ                     (&fDirector,"electag_indexAK4TPEMJ"),
      ntagsAK4TPEMJ                             (&fDirector,"ntagsAK4TPEMJ"),
      ip2d_wAK4TPEMJ                            (&fDirector,"ip2d_wAK4TPEMJ"),
      ip2dflip_wAK4TPEMJ                        (&fDirector,"ip2dflip_wAK4TPEMJ"),
      ip2dpos_wAK4TPEMJ                         (&fDirector,"ip2dpos_wAK4TPEMJ"),
      ip2dneg_wAK4TPEMJ                         (&fDirector,"ip2dneg_wAK4TPEMJ"),
      ip2dspc_wAK4TPEMJ                         (&fDirector,"ip2dspc_wAK4TPEMJ"),
      ip2dspcflip_wAK4TPEMJ                     (&fDirector,"ip2dspcflip_wAK4TPEMJ"),
      ip2dspcpos_wAK4TPEMJ                      (&fDirector,"ip2dspcpos_wAK4TPEMJ"),
      ip2dspcneg_wAK4TPEMJ                      (&fDirector,"ip2dspcneg_wAK4TPEMJ"),
      ip3d_wAK4TPEMJ                            (&fDirector,"ip3d_wAK4TPEMJ"),
      ip3dflip_wAK4TPEMJ                        (&fDirector,"ip3dflip_wAK4TPEMJ"),
      ip3dpos_wAK4TPEMJ                         (&fDirector,"ip3dpos_wAK4TPEMJ"),
      ip3dneg_wAK4TPEMJ                         (&fDirector,"ip3dneg_wAK4TPEMJ"),
      ip3dspc_wAK4TPEMJ                         (&fDirector,"ip3dspc_wAK4TPEMJ"),
      ip3dspcflip_wAK4TPEMJ                     (&fDirector,"ip3dspcflip_wAK4TPEMJ"),
      ip3dspcpos_wAK4TPEMJ                      (&fDirector,"ip3dspcpos_wAK4TPEMJ"),
      ip3dspcneg_wAK4TPEMJ                      (&fDirector,"ip3dspcneg_wAK4TPEMJ"),
      sv0_wAK4TPEMJ                             (&fDirector,"sv0_wAK4TPEMJ"),
      sv1_wAK4TPEMJ                             (&fDirector,"sv1_wAK4TPEMJ"),
      sv1flip_wAK4TPEMJ                         (&fDirector,"sv1flip_wAK4TPEMJ"),
      sv2_wAK4TPEMJ                             (&fDirector,"sv2_wAK4TPEMJ"),
      sv2flip_wAK4TPEMJ                         (&fDirector,"sv2flip_wAK4TPEMJ"),
      cmb_wAK4TPEMJ                             (&fDirector,"cmb_wAK4TPEMJ"),
      jetp_wAK4TPEMJ                            (&fDirector,"jetp_wAK4TPEMJ"),
      jetpneg_wAK4TPEMJ                         (&fDirector,"jetpneg_wAK4TPEMJ"),
      trkc_wAK4TPEMJ                            (&fDirector,"trkc_wAK4TPEMJ"),
      trkcneg_wAK4TPEMJ                         (&fDirector,"trkcneg_wAK4TPEMJ"),
      softm_wAK4TPEMJ                           (&fDirector,"softm_wAK4TPEMJ"),
      softm2_wAK4TPEMJ                          (&fDirector,"softm2_wAK4TPEMJ"),
      softmchi2_wAK4TPEMJ                       (&fDirector,"softmchi2_wAK4TPEMJ"),
      softe_wAK4TPEMJ                           (&fDirector,"softe_wAK4TPEMJ"),
      jfitNN_wAK4TPEMJ                          (&fDirector,"jfitNN_wAK4TPEMJ"),
      jfitCOMBNN_wAK4TPEMJ                      (&fDirector,"jfitCOMBNN_wAK4TPEMJ"),
      jfitNNneg_wAK4TPEMJ                       (&fDirector,"jfitNNneg_wAK4TPEMJ"),
      jfitCOMBNNneg_wAK4TPEMJ                   (&fDirector,"jfitCOMBNNneg_wAK4TPEMJ"),
      jfitCOMBNNpos_wAK4TPEMJ                   (&fDirector,"jfitCOMBNNpos_wAK4TPEMJ"),
      ipplus_ntrkAK4TPEMJ                       (&fDirector,"ipplus_ntrkAK4TPEMJ"),
      svplus_ntrkAK4TPEMJ                       (&fDirector,"svplus_ntrkAK4TPEMJ"),
      sv0plus_ntrkAK4TPEMJ                      (&fDirector,"sv0plus_ntrkAK4TPEMJ"),
      ip2d_pbAK4TPEMJ                           (&fDirector,"ip2d_pbAK4TPEMJ"),
      ip2d_puAK4TPEMJ                           (&fDirector,"ip2d_puAK4TPEMJ"),
      ip2d_ntrkAK4TPEMJ                         (&fDirector,"ip2d_ntrkAK4TPEMJ"),
      ip2d_trkwAK4TPEMJ                         (&fDirector,"ip2d_trkwAK4TPEMJ"),
      ip3d_pbAK4TPEMJ                           (&fDirector,"ip3d_pbAK4TPEMJ"),
      ip3d_puAK4TPEMJ                           (&fDirector,"ip3d_puAK4TPEMJ"),
      ip3d_ntrkAK4TPEMJ                         (&fDirector,"ip3d_ntrkAK4TPEMJ"),
      ip3d_trkwAK4TPEMJ                         (&fDirector,"ip3d_trkwAK4TPEMJ"),
      jetp_trkwAK4TPEMJ                         (&fDirector,"jetp_trkwAK4TPEMJ"),
      jetpneg_trkwAK4TPEMJ                      (&fDirector,"jetpneg_trkwAK4TPEMJ"),
      ip_trkindexAK4TPEMJ                       (&fDirector,"ip_trkindexAK4TPEMJ"),
      ip_trkgradeAK4TPEMJ                       (&fDirector,"ip_trkgradeAK4TPEMJ"),
      ip_trkv0intAK4TPEMJ                       (&fDirector,"ip_trkv0intAK4TPEMJ"),
      ip_trkd0valAK4TPEMJ                       (&fDirector,"ip_trkd0valAK4TPEMJ"),
      ip_trkd0sigAK4TPEMJ                       (&fDirector,"ip_trkd0sigAK4TPEMJ"),
      ip_trkz0valAK4TPEMJ                       (&fDirector,"ip_trkz0valAK4TPEMJ"),
      ip_trkz0sigAK4TPEMJ                       (&fDirector,"ip_trkz0sigAK4TPEMJ"),
      sv1_pbAK4TPEMJ                            (&fDirector,"sv1_pbAK4TPEMJ"),
      sv1_puAK4TPEMJ                            (&fDirector,"sv1_puAK4TPEMJ"),
      sv2_pbAK4TPEMJ                            (&fDirector,"sv2_pbAK4TPEMJ"),
      sv2_puAK4TPEMJ                            (&fDirector,"sv2_puAK4TPEMJ"),
      sv_okAK4TPEMJ                             (&fDirector,"sv_okAK4TPEMJ"),
      sv_ntrkvAK4TPEMJ                          (&fDirector,"sv_ntrkvAK4TPEMJ"),
      sv_massAK4TPEMJ                           (&fDirector,"sv_massAK4TPEMJ"),
      sv_efrcAK4TPEMJ                           (&fDirector,"sv_efrcAK4TPEMJ"),
      sv_nv2tAK4TPEMJ                           (&fDirector,"sv_nv2tAK4TPEMJ"),
      sv_xAK4TPEMJ                              (&fDirector,"sv_xAK4TPEMJ"),
      sv_yAK4TPEMJ                              (&fDirector,"sv_yAK4TPEMJ"),
      sv_zAK4TPEMJ                              (&fDirector,"sv_zAK4TPEMJ"),
      sv_err_xAK4TPEMJ                          (&fDirector,"sv_err_xAK4TPEMJ"),
      sv_err_yAK4TPEMJ                          (&fDirector,"sv_err_yAK4TPEMJ"),
      sv_err_zAK4TPEMJ                          (&fDirector,"sv_err_zAK4TPEMJ"),
      sv_cov_xyAK4TPEMJ                         (&fDirector,"sv_cov_xyAK4TPEMJ"),
      sv_cov_xzAK4TPEMJ                         (&fDirector,"sv_cov_xzAK4TPEMJ"),
      sv_cov_yzAK4TPEMJ                         (&fDirector,"sv_cov_yzAK4TPEMJ"),
      sv_chi2AK4TPEMJ                           (&fDirector,"sv_chi2AK4TPEMJ"),
      sv_ndofAK4TPEMJ                           (&fDirector,"sv_ndofAK4TPEMJ"),
      sv_trkindexAK4TPEMJ                       (&fDirector,"sv_trkindexAK4TPEMJ"),
      sv_badtrkindexAK4TPEMJ                    (&fDirector,"sv_badtrkindexAK4TPEMJ"),
      sv0_okAK4TPEMJ                            (&fDirector,"sv0_okAK4TPEMJ"),
      sv0_ntrkvAK4TPEMJ                         (&fDirector,"sv0_ntrkvAK4TPEMJ"),
      sv0_massAK4TPEMJ                          (&fDirector,"sv0_massAK4TPEMJ"),
      sv0_efrcAK4TPEMJ                          (&fDirector,"sv0_efrcAK4TPEMJ"),
      sv0_nv2tAK4TPEMJ                          (&fDirector,"sv0_nv2tAK4TPEMJ"),
      sv0_xAK4TPEMJ                             (&fDirector,"sv0_xAK4TPEMJ"),
      sv0_yAK4TPEMJ                             (&fDirector,"sv0_yAK4TPEMJ"),
      sv0_zAK4TPEMJ                             (&fDirector,"sv0_zAK4TPEMJ"),
      sv0_err_xAK4TPEMJ                         (&fDirector,"sv0_err_xAK4TPEMJ"),
      sv0_err_yAK4TPEMJ                         (&fDirector,"sv0_err_yAK4TPEMJ"),
      sv0_err_zAK4TPEMJ                         (&fDirector,"sv0_err_zAK4TPEMJ"),
      sv0_cov_xyAK4TPEMJ                        (&fDirector,"sv0_cov_xyAK4TPEMJ"),
      sv0_cov_xzAK4TPEMJ                        (&fDirector,"sv0_cov_xzAK4TPEMJ"),
      sv0_cov_yzAK4TPEMJ                        (&fDirector,"sv0_cov_yzAK4TPEMJ"),
      sv0_chi2AK4TPEMJ                          (&fDirector,"sv0_chi2AK4TPEMJ"),
      sv0_ndofAK4TPEMJ                          (&fDirector,"sv0_ndofAK4TPEMJ"),
      sv0_trkindexAK4TPEMJ                      (&fDirector,"sv0_trkindexAK4TPEMJ"),
      jfit_pbAK4TPEMJ                           (&fDirector,"jfit_pbAK4TPEMJ"),
      jfit_puAK4TPEMJ                           (&fDirector,"jfit_puAK4TPEMJ"),
      jfit_pcAK4TPEMJ                           (&fDirector,"jfit_pcAK4TPEMJ"),
      jfitCOMBNN_pbAK4TPEMJ                     (&fDirector,"jfitCOMBNN_pbAK4TPEMJ"),
      jfitCOMBNN_puAK4TPEMJ                     (&fDirector,"jfitCOMBNN_puAK4TPEMJ"),
      jfitCOMBNN_pcAK4TPEMJ                     (&fDirector,"jfitCOMBNN_pcAK4TPEMJ"),
      jfit_nvtxntAK4TPEMJ                       (&fDirector,"jfit_nvtxntAK4TPEMJ"),
      jfit_nvtx1tAK4TPEMJ                       (&fDirector,"jfit_nvtx1tAK4TPEMJ"),
      jfit_ntrkAK4TPEMJ                         (&fDirector,"jfit_ntrkAK4TPEMJ"),
      jfit_efrcAK4TPEMJ                         (&fDirector,"jfit_efrcAK4TPEMJ"),
      jfit_massAK4TPEMJ                         (&fDirector,"jfit_massAK4TPEMJ"),
      jfit_sig3dAK4TPEMJ                        (&fDirector,"jfit_sig3dAK4TPEMJ"),
      jfit_dphiAK4TPEMJ                         (&fDirector,"jfit_dphiAK4TPEMJ"),
      jfit_detaAK4TPEMJ                         (&fDirector,"jfit_detaAK4TPEMJ"),
      jfit_phiAK4TPEMJ                          (&fDirector,"jfit_phiAK4TPEMJ"),
      jfit_thetaAK4TPEMJ                        (&fDirector,"jfit_thetaAK4TPEMJ"),
      jfit_phierrAK4TPEMJ                       (&fDirector,"jfit_phierrAK4TPEMJ"),
      jfit_thetaerrAK4TPEMJ                     (&fDirector,"jfit_thetaerrAK4TPEMJ"),
      jfitFlip_pbAK4TPEMJ                       (&fDirector,"jfitFlip_pbAK4TPEMJ"),
      jfitFlip_puAK4TPEMJ                       (&fDirector,"jfitFlip_puAK4TPEMJ"),
      jfitFlip_pcAK4TPEMJ                       (&fDirector,"jfitFlip_pcAK4TPEMJ"),
      jfitFlipCOMBNN_pbAK4TPEMJ                 (&fDirector,"jfitFlipCOMBNN_pbAK4TPEMJ"),
      jfitFlipCOMBNN_puAK4TPEMJ                 (&fDirector,"jfitFlipCOMBNN_puAK4TPEMJ"),
      jfitFlipCOMBNN_pcAK4TPEMJ                 (&fDirector,"jfitFlipCOMBNN_pcAK4TPEMJ"),
      jfitCOMBNNIP3DPos_pbAK4TPEMJ              (&fDirector,"jfitCOMBNNIP3DPos_pbAK4TPEMJ"),
      jfitCOMBNNIP3DPos_puAK4TPEMJ              (&fDirector,"jfitCOMBNNIP3DPos_puAK4TPEMJ"),
      jfitCOMBNNIP3DPos_pcAK4TPEMJ              (&fDirector,"jfitCOMBNNIP3DPos_pcAK4TPEMJ"),
      jfitFlip_nvtxntAK4TPEMJ                   (&fDirector,"jfitFlip_nvtxntAK4TPEMJ"),
      jfitFlip_nvtx1tAK4TPEMJ                   (&fDirector,"jfitFlip_nvtx1tAK4TPEMJ"),
      jfitFlip_ntrkAK4TPEMJ                     (&fDirector,"jfitFlip_ntrkAK4TPEMJ"),
      jfitFlip_efrcAK4TPEMJ                     (&fDirector,"jfitFlip_efrcAK4TPEMJ"),
      jfitFlip_massAK4TPEMJ                     (&fDirector,"jfitFlip_massAK4TPEMJ"),
      jfitFlip_sig3dAK4TPEMJ                    (&fDirector,"jfitFlip_sig3dAK4TPEMJ"),
      jfitFlip_dphiAK4TPEMJ                     (&fDirector,"jfitFlip_dphiAK4TPEMJ"),
      jfitFlip_detaAK4TPEMJ                     (&fDirector,"jfitFlip_detaAK4TPEMJ"),
      jfitFlip_phiAK4TPEMJ                      (&fDirector,"jfitFlip_phiAK4TPEMJ"),
      jfitFlip_thetaAK4TPEMJ                    (&fDirector,"jfitFlip_thetaAK4TPEMJ"),
      jfitFlip_phierrAK4TPEMJ                   (&fDirector,"jfitFlip_phierrAK4TPEMJ"),
      jfitFlip_thetaerrAK4TPEMJ                 (&fDirector,"jfitFlip_thetaerrAK4TPEMJ"),
      jfit_probchi2AK4TPEMJ                     (&fDirector,"jfit_probchi2AK4TPEMJ"),
      jfit_vtxpositionAK4TPEMJ                  (&fDirector,"jfit_vtxpositionAK4TPEMJ"),
      jfit_vtxerrorAK4TPEMJ                     (&fDirector,"jfit_vtxerrorAK4TPEMJ"),
      jfit_trackindexAtVtxAK4TPEMJ              (&fDirector,"jfit_trackindexAtVtxAK4TPEMJ"),
      jfit_trackphiAtVtxAK4TPEMJ                (&fDirector,"jfit_trackphiAtVtxAK4TPEMJ"),
      jfit_trackthetaAtVtxAK4TPEMJ              (&fDirector,"jfit_trackthetaAtVtxAK4TPEMJ"),
      jfit_trackptAtVtxAK4TPEMJ                 (&fDirector,"jfit_trackptAtVtxAK4TPEMJ"),
      jfit_twotrkvtx_xAK4TPEMJ                  (&fDirector,"jfit_twotrkvtx_xAK4TPEMJ"),
      jfit_twotrkvtx_yAK4TPEMJ                  (&fDirector,"jfit_twotrkvtx_yAK4TPEMJ"),
      jfit_twotrkvtx_zAK4TPEMJ                  (&fDirector,"jfit_twotrkvtx_zAK4TPEMJ"),
      jfit_twotrkvtx_errxAK4TPEMJ               (&fDirector,"jfit_twotrkvtx_errxAK4TPEMJ"),
      jfit_twotrkvtx_erryAK4TPEMJ               (&fDirector,"jfit_twotrkvtx_erryAK4TPEMJ"),
      jfit_twotrkvtx_errzAK4TPEMJ               (&fDirector,"jfit_twotrkvtx_errzAK4TPEMJ"),
      jfit_twotrkvtx_massAK4TPEMJ               (&fDirector,"jfit_twotrkvtx_massAK4TPEMJ"),
      jfit_twotrkvtx_probchi2AK4TPEMJ           (&fDirector,"jfit_twotrkvtx_probchi2AK4TPEMJ"),
      jfit_twotrkvtx_isNeutralAK4TPEMJ          (&fDirector,"jfit_twotrkvtx_isNeutralAK4TPEMJ"),
      jfit_twotrkvtx_trkindex1AK4TPEMJ          (&fDirector,"jfit_twotrkvtx_trkindex1AK4TPEMJ"),
      jfit_twotrkvtx_trkindex2AK4TPEMJ          (&fDirector,"jfit_twotrkvtx_trkindex2AK4TPEMJ"),
      jfitFlip_probchi2AK4TPEMJ                 (&fDirector,"jfitFlip_probchi2AK4TPEMJ"),
      jfitFlip_vtxpositionAK4TPEMJ              (&fDirector,"jfitFlip_vtxpositionAK4TPEMJ"),
      jfitFlip_vtxerrorAK4TPEMJ                 (&fDirector,"jfitFlip_vtxerrorAK4TPEMJ"),
      jfitFlip_ntracksAK4TPEMJ                  (&fDirector,"jfitFlip_ntracksAK4TPEMJ"),
      njetsAK6TPEMJ                             (&fDirector,"njetsAK6TPEMJ"),
      nghostsAK6TPEMJ                           (&fDirector,"nghostsAK6TPEMJ"),
      phiAK6TPEMJ                               (&fDirector,"phiAK6TPEMJ"),
      etaAK6TPEMJ                               (&fDirector,"etaAK6TPEMJ"),
      ptAK6TPEMJ                                (&fDirector,"ptAK6TPEMJ"),
      eAK6TPEMJ                                 (&fDirector,"eAK6TPEMJ"),
      ptCalibFAK6TPEMJ                          (&fDirector,"ptCalibFAK6TPEMJ"),
      eCalibFAK6TPEMJ                           (&fDirector,"eCalibFAK6TPEMJ"),
      etaCalibFAK6TPEMJ                         (&fDirector,"etaCalibFAK6TPEMJ"),
      phiCalibFAK6TPEMJ                         (&fDirector,"phiCalibFAK6TPEMJ"),
      emfAK6TPEMJ                               (&fDirector,"emfAK6TPEMJ"),
      hecfAK6TPEMJ                              (&fDirector,"hecfAK6TPEMJ"),
      timeAK6TPEMJ                              (&fDirector,"timeAK6TPEMJ"),
      fbadQAK6TPEMJ                             (&fDirector,"fbadQAK6TPEMJ"),
      tg3fAK6TPEMJ                              (&fDirector,"tg3fAK6TPEMJ"),
      fmaxAK6TPEMJ                              (&fDirector,"fmaxAK6TPEMJ"),
      qualityAK6TPEMJ                           (&fDirector,"qualityAK6TPEMJ"),
      n90AK6TPEMJ                               (&fDirector,"n90AK6TPEMJ"),
      n90constituentsAK6TPEMJ                   (&fDirector,"n90constituentsAK6TPEMJ"),
      BCH_CORR_CELLAK6TPEMJ                     (&fDirector,"BCH_CORR_CELLAK6TPEMJ"),
      SamplingMaxAK6TPEMJ                       (&fDirector,"SamplingMaxAK6TPEMJ"),
      JVFAK6TPEMJ                               (&fDirector,"JVFAK6TPEMJ"),
      nTrkAK6TPEMJ                              (&fDirector,"nTrkAK6TPEMJ"),
      sumPtTrkAK6TPEMJ                          (&fDirector,"sumPtTrkAK6TPEMJ"),
      OriginIndexAK6TPEMJ                       (&fDirector,"OriginIndexAK6TPEMJ"),
      HECQualityAK6TPEMJ                        (&fDirector,"HECQualityAK6TPEMJ"),
      NegativeEAK6TPEMJ                         (&fDirector,"NegativeEAK6TPEMJ"),
      ntrakAK6TPEMJ                             (&fDirector,"ntrakAK6TPEMJ"),
      trakassoc_trkindexAK6TPEMJ                (&fDirector,"trakassoc_trkindexAK6TPEMJ"),
      nmuonAK6TPEMJ                             (&fDirector,"nmuonAK6TPEMJ"),
      nmuon2AK6TPEMJ                            (&fDirector,"nmuon2AK6TPEMJ"),
      muonassoc_muindexAK6TPEMJ                 (&fDirector,"muonassoc_muindexAK6TPEMJ"),
      muonassoc_ptrelAK6TPEMJ                   (&fDirector,"muonassoc_ptrelAK6TPEMJ"),
      muonassoc_softmuwAK6TPEMJ                 (&fDirector,"muonassoc_softmuwAK6TPEMJ"),
      nelecAK6TPEMJ                             (&fDirector,"nelecAK6TPEMJ"),
      elecassoc_trkindexAK6TPEMJ                (&fDirector,"elecassoc_trkindexAK6TPEMJ"),
      ntagElecAK6TPEMJ                          (&fDirector,"ntagElecAK6TPEMJ"),
      electag_ptrelAK6TPEMJ                     (&fDirector,"electag_ptrelAK6TPEMJ"),
      electag_d0AK6TPEMJ                        (&fDirector,"electag_d0AK6TPEMJ"),
      electag_indexAK6TPEMJ                     (&fDirector,"electag_indexAK6TPEMJ"),
      ntagsAK6TPEMJ                             (&fDirector,"ntagsAK6TPEMJ"),
      ip2d_wAK6TPEMJ                            (&fDirector,"ip2d_wAK6TPEMJ"),
      ip2dflip_wAK6TPEMJ                        (&fDirector,"ip2dflip_wAK6TPEMJ"),
      ip2dpos_wAK6TPEMJ                         (&fDirector,"ip2dpos_wAK6TPEMJ"),
      ip2dneg_wAK6TPEMJ                         (&fDirector,"ip2dneg_wAK6TPEMJ"),
      ip2dspc_wAK6TPEMJ                         (&fDirector,"ip2dspc_wAK6TPEMJ"),
      ip2dspcflip_wAK6TPEMJ                     (&fDirector,"ip2dspcflip_wAK6TPEMJ"),
      ip2dspcpos_wAK6TPEMJ                      (&fDirector,"ip2dspcpos_wAK6TPEMJ"),
      ip2dspcneg_wAK6TPEMJ                      (&fDirector,"ip2dspcneg_wAK6TPEMJ"),
      ip3d_wAK6TPEMJ                            (&fDirector,"ip3d_wAK6TPEMJ"),
      ip3dflip_wAK6TPEMJ                        (&fDirector,"ip3dflip_wAK6TPEMJ"),
      ip3dpos_wAK6TPEMJ                         (&fDirector,"ip3dpos_wAK6TPEMJ"),
      ip3dneg_wAK6TPEMJ                         (&fDirector,"ip3dneg_wAK6TPEMJ"),
      ip3dspc_wAK6TPEMJ                         (&fDirector,"ip3dspc_wAK6TPEMJ"),
      ip3dspcflip_wAK6TPEMJ                     (&fDirector,"ip3dspcflip_wAK6TPEMJ"),
      ip3dspcpos_wAK6TPEMJ                      (&fDirector,"ip3dspcpos_wAK6TPEMJ"),
      ip3dspcneg_wAK6TPEMJ                      (&fDirector,"ip3dspcneg_wAK6TPEMJ"),
      sv0_wAK6TPEMJ                             (&fDirector,"sv0_wAK6TPEMJ"),
      sv1_wAK6TPEMJ                             (&fDirector,"sv1_wAK6TPEMJ"),
      sv1flip_wAK6TPEMJ                         (&fDirector,"sv1flip_wAK6TPEMJ"),
      sv2_wAK6TPEMJ                             (&fDirector,"sv2_wAK6TPEMJ"),
      sv2flip_wAK6TPEMJ                         (&fDirector,"sv2flip_wAK6TPEMJ"),
      cmb_wAK6TPEMJ                             (&fDirector,"cmb_wAK6TPEMJ"),
      jetp_wAK6TPEMJ                            (&fDirector,"jetp_wAK6TPEMJ"),
      jetpneg_wAK6TPEMJ                         (&fDirector,"jetpneg_wAK6TPEMJ"),
      trkc_wAK6TPEMJ                            (&fDirector,"trkc_wAK6TPEMJ"),
      trkcneg_wAK6TPEMJ                         (&fDirector,"trkcneg_wAK6TPEMJ"),
      softm_wAK6TPEMJ                           (&fDirector,"softm_wAK6TPEMJ"),
      softm2_wAK6TPEMJ                          (&fDirector,"softm2_wAK6TPEMJ"),
      softmchi2_wAK6TPEMJ                       (&fDirector,"softmchi2_wAK6TPEMJ"),
      softe_wAK6TPEMJ                           (&fDirector,"softe_wAK6TPEMJ"),
      jfitNN_wAK6TPEMJ                          (&fDirector,"jfitNN_wAK6TPEMJ"),
      jfitCOMBNN_wAK6TPEMJ                      (&fDirector,"jfitCOMBNN_wAK6TPEMJ"),
      jfitNNneg_wAK6TPEMJ                       (&fDirector,"jfitNNneg_wAK6TPEMJ"),
      jfitCOMBNNneg_wAK6TPEMJ                   (&fDirector,"jfitCOMBNNneg_wAK6TPEMJ"),
      jfitCOMBNNpos_wAK6TPEMJ                   (&fDirector,"jfitCOMBNNpos_wAK6TPEMJ"),
      ipplus_ntrkAK6TPEMJ                       (&fDirector,"ipplus_ntrkAK6TPEMJ"),
      svplus_ntrkAK6TPEMJ                       (&fDirector,"svplus_ntrkAK6TPEMJ"),
      sv0plus_ntrkAK6TPEMJ                      (&fDirector,"sv0plus_ntrkAK6TPEMJ"),
      ip2d_pbAK6TPEMJ                           (&fDirector,"ip2d_pbAK6TPEMJ"),
      ip2d_puAK6TPEMJ                           (&fDirector,"ip2d_puAK6TPEMJ"),
      ip2d_ntrkAK6TPEMJ                         (&fDirector,"ip2d_ntrkAK6TPEMJ"),
      ip2d_trkwAK6TPEMJ                         (&fDirector,"ip2d_trkwAK6TPEMJ"),
      ip3d_pbAK6TPEMJ                           (&fDirector,"ip3d_pbAK6TPEMJ"),
      ip3d_puAK6TPEMJ                           (&fDirector,"ip3d_puAK6TPEMJ"),
      ip3d_ntrkAK6TPEMJ                         (&fDirector,"ip3d_ntrkAK6TPEMJ"),
      ip3d_trkwAK6TPEMJ                         (&fDirector,"ip3d_trkwAK6TPEMJ"),
      jetp_trkwAK6TPEMJ                         (&fDirector,"jetp_trkwAK6TPEMJ"),
      jetpneg_trkwAK6TPEMJ                      (&fDirector,"jetpneg_trkwAK6TPEMJ"),
      ip_trkindexAK6TPEMJ                       (&fDirector,"ip_trkindexAK6TPEMJ"),
      ip_trkgradeAK6TPEMJ                       (&fDirector,"ip_trkgradeAK6TPEMJ"),
      ip_trkv0intAK6TPEMJ                       (&fDirector,"ip_trkv0intAK6TPEMJ"),
      ip_trkd0valAK6TPEMJ                       (&fDirector,"ip_trkd0valAK6TPEMJ"),
      ip_trkd0sigAK6TPEMJ                       (&fDirector,"ip_trkd0sigAK6TPEMJ"),
      ip_trkz0valAK6TPEMJ                       (&fDirector,"ip_trkz0valAK6TPEMJ"),
      ip_trkz0sigAK6TPEMJ                       (&fDirector,"ip_trkz0sigAK6TPEMJ"),
      sv1_pbAK6TPEMJ                            (&fDirector,"sv1_pbAK6TPEMJ"),
      sv1_puAK6TPEMJ                            (&fDirector,"sv1_puAK6TPEMJ"),
      sv2_pbAK6TPEMJ                            (&fDirector,"sv2_pbAK6TPEMJ"),
      sv2_puAK6TPEMJ                            (&fDirector,"sv2_puAK6TPEMJ"),
      sv_okAK6TPEMJ                             (&fDirector,"sv_okAK6TPEMJ"),
      sv_ntrkvAK6TPEMJ                          (&fDirector,"sv_ntrkvAK6TPEMJ"),
      sv_massAK6TPEMJ                           (&fDirector,"sv_massAK6TPEMJ"),
      sv_efrcAK6TPEMJ                           (&fDirector,"sv_efrcAK6TPEMJ"),
      sv_nv2tAK6TPEMJ                           (&fDirector,"sv_nv2tAK6TPEMJ"),
      sv_xAK6TPEMJ                              (&fDirector,"sv_xAK6TPEMJ"),
      sv_yAK6TPEMJ                              (&fDirector,"sv_yAK6TPEMJ"),
      sv_zAK6TPEMJ                              (&fDirector,"sv_zAK6TPEMJ"),
      sv_err_xAK6TPEMJ                          (&fDirector,"sv_err_xAK6TPEMJ"),
      sv_err_yAK6TPEMJ                          (&fDirector,"sv_err_yAK6TPEMJ"),
      sv_err_zAK6TPEMJ                          (&fDirector,"sv_err_zAK6TPEMJ"),
      sv_cov_xyAK6TPEMJ                         (&fDirector,"sv_cov_xyAK6TPEMJ"),
      sv_cov_xzAK6TPEMJ                         (&fDirector,"sv_cov_xzAK6TPEMJ"),
      sv_cov_yzAK6TPEMJ                         (&fDirector,"sv_cov_yzAK6TPEMJ"),
      sv_chi2AK6TPEMJ                           (&fDirector,"sv_chi2AK6TPEMJ"),
      sv_ndofAK6TPEMJ                           (&fDirector,"sv_ndofAK6TPEMJ"),
      sv_trkindexAK6TPEMJ                       (&fDirector,"sv_trkindexAK6TPEMJ"),
      sv_badtrkindexAK6TPEMJ                    (&fDirector,"sv_badtrkindexAK6TPEMJ"),
      sv0_okAK6TPEMJ                            (&fDirector,"sv0_okAK6TPEMJ"),
      sv0_ntrkvAK6TPEMJ                         (&fDirector,"sv0_ntrkvAK6TPEMJ"),
      sv0_massAK6TPEMJ                          (&fDirector,"sv0_massAK6TPEMJ"),
      sv0_efrcAK6TPEMJ                          (&fDirector,"sv0_efrcAK6TPEMJ"),
      sv0_nv2tAK6TPEMJ                          (&fDirector,"sv0_nv2tAK6TPEMJ"),
      sv0_xAK6TPEMJ                             (&fDirector,"sv0_xAK6TPEMJ"),
      sv0_yAK6TPEMJ                             (&fDirector,"sv0_yAK6TPEMJ"),
      sv0_zAK6TPEMJ                             (&fDirector,"sv0_zAK6TPEMJ"),
      sv0_err_xAK6TPEMJ                         (&fDirector,"sv0_err_xAK6TPEMJ"),
      sv0_err_yAK6TPEMJ                         (&fDirector,"sv0_err_yAK6TPEMJ"),
      sv0_err_zAK6TPEMJ                         (&fDirector,"sv0_err_zAK6TPEMJ"),
      sv0_cov_xyAK6TPEMJ                        (&fDirector,"sv0_cov_xyAK6TPEMJ"),
      sv0_cov_xzAK6TPEMJ                        (&fDirector,"sv0_cov_xzAK6TPEMJ"),
      sv0_cov_yzAK6TPEMJ                        (&fDirector,"sv0_cov_yzAK6TPEMJ"),
      sv0_chi2AK6TPEMJ                          (&fDirector,"sv0_chi2AK6TPEMJ"),
      sv0_ndofAK6TPEMJ                          (&fDirector,"sv0_ndofAK6TPEMJ"),
      sv0_trkindexAK6TPEMJ                      (&fDirector,"sv0_trkindexAK6TPEMJ"),
      jfit_pbAK6TPEMJ                           (&fDirector,"jfit_pbAK6TPEMJ"),
      jfit_puAK6TPEMJ                           (&fDirector,"jfit_puAK6TPEMJ"),
      jfit_pcAK6TPEMJ                           (&fDirector,"jfit_pcAK6TPEMJ"),
      jfitCOMBNN_pbAK6TPEMJ                     (&fDirector,"jfitCOMBNN_pbAK6TPEMJ"),
      jfitCOMBNN_puAK6TPEMJ                     (&fDirector,"jfitCOMBNN_puAK6TPEMJ"),
      jfitCOMBNN_pcAK6TPEMJ                     (&fDirector,"jfitCOMBNN_pcAK6TPEMJ"),
      jfit_nvtxntAK6TPEMJ                       (&fDirector,"jfit_nvtxntAK6TPEMJ"),
      jfit_nvtx1tAK6TPEMJ                       (&fDirector,"jfit_nvtx1tAK6TPEMJ"),
      jfit_ntrkAK6TPEMJ                         (&fDirector,"jfit_ntrkAK6TPEMJ"),
      jfit_efrcAK6TPEMJ                         (&fDirector,"jfit_efrcAK6TPEMJ"),
      jfit_massAK6TPEMJ                         (&fDirector,"jfit_massAK6TPEMJ"),
      jfit_sig3dAK6TPEMJ                        (&fDirector,"jfit_sig3dAK6TPEMJ"),
      jfit_dphiAK6TPEMJ                         (&fDirector,"jfit_dphiAK6TPEMJ"),
      jfit_detaAK6TPEMJ                         (&fDirector,"jfit_detaAK6TPEMJ"),
      jfit_phiAK6TPEMJ                          (&fDirector,"jfit_phiAK6TPEMJ"),
      jfit_thetaAK6TPEMJ                        (&fDirector,"jfit_thetaAK6TPEMJ"),
      jfit_phierrAK6TPEMJ                       (&fDirector,"jfit_phierrAK6TPEMJ"),
      jfit_thetaerrAK6TPEMJ                     (&fDirector,"jfit_thetaerrAK6TPEMJ"),
      jfitFlip_pbAK6TPEMJ                       (&fDirector,"jfitFlip_pbAK6TPEMJ"),
      jfitFlip_puAK6TPEMJ                       (&fDirector,"jfitFlip_puAK6TPEMJ"),
      jfitFlip_pcAK6TPEMJ                       (&fDirector,"jfitFlip_pcAK6TPEMJ"),
      jfitFlipCOMBNN_pbAK6TPEMJ                 (&fDirector,"jfitFlipCOMBNN_pbAK6TPEMJ"),
      jfitFlipCOMBNN_puAK6TPEMJ                 (&fDirector,"jfitFlipCOMBNN_puAK6TPEMJ"),
      jfitFlipCOMBNN_pcAK6TPEMJ                 (&fDirector,"jfitFlipCOMBNN_pcAK6TPEMJ"),
      jfitCOMBNNIP3DPos_pbAK6TPEMJ              (&fDirector,"jfitCOMBNNIP3DPos_pbAK6TPEMJ"),
      jfitCOMBNNIP3DPos_puAK6TPEMJ              (&fDirector,"jfitCOMBNNIP3DPos_puAK6TPEMJ"),
      jfitCOMBNNIP3DPos_pcAK6TPEMJ              (&fDirector,"jfitCOMBNNIP3DPos_pcAK6TPEMJ"),
      jfitFlip_nvtxntAK6TPEMJ                   (&fDirector,"jfitFlip_nvtxntAK6TPEMJ"),
      jfitFlip_nvtx1tAK6TPEMJ                   (&fDirector,"jfitFlip_nvtx1tAK6TPEMJ"),
      jfitFlip_ntrkAK6TPEMJ                     (&fDirector,"jfitFlip_ntrkAK6TPEMJ"),
      jfitFlip_efrcAK6TPEMJ                     (&fDirector,"jfitFlip_efrcAK6TPEMJ"),
      jfitFlip_massAK6TPEMJ                     (&fDirector,"jfitFlip_massAK6TPEMJ"),
      jfitFlip_sig3dAK6TPEMJ                    (&fDirector,"jfitFlip_sig3dAK6TPEMJ"),
      jfitFlip_dphiAK6TPEMJ                     (&fDirector,"jfitFlip_dphiAK6TPEMJ"),
      jfitFlip_detaAK6TPEMJ                     (&fDirector,"jfitFlip_detaAK6TPEMJ"),
      jfitFlip_phiAK6TPEMJ                      (&fDirector,"jfitFlip_phiAK6TPEMJ"),
      jfitFlip_thetaAK6TPEMJ                    (&fDirector,"jfitFlip_thetaAK6TPEMJ"),
      jfitFlip_phierrAK6TPEMJ                   (&fDirector,"jfitFlip_phierrAK6TPEMJ"),
      jfitFlip_thetaerrAK6TPEMJ                 (&fDirector,"jfitFlip_thetaerrAK6TPEMJ"),
      jfit_probchi2AK6TPEMJ                     (&fDirector,"jfit_probchi2AK6TPEMJ"),
      jfit_vtxpositionAK6TPEMJ                  (&fDirector,"jfit_vtxpositionAK6TPEMJ"),
      jfit_vtxerrorAK6TPEMJ                     (&fDirector,"jfit_vtxerrorAK6TPEMJ"),
      jfit_trackindexAtVtxAK6TPEMJ              (&fDirector,"jfit_trackindexAtVtxAK6TPEMJ"),
      jfit_trackphiAtVtxAK6TPEMJ                (&fDirector,"jfit_trackphiAtVtxAK6TPEMJ"),
      jfit_trackthetaAtVtxAK6TPEMJ              (&fDirector,"jfit_trackthetaAtVtxAK6TPEMJ"),
      jfit_trackptAtVtxAK6TPEMJ                 (&fDirector,"jfit_trackptAtVtxAK6TPEMJ"),
      jfit_twotrkvtx_xAK6TPEMJ                  (&fDirector,"jfit_twotrkvtx_xAK6TPEMJ"),
      jfit_twotrkvtx_yAK6TPEMJ                  (&fDirector,"jfit_twotrkvtx_yAK6TPEMJ"),
      jfit_twotrkvtx_zAK6TPEMJ                  (&fDirector,"jfit_twotrkvtx_zAK6TPEMJ"),
      jfit_twotrkvtx_errxAK6TPEMJ               (&fDirector,"jfit_twotrkvtx_errxAK6TPEMJ"),
      jfit_twotrkvtx_erryAK6TPEMJ               (&fDirector,"jfit_twotrkvtx_erryAK6TPEMJ"),
      jfit_twotrkvtx_errzAK6TPEMJ               (&fDirector,"jfit_twotrkvtx_errzAK6TPEMJ"),
      jfit_twotrkvtx_massAK6TPEMJ               (&fDirector,"jfit_twotrkvtx_massAK6TPEMJ"),
      jfit_twotrkvtx_probchi2AK6TPEMJ           (&fDirector,"jfit_twotrkvtx_probchi2AK6TPEMJ"),
      jfit_twotrkvtx_isNeutralAK6TPEMJ          (&fDirector,"jfit_twotrkvtx_isNeutralAK6TPEMJ"),
      jfit_twotrkvtx_trkindex1AK6TPEMJ          (&fDirector,"jfit_twotrkvtx_trkindex1AK6TPEMJ"),
      jfit_twotrkvtx_trkindex2AK6TPEMJ          (&fDirector,"jfit_twotrkvtx_trkindex2AK6TPEMJ"),
      jfitFlip_probchi2AK6TPEMJ                 (&fDirector,"jfitFlip_probchi2AK6TPEMJ"),
      jfitFlip_vtxpositionAK6TPEMJ              (&fDirector,"jfitFlip_vtxpositionAK6TPEMJ"),
      jfitFlip_vtxerrorAK6TPEMJ                 (&fDirector,"jfitFlip_vtxerrorAK6TPEMJ"),
      jfitFlip_ntracksAK6TPEMJ                  (&fDirector,"jfitFlip_ntracksAK6TPEMJ"),
      ExMissMETTopo                             (&fDirector,"ExMissMETTopo"),
      EyMissMETTopo                             (&fDirector,"EyMissMETTopo"),
      EtSumMETTopo                              (&fDirector,"EtSumMETTopo"),
      ExMissMETMuon                             (&fDirector,"ExMissMETMuon"),
      EyMissMETMuon                             (&fDirector,"EyMissMETMuon"),
      EtSumMETMuon                              (&fDirector,"EtSumMETMuon"),
      mutype                                    (&fDirector,"mutype"),
      muphi                                     (&fDirector,"muphi"),
      mupt                                      (&fDirector,"mupt"),
      mueta                                     (&fDirector,"mueta"),
      muauthor                                  (&fDirector,"muauthor"),
      mumuChi2                                  (&fDirector,"mumuChi2"),
      mumatchChi2                               (&fDirector,"mumatchChi2"),
      muetCone20                                (&fDirector,"muetCone20"),
      muetCone30                                (&fDirector,"muetCone30"),
      muetCone40                                (&fDirector,"muetCone40"),
      mueLossPar                                (&fDirector,"mueLossPar"),
      mueLossMeas                               (&fDirector,"mueLossMeas"),
      munMDTHits                                (&fDirector,"munMDTHits"),
      munMDTHoles                               (&fDirector,"munMDTHoles"),
      munCSCEtaHits                             (&fDirector,"munCSCEtaHits"),
      munCSCEtaHoles                            (&fDirector,"munCSCEtaHoles"),
      munCSCPhiHits                             (&fDirector,"munCSCPhiHits"),
      munCSCPhiHoles                            (&fDirector,"munCSCPhiHoles"),
      munRPCEtaHits                             (&fDirector,"munRPCEtaHits"),
      munRPCEtaHoles                            (&fDirector,"munRPCEtaHoles"),
      munRPCPhiHits                             (&fDirector,"munRPCPhiHits"),
      munRPCPhiHoles                            (&fDirector,"munRPCPhiHoles"),
      munTGCEtaHits                             (&fDirector,"munTGCEtaHits"),
      munTGCEtaHoles                            (&fDirector,"munTGCEtaHoles"),
      munTGCPhiHits                             (&fDirector,"munTGCPhiHits"),
      munTGCPhiHoles                            (&fDirector,"munTGCPhiHoles"),
      mutrkindex                                (&fDirector,"mutrkindex"),
      elecEorP                                  (&fDirector,"elecEorP"),
      elecIdx                                   (&fDirector,"elecIdx"),
      elecAut                                   (&fDirector,"elecAut"),
      elecE                                     (&fDirector,"elecE"),
      eleceta                                   (&fDirector,"eleceta"),
      elecphi                                   (&fDirector,"elecphi"),
      elecType                                  (&fDirector,"elecType")
      { }
   ~ntuple_vtuple();
   Int_t   Version() const {return 1;}
   void    Begin(::TTree *tree);
   void    SlaveBegin(::TTree *tree);
   void    Init(::TTree *tree);
   Bool_t  Notify();
   Bool_t  Process(Long64_t entry);
   void    SlaveTerminate();
   void    Terminate();

   ClassDef(ntuple_vtuple,0);


//inject the user's code
#include "junk_macro_parsettree_vtuple.C"
};

#endif


#ifdef __MAKECINT__
#pragma link C++ class ntuple_vtuple::TStlPx_vector_int_-;
#pragma link C++ class ntuple_vtuple::TStlPx_vector_float_-;
#pragma link C++ class ntuple_vtuple::TStlPx_vector_vector_int__-;
#pragma link C++ class ntuple_vtuple::TStlPx_vector_vector_float__-;
#pragma link C++ class ntuple_vtuple::TStlPx_vector_bool_-;
#pragma link6 C++ class vector<float>;
#pragma link6 C++ class vector<Long64_t>;
#pragma link6 C++ class vector<unsigned long>;
#pragma link6 C++ class vector<short>;
#pragma link6 C++ class vector<unsigned short>;
#pragma link C++ class vector<vector<int> >;
#pragma link C++ class vector<float>;
#pragma link C++ class vector<vector<float> >;
#pragma link C++ class vector<vector<vector<int> > >;
#pragma link C++ class vector<vector<vector<float> > >;
#pragma link C++ class vector<vector<bool> >;
#pragma link C++ class ntuple_vtuple;
#endif


inline ntuple_vtuple::~ntuple_vtuple() {
   // destructor. Clean up helpers.

}

inline void ntuple_vtuple::Init(TTree *tree)
{
//   Set branch addresses
   if (tree == 0) return;
   fChain = tree;
   fDirector.SetTree(fChain);
   if (htemp == 0) {
      htemp = fDirector.CreateHistogram(GetOption());
      htemp->SetTitle("junk_macro_parsettree_vtuple.C");
      fObject = htemp;
   }
}

Bool_t ntuple_vtuple::Notify()
{
   // Called when loading a new file.
   // Get branch pointers.
   fDirector.SetTree(fChain);
   junk_macro_parsettree_vtuple_Notify();
   
   return kTRUE;
}
   

inline void ntuple_vtuple::Begin(TTree *tree)
{
   // The Begin() function is called at the start of the query.
   // When running with PROOF Begin() is only called on the client.
   // The tree argument is deprecated (on PROOF 0 is passed).

   TString option = GetOption();
   junk_macro_parsettree_vtuple_Begin(tree);

}

inline void ntuple_vtuple::SlaveBegin(TTree *tree)
{
   // The SlaveBegin() function is called after the Begin() function.
   // When running with PROOF SlaveBegin() is called on each slave server.
   // The tree argument is deprecated (on PROOF 0 is passed).

   Init(tree);

   junk_macro_parsettree_vtuple_SlaveBegin(tree);

}

inline Bool_t ntuple_vtuple::Process(Long64_t entry)
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
   junk_macro_parsettree_vtuple();
   junk_macro_parsettree_vtuple_Process(entry);
   return kTRUE;

}

inline void ntuple_vtuple::SlaveTerminate()
{
   // The SlaveTerminate() function is called after all entries or objects
   // have been processed. When running with PROOF SlaveTerminate() is called
   // on each slave server.
   junk_macro_parsettree_vtuple_SlaveTerminate();
}

inline void ntuple_vtuple::Terminate()
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
   junk_macro_parsettree_vtuple_Terminate();
}
