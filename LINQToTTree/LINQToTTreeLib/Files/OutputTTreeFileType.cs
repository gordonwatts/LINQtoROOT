using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Utils;
using LINQToTTreeLib.Variables;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ROOTNET.Interface;

namespace LINQToTTreeLib.Files
{
    /// <summary>
    /// Output type that represents a ROOT file with a single TTree in it.
    /// </summary>
    [CPPObjectRepresentationType("std::pair<TFile*,TTree*>")]
    class OutputTTreeFileType : IValue
    {
        /// <summary>
        /// Get the output file spec for the ROOT file.
        /// </summary>
        public Func<FileInfo> OutputFileFunc
        {
            get; private set;
        }

        /// <summary>
        /// Initialize with the proper file to stash.
        /// </summary>
        /// <param name="outputFile"></param>
        public OutputTTreeFileType(Func<FileInfo> outputFile)
        {
            this.OutputFileFunc = outputFile;
        }

        /// <summary>
        /// The actual value of the object is initialize to null values
        /// </summary>
        public string RawValue { get { return $"std::pair<TFile*,TTree*>(0,0)"; } }

        /// <summary>
        /// Get the type this object is holding. Which is just us.
        /// </summary>
        public Type Type { get { return typeof(OutputTTreeFileType); } }

        /// <summary>
        /// We don't hold onto a variable (we are a dummy, in some sense).
        /// </summary>
        public IEnumerable<IDeclaredParameter> Dependants
        {
            get
            {
                return Enumerable.Empty<IDeclaredParameter>();
            }
        }

        /// <summary>
        /// No renaming can happen since we hold what is basically a constant value.
        /// </summary>
        /// <param name="oldname"></param>
        /// <param name="newname"></param>
        public void RenameRawValue(string oldname, string newname)
        {
        }
    }

    /// <summary>
    /// Enable read/write of this particular guy.
    /// </summary>
    [Export(typeof(IVariableSaver))]
    class OutputTTreeFileTypeSaver : IVariableSaver
    {
        public bool CanHandle(IDeclaredParameter iVariable)
        {
            return iVariable.Type == typeof(OutputTTreeFileType);
        }

        /// <summary>
        /// Return the list of all the things we want to cache to make up this variable.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public string[] GetCachedNames(IDeclaredParameter v)
        {
            return new string[] { v.RawValue, $"{v.RawValue}_size"};
        }

        /// <summary>
        /// Include files that need to be used. Since this is an file stream...
        /// </summary>
        /// <param name="iVariable"></param>
        /// <returns></returns>
        public IEnumerable<string> IncludeFiles(IDeclaredParameter iVariable)
        {
            yield return "TFile.h";
            yield return "TTree.h";
            yield return "TH1D.h";
        }

        /// <summary>
        /// Load up the item from the ROOT file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="iVariable"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public T LoadResult<T>(IDeclaredParameter iVariable, NTObject[] objs, int cycle)
        {
            var f = GetFileInfo(iVariable, objs, cycle);
            return f == null
                ? (T)(object)null
                : (T)(object)(new FileInfo[] { f });
        }

        /// <summary>
        /// Return the file info for this output.
        /// </summary>
        /// <param name="iVariable"></param>
        /// <param name="obj"></param>
        /// <param name="cycle">The cycle number for this file. If null, then the raw file as written by the code.</param>
        /// <returns></returns>
        private FileInfo GetFileInfo(IDeclaredParameter iVariable, NTObject[] obj, int? cycle = null, bool doChecks = true)
        {
            // Fetch out the path and the size in bytes of the file.
            GetFilePathFromObjects(obj, out NTH1 hPath, out NTH1 hSize);

            // Deal with the cycle - we just add an index onto the filename.
            var name = hPath.Title;
            if (cycle.HasValue)
            {
                name = $"{Path.GetDirectoryName(name)}\\{Path.GetFileNameWithoutExtension(name)}_{cycle.Value}{Path.GetExtension(name)}";
            }

            // See if the file is there, and make sure its size is the same.
            // That will have to do for the cache lookup.
            // Funny conversion are b.c. we are in the middle of a crazy generic here.
            var f = new FileInfo(name);
            if (doChecks
                && (!f.Exists || (f.Length != (long)hSize.GetBinContent(1))))
            {
                return null;
            }

            // Return the file
            return f;
        }

        /// <summary>
        /// Extract the file name from the NTObjects we are getting fed.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="hPath"></param>
        /// <param name="hSize"></param>
        private static void GetFilePathFromObjects(NTObject[] obj, out NTH1 hPath, out NTH1 hSize)
        {
            hPath = null;
            hSize = null;
            foreach (var h in obj)
            {
                if (h.Name.EndsWith("_size"))
                {
                    hSize = h as NTH1;
                }
                else
                {
                    hPath = h as NTH1;
                }
            }

            if (hPath == null || hSize == null)
            {
                throw new InvalidOperationException("Internal error - cache is missing either the path for a CSV file or its size");
            }
        }

        /// <summary>
        /// We are going to do the rename here.
        /// </summary>
        /// <param name="iVariable"></param>
        /// <param name="obj"></param>
        /// <param name="cycle"></param>
        public void RenameForQueryCycle(IDeclaredParameter iVariable, NTObject[] obj, int cycle)
        {
            var currentFile = GetFileInfo(iVariable, obj);
            if (currentFile == null)
            {
                // If there is no current file - that manes that we are being asked to rename something that doesn't exist!
                GetFilePathFromObjects(obj, out NTH1 hPath, out NTH1 hSize);
                var pname = hPath == null ? "<noname>" : hPath.Title;
                var length = hSize == null ? 0 : (long) hSize.GetBinContent(1);
                throw new InvalidOperationException($"Unable to find the output file to rename (was looking for '{pname}' with no cycle and legnth {length}).");
                return;
            }
            var newFile = GetFileInfo(iVariable, obj, cycle, doChecks: false);

            if (newFile.Exists)
            {
                newFile.Delete();
            }

            currentFile.MoveTo(newFile.FullName);
        }

        /// <summary>
        /// Generate the code to save this to a ROOT file to be passed back.
        /// We don't pass back the whole file - rather we just do the file path.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public IEnumerable<string> SaveToFile(IDeclaredParameter v)
        {
            // To save this right we need to store a string with a lookup key. Unfortunately, TObjString
            // won't do that - it's Name is the same as its value. So lets just use a TH1 - as it has a name and
            // title. :-)

            if (v.InitialValue == null)
            {
                throw new InvalidOperationException($"Unable to save OutputTextFileType because it's parameter has no declared value!");
            }
            var rootFileInfo = v.InitialValue as OutputTTreeFileType;

            // Close the file.
            yield return $"{v.RawValue}.first->Write();";
            yield return $"{v.RawValue}.first->Close();";

            // Write out the path.
            yield return $"TH1D *{v.RawValue}_hist = new TH1D(\"{v.RawValue}\", \"{rootFileInfo.OutputFileFunc().FullName.AddCPPEscapeCharacters()}\", 1, 0.0, 1.0);";
            yield return v.RawValue + "_hist->SetDirectory(0);";
            yield return "Book(" + v.RawValue + "_hist);";

            // Write out the mod time and the file size.
            yield return $"Long_t {v.RawValue}_size;";
            yield return $"Long_t {v.RawValue}_modification_time;";
            yield return $"gSystem->GetPathInfo({v.RawValue}.first->GetName(), 0, &{v.RawValue}_size, 0, &{v.RawValue}_modification_time);";

            foreach (var s in SaveIntValue($"{v.RawValue}_size"))
            {
                yield return s;
            }
        }

        /// <summary>
        /// Write out an int value.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        private IEnumerable<string> SaveIntValue(string v)
        {
            yield return string.Format("TH1D *{0}_hist = new TH1D(\"{0}\", \"var transport\", 1, 0.0, 1.0);", v);
            yield return $"{v}_hist->SetBinContent(1, {v});";
            yield return v + "_hist->SetDirectory(0);";
            yield return $"Book({v}_hist);";
        }
    }
}
