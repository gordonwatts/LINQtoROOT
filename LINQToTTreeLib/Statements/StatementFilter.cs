using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqToTTreeInterfacesLib;

namespace LINQToTTreeLib.Statements
{
    /// <summary>
    /// Deal with a "Where"-like clause. Basically, we have an expression which we evaluate, and we make sure that
    /// it goes!
    /// </summary>
    public class StatementFilter : StatementInlineBlock
    {
        /// <summary>
        /// Get the expresion we are going to test
        /// </summary>
        public IValue TestExpression { get; private set; }

        /// <summary>
        /// testExpression is what we test against to see if we should fire!
        /// </summary>
        /// <param name="testExpression"></param>
        public StatementFilter(IValue testExpression)
        {
            TestExpression = testExpression;
        }
    }
}
