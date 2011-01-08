using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using TTreeDataModel;

namespace TTreeClassGenerator
{
    /// <summary>
    /// Starting from the class interface, generate a class that can be used in LINQ queires.
    /// All classes are generated in a single output file.
    /// </summary>
    public class ClassGenerator
    {
        /// <summary>
        /// GIven an existing file that contains the XML class spec, generate a CS file
        /// that conatins all the definitions needed. Everything will be burried insize
        /// the namespaceName.
        /// </summary>
        /// <param name="inputXMLFile"></param>
        /// <param name="outputCSFile"></param>
        public void GenerateClasss(FileInfo inputXMLFile, FileInfo outputCSFile, string namespaceName)
        {
            ///
            /// Quick checks
            /// 

            if (inputXMLFile == null)
                throw new ArgumentNullException("inputXMLFile");
            if (!inputXMLFile.Exists)
                throw new ArgumentException("inputXMLFile does not currently exist!");

            ///
            /// Load up the XML file
            /// 

            var classSpec = LoadFromXMLFile(inputXMLFile);

            ///
            /// Now do the actual work!
            /// 

            GenerateClasss(classSpec, outputCSFile, namespaceName);
        }

        /// <summary>
        /// Generate the classes given the input classes
        /// </summary>
        /// <param name="classSpec"></param>
        /// <param name="outputCSFile"></param>
        /// <param name="namespaceName"></param>
        public void GenerateClasss(ROOTClassShell[] classSpec, FileInfo outputCSFile, string namespaceName)
        {
            ///
            /// Parameter checks
            /// 

            if (outputCSFile == null)
                throw new ArgumentNullException("outputCSFile");
            if (string.IsNullOrWhiteSpace(namespaceName))
                throw new ArgumentNullException("namespaceName");

            bool isbad = namespaceName.Any(c => !char.IsLetterOrDigit(c));
            if (isbad || !char.IsLetter(namespaceName[0]))
            {
                throw new ArgumentException("Namespace is not a legal C++ name");
            }

            if (classSpec == null)
                throw new ArgumentNullException("classSpec");

            ///
            /// Ok, open the output file
            /// 

            using (var output = outputCSFile.CreateText())
            {
                ///
                /// Write out the header and using clauses
                /// 

                output.WriteLine("//");
                output.WriteLine("// Automatically Generated File - do not modify!");
                output.WriteLine("// Generated on {0} by TTreeClassGenerator::ClassGenerator", DateTime.Now);
                output.WriteLine("// Translated ntuple classes");
                foreach (var cls in classSpec)
                {
                    output.WriteLine("// - ntuple {0}", cls.Name);
                }

                output.WriteLine("//");
                output.WriteLine();

                output.WriteLine("using System.IO;");
                output.WriteLine("using LINQToTTreeLib;");

                output.WriteLine();
                output.WriteLine("namespace {0} {{", namespaceName);

                ///
                /// Now, write something out for each class
                /// 

                foreach (var cls in classSpec)
                {
                    output.WriteLine("  public class {0}", cls.Name);
                    output.WriteLine("  {");

                    /// These fields will never be set or accessed - so we turn off some warnings the compiler will generate.
                    output.WriteLine("#pragma warning disable 0649");

                    foreach (var item in cls.Items)
                    {
                        WriteItem(item, output);
                    }

                    output.WriteLine("#pragma warning restore 0649");

                    output.WriteLine("  }"); // End of the class

                    ///
                    /// Next, write out the queriable classes so we can actually do the queires
                    /// against the trees!
                    /// 

                    output.WriteLine("  class Queryable{0} : QueriableTTree<{0}>", cls.Name);
                    output.WriteLine("  {");
                    output.WriteLine("    public Queryable{0} (FileInfo rootFile, string treeName = \"{0}\")", cls.Name);
                    output.WriteLine("              : base (rootFile, treeName)");
                    output.WriteLine("    {}");
                    output.WriteLine("  }");
                }

                ///
                /// Done!
                /// 

                output.WriteLine("}"); // For the namespace

                output.Close();
            }
        }

        /// <summary>
        /// Write out the data in a single item
        /// </summary>
        /// <param name="item"></param>
        /// <param name="output"></param>
        private void WriteItem(IClassItem item, StreamWriter output)
        {
            if (item == null)
                throw new ArgumentNullException("item - can't have a null item in an ntuple!");

            string t = item.ItemType;
            output.WriteLine("    public {0} {1};", t, item.Name);
        }

        /// <summary>
        /// Loads in an input file and returns the classes.
        /// </summary>
        /// <param name="inputXMLFile"></param>
        /// <returns></returns>
        private ROOTClassShell[] LoadFromXMLFile(FileInfo inputXMLFile)
        {
            XmlSerializer x = new XmlSerializer(typeof(ROOTClassShell[]));
            using (var reader = inputXMLFile.OpenText())
            {
                var result = x.Deserialize(reader) as ROOTClassShell[];
                reader.Close();
                return result;
            }
        }
    }
}
