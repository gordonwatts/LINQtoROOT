using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace CmdTFileParser
{
    class Program
    {
        /// <summary>
        /// Command line interface to parse all the TTree's in a
        /// TFile and make the XML file that specifies their layout
        /// for later object generation.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            ///
            /// Parse the inputs
            /// 

            List<FileInfo> rootFiles = new List<FileInfo>();
            List<FileInfo> libraries = new List<FileInfo>();
            string specialFile = "";
            FileInfo outputFile = new FileInfo("ntupleinfo.ntupom");
            bool doExistanceCheck = true;

            foreach (var arg in args)
            {
                if (arg == "-o")
                {
                    specialFile = "o";
                    doExistanceCheck = false;
                    continue;
                }

                FileInfo f = new FileInfo(arg);
                if (doExistanceCheck && !f.Exists)
                {
                    Console.WriteLine("Could not file file {0}", f.FullName);
                    return;
                }
                doExistanceCheck = true;

                if (specialFile.Length > 0)
                {
                    if (specialFile == "o")
                        outputFile = f;
                    specialFile = "";
                }
                else if (f.Extension == ".root")
                {
                    rootFiles.Add(f);
                }
                else
                {
                    libraries.Add(f);
                }
            }

            if (rootFiles.Count == 0)
            {
                Console.WriteLine("no input root files to scan!");
                return;
            }

            ///
            /// Next, load up all the libraries
            /// 

            var gSystem = ROOTNET.NTSystem.gSystem;
            foreach (var lib in libraries)
            {
                gSystem.Load(lib.FullName);
            }

            ///
            /// And now process the root files!
            /// 

            var converter = new TTreeParser.ParseTFile();
            var rootClassList = from f in rootFiles
                                from c in converter.ParseFile(f)
                                select c;
            var allClasses = rootClassList.ToArray();
            if (allClasses.Length == 0)
            {
                Console.WriteLine("No classes were found in the input files!");
                return;
            }

            ///
            /// Write out the output xml file now
            /// 

            XmlSerializer xmlout = new XmlSerializer(typeof(TTreeDataModel.ROOTClassShell[]));
            using (var output = outputFile.CreateText())
            {
                xmlout.Serialize(output, allClasses);
                output.Close();
            }
        }
    }
}
