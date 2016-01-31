using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.Utils;
using LINQToTTreeLib.Variables;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq.Expressions;
using System.Text;
using System.Linq;

namespace LINQToTTreeLib.Files
{
    /// <summary>
    /// Part of our LINQToTree-relinq infrastructure - take the result operator and generate code for it.
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
        /// We want to print the results out to a file.
        /// </summary>
        /// <param name="resultOperator"></param>
        /// <param name="queryModel"></param>
        /// <param name="_codeEnv"></param>
        /// <param name="_codeContext"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        /// <remarks>
        /// We can handle several types of streams here:
        /// 1) a stream of double's - this is just one column.
        /// 2) A stream of Tuples
        /// 3) A stream of custom objects
        /// </remarks>
        public Expression ProcessResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, IGeneratedQueryCode gc, ICodeContext cc, CompositionContainer container)
        {
            // Argument checking
            var asCSV = resultOperator as AsCSVResultOperator;
            if (asCSV == null)
                throw new ArgumentException("resultOperaton");

            // Declare the file variable.
            gc.AddIncludeFile("<fstream>");
            gc.AddIncludeFile("<iostream>");

            var stream = DeclarableParameter.CreateDeclarableParameterExpression(typeof(OutputCSVTextFileType));
            stream.InitialValue = new OutputCSVTextFileType(asCSV.OutputFile);

            var headerline = new StringBuilder();
            bool first = true;
            foreach (var h in asCSV.HeaderColumns)
            {
                if (!first)
                {
                    headerline.Append(", ");
                }
                headerline.Append(h);
                first = false;
            }
            gc.AddInitalizationStatement(new Statements.StatementSimpleStatement($"{stream.RawValue} << \"{headerline.ToString()}\" << std::endl;"));

            // Printing out the item values really depends on what we are looking at (single stream, tuple, etc.).
            var streamType = queryModel.SelectClause.Selector.Type;
            var streamSelector = queryModel.SelectClause.Selector;

            var itemValues = new List<Expression>();
            if (streamType == typeof(double))
            {
                // A single stream of doubles
                itemValues.Add(queryModel.SelectClause.Selector);

            } else if (streamType.Name.StartsWith("Tuple"))
            {
                var targs = streamType.GenericTypeArguments.Zip(Enumerable.Range(1, 100), (t, c) => Tuple.Create(t, c));
                foreach (var templateType in targs)
                {
                    itemValues.Add(Expression.PropertyOrField(streamSelector, $"Item{templateType.Item2}"));
                }
            } else if (streamType.GetFields().Length > 0)
            {
                foreach (var fName in streamType.GetFields().Select(f => f.Name))
                {
                    itemValues.Add(Expression.PropertyOrField(streamSelector, fName));
                }
            } else
            {
                throw new InvalidOperationException($"Do not know how to generate CSV file from a sequence of {streamType.Name} objects!");
            }

            // We are just going to print out the line with the item in it.
            var itemAsValues = itemValues.Select(iv => ExpressionToCPP.GetExpression(iv, gc, cc, container));
            var pstatement = new StatementCSVDump(stream, itemAsValues.ToArray());
            gc.Add(pstatement);

            // The return is a file path in the C# world. But here in C++, what should be returned?
            // We will use a string.
            return stream;
        }
    }
}
