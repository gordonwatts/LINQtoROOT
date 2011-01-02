using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqToTTreeInterfacesLib;

namespace LINQToTTreeLib.Statements
{
    /// <summary>
    /// Looping over something in the ntuple that looks like an array (i.e. has an IEnumerable guy). In this case
    /// we are going to be doing it over a Vector.
    /// </summary>
    public class StatementLoopOnVector : StatementInlineBlock
    {
        public StatementLoopOnVector(LinqToTTreeInterfacesLib.IValue arrayToIterateOver, string iteratorVarName)
        {
            VectorToLoopOver = arrayToIterateOver;
            IteratorName = iteratorVarName;
        }
        /// <summary>
        /// Get the name of the variable that we are going to be looping over.
        /// </summary>
        public IValue VectorToLoopOver { get; private set; }

        /// <summary>
        /// Get the name of the iterator that we are using for looping.
        /// </summary>
        public string IteratorName { get; private set; }
    }
}
