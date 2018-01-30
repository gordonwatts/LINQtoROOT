using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Utils;
using LINQToTTreeLib.Variables;
using ROOTNET.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LINQToTTreeLib.Files
{
    /// <summary>
    /// Represents a text file that is written out by the TSelector while running.
    /// </summary>
    [CPPObjectRepresentationType("std::ofstream")]
    class OutputCSVTextFileType : IValue
    {
        /// <summary>
        /// Get the output file spec
        /// </summary>
        public Func<FileInfo> OutputFile
        {
            get; private set;
        }

        /// <summary>
        /// Initialize with the proper file to stash.
        /// </summary>
        /// <param name="outputFile"></param>
        public OutputCSVTextFileType(FileInfo outputFile)
        {
            this.OutputFile = () => outputFile;
        }

        /// <summary>
        /// Delayed resolution (for query hash names,etc.)
        /// </summary>
        /// <param name="outputFile"></param>
        public OutputCSVTextFileType(Func<FileInfo> outputFile)
        {
            this.OutputFile = outputFile;
        }

        /// <summary>
        /// The actual value of the object, which in this case is the
        /// file stream constructor
        /// </summary>
        public string RawValue
        {
            get
            {
                return $"std::ofstream(\"<><>{OutputFile().FullName.AddCPPEscapeCharacters()}<><>\")";
            }
        }

        /// <summary>
        /// Get the type this object is holding. Which is just us.
        /// </summary>
        public Type Type { get { return typeof(OutputCSVTextFileType); } }

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
    class OutputTextFileTypeSaver : IVariableSaver
    {
        public bool CanHandle(IDeclaredParameter iVariable)
        {
            return iVariable.Type == typeof(OutputCSVTextFileType);
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
            yield return "<fstream>";
            yield return "TH1D.h";
            yield return "TSystem.h";
        }

        /// <summary>
        /// Load up the item from the ROOT file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="iVariable"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public Task<T> LoadResult<T>(IDeclaredParameter iVariable, RunInfo[] obj)
        {
            var f = GetFileInfo(iVariable, obj.Select(ri => ri._result).ToArray(), obj.First()._cycle);
            return Task.FromResult(f == null
                ? (T)(object) null
                : (T)(object)(new FileInfo[] { f }));
        }

        /// <summary>
        /// Fix up the filename for the cycle we have to deal with.
        /// </summary>
        /// <param name="iVariable"></param>
        /// <param name="obj"></param>
        /// <param name="cycle"></param>
        public void RenameForQueryCycle(IDeclaredParameter iVariable, NTObject[] obj, int cycle, DirectoryInfo queryDirectory)
        {
            if (obj == null || iVariable == null)
            {
                throw new ArgumentException("Null argument not permitted");
            }

            var currentFile = GetFileInfo(iVariable, obj, alternatDirectory: queryDirectory);
            if (currentFile == null)
            {
                // If there is no current file - that manes that we are being asked to rename something that doesn't exist!
                GetFilePathFromObjects(obj, out NTH1 hPath, out NTH1 hSize);
                var pname = hPath == null ? "<noname>" : hPath.Title;
                var length = hSize == null ? 0 : (long) hSize.GetBinContent(1);
                throw new InvalidOperationException($"Unable to find the output file to rename (was looking for '{pname}' with no cycle and legnth {length}).");
            }
            var newFile = GetFileInfo(iVariable, obj, cycle, doChecks: false);

            if (newFile.Exists)
            {
                newFile.Delete();
            }

            currentFile.MoveTo(newFile.FullName);
        }

        /// <summary>
        /// Return the file info for this output.
        /// </summary>
        /// <param name="iVariable"></param>
        /// <param name="obj"></param>
        /// <param name="cycle">The cycle number for this file. If null, then the raw file as written by the code.</param>
        /// <returns></returns>
        private FileInfo GetFileInfo(IDeclaredParameter iVariable, NTObject[] obj, int? cycle = null, bool doChecks = true, DirectoryInfo alternatDirectory = null)
        {
            // Fetch out the path and the size in bytes of the file.
            GetFilePathFromObjects(obj, out NTH1 hPath, out NTH1 hSize);

            if (hPath == null || hSize == null)
            {
                throw new InvalidOperationException("Internal error - cache is missing either the path for a CSV file or its size");
            }

            // Deal with the cycle - we just add an index onto the filename.
            var filename = Path.GetFileName(hPath.Title);
            var directory = Path.GetDirectoryName(hPath.Title);
            if (cycle.HasValue)
            {
                filename = $"{Path.GetFileNameWithoutExtension(filename)}_{cycle.Value}{Path.GetExtension(filename)}";
            }

            // If no checks are required, return the ideal location of the file.
            if (!doChecks)
            {
                return new FileInfo(Path.Combine(directory, filename));
            }

            // Since we are doing checks, look in both places for the file.
            var directoriesToSearch = new[] { new DirectoryInfo(directory), alternatDirectory };
            var bestFile = directoriesToSearch
                .Where(d => d != null)
                .Select(d => new FileInfo(Path.Combine(d.FullName, filename)))
                .Where(f => f.Exists)
                .Where(f => f.Length == (long)hSize.GetBinContent(1))
                .OrderByDescending(f => f.LastWriteTime)
                .FirstOrDefault();

            return bestFile;
        }

        private static void GetFilePathFromObjects(NTObject[] obj, out NTH1 hPath, out NTH1 hSize)
        {
            hPath = null;
            hSize = null;
            foreach (var h in obj.Where(o => o != null))
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

            // Close the file.
            yield return $"{v.RawValue}.close();";

            // Write out the path.
            var fileAsCPPString = (v.InitialValue as OutputCSVTextFileType).OutputFile().FullName.AddCPPEscapeCharacters();
            yield return string.Format("TH1D *{0}_hist = new TH1D(\"{0}\", \"{1}\", 1, 0.0, 1.0);", v.RawValue, fileAsCPPString);
            yield return v.RawValue + "_hist->SetDirectory(0);";
            yield return "Book(" + v.RawValue + "_hist);";

            // Write out the mod time and the file size.
            yield return $"Long64_t {v.RawValue}_size;";
            yield return $"Long_t {v.RawValue}_modification_time;";
            yield return $"gSystem->GetPathInfo(\"<><>{fileAsCPPString}<><>\", 0, &{v.RawValue}_size, 0, &{v.RawValue}_modification_time);";

            foreach (var s in SaveIntValue($"{v.RawValue}_size"))
            {
                yield return s;
            }
        }

        private IEnumerable<string> SaveIntValue(string v)
        {
            yield return string.Format("TH1D *{0}_hist = new TH1D(\"{0}\", \"var transport\", 1, 0.0, 1.0);",v);
            yield return $"{v}_hist->SetBinContent(1, {v});";
            yield return v + "_hist->SetDirectory(0);";
            yield return $"Book({v}_hist);";
        }
    }
}
