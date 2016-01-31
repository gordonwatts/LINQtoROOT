using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.Utils;
using LINQToTTreeLib.Variables;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq.Expressions;

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
            // Argument checking
            var asCSV = resultOperator as AsCSVResultOperator;
            if (asCSV == null)
                throw new ArgumentException("resultOperaton");

            // Open and close the file
            gc.AddIncludeFile("<fstream>");
            gc.AddIncludeFile("<iostream>");

            var stream = DeclarableParameter.CreateDeclarableParameterExpression(typeof(OutputCSVTextFileType));
            stream.InitialValue = new OutputCSVTextFileType(asCSV.OutputFile, asCSV.HeaderColumns);

            // We are just going to print out the line with the item in it.
            var itemValue = ExpressionToCPP.GetExpression(queryModel.SelectClause.Selector, gc, cc, container);
            var pstatement = new StatementCSVDump(stream, itemValue);
            gc.Add(pstatement);

            // The return is a file path in the C# world. But here in C++, what should be returned?
            // We will use a string.
            return stream;
        }
    }
}
