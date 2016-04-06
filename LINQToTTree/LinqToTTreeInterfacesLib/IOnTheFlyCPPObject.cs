using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqToTTreeInterfacesLib
{
    /// <summary>
    /// If your object can generate C++ code on-the-fly for the
    /// LINQToTTree system, implement this interface.
    /// </summary>
    public interface IOnTheFlyCPPObject
    {
        /// <summary>
        /// Return a list of include files. Null is just fine.
        /// </summary>
        /// <returns></returns>
        string[] IncludeFiles();

        /// <summary>
        /// Generate the lines of code for a given method. You must follow appropriate coding guideliens:
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> LinesOfCode(string methodName);
    }
}
