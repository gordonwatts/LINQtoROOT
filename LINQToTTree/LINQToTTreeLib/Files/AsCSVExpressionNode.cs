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
    /// Convert the method into an QueryModel information block.
    /// </summary>
    class AsCSVExpressionNode : ResultOperatorExpressionNodeBase
    {
        /// <summary>
        /// What extensions can we handle in this (amazingly) complex code?
        /// </summary>
        public static MethodInfo[] SupportedMethods =
             new[] {
                GetSupportedMethod (() => FileHelperQueryExtensions.AsCSV((IQueryable<double>) null, (FileInfo) null, (string) null)),
             };

        /// <summary>
        /// Hold onto the colunm name
        /// </summary>
        private Expression _columnName;

        /// <summary>
        /// Hold onto the file info.
        /// </summary>
        private Expression _fileInfo;

        /// <summary>
        /// The expression node parser.
        /// </summary>
        /// <param name="parseInfo"></param>
        public AsCSVExpressionNode(MethodCallExpressionParseInfo parseInfo, Expression fileInfo, Expression columnName)
          : base(parseInfo, null, null)
        {
            _fileInfo = fileInfo;
            _columnName = columnName;
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
            return new AsCSVResultOperator((_fileInfo as ConstantExpression).Value as FileInfo, (_columnName as ConstantExpression).Value as string);

        }
    }
}
