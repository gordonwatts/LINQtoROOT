using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LINQToTTreeLib.Statements
{
    /// <summary>
    /// Looping over something in the ntuple that looks like an array (i.e. has an IEnumerable guy). In this case
    /// we are going to be doing it over a Vector.
    /// </summary>
    public class StatementLoopOnVector : StatementInlineBlock
    {
        public StatementLoopOnVector(Expression arrayToIterateOver, string iteratorVarName)
        {
            ///
            /// Simple checks to make sure that we actually have enumerable
            /// object to run over here
            /// 

            if (!arrayToIterateOver.Type.IsArray)
                throw new NotImplementedException("Can't iterate over object of type '" + arrayToIterateOver.Type.Name + "' because it isn't listed as an array (" + arrayToIterateOver.ToString() + ").");

            ///
            /// Save for later
            /// 

            VectorToLoopOver = arrayToIterateOver;
            IteratorVariable = Expression.Variable(typeof(int), iteratorVarName);

            ///
            /// Now just create the expression to access this guy.
            /// 

            ObjectReference = Expression.MakeBinary(ExpressionType.ArrayIndex, VectorToLoopOver, IteratorVariable);
        }

        /// <summary>
        /// Get the name of the variable that we are going to be looping over.
        /// </summary>
        public Expression VectorToLoopOver { get; private set; }

        /// <summary>
        /// Get the name of the iterator that we are using for looping.
        /// </summary>
        public ParameterExpression IteratorVariable { get; private set; }

        /// <summary>
        /// Get the value that points to the actual object that can reference the array
        /// </summary>
        public Expression ObjectReference { get; private set; }

        /// <summary>
        /// Code up the loop.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<string> CodeItUp()
        {
            throw new NotImplementedException();
#if false
            ///
            /// EMpty statement means no need to do the loop
            /// 

            if (Statements.Any())
            {
                yield return "for (int " + IteratorVariable + "=0; " + IteratorVariable + " < " + VectorToLoopOver.RawValue + "->size(); " + IteratorVariable + "++)";
                foreach (var l in base.CodeItUp())
                {
                    yield return l;
                }
            }
#endif
        }
    }
}
