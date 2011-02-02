///
/// Simple demo program that shows one how to do some very basic things
/// with LINQ and a local ntuple.
/// 
using System;
using System.IO;
using System.Linq;
using ROOTTTreeDataModel;

namespace DemoJetShapes
{
    class Program
    {
        /// <summary>
        /// Simple demo to look at jets in a local root-tuple.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            FileInfo rootFile = new FileInfo(@"..\..\..\output.root");
            if (!rootFile.Exists)
            {
                Console.WriteLine("Unable to find the input file '" + rootFile.FullName + "'");
                return;
            }

            //var rf = new Queryablebtag(rootFile);
            var rf1 = Queryablebtag.Create(rootFile);

            ///
            /// Count the # of events in the file
            /// 

            int count = rf1.Count();
            Console.WriteLine("The number of events in the ntuple, counted the hard way is {0}.", count);
        }
    }
}
