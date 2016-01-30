using LinqToTTreeInterfacesLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using System.ComponentModel.Composition.Hosting;
using System.Linq.Expressions;
using System.ComponentModel.Composition;
using LINQToTTreeLib.Variables;
using System.IO;
using LINQToTTreeLib.Expressions;

namespace LINQToTTreeLib.Files
{
    /// <summary>
    /// Basics to implement the AsCSV result operator.
    /// </summary>
    [Export(typeof(IQVScalarResultOperator))]
    class ROAsCSV : IQVScalarResultOperator
    {
        /// <summary>
        /// We deal with the AsCSV result operator.
        /// </summary>
        /// <param name="resultOperatorType"></param>
        /// <returns></returns>
        public bool CanHandle(Type resultOperatorType)
        {
            return resultOperatorType == typeof(AsCSVResultOperator);
        }

        public Tuple<bool, Expression> ProcessIdentityQuery(ResultOperatorBase resultOperator, QueryModel queryModel, IGeneratedQueryCode _codeEnv, ICodeContext _codeContext, CompositionContainer container)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// We want to save the result out to a file.
        /// </summary>
        /// <param name="resultOperator"></param>
        /// <param name="queryModel"></param>
        /// <param name="_codeEnv"></param>
        /// <param name="_codeContext"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        public Expression ProcessResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, IGeneratedQueryCode gc, ICodeContext cc, CompositionContainer container)
        {
            // We are just going to print out the line with the item in it.
            var itemValue = ExpressionToCPP.GetExpression(queryModel.SelectClause.Selector, gc, cc, container);
            var pstatement = new StatementCSVDump(new ValSimple("cout", typeof(Stream)), itemValue);
            gc.Add(pstatement);

            return Expression.Constant(1);
        }
    }
}
