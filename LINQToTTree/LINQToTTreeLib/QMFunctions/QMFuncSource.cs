using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Utils;
using System.Collections.Generic;
using System.Linq;

namespace LINQToTTreeLib.QMFunctions
{
    /// <summary>
    /// Everything needed to reference a function for a QueryModel, as well as emit it to C++
    /// </summary>
    public class QMFuncSource : IQMFunctionSource
    {
        private QMFuncHeader _headerInfo;

        /// <summary>
        /// Create the QM Func Source starting from a pre-parsed bunch of header info.
        /// </summary>
        /// <param name="f"></param>
        public QMFuncSource(QMFuncHeader f)
        {
            this._headerInfo = f;
            Name = "QMFunction".CreateUniqueVariableName();
            Arguments = f.Arguments.Select(a => new QMFunctionArgument(a));
            StatementBlock = new StatementInlineBlock();
        }

        /// <summary>
        /// Empty ctor.
        /// </summary>
        public QMFuncSource()
        {
            this._headerInfo = null;
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
            return _headerInfo.QMText == qmText;
        }
    }
}
