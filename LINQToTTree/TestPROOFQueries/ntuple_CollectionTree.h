/////////////////////////////////////////////////////////////////////////
//   This class has been automatically generated 
//   (at Thu Apr 12 03:22:36 2012 by ROOT version 5.30/01)
//   from TTree CollectionTree/CollectionTree
//   found on file: C:\Users\gwatts\Documents\Visual Studio 2010\Projects\HVAssociatedTests\EVNT-short.root
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
class EventInfo_p3;
class McEventCollection_p4;
class GenEvent_p4;
class GenVertex_p4;
class GenParticle_p4;


// Header needed by this particular proxy
#include <vector>
#include "utility"


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
   struct TPx_EventInfo_p3
   {
      TPx_EventInfo_p3(TBranchProxyDirector* director,const char *top,const char *mid=0) :
         ffPrefix                                (top,mid),
         obj                                     (director, top, mid),
         m_AllTheData                            (director, "m_AllTheData")
      {};
      TPx_EventInfo_p3(TBranchProxyDirector* director, TBranchProxy *parent, const char *membername, const char *top=0, const char *mid=0) :
         ffPrefix                                (top,mid),
         obj                                     (director, parent, membername),
         m_AllTheData                            (director, "m_AllTheData")
      {};
      TBranchProxyHelper                       ffPrefix;
      InjecTBranchProxyInterface();
      TBranchProxy obj;

      TStlSimpleProxy<vector<unsigned int> >   m_AllTheData;
   };
   struct TStlPx_vector_long_
   {
      TStlPx_vector_long_(TBranchProxyDirector* director,const char *top,const char *mid=0) :
         ffPrefix(top,mid),
         obj(director, top, mid)
      {};
      TStlPx_vector_long_(TBranchProxyDirector* director, TBranchProxy *parent, const char *membername, const char *top=0, const char *mid=0) :
         ffPrefix(top,mid),
         obj(director, parent, membername)
      {};
      TBranchProxyHelper ffPrefix;
      InjecTBranchProxyInterface();
      const vector<long>& At(UInt_t i) {
         static vector<long> default_val;
         if (!obj.Read()) return default_val;
         vector<long> *temp = (vector<long> *)( obj.GetProxy()->GetStlStart(i) );
         if (temp) return *temp; else return default_val;
      }
      const vector<long>& operator[](Int_t i) { return At(i); }
      const vector<long>& operator[](UInt_t i) { return At(i); }
      Int_t GetEntries() { return obj.GetPtr()->size(); }
      const vector<long>* operator->() { return obj.GetPtr(); }
      operator vector<long>*() { return obj.GetPtr(); }
      TObjProxy<vector<long> > obj;

   };
   struct TStlPx_GenEvent_p4
   {
      TStlPx_GenEvent_p4(TBranchProxyDirector* director,const char *top,const char *mid=0) :
         ffPrefix                          (top,mid),
         obj                               (director, top, mid),
         m_signalProcessId                 (director, ffPrefix, "m_signalProcessId"),
         m_eventNbr                        (director, ffPrefix, "m_eventNbr"),
         m_eventScale                      (director, ffPrefix, "m_eventScale"),
         m_alphaQCD                        (director, ffPrefix, "m_alphaQCD"),
         m_alphaQED                        (director, ffPrefix, "m_alphaQED"),
         m_signalProcessVtx                (director, ffPrefix, "m_signalProcessVtx"),
         m_weights                         (director, ffPrefix, "m_weights"),
         m_pdfinfo                         (director, ffPrefix, "m_pdfinfo"),
         m_randomStates                    (director, ffPrefix, "m_randomStates"),
         m_verticesBegin                   (director, ffPrefix, "m_verticesBegin"),
         m_verticesEnd                     (director, ffPrefix, "m_verticesEnd"),
         m_particlesBegin                  (director, ffPrefix, "m_particlesBegin"),
         m_particlesEnd                    (director, ffPrefix, "m_particlesEnd")
      {};
      TStlPx_GenEvent_p4(TBranchProxyDirector* director, TBranchProxy *parent, const char *membername, const char *top=0, const char *mid=0) :
         ffPrefix                          (top,mid),
         obj                               (director, parent, membername),
         m_signalProcessId                 (director, ffPrefix, "m_signalProcessId"),
         m_eventNbr                        (director, ffPrefix, "m_eventNbr"),
         m_eventScale                      (director, ffPrefix, "m_eventScale"),
         m_alphaQCD                        (director, ffPrefix, "m_alphaQCD"),
         m_alphaQED                        (director, ffPrefix, "m_alphaQED"),
         m_signalProcessVtx                (director, ffPrefix, "m_signalProcessVtx"),
         m_weights                         (director, ffPrefix, "m_weights"),
         m_pdfinfo                         (director, ffPrefix, "m_pdfinfo"),
         m_randomStates                    (director, ffPrefix, "m_randomStates"),
         m_verticesBegin                   (director, ffPrefix, "m_verticesBegin"),
         m_verticesEnd                     (director, ffPrefix, "m_verticesEnd"),
         m_particlesBegin                  (director, ffPrefix, "m_particlesBegin"),
         m_particlesEnd                    (director, ffPrefix, "m_particlesEnd")
      {};
      TBranchProxyHelper                 ffPrefix;
      InjecTBranchProxyInterface();
      Int_t GetEntries() { return obj.GetEntries(); }
      TStlProxy obj;

      TStlIntProxy                       m_signalProcessId;
      TStlIntProxy                       m_eventNbr;
      TStlDoubleProxy                    m_eventScale;
      TStlDoubleProxy                    m_alphaQCD;
      TStlDoubleProxy                    m_alphaQED;
      TStlIntProxy                       m_signalProcessVtx;
      TStlSimpleProxy<vector<double> >   m_weights;
      TStlSimpleProxy<vector<double> >   m_pdfinfo;
      TStlPx_vector_long_                m_randomStates;
      TStlUIntProxy                      m_verticesBegin;
      TStlUIntProxy                      m_verticesEnd;
      TStlUIntProxy                      m_particlesBegin;
      TStlUIntProxy                      m_particlesEnd;
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
         vector<float> *temp = (vector<float> *)( obj.GetProxy()->GetStlStart(i) );
         if (temp) return *temp; else return default_val;
      }
      const vector<float>& operator[](Int_t i) { return At(i); }
      const vector<float>& operator[](UInt_t i) { return At(i); }
      Int_t GetEntries() { return obj.GetPtr()->size(); }
      const vector<float>* operator->() { return obj.GetPtr(); }
      operator vector<float>*() { return obj.GetPtr(); }
      TObjProxy<vector<float> > obj;

   };
   struct TStlPx_GenVertex_p4
   {
      TStlPx_GenVertex_p4(TBranchProxyDirector* director,const char *top,const char *mid=0) :
         ffPrefix                       (top,mid),
         obj                            (director, top, mid),
         m_x                            (director, ffPrefix, "m_x"),
         m_y                            (director, ffPrefix, "m_y"),
         m_z                            (director, ffPrefix, "m_z"),
         m_t                            (director, ffPrefix, "m_t"),
         m_particlesIn                  (director, ffPrefix, "m_particlesIn"),
         m_particlesOut                 (director, ffPrefix, "m_particlesOut"),
         m_id                           (director, ffPrefix, "m_id"),
         m_weights                      (director, ffPrefix, "m_weights"),
         m_barcode                      (director, ffPrefix, "m_barcode")
      {};
      TStlPx_GenVertex_p4(TBranchProxyDirector* director, TBranchProxy *parent, const char *membername, const char *top=0, const char *mid=0) :
         ffPrefix                       (top,mid),
         obj                            (director, parent, membername),
         m_x                            (director, ffPrefix, "m_x"),
         m_y                            (director, ffPrefix, "m_y"),
         m_z                            (director, ffPrefix, "m_z"),
         m_t                            (director, ffPrefix, "m_t"),
         m_particlesIn                  (director, ffPrefix, "m_particlesIn"),
         m_particlesOut                 (director, ffPrefix, "m_particlesOut"),
         m_id                           (director, ffPrefix, "m_id"),
         m_weights                      (director, ffPrefix, "m_weights"),
         m_barcode                      (director, ffPrefix, "m_barcode")
      {};
      TBranchProxyHelper              ffPrefix;
      InjecTBranchProxyInterface();
      Int_t GetEntries() { return obj.GetEntries(); }
      TStlProxy obj;

      TStlFloatProxy                  m_x;
      TStlFloatProxy                  m_y;
      TStlFloatProxy                  m_z;
      TStlFloatProxy                  m_t;
      TStlSimpleProxy<vector<int> >   m_particlesIn;
      TStlSimpleProxy<vector<int> >   m_particlesOut;
      TStlIntProxy                    m_id;
      TStlPx_vector_float_            m_weights;
      TStlIntProxy                    m_barcode;
   };
   struct TStlPx_GenParticle_p4
   {
      TStlPx_GenParticle_p4(TBranchProxyDirector* director,const char *top,const char *mid=0) :
         ffPrefix                               (top,mid),
         obj                                    (director, top, mid),
         m_px                                   (director, ffPrefix, "m_px"),
         m_py                                   (director, ffPrefix, "m_py"),
         m_pz                                   (director, ffPrefix, "m_pz"),
         m_m                                    (director, ffPrefix, "m_m"),
         m_pdgId                                (director, ffPrefix, "m_pdgId"),
         m_status                               (director, ffPrefix, "m_status"),
         m_flow                                 (director, ffPrefix, "m_flow"),
         m_thetaPolarization                    (director, ffPrefix, "m_thetaPolarization"),
         m_phiPolarization                      (director, ffPrefix, "m_phiPolarization"),
         m_prodVtx                              (director, ffPrefix, "m_prodVtx"),
         m_endVtx                               (director, ffPrefix, "m_endVtx"),
         m_barcode                              (director, ffPrefix, "m_barcode"),
         m_recoMethod                           (director, ffPrefix, "m_recoMethod")
      {};
      TStlPx_GenParticle_p4(TBranchProxyDirector* director, TBranchProxy *parent, const char *membername, const char *top=0, const char *mid=0) :
         ffPrefix                               (top,mid),
         obj                                    (director, parent, membername),
         m_px                                   (director, ffPrefix, "m_px"),
         m_py                                   (director, ffPrefix, "m_py"),
         m_pz                                   (director, ffPrefix, "m_pz"),
         m_m                                    (director, ffPrefix, "m_m"),
         m_pdgId                                (director, ffPrefix, "m_pdgId"),
         m_status                               (director, ffPrefix, "m_status"),
         m_flow                                 (director, ffPrefix, "m_flow"),
         m_thetaPolarization                    (director, ffPrefix, "m_thetaPolarization"),
         m_phiPolarization                      (director, ffPrefix, "m_phiPolarization"),
         m_prodVtx                              (director, ffPrefix, "m_prodVtx"),
         m_endVtx                               (director, ffPrefix, "m_endVtx"),
         m_barcode                              (director, ffPrefix, "m_barcode"),
         m_recoMethod                           (director, ffPrefix, "m_recoMethod")
      {};
      TBranchProxyHelper                      ffPrefix;
      InjecTBranchProxyInterface();
      Int_t GetEntries() { return obj.GetEntries(); }
      TStlProxy obj;

      TStlFloatProxy                          m_px;
      TStlFloatProxy                          m_py;
      TStlFloatProxy                          m_pz;
      TStlFloatProxy                          m_m;
      TStlIntProxy                            m_pdgId;
      TStlIntProxy                            m_status;
      TStlObjProxy<vector<pair<int,int> > >   m_flow;
      TStlFloatProxy                          m_thetaPolarization;
      TStlFloatProxy                          m_phiPolarization;
      TStlIntProxy                            m_prodVtx;
      TStlIntProxy                            m_endVtx;
      TStlIntProxy                            m_barcode;
      TStlShortProxy                          m_recoMethod;
   };
   struct TPx_McEventCollection_p4
   {
      TPx_McEventCollection_p4(TBranchProxyDirector* director,const char *top,const char *mid=0) :
         ffPrefix               (top,mid),
         obj                    (director, top, mid),
         m_genEvents            (director, "m_genEvents"),
         m_genVertices          (director, "m_genVertices"),
         m_genParticles         (director, "m_genParticles")
      {};
      TPx_McEventCollection_p4(TBranchProxyDirector* director, TBranchProxy *parent, const char *membername, const char *top=0, const char *mid=0) :
         ffPrefix               (top,mid),
         obj                    (director, parent, membername),
         m_genEvents            (director, "m_genEvents"),
         m_genVertices          (director, "m_genVertices"),
         m_genParticles         (director, "m_genParticles")
      {};
      TBranchProxyHelper      ffPrefix;
      InjecTBranchProxyInterface();
      TBranchProxy obj;

      TStlPx_GenEvent_p4      m_genEvents;
      TStlPx_GenVertex_p4     m_genVertices;
      TStlPx_GenParticle_p4   m_genParticles;
   };

   // Proxy for each of the branches, leaves and friends of the tree
   TPx_McEventCollection_p4                 McEventCollection_p4_GEN_EVENT;
   TStlPx_GenVertex_p4                      m_genVertices;
   TStlPx_GenParticle_p4                    m_genParticles;


   ntuple_CollectionTree(TTree *tree=0) : 
      fChain(0),
      htemp(0),
      fDirector(tree,-1),
      fClass                (TClass::GetClass("ntuple_CollectionTree")),
      McEventCollection_p4_GEN_EVENT          (&fDirector,"McEventCollection_p4_GEN_EVENT"),
      m_genVertices                           (&fDirector,"m_genVertices"),
      m_genParticles                          (&fDirector,"m_genParticles")
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
#pragma link C++ class ntuple_CollectionTree::TPx_EventInfo_p3-;
#pragma link C++ class ntuple_CollectionTree::TStlPx_vector_long_-;
#pragma link C++ class ntuple_CollectionTree::TStlPx_GenEvent_p4-;
#pragma link C++ class ntuple_CollectionTree::TStlPx_vector_float_-;
#pragma link C++ class ntuple_CollectionTree::TStlPx_GenVertex_p4-;
#pragma link C++ class ntuple_CollectionTree::TStlPx_GenParticle_p4-;
#pragma link C++ class ntuple_CollectionTree::TPx_McEventCollection_p4-;
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
