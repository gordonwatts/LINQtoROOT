using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LINQToTTreeLib.Tests.Files
{
    static class FileUtilsForTesting
    {
        public static IEnumerable<string> ReadAllLines (this FileInfo file)
        {
            using (var rdr = file.OpenText())
            {
                string l = "ops";
                while ((l = rdr.ReadLine()) != null)
                {
                    if (l != null)
                    {
                        yield return l;
                    }
                }
            }
        }
    }
}
