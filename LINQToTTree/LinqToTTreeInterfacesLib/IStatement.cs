using System.Collections.Generic;

namespace LinqToTTreeInterfacesLib
{
    /// <summary>
    /// Any statement that is to be translated needs to be stored here. These are strictly C++
    /// statements, and not meant to be language agnostic.
    /// </summary>
    public interface IStatement
    {
        /// <summary>
        /// Generate the C++ code. If, eventually, we want to make this lang agnostic, we need
        /// to pass in an object that is responsible for changing info into statements...
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> CodeItUp();
    }
}
