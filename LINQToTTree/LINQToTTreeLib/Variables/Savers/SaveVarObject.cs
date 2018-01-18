using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.TypeHandlers.ROOT;
using ROOTNET.Interface;

namespace LINQToTTreeLib.Variables.Savers
{
    /// <summary>
    /// Manage taking some object accross the wire back to the client once the processing
    /// of the tree is complete. At the moment we know only how to deal with ROOT objects! :-)
    /// </summary>
    [Export(typeof(IVariableSaver))]
    public class SaveVarObject : IVariableSaver
    {
        /// <summary>
        /// Return OK for any VarObject that is a named root object.
        /// </summary>
        /// <param name="iVariable"></param>
        /// <returns></returns>
        public bool CanHandle(IDeclaredParameter iVariable)
        {
            return iVariable.Type.GetInterface("NTNamed") != null;
        }

        /// <summary>
        /// Return the list of all the things we want to cache to make up this variable.
        /// </summary>
        /// <param name="iVariable"></param>
        /// <returns></returns>
        public string[] GetCachedNames(IDeclaredParameter iVariable)
        {
            return new string[] { iVariable.RawValue };
        }

        /// <summary>
        /// The lines of code that can be used to save this guy to a file. This is actually quite easy
        /// - we just book it!
        /// </summary>
        /// <param name="iVariable"></param>
        /// <returns></returns>
        public IEnumerable<string> SaveToFile(IDeclaredParameter iVariable)
        {
            StringBuilder bld = new StringBuilder();
            bld.AppendFormat("{0}->SetName(\"{0}\");", iVariable.RawValue);
            yield return bld.ToString();
            yield return "Book(" + iVariable.RawValue + ");";
        }

        /// <summary>
        /// In order to setup the above code we do not need to implement any special code... or pull anything extra in.
        /// </summary>
        /// <param name="iVariable"></param>
        /// <returns></returns>
        public IEnumerable<string> IncludeFiles(IDeclaredParameter iVariable)
        {
            return Enumerable.Empty<string>();
        }

        /// <summary>
        /// Ok, back on the client with the result. We need to try and load the thing in.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="iVariable"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public T LoadResult<T>(IDeclaredParameter iVariable, RunInfo[] obj)
        {
            if (obj == null)
                throw new ArgumentNullException("Obj cannot be null");

            var named = obj[0]._result as ROOTNET.Interface.NTNamed;
            if (named == null)
                throw new ArgumentException("Object isn't named");

            if (iVariable == null)
                throw new ArgumentNullException("Variable can't be null");

            var rootObjInfo = iVariable.InitialValue as TypeHandlerROOT.ROOTObjectStaticHolder;
            if (rootObjInfo == null)
                throw new InvalidOperationException("iVariable must be a ROOTObjectCopiedValue!");

            var oldHSet = ROOTNET.NTH1.AddDirectoryStatus();
            ROOTNET.NTH1.AddDirectory(false);
            var result = named.Clone() as ROOTNET.Interface.NTNamed;
            ROOTNET.NTH1.AddDirectory(oldHSet);

            ///
            /// Restore name and title - which might be different since our cache is blind
            /// to those things.
            /// 

            result.Name = rootObjInfo.OriginalName;
            result.Title = rootObjInfo.OriginalTitle;

            return (T)result;
        }

        /// <summary>
        /// Since ROOT objects are in-memory objects, no need to do any renameing.
        /// </summary>
        /// <param name="iVariable"></param>
        /// <param name="obj"></param>
        /// <param name="cycle"></param>
        public void RenameForQueryCycle(IDeclaredParameter iVariable, NTObject[] obj, int cycle, DirectoryInfo queryDirectory)
        {
        }
    }
}
