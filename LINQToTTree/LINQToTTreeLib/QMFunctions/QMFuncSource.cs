using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LINQToTTreeLib.QMFunctions
{
    /// <summary>
    /// Everything needed to reference a function for a QueryModel, as well as emit it to C++
    /// </summary>
    public class QMFuncSource : IQMFunctionSource
    {
        /// <summary>
        /// Get the header info for this function.
        /// </summary>
        private QMFuncHeader _header;

        /// <summary>
        /// Create the QM Func Source starting from a pre-parsed bunch of header info.
        /// </summary>
        /// <param name="f"></param>
        public QMFuncSource(QMFuncHeader f)
        {
            this._header = f;
            Name = "QMFunction".CreateUniqueVariableName();
            Arguments = f.Arguments.Select(a => new QMFunctionArgument(a));
            StatementBlock = null;

            CacheVariable = DeclarableParameter.CreateDeclarableParameterExpression(ResultType);
            CacheVariableGood = DeclarableParameter.CreateDeclarableParameterExpression(typeof(bool));
        }

        /// <summary>
        /// Get the cache variable used by this fellow.
        /// </summary>
        public IDeclaredParameter CacheVariable { get; private set; }

        /// <summary>
        /// Get the cache variable good used by this fellow.
        /// </summary>
        public IDeclaredParameter CacheVariableGood { get; private set; }

        /// <summary>
        /// Empty ctor.
        /// </summary>
        public QMFuncSource()
        {
            this._header = null;
        }

        /// <summary>
        /// The name this member function in C++ source code.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Returns an in-order list of arguments used by this function.
        /// </summary>
        public IEnumerable<IQMArgument> Arguments { get; private set; }

        /// <summary>
        /// The list of statements that actually process this guy.
        /// </summary>
        public IStatementCompound StatementBlock { get; private set; }

        /// <summary>
        /// Return true if the QM text translation matches the QM we are holding onto here.
        /// </summary>
        /// <param name="qmText"></param>
        /// <returns></returns>
        internal bool Matches(string qmText)
        {
            return _header.QMText == qmText;
        }

        /// <summary>
        /// Get the type of this function's return.
        /// </summary>
        public Type ResultType { get { return _header.QM.GetResultType(); } }

        /// <summary>
        /// Remember the code body for later use, along with the result that we will be
        /// returning.
        /// </summary>
        /// <param name="statements"></param>
        /// <param name="resultExpression"></param>
        public void SetCodeBody(IStatementCompound statements)
        {
            StatementBlock = statements;
        }

        /// <summary>
        /// Return the QM text
        /// </summary>
        public string QueryModelText { get { return _header.QMText; } }

        /// <summary>
        /// See if this executable function matches another one.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Matches(IQMFuncExecutable other)
        {
            return QueryModelText == other.QueryModelText;
        }

        /// <summary>
        /// Rename all references to this function in our block.
        /// </summary>
        /// <param name="oldfname"></param>
        /// <param name="newfname"></param>
        public void RenameFunctionReference(string oldfname, string newfname)
        {
            StatementBlock.RenameVariable(oldfname, newfname);
        }
    }
}
