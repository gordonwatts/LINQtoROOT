using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq.Expressions;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ResultOperators;

namespace LINQToTTreeLib.ResultOperators
{
    /// <summary>
    /// Do the any and all operators
    /// </summary>
    [Export(typeof(IQVScalarResultOperator))]
    class ROAnyAll : IQVScalarResultOperator
    {
        /// <summary>
        /// We can do onlly the any and all guys.
        /// </summary>
        /// <param name="resultOperatorType"></param>
        /// <returns></returns>
        public bool CanHandle(Type resultOperatorType)
        {
            return (resultOperatorType == typeof(AllResultOperator)
                || resultOperatorType == typeof(AnyResultOperator));
        }

        /// <summary>
        /// Write up the code for the any or the all
        /// </summary>
        /// <param name="resultOperator"></param>
        /// <param name="queryModel"></param>
        /// <param name="gc"></param>
        /// <param name="cc"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        public Tuple<Expression, IValue> ProcessResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, IGeneratedQueryCode gc, ICodeContext cc, CompositionContainer container)
        {
            if (cc == null)
                throw new ArgumentNullException("_codeContext");

            var all = resultOperator as AllResultOperator;
            var any = resultOperator as AnyResultOperator;

            ///
            /// Next, change the predicate into something that can be tested (as an if statement)
            /// For All:
            ///   initial: val = true;
            ///   if (!pred) { val = false; break;}
            /// For Any:
            ///   initial: val = false;
            ///   if (pred) { val = true; break; }
            /// 

            IValue predicate = null;
            string initialValue = "";
            if (all != null)
            {
                var notPredicate = Expression.Not(all.Predicate);
                predicate = ExpressionToCPP.GetExpression(notPredicate, gc, cc, container);
                initialValue = "true";
            }
            else
            {
                predicate = new Variables.ValSimple("true", typeof(bool));
                initialValue = "false";
            }

            ///
            /// The result is a simple bool. This is what we will be handing back.
            /// 

            var aresult = Expression.Variable(typeof(bool), typeof(bool).CreateUniqueVariableName());
            var initialValue = new Variables.ValSimple(initialValue, typeof(bool));

            ///
            /// And the statements now
            /// 

            var ifstatement = new Statements.StatementFilter(predicate);
            ifstatement.Add(new Statements.StatementFlipBool(aresult));
            ifstatement.Add(new Statements.StatementBreak());

            gc.Add(ifstatement);

            ///
            /// Done!
            /// 

            return aresult;
        }
    }
}
