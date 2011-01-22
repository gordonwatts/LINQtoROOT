///
/// Copied from the SFrame object (SCycleOutput), and modified for our use.
///

#include "FlowOutputObject.h"


// $Id: SCycleOutput.cxx 191 2010-08-04 08:00:45Z krasznaa $
/***************************************************************************
 * @Project: SFrame - ROOT-based analysis framework for ATLAS
 * @Package: Core
 *
 * @author Stefan Ask       <Stefan.Ask@cern.ch>           - Manchester
 * @author David Berge      <David.Berge@cern.ch>          - CERN
 * @author Johannes Haller  <Johannes.Haller@cern.ch>      - Hamburg
 * @author A. Krasznahorkay <Attila.Krasznahorkay@cern.ch> - CERN/Debrecen
 *
 ***************************************************************************/

// System include(s):
#include <string.h>

// STL include(s):
#include <vector>
#include <iostream>

// ROOT include(s):
#include <TCollection.h>
#include <TList.h>
#include <TMethodCall.h>
#include <TDirectory.h>
#include <TTree.h>
#include <TKey.h>
#include <TObjArray.h>
#include <TObjString.h>

using std::cout;
using std::endl;

ClassImp( FlowOutputObject );

/**
 * The constructor just initialises the base class and the member(s)
 * to meaningul defauls.
 *
 * @param object Pointer to the object that should be wrapped
 * @param name Name of this object. The SCycleBase* classes should
 *             make sure that this is unique
 */
FlowOutputObject::FlowOutputObject( TObject* object, const char* name,
                            const char* path )
   : TNamed( name, "PlotFramework flow output object" ), m_object( object ),
     m_path( path ) {

}

/**
 * Since this wrapper class owns the object it wraps, it deletes it
 * when it is deleted itself.
 */
FlowOutputObject::~FlowOutputObject() {

   //if( m_object ) delete m_object;
}

/**
 * @returns A pointer to the wrapped object
 */
TObject* FlowOutputObject::GetObject() const {

   return m_object;
}

/**
 * @param object Pointer to the wrapped object
 */
void FlowOutputObject::SetObject( TObject* object ) {

   m_object = object;
   return;
}

const TString& FlowOutputObject::GetPath() const {

   return m_path;
}

void FlowOutputObject::SetPath( const TString& path ) {

   m_path = path;
   return;
}

/**
 * Now this is a tricky one. In order to be able to merge trees, histograms,
 * and any other kinds of ROOT objects, this function has to be very generic.
 * (Remember, TObject doesn't define a "Merge" function!) Much of the code
 * here is inspired by ROOT code. (I mean I never would've figured out the
 * usage of TMethodCall without seeing some examples...)
 *
 * @param coll Collection of objects to merge into this one. Usually PROOF
 *             takes care of creating it.
 */
Int_t FlowOutputObject::Merge( TCollection* coll ) {

   //
   // Return right away if the input is flawed:
   //
   if( ! coll ) return 0;
   if( coll->IsEmpty() ) return 0;

   //
   // Select the elements from the collection that can actually be merged:
   //
   TList list;
   TIter next( coll );
   TObject* obj = 0;
   while( ( obj = next() ) ) {

      //
      // See if it is an SCycleOutput object itself:
      //
      FlowOutputObject* sobj = dynamic_cast< FlowOutputObject* >( obj );
      if( ! sobj ) {
		  cout << "ERROR: Attempt to merage " << obj->ClassName() << " into " << this->ClassName() << "! Not!" << endl;
		  continue;
      }

      //
      // See if it holds the same kind of object as this output object:
      //

	  TObject* mobj = sobj->GetObject();
      if( ! mobj ) continue;
      if( strcmp( mobj->ClassName(), this->GetObject()->ClassName() ) ) {
         cout << "ERROR: Trying to merge \"" << mobj->ClassName()
                       << "\" object into \"" << this->GetObject()->ClassName() << endl;
         continue;
      }

      //
      // If everything is fine, add it to the list of objects to merge:
      //
      list.Add( mobj );
   }

   //
   // Stop if the list is empty:
   //
   if( list.IsEmpty() ) {
      return 0;
   }

   //
   // Make sure that my object supports merging:
   //
   TMethodCall mergeMethod;
   mergeMethod.InitWithPrototype( this->GetObject()->IsA(), "Merge", "TCollection*" );
   if( ! mergeMethod.IsValid() ) {
      cout << "ERROR: Object type \"" << this->GetObject()->ClassName()
                    << "\" doesn't support merging" << endl;
      return 0;
   }

   //
   // Execute the merging:
   //

   mergeMethod.SetParam( ( Long_t ) &list );
   mergeMethod.Execute( this->GetObject() );

   //
   // A little feedback of what we've done:
   //

   return 1;

}

/**
 * This function is also quite tricky. It basically does two things:
 *
 *   - If the output file already contains an object with the same name as the
 *     object that we want to save, then instead of overwriting it, it merges
 *     the new object into the old one.
 *   - If there is no pre-existing object in the file, then it just "simply"
 *     creates the specified output directory in the file, and saves the
 *     object in it.
 */
Int_t FlowOutputObject::Write( const char* name, Int_t option,
                           Int_t bufsize ) const {

   // Nothing to be done with no object:
   if( ! m_object ) return -1;

   //
   // Remember both the current directory, and create the directory for the
   // output object:
   //
   TDirectory* origDir = gDirectory;
   TDirectory* outDir = MakeDirectory( m_path );

   //
   // Check if the output directory already holds such an object:
   //
   TObject* original_obj;
   if( ( original_obj = outDir->Get( m_object->GetName() ) ) ) {

      //
      // Check that it's the same type as the object that we want to save:
      //
      if( strcmp( original_obj->ClassName(), m_object->ClassName() ) ) {
         cout << "ERROR: Object in file (\"" << original_obj->ClassName()
                       << "\") is not the same type as the object in memory (\""
                       << m_object->ClassName() << "\")" << endl;
         return 0;
      }

      //
      // Try to merge the new object into the old one:
      //
      TMethodCall mergeMethod;
      mergeMethod.InitWithPrototype( original_obj->IsA(), "Merge", "TCollection*" );
      if( ! mergeMethod.IsValid() ) {
         cout << "ERROR: Object type \"" << original_obj->ClassName()
                       << "\" doesn't support merging" << endl;
         return 0;
      }

      //
      // Remember the key of this object, to be able to remove it after the merging:
      //
      TKey* oldKey = outDir->GetKey( m_object->GetName() );

      //
      // Execute the merging:
      //
      TList list;
      list.Add( m_object );
      mergeMethod.SetParam( ( Long_t ) &list );
      mergeMethod.Execute( original_obj );

      //
      // Remove the old object from the file:
      //
      oldKey->Delete();
      delete oldKey;

      // Return gracefully:
      return 1;

   }

   //
   // TTree-s have to be handled in a special way:
   //
   TTree* tobject = dynamic_cast< TTree* >( m_object );
   if( tobject ) tobject->SetDirectory( outDir );

   //
   // Write out the object:
   //
   outDir->cd();
   Int_t ret = m_object->Write( name, option, bufsize );
   if( tobject ) tobject->AutoSave();
   origDir->cd();

   // Remove the memory-resident TTree from the directory:
   if( tobject ) tobject->SetDirectory( 0 );

   return ret;

}

Int_t FlowOutputObject::Write( const char* name, Int_t option,
                           Int_t bufsize ) {

   return const_cast< const FlowOutputObject* >( this )->Write( name, option, bufsize );
}

/**
 * Function accessing/creating the required directory in the output file:
 *
 * @param path Directory name with slashes. (e.g. "my/directory")
 * @returns Pointer to the created directory
 */
TDirectory* FlowOutputObject::MakeDirectory( const TString& path ) const {

   if( ! path.Length() ) return gDirectory;

   TDirectory* dir = 0;
   if( ! ( dir = gDirectory->GetDirectory( path ) ) ) {

      //
      // Break up the path name at the slashes:
      //
      TObjArray* directories = path.Tokenize( "/" );

      //
      // Create each necessary directory:
      //
      dir = gDirectory;
      TDirectory* tempDir = 0;
      for( Int_t i = 0; i < directories->GetSize(); ++i ) {

         TObjString* path_element = dynamic_cast< TObjString* >( directories->At( i ) );
         if( ! path_element ) continue;
         if( path_element->GetString() == "" ) continue;

         if( ! ( tempDir = dir->GetDirectory( path_element->GetString() ) ) ) {
            if( ! ( tempDir = dir->mkdir( path_element->GetString(), "dummy title" ) ) ) {
               cout << "ERROR: Couldn't create directory: " << path
                     << " in the output file!" << endl;
			   return 0;
            }
         }
         dir = tempDir;

      }

      // Delete the object created by TString::Tokenize(...):
      delete directories;

   }

   return dir;

}
