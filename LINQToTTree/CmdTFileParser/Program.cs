using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using TTreeDataModel;
using TTreeParser;

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
            FileInfo outputFile = null;
            DirectoryInfo outputDir = null;
            bool doExistanceCheck = true;

            foreach (var arg in args)
            {
                if (arg == "-o")
                {
                    specialFile = "o";
                    doExistanceCheck = false;
                    continue;
                }

                if (arg == "-d")
                {
                    specialFile = "outD";
                    doExistanceCheck = false;
                    continue;
                }

                if (specialFile == "outD")
                {
                    DirectoryInfo inf = new DirectoryInfo(arg);
                    if (!inf.Exists)
                        inf.Create();
                    outputDir = inf;
                    specialFile = "";
                    continue;
                }

                FileInfo f = new FileInfo(arg);
                if (doExistanceCheck && !f.Exists)
                {
                    SimpleLogging.Log("Could not file file {0}", f.FullName);
                    return;
                }
                doExistanceCheck = true;

                if (specialFile.Length > 0)
                {
                    if (specialFile == "o")
                        outputFile = f;
                    specialFile = "";
                }
                else if (f.Extension == ".root" || f.Name.Contains(".root."))
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
                SimpleLogging.Log("no input root files to scan!");
                return;
            }

            // Output directory for all our files is where the root file is located if we don't have anything better!
            if (outputDir == null)
            {
                outputDir = rootFiles[0].Directory;
            }

            if (outputFile == null)
                outputFile = new FileInfo(string.Format("{0}\\{1}", outputDir.FullName, Path.ChangeExtension(rootFiles[0].Name, "ntupom")));

            ///
            /// Make sure to write everything to a log file for the next step!
            /// 

            using (var logFile = File.CreateText(Path.ChangeExtension(outputFile.FullName, ".log")))
            {
                SimpleLogging.AddStream(logFile);

                ///
                /// Next, load up all the libraries
                /// 

                var gSystem = ROOTNET.NTSystem.gSystem;
                foreach (var lib in libraries)
                {
                    gSystem.Load(lib.FullName);
                }

                ///
                /// Next, we need to find all the classes that are in those libraries,
                /// and then figure out where the location of the classes is.
                /// 

                var loadedNames = (from s in libraries
                                   select Path.GetFileNameWithoutExtension(s.Name)).ToArray();

                var usedClasses = from cls in ROOTNET.NTROOT.gROOT.ListOfClasses.Cast<ROOTNET.Interface.NTClass>()
                                  let shared = cls.SharedLibs
                                  where shared != null
                                  let name = Path.GetFileNameWithoutExtension(shared.Split().First())
                                  where loadedNames.Contains(name)
                                  select cls;

                var sourcefiles = from cls in usedClasses
                                  select cls.GetImplFileName();

                ///
                /// And now process the root files!
                /// 

                var converter = new TTreeParser.ParseTFile
                {
                    ProxyGenerationLocation = outputDir
                };

                var rootClassList = from f in rootFiles
                                    from c in converter.ParseFile(f)
                                    select c;
                var allClasses = rootClassList.ToArray();
                if (allClasses.Length == 0)
                {
                    SimpleLogging.Log("No classes were found in the input files!");
                    return;
                }

                ///
                /// Write out the output xml file now
                /// 

                NtupleTreeInfo results = new NtupleTreeInfo() { Classes = allClasses, ClassImplimintationFiles = sourcefiles.ToArray() };
                XmlSerializer xmlout = new XmlSerializer(typeof(NtupleTreeInfo));
                using (var output = outputFile.CreateText())
                {
                    xmlout.Serialize(output, results);
                }
            }
        }
    }
}
