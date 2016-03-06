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
            var asTTree = resultOperator as AsTTreeResultOperator;
            if (asTTree == null)
                throw new ArgumentException("resultOperaton");

            // Declare the includes.
            gc.AddIncludeFile("<map>");
            gc.AddIncludeFile("TSystem.h");
            gc.AddIncludeFile("TFile.h");
            gc.AddIncludeFile("TTree.h");

            // Declare the TTree and the file we will be using!
            // Initialization is not important as we will over-write this directly.
            var stream = DeclarableParameter.CreateDeclarableParameterExpression(typeof(OutputTTreeFileType));
            stream.InitialValue = new OutputTTreeFileType(asTTree.OutputFile);

            // Generate a real filename. We are going to key the file by the cache key. Unfortunately, at this
            // point in the generation the cache key isn't known. So we have to have a 'promise' that can be used
            // for later when the code is actually generated.
            var outputFile = GenerateUniqueFile(asTTree.OutputFile, cc);

            // Open the file and declare the tree
            gc.AddInitalizationStatement(new StatementSimpleStatement(() => $"{stream.RawValue}.first = new TFile(\"{outputFile().FullName.AddCPPEscapeCharacters()}\",\"RECREATE\")"));
            gc.AddInitalizationStatement(new StatementSimpleStatement($"{stream.RawValue}.second = new TTree(\"mytree\", \"Tree Generated by LINQToTTree\")"));

            // Get the list of item values we are going to need here.
            List<Expression> itemValues = ExtractItemValueExpressions(queryModel);

            // We are just going to print out the line with the item in it.
            var itemAsValues = itemValues.Select(iv => ExpressionToCPP.GetExpression(iv, gc, cc, container));
            var pstatement = new StatementFillTree(stream, itemAsValues.Zip(asTTree.HeaderColumns, (i, h) => Tuple.Create(i, h)).ToArray());

            gc.Add(pstatement);

            // The return is a file path in the C# world. But here in C++, what should be returned?
            // We will use a string.
            return stream;
        }
    }
}
