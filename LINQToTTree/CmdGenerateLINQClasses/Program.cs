using System;
using System.IO;

namespace CmdGenerateLINQClasses
{
    class Program
    {
        static void Usage()
        {
            Console.WriteLine("CmdGenerateLINQClasses <input-xml-file> <output-file.cs>");
        }
        /// <summary>
        /// Generate a set of classes reading in an XML file
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            ///
            /// Parse the command line arguments
            /// 

            string CSharpNtupleNamespace = "ROOTTTreeDataModel";

            if (args.Length != 2)
            {
                Usage();
                return;
            }

            FileInfo inputXMLFile = new FileInfo(args[0]);
            FileInfo outputCSFile = new FileInfo(args[1]);

            if (!inputXMLFile.Exists)
            {
                Console.WriteLine("Could not find input xml file '" + inputXMLFile.FullName + "'");
                return;
            }

            if (!outputCSFile.Directory.Exists)
            {
                Console.WriteLine("Could not find output cs file's parent directory '" + outputCSFile.Directory.FullName + "'");
                return;
            }

            ///
            /// Ok - the actual generation is pretty simple!
            /// 

            var generator = new TTreeClassGenerator.ClassGenerator();
            generator.GenerateClasss(inputXMLFile, outputCSFile, CSharpNtupleNamespace);
        }
    }
}
