///
/// This is what we store in our out going data for the round-trip back from
/// PROOF (or wherever). This is copied from the SFrame project that can be
/// found on source forge. THANKS!!!
///

#ifndef __FlowOutputObject__
#define __FlowOutputObject__

#include <TObject.h>
#include <TDirectory.h>
#include <TString.h>
#include <TNamed.h>

class FlowOutputObject : public TNamed
{
public:
	/// Constructor with child object and name
   FlowOutputObject( TObject* object = 0, const char* name = "",
                 const char* path = "" );
   /// Destructor
   virtual ~FlowOutputObject();

   /// Get the wrapped object
   TObject* GetObject() const;
   /// Set the pointer to the wrapped object
   void SetObject( TObject* object );

   /// Get the output path of the wrapped object
   const TString& GetPath() const;
   /// Set the output path of the wrapped object
   void SetPath( const TString& path );

   /// Merge the contents of other objects into this one
   Int_t Merge( TCollection* list );
   /// Write the wrapped object in the correct output directory (const version)
   virtual Int_t Write( const char* name = 0, Int_t option = 0,
                        Int_t bufsize = 0 ) const;
   /// Write the wrapped object in the correct output directory (non-const version)
   virtual Int_t Write( const char* name = 0, Int_t option = 0,
                        Int_t bufsize = 0 );

private:
   /// Return the requested output directory
   TDirectory* MakeDirectory( const TString& path ) const;

   /// The object that this class wraps
   TObject* m_object;
   /// Path of the object in the output file
   TString m_path;

   ClassDef( FlowOutputObject, 1 );

}; // class SCycleOutput

#endif
