using System;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Statements;

namespace LINQToTTreeLib.Variables
{
    /// <summary>
    /// A simple value that can be iterated over. In C++ this is represented by a vector of the type
    /// that we have (i.e. we can use things like v.size() to get the size of the thing we are iterating
    /// over).
    /// </summary>
    class ValEnumerableVector : IValue, ISequenceAccessor
    {
        public ValEnumerableVector(string rawVal, Type t)
        {
            // TODO: Complete member initialization
            RawValue = rawVal;
            Type = t;
        }

        /// <summary>
        /// Get the raw value
        /// </summary>
        public string RawValue { get; private set; }

        /// <summary>
        /// Get the type of this item
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// We want to generate some loop statements.
        /// </summary>
        /// <param name="env"></param>
        /// <param name="context"></param>
        /// <param name="indexName"></param>
        public void AddLoop(IGeneratedCode env, ICodeContext context, string indexName)
        {
            var loopstatement = new StatementLoopOnVector(this, indexName);
            env.Add(loopstatement);
            context.Add(indexName, loopstatement.ObjectReference);
        }
    }
}
