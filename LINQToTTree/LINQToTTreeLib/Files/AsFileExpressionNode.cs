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
using LINQToTTreeLib.Utils;

namespace LINQToTTreeLib.Files
{
    /// <summary>
    /// Base class for dealing with expressions that result in some sort of file
    /// with columns being written out.
    /// </summary>
    abstract class AsFileExpressionNode : ResultOperatorExpressionNodeBase
    {
        /// <summary>
        /// Hold onto the column name
        /// </summary>
        protected string[] _columnNames;

        /// <summary>
        /// Filename pattern we should base the output file on.
        /// </summary>
        protected Expression _fileInfo;

        private static int _counter = 0;

        /// <summary>
        /// Traverse the leaves of a type. Blow up if we can't figure out how to traverse it.
        /// </summary>
        /// <param name="outputType"></param>
        /// <param name="visitor"></param>
        protected void TraverseColumnsForOutput(Type outputType, Action<string> visitor, string prefix = null)
        {
            var namingPrefix = string.IsNullOrWhiteSpace(prefix) ? "" : $"{prefix}.";

            if (outputType.TypeIsEasilyDumped())
            {
                // Simple leaf node.
                if (string.IsNullOrWhiteSpace(prefix))
                {
                    visitor($"{outputType.Name}_{_counter}");
                    _counter++;
                } else
                {
                    visitor(prefix);
                }
            }
            else if (outputType.Name.StartsWith("Tuple"))
            {
                // Tuple - Loop through all its internal bits
                var genericArgs = outputType.GetGenericArguments();
                foreach (var pIndex in genericArgs.Zip(Enumerable.Range(1, genericArgs.Length), (a, c) => Tuple.Create(a, c)))
                {
                    TraverseColumnsForOutput(pIndex.Item1, visitor, $"{namingPrefix}Item{pIndex.Item2}");
                }
            }
            else
            {
                // Get a list of all field and property names, and go down one level.
                var allNames = outputType.GetFieldsInDeclOrder().Select(f => Tuple.Create(f.FieldType, f.Name))
                    .Concat(outputType.GetProperties().Select(p => Tuple.Create(p.PropertyType, p.Name)));

                foreach (var f in allNames)
                {
                    TraverseColumnsForOutput(f.Item1, visitor, $"{namingPrefix}{f.Item2}");
                }
            }

        }

        /// <summary>
        /// The expression node parser.
        /// </summary>
        /// <param name="parseInfo"></param>
        public AsFileExpressionNode(MethodCallExpressionParseInfo parseInfo, Expression fileInfo, Expression columnNames)
          : base(parseInfo, null, null)
        {
            _fileInfo = fileInfo;

            // Next, figure out what it is we are dumping. To do this, we look at the IQueriable
            // generic argument, and then depending on what we find, we extract type and column
            // information.
            var objectTypeToDump = parseInfo.ParsedExpression.Arguments[0].Type.GetGenericArguments()[0];
            var defaultColumnNames = new List<string>();
            TraverseColumnsForOutput(objectTypeToDump, n => defaultColumnNames.Add(n));

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
        /// Resolve anything in the expression.
        /// </summary>
        /// <param name="inputParameter"></param>
        /// <param name="expressionToBeResolved"></param>
        /// <param name="clauseGenerationContext"></param>
        /// <returns></returns>
        public override Expression Resolve(ParameterExpression inputParameter, Expression expressionToBeResolved, ClauseGenerationContext clauseGenerationContext)
        {
            throw new NotImplementedException();
        }
    }
}
