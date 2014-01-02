using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

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
        }

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
        public IEnumerable<QMFunctionArgument> Arguments { get; private set; }

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
        /// The expression that gathers up the result after the statemenet body code has
        /// been run.
        /// </summary>
        public Expression Result { get; private set; }

        /// <summary>
        /// Get the type of this function's return.
        /// </summary>
        public Type ResultType { get { return Result.Type; } }

        /// <summary>
        /// The function call that will invoke us.
        /// </summary>
        private IValue _functionCall;

        /// <summary>
        /// Remember the code body for later use, along with the result that we will be
        /// returning.
        /// </summary>
        /// <param name="statements"></param>
        /// <param name="resultExpression"></param>
        public void SetCodeBody(IStatementCompound statements, Expression resultExpression)
        {
            StatementBlock = statements;
            Result = resultExpression;
        }

        /// <summary>
        /// Get the list of arguments for this function
        /// </summary>
        IEnumerable<object> IQMFunctionSource.Arguments
        {
            get { return _header.Arguments; }
        }
    }
}
