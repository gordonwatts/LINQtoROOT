using Remotion.Linq.Parsing.Structure.IntermediateModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Remotion.Linq.Clauses;
using System.Linq.Expressions;
using System.Reflection;
using System.IO;

namespace LINQToTTreeLib.Files
{
    /// <summary>
    /// Convert the AsCSV extension method into an QueryModel information block.
    /// </summary>
    class AsCSVExpressionNode : ResultOperatorExpressionNodeBase
    {
        /// <summary>
        /// What extensions can we handle here?
        /// </summary>
        public static MethodInfo[] SupportedMethods =
             new[] {
                GetSupportedMethod (() => FileHelperQueryExtensions.AsCSV((IQueryable<double>) null, (FileInfo) null, (string) null)),
                GetSupportedMethod (() => FileHelperQueryExtensions.AsCSV((IQueryable<Tuple<double, double>>) null, (FileInfo)null, (string) null, (string) null))
             };

        /// <summary>
        /// Hold onto the colunm name
        /// </summary>
        private Expression[] _columnNames;

        /// <summary>
        /// Hold onto the file info.
        /// </summary>
        private Expression _fileInfo;

        /// <summary>
        /// The expression node parser.
        /// </summary>
        /// <param name="parseInfo"></param>
        public AsCSVExpressionNode(MethodCallExpressionParseInfo parseInfo, Expression fileInfo, Expression columnName1, Expression columnName2, Expression columnName3, Expression columnName4)
          : base(parseInfo, null, null)
        {
            _fileInfo = fileInfo;
            List<Expression> names = new List<Expression>();
            if (columnName1 != null)
                names.Add(columnName1);
            if (columnName2 != null)
                names.Add(columnName2);
            if (columnName3 != null)
                names.Add(columnName3);
            if (columnName4 != null)
                names.Add(columnName4);

            _columnNames = names.ToArray();
        }

        public override Expression Resolve(ParameterExpression inputParameter, Expression expressionToBeResolved, ClauseGenerationContext clauseGenerationContext)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Create the actual re-linq resolution operator.
        /// </summary>
        /// <param name="clauseGenerationContext"></param>
        /// <returns></returns>
        protected override ResultOperatorBase CreateResultOperator(ClauseGenerationContext clauseGenerationContext)
        {
#if false
            // Don't think this is needed as there is no way for a query expression to come in for everything else.
            var resolvedParameter = Source.Resolve(
                 _parameterLambda.Parameters[0],
                 _parameterLambda.Body,
                 clauseGenerationContext);
#endif
            return new AsCSVResultOperator((_fileInfo as ConstantExpression).Value as FileInfo, _columnNames.Select(cn => (cn as ConstantExpression).Value as string).ToArray());

        }
    }
}
