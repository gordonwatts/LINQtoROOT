﻿using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.QMFunctions;
using LINQToTTreeLib.ResultOperators;
using LINQToTTreeLib.TypeHandlers;
using LINQToTTreeLib.TypeHandlers.CPPCode;
using LINQToTTreeLib.TypeHandlers.ReplacementMethodCalls;
using LINQToTTreeLib.TypeHandlers.ROOT;
using LINQToTTreeLib.TypeHandlers.TranslationTypes;
using LINQToTTreeLib.Utils;
using Remotion.Linq;
using System;
using System.Collections.Generic;

namespace LINQToTTreeLib.Tests
{
    public class DummyQueryExectuor : IQueryExecutor
    {
        public IEnumerable<T> ExecuteCollection<T>(QueryModel queryModel)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Create the dummy query executor.
        /// </summary>
        /// <param name="baseType">The base type we are looping over</param>
        public DummyQueryExectuor(Type baseType, bool doExecution = true)
        {
            _doExecution = doExecution;
            _baseType = baseType;
        }

        /// <summary>
        /// The result of the query
        /// </summary>
        public GeneratedCode Result { get; private set; }

        public static GeneratedCode FinalResult { get; private set; }

        public static bool GlobalInitalized = false;

        public static QueryModel LastQueryModel { get; private set; }

        /// <summary>
        /// The only thing that we are going to *even* allow!
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        public T ExecuteScalar<T>(QueryModel queryModel)
        {
            CommonExecute(queryModel);
            return default(T);
        }

        private void CommonExecute(QueryModel queryModel)
        {
            LastQueryModel = queryModel;

            Result = new GeneratedCode();
            foreach (var f in LINQToTTreeLib.QMFunctions.QMFuncFinder.FindQMFunctions(queryModel))
            {
                Result.Add(new QMFuncSource(f));
            }

            if (!_doExecution)
                return;

            if (!GlobalInitalized)
            {
                GlobalInitalized = true;
                MEFUtilities.AddPart(new QVResultOperators());
                MEFUtilities.AddPart(new ROCount());
                MEFUtilities.AddPart(new ROTakeSkipOperators());
                MEFUtilities.AddPart(new ROAggregate());
                MEFUtilities.AddPart(new ROMinMax());
                MEFUtilities.AddPart(new ROAnyAll());
                MEFUtilities.AddPart(new ROUniqueCombinations());
                MEFUtilities.AddPart(new ROPairWiseAll());
                MEFUtilities.AddPart(new ROAsQueriable());
                MEFUtilities.AddPart(new ROSum());
                MEFUtilities.AddPart(new ROFirstLast());
                MEFUtilities.AddPart(new ROGroup());

                MEFUtilities.AddPart(new TypeHandlerROOT());
                MEFUtilities.AddPart(new TypeHandlerHelpers());
                MEFUtilities.AddPart(new TypeHandlerReplacementCall());
                MEFUtilities.AddPart(new TypeHandlerCache());
                MEFUtilities.AddPart(new TypeHandlerCPPCode());
                MEFUtilities.AddPart(new TypeHandlerTranslationClass());
                MEFUtilities.AddPart(new GroupByArrayFactor());
                MEFUtilities.AddPart(new GroupByFactory());

                MEFUtilities.AddPart(new ArrayArrayInfoFactory());
                MEFUtilities.AddPart(new EnumerableRangeArrayTypeFactory());
                MEFUtilities.AddPart(new SubQueryArrayTypeFactory());
                MEFUtilities.AddPart(new TranslatedArrayInfoFactory());
                MEFUtilities.AddPart(new HandleGroupType());
                MEFUtilities.AddPart(new SubQueryExpressionArrayInfoFactory());
            }

            var qv = new QueryVisitor(Result, new CodeContext() { BaseNtupleObjectType = _baseType }, MEFUtilities.MEFContainer);
            MEFUtilities.Compose(qv);

            MEFInitialPartCount = MEFUtilities.CountParts();

            qv.VisitQueryModel(queryModel);

            FinalResult = Result;
        }

        public static int MEFInitialPartCount { get; private set; }

        /// <summary>
        /// Allow a single guy to run as well.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryModel"></param>
        /// <param name="returnDefaultWhenEmpty"></param>
        /// <returns></returns>
        public T ExecuteSingle<T>(QueryModel queryModel, bool returnDefaultWhenEmpty)
        {
            //CommonExecute(queryModel);
            //return default(T);
            throw new NotImplementedException("You can't use this query '{0}' as it returns a Single element rather than a scalar. Use a different result operator (like Count, for example)");
        }

        /// <summary>
        /// The type of the master ntuple object.
        /// </summary>
        private Type _baseType;
        private bool _doExecution;
    }
}
