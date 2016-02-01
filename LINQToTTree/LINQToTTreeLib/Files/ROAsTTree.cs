﻿using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Utils;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace LINQToTTreeLib.Files
{
    /// <summary>
    /// Part of our LINQToTree-relinq infrastructure - take the result operator and generate code for it.
    /// </summary>
    [Export(typeof(IQVScalarResultOperator))]
    class ROAsTTree : ROAsFile
    {
        /// <summary>
        /// We deal with the AsCSV result operator.
        /// </summary>
        /// <param name="resultOperatorType"></param>
        /// <returns></returns>
        public override bool CanHandle(Type resultOperatorType)
        {
            return resultOperatorType == typeof(AsTTreeResultOperator);
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
        public override Expression ProcessResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, IGeneratedQueryCode gc, ICodeContext cc, CompositionContainer container)
        {
            // Argument checking
            var asCSV = resultOperator as AsFileResultOperator;
            if (asCSV == null)
                throw new ArgumentException("resultOperaton");

            // Declare the includes.
            gc.AddIncludeFile("<fstream>");
            gc.AddIncludeFile("<iostream>");

            // Declare the TTree and the file we will be using!
            var stream = DeclarableParameter.CreateDeclarableParameterExpression(typeof(OutputCSVTextFileType));
            stream.InitialValue = new OutputCSVTextFileType(asCSV.OutputFile);

            // Open the file and declare the tree
            gc.AddInitalizationStatement(new StatementSimpleStatement($"{stream.RawValue}.first = new TFile(\"{asCSV.OutputFile.FullName.AddCPPEscapeCharacters()}\",\"RECREATE\")"));
            gc.AddInitalizationStatement(new StatementSimpleStatement($"{stream.RawValue}.second = new TTree(\"mytree\")"));

            // Get the list of item values we are going to need here.
            List<Expression> itemValues = ExtractItemValueExpressions(queryModel);

            // We are just going to print out the line with the item in it.
            var itemAsValues = itemValues.Select(iv => ExpressionToCPP.GetExpression(iv, gc, cc, container));
            var pstatement = new StatementSimpleStatement($"{stream.RawValue}.second->Fill()");
            gc.Add(pstatement);

            // The return is a file path in the C# world. But here in C++, what should be returned?
            // We will use a string.
            return stream;
        }
    }
}
