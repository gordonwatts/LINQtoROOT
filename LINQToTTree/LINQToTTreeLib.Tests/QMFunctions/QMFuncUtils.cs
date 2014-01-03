using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.QMFunctions;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Variables;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using System.Linq.Expressions;

namespace LINQToTTreeLib.Tests.QMFunctions
{
    public static class QMFuncUtils
    {
        /// <summary>
        /// Generate a function with no arguments that returns an int given that name. The
        /// actual statement is a very simple constant.
        /// </summary>
        /// <param name="fname"></param>
        /// <returns></returns>
        public static QMFuncSource GenerateFunction()
        {
            int[] ints = new int[10];
            var qmb = new QueryModelBuilder();
            qmb.AddClause(new MainFromClause("i", typeof(int), Expression.Constant(ints)));
            qmb.AddClause(new SelectClause(Expression.Constant(1)));
            qmb.AddResultOperator(new Remotion.Linq.Clauses.ResultOperators.CountResultOperator());

            var h = new QMFuncHeader() { Arguments = new object[] { }, QM = qmb.Build() };
            h.QMText = h.QM.ToString();
            var f = new QMFuncSource(h);

            var p = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var st = new StatementAssign(p, new ValSimple("5", typeof(int)), new IDeclaredParameter[] { });
            var inlineblock = new StatementInlineBlock();
            inlineblock.Add(st);
            inlineblock.Add(new StatementReturn(p));
            f.SetCodeBody(inlineblock);

            return f;
        }

    }
}
