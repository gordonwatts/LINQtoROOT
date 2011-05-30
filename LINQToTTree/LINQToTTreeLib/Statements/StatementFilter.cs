using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// Return the code for this if statement.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<string> CodeItUp()
        {
            ///
            /// If there are no held statements, then we can optimize away the
            /// if statement!
            /// 

            if (Statements.Any())
            {

                ///
                /// Now, emit the if statement and work from there
                /// 

                yield return "if (" + TestExpression.RawValue + ")";
                foreach (var l in base.CodeItUp())
                {
                    yield return l;
                }
            }
        }

        /// <summary>
        /// We don't have code to carefully do checks - so we just blow off the combination here.
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        public override bool TryCombineStatement(IStatement statement)
        {
            return false;
        }
    }
}
