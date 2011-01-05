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
        /// Return all trees that are in a given directory
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public IEnumerable<ROOTClassShell> ParseTDirectory(ROOTNET.Interface.NTDirectory dir)
        {
            var converter = new ParseTTree();

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

            return ParseTDirectory(f);
        }
    }
}
