using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TTreeDataModel;

namespace TTreeParser
{
    /// <summary>
    /// Drive the parsing of a file - and do all the trees in it. Returns all the types required for it.
    /// Only looks for trees that are in the base of the file! We look only at the tree's that are in
    /// the root level of the file.
    /// </summary>
    public class ParseTFile
    {
        /// <summary>
        /// Where should the proxy be generated?
        /// </summary>
        public DirectoryInfo ProxyGenerationLocation { get; set; }

        /// <summary>
        /// Return all trees that are in a given directory
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public IEnumerable<ROOTClassShell> ParseTDirectory(ROOTNET.Interface.NTDirectory dir)
        {
            var converter = new ParseTTree();
            converter.ProxyGenerationLocation = ProxyGenerationLocation;

            foreach (var key in dir.ListOfKeys.Cast<ROOTNET.Interface.NTKey>())
            {
                var c = ROOTNET.NTClass.GetClass(key.GetClassName());
                if (c != null)
                {
                    if (c.InheritsFrom("TTree"))
                    {
                        var t = key.ReadObj() as ROOTNET.Interface.NTTree;
                        if (t != null)
                        {
                            foreach (var cshell in converter.GenerateClasses(t))
                            {
                                yield return cshell;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Look for all trees at the base directory of a TFile. We aren't
        /// scanning recursively!!
        /// </summary>
        /// <param name="inputFile"></param>
        /// <returns></returns>
        public IEnumerable<ROOTClassShell> ParseFile(FileInfo inputFile)
        {
            if (!inputFile.Exists)
                throw new ArgumentException("input file does not exist");

            var f = new ROOTNET.NTFile(inputFile.FullName, "READ");
            if (!f.IsOpen())
            {
                throw new ArgumentException("Failed to open file in ROOT");
            }

            ///
            /// See if we can find a file with further stuff we should be adding
            /// 

            var fFurtherInfo = new FileInfo(Path.ChangeExtension(inputFile.FullName, ".root-extra-info"));
            Dictionary<string, string[]> extraInfo = fFurtherInfo.ParseAsINIFile(true);

            foreach (var cls in ParseTDirectory(f))
            {
                if (extraInfo.ContainsKey("CINT"))
                {
                    cls.CINTExtraInfo = extraInfo["CINT"];
                }
                if (extraInfo.ContainsKey("CreateDictionary"))
                {
                    ///
                    /// Do some basic syntax checking
                    /// 

                    var allDictTypes = from l in extraInfo["CreateDictionary"]
                                       let spec = GetDictSpec(l)
                                       where spec != null
                                       select spec;
                    cls.ClassesToGenerate = allDictTypes.ToArray();
                }
                yield return cls;
            }
        }

        /// <summary>
        /// Convert a string into class / includes.
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        private ClassForDictionary GetDictSpec(string l)
        {
            if (string.IsNullOrWhiteSpace(l))
                return null;
            if (l.StartsWith("#") || l.StartsWith("//"))
                return null;

            var bySemi = l.Split(';');
            if (bySemi.Length > 2)
                throw new ArgumentException("Line in the root-extra-info file '" + l + "' has more than one semicolon in it - please see the docs on how to format this file.");

            var result = new ClassForDictionary() { classSpec = bySemi[0].Trim(), includeFiles = "" };
            if (bySemi.Length == 2)
                result.includeFiles = bySemi[1].Trim();

            return result;
        }
    }
}
