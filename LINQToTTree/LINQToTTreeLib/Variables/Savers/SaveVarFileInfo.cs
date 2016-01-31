using LinqToTTreeInterfacesLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ROOTNET.Interface;
using System.IO;

namespace LINQToTTreeLib.Variables.Savers
{
    /// <summary>
    /// Variable saver to deal with FIleInfo results - basically a file generated on the remote
    /// guy and sent back to the .net world.
    /// </summary>
    [Export(typeof(IVariableSaver))]
    class SaveVarFileInfo : IVariableSaver
    {
        /// <summary>
        /// Only FileInfo objects can be dealt with.
        /// </summary>
        /// <param name="iVariable"></param>
        /// <returns></returns>
        public bool CanHandle(IDeclaredParameter iVariable)
        {
            return iVariable.Type == typeof(FileInfo);
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
        /// Return a list of include files needed when saving this sort of variable.
        /// Since we are storing filenames as paths, we will just cache it here.
        /// </summary>
        /// <param name="iVariable"></param>
        /// <returns></returns>
        public IEnumerable<string> IncludeFiles(IDeclaredParameter iVariable)
        {
            yield return "TH1F.h";
        }

        /// <summary>
        /// For anything generic, this can't go.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="iVariable"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public T LoadResult<T>(IDeclaredParameter iVariable, NTObject[] obj)
        {
            var s = obj[0] as NTH1F;
            if (s == null) throw
                    new InvalidOperationException($"FileInfo cached value should be a TObjString object, but is {s.GetType().Name}.");

            // We have to do this funny type conversion b.c. though we will only be called with a
            // T == FileInfo, the compiler doesn't know that. It could be a "FileInfo" or an "int" as far
            // as it is concerned.
            object o = new FileInfo(s.Title);
            return (T)o;
        }

        /// <summary>
        /// Return a sequence of C++ code that will end with the var being cached. This should
        /// occur in SlaveTerminate in TSelector.
        /// </summary>
        /// <param name="iVariable"></param>
        /// <returns></returns>
        public IEnumerable<string> SaveToFile(IDeclaredParameter iVariable)
        {
            // To save this right we need to store a string with a lookup key. Unfortunately, TObjString
            // won't do that - it's Name is the same as its value. So lets just use a TH1 - as it has a name and
            // title. :-)

            yield return string.Format("TH1F *{0}_hist = new TH1F(\"{0}\", {1}.c_str(), 1, 0.0, 1.0);", iVariable.RawValue, iVariable.RawValue);
            yield return iVariable.RawValue + "_hist->SetDirectory(0);";
            yield return "Book(" + iVariable.RawValue + "_hist);";
        }
    }
}
