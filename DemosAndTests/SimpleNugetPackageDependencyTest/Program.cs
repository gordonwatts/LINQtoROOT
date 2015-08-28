using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleNugetPackageDependencyTest
{
    /// <summary>
    /// Really simple code that uses LINQToROOT using the nuget package.
    /// For testing, note that the package referenced is often located locally... this is just
    /// to help make sure that everything about the *build* is working properly.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            var rootFile = new FileInfo(@"..\..\..\..\hvsample.root");
            if (!rootFile.Exists)
            {
                Console.WriteLine("Unable to find the input file '" + rootFile.FullName + "'");
                return;
            }

            var rf1 = ROOTLINQ.QueryableCollectionTree.CreateQueriable(rootFile);

            var c = rf1.Count();
            Console.WriteLine("Number of events: {0}", c);
        }
    }
}
