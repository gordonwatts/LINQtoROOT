using System;
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
        /// Return the column headers for this text file.
        /// </summary>
        public string[] ColumnHeaders { get; private set; }

        /// <summary>
        /// Initialize with the proper file to stash.
        /// </summary>
        /// <param name="outputFile"></param>
        public OutputCSVTextFileType(FileInfo outputFile, string[] colHeaders)
        {
            this.OutputFile = outputFile;
            this.ColumnHeaders = colHeaders;
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
        /// Include files that need to be used. Since this is an fstream...
        /// </summary>
        /// <param name="iVariable"></param>
        /// <returns></returns>
        public IEnumerable<string> IncludeFiles(IDeclaredParameter iVariable)
        {
            yield return "<fstream>";
        }

        /// <summary>
        /// Load up the item from the ROOT file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="iVariable"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public T LoadResult<T>(IDeclaredParameter iVariable, NTObject obj)
        {
            var s = obj as NTH1F;
            if (s == null) throw
                    new InvalidOperationException($"FileInfo cached value should be a TObjString object, but is {s.GetType().Name}.");

            // We have to do this funny type conversion b.c. though we will only be called with a
            // T == FileInfo, the compiler doesn't know that. It could be a "FileInfo" or an "int" as far
            // as it is concerned.
            object o = new FileInfo(s.Title);
            return (T)o;
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

            yield return $"{v.RawValue}.close();";
            yield return string.Format("TH1F *{0}_hist = new TH1F(\"{0}\", \"{1}\", 1, 0.0, 1.0);", v.RawValue, (v.InitialValue as OutputCSVTextFileType).OutputFile.FullName.AddCPPEscapeCharacters());
            yield return v.RawValue + "_hist->SetDirectory(0);";
            yield return "Book(" + v.RawValue + "_hist);";
        }
    }
}
