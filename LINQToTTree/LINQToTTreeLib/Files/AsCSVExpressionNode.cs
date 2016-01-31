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
                GetSupportedMethod (() => FileHelperQueryExtensions.AsCSV((IQueryable<object>) null, (FileInfo) null, (string[]) null)),
             };

        /// <summary>
        /// Hold onto the colunm name
        /// </summary>
        private string[] _columnNames;

        /// <summary>
        /// Hold onto the file info.
        /// </summary>
        private Expression _fileInfo;

        /// <summary>
        /// The expression node parser.
        /// </summary>
        /// <param name="parseInfo"></param>
        public AsCSVExpressionNode(MethodCallExpressionParseInfo parseInfo, Expression fileInfo, Expression columnNames)
          : base(parseInfo, null, null)
        {
            _fileInfo = fileInfo;

            // If this is a custom object, then we may be getting the column titles from there. If not,
            // then pull from the list that was given to us as part of this ctor.
            var selectExpr = (parseInfo.Source as SelectExpressionNode);
            if (selectExpr == null)
                throw new ArgumentException($"Unable to deal with AsCsv when not part of a Select statement - it showed up as a {parseInfo.Source.GetType().Name}");

            // Next, figure out how many columns we have, and the
            // default names (if the user hasn't given us any) by looking at the generic argument
            // to IQuerable.
            var objectTypeToDump = selectExpr.NodeResultType.GetGenericArguments()[0];
            var defaultColumnNames = new List<string>();
            if (TypeIsEasilyDumped(objectTypeToDump))
            {
                defaultColumnNames.Add("col1");
            }
            else if (objectTypeToDump.Name.StartsWith("Tuple"))
            {
                // Tuple - so just name it something random.
                var genericArgs = objectTypeToDump.GetGenericArguments();
                foreach (var pIndex in Enumerable.Range(1, genericArgs.Length))
                {
                    if (!TypeIsEasilyDumped(genericArgs[pIndex-1]))
                    {
                        throw new ArgumentException($"Unable to serialize type {genericArgs[pIndex - 1].Name} in Tuple");
                    }
                    defaultColumnNames.Add($"col{pIndex}");
                }
            }
            else
            {
                foreach (var f in objectTypeToDump.GetFields())
                {
                    if (!TypeIsEasilyDumped(f.FieldType))
                    {
                        throw new ArgumentException($"Unable to serialize type {f.FieldType.Name} in {objectTypeToDump.Name}");
                    }
                    defaultColumnNames.Add(f.Name);
                }
            }

            // Next, look at the columns that were given to us. Make sure there aren't too many.
            var finalColNames = new List<string>();
            if (columnNames != null)
            {
                var givenNames = (columnNames as ConstantExpression).Value as string[];
                if (givenNames != null)
                {
                    if (givenNames.Length > defaultColumnNames.Count)
                    {
                        throw new ArgumentException("More column headers were given than are present in the data!");
                    }
                    finalColNames.AddRange(givenNames);
                }
            }
            finalColNames.AddRange(defaultColumnNames.Skip(finalColNames.Count));
            _columnNames = finalColNames.ToArray();
        }

        /// <summary>
        /// Is this something that is easy to dump? Only int and long fit this for now.
        /// </summary>
        /// <param name="objectToDump"></param>
        /// <returns></returns>
        private bool TypeIsEasilyDumped(Type objectToDump)
        {
            return objectToDump == typeof(int)
                || objectToDump == typeof(double);
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
            return new AsCSVResultOperator((_fileInfo as ConstantExpression).Value as FileInfo, _columnNames);

        }
    }
}
