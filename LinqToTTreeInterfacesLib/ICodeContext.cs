using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqToTTreeInterfacesLib
{
    /// <summary>
    /// Context for coding - this is the temprorary stuff that follows along during code generation... but doesn't
    /// need to be kept around once we are done.
    /// </summary>
    public interface ICodeContext
    {
        /// <summary>
        /// Add a variable mapping. Used most often for dealing with parameters and the like
        /// </summary>
        /// <param name="varName"></param>
        /// <param name="replacementName"></param>
        void Add(string varName, IValue replacementName);

        /// <summary>
        /// Lookup a replacement
        /// </summary>
        /// <param name="varname"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        IValue GetReplacement(string varname, Type type);
    }
}
