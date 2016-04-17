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
                    if (!TypeIsEasilyDumped(genericArgs[pIndex - 1]))
                    {
                        throw new ArgumentException($"Unable to serialize type {genericArgs[pIndex - 1].Name} in Tuple");
                    }
                    defaultColumnNames.Add($"col{pIndex}");
                }
            }
            else
            {
                // Get a list of all field and property names
                var allNames = objectTypeToDump.GetFieldsInDeclOrder().Select(f => Tuple.Create(f.FieldType, f.Name))
                    .Concat(objectTypeToDump.GetProperties().Select(p => Tuple.Create(p.PropertyType, p.Name)));

                foreach (var f in allNames)
                {
                    if (!TypeIsEasilyDumped(f.Item1))
                    {
                        throw new ArgumentException($"Unable to serialize type {f.Item1.Name} in {objectTypeToDump.Name}");
                    }
                    defaultColumnNames.Add(f.Item2);
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
        protected virtual bool TypeIsEasilyDumped(Type objectToDump)
        {
            return objectToDump == typeof(int)
                || objectToDump == typeof(double);
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
