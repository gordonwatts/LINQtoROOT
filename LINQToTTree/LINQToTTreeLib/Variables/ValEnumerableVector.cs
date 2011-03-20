using System;
using System.Linq.Expressions;
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
        public ValEnumerableVector(Expression vector)
        {
            RawVectorValue = vector;
        }

        private Expression RawVectorValue { get; set; }

        /// <summary>
        /// Get the raw value
        /// </summary>
        public string RawValue
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Get the type of this item
        /// </summary>
        public Type Type { get { return RawVectorValue.Type; } }

        /// <summary>
        /// We want to generate some loop statements.
        /// </summary>
        /// <param name="env"></param>
        /// <param name="context"></param>
        /// <param name="indexName"></param>
        public IVariable AddLoop(IGeneratedCode env, ICodeContext context, string indexName, Action<IVariableScopeHolder> popVariableContext)
        {
            var loopstatement = new StatementLoopOnVector(RawVectorValue, typeof(int).CreateUniqueVariableName());
            env.Add(loopstatement);
            popVariableContext(context.Add(indexName, loopstatement.ObjectReference));

            return new VarDeclared(indexName);
        }
    }
}
