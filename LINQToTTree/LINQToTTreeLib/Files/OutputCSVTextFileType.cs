﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LINQToTTreeLib.Variables;
using LinqToTTreeInterfacesLib;
using System.IO;
using LINQToTTreeLib.Utils;
using ROOTNET.Interface;
using System.ComponentModel.Composition;

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
        public FileInfo OutputFile
        {
            get; private set;
        }

        /// <summary>
        /// Initialize with the proper file to stash.
        /// </summary>
        /// <param name="outputFile"></param>
        public OutputCSVTextFileType(FileInfo outputFile)
        {
            this.OutputFile = outputFile;
        }

        /// <summary>
        /// The actual value of the object, which in this case is the
        /// filestream ctor
        /// </summary>
        public string RawValue { get { return $"std::ofstream(\"{OutputFile.FullName.AddCPPEscapeCharacters()}\")"; } }

        /// <summary>
        /// Get the type this object is holding. Which is just us.
        /// </summary>
        public Type Type { get { return typeof(OutputCSVTextFileType); } }

        /// <summary>
        /// No renaming can happen since we hold what is basically a constnat value.
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
        /// Include files that need to be used. Since this is an fstream...
        /// </summary>
        /// <param name="iVariable"></param>
        /// <returns></returns>
        public IEnumerable<string> IncludeFiles(IDeclaredParameter iVariable)
        {
            yield return "<fstream>";
            yield return "TH1I.h";
            yield return "TSystem.h";
        }

        /// <summary>
        /// Load up the item from the ROOT file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="iVariable"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public T LoadResult<T>(IDeclaredParameter iVariable, NTObject[] obj)
        {
            // Fetch out the path and the size in bytes of the file.
            NTH1I hPath = null, hSize = null;

            foreach (var h in obj)
            {
                if (h.Name.EndsWith("_size"))
                {
                    hSize = h as NTH1I;
                } else
                {
                    hPath = h as NTH1I;
                }
            }

            if (hPath == null || hSize == null)
            {
                throw new InvalidOperationException("Internal error - cache is missing either the path for a CSV file or its size");
            }

            // See if the file is there, and make sure its size is the same.
            // That will have to do for the cache lookup.
            // Funny conversion are b.c. we are in the middle of a crazy generic here.

            var f = new FileInfo(hPath.Title);
            if (!f.Exists)
            {
                return (T) (object) null;
            }

            if (f.Length != (int) hSize.GetBinContent(1))
            {
                return (T)(object)null;
            }

            return (T)(object)f;
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
            var fileAsCPPString = (v.InitialValue as OutputCSVTextFileType).OutputFile.FullName.AddCPPEscapeCharacters();
            yield return string.Format("TH1I *{0}_hist = new TH1I(\"{0}\", \"{1}\", 1, 0.0, 1.0);", v.RawValue, fileAsCPPString);
            yield return v.RawValue + "_hist->SetDirectory(0);";
            yield return "Book(" + v.RawValue + "_hist);";

            // Write out the mod time and the file size.
            yield return $"Long_t {v.RawValue}_size;";
            yield return $"Long_t {v.RawValue}_modification_time;";
            yield return $"gSystem->GetPathInfo(\"{fileAsCPPString}\", 0, &{v.RawValue}_size, 0, &{v.RawValue}_modification_time);";

            foreach (var s in SaveIntValue($"{v.RawValue}_size"))
            {
                yield return s;
            }
        }

        private IEnumerable<string> SaveIntValue(string v)
        {
            yield return string.Format("TH1I *{0}_hist = new TH1I(\"{0}\", \"var transport\", 1, 0.0, 1.0);",v);
            yield return $"{v}_hist->SetBinContent(1, {v});";
            yield return v + "_hist->SetDirectory(0);";
            yield return $"Book({v}_hist);";
        }
    }
}