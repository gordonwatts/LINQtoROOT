using System;
using System.Collections.Generic;
using LINQToTTreeLib.ResultOperators;
using LINQToTTreeLib.TypeHandlers;
using LINQToTTreeLib.TypeHandlers.CPPCode;
using LINQToTTreeLib.TypeHandlers.ROOT;
using LINQToTTreeLib.TypeHandlers.TranslationTypes;
using LINQToTTreeLib.Utils;
using Remotion.Linq;

namespace LINQToTTreeLib.Tests
{
    class DummyQueryExectuor : IQueryExecutor
    {
        public IEnumerable<T> ExecuteCollection<T>(QueryModel queryModel)
        {
            throw new NotImplementedException();
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

            if (!GlobalInitalized)
            {
                GlobalInitalized = true;
                MEFUtilities.AddPart(new QVResultOperators());
                MEFUtilities.AddPart(new ROCount());
                MEFUtilities.AddPart(new ROTakeSkipOperators());
                MEFUtilities.AddPart(new ROAggregate());
                MEFUtilities.AddPart(new ROMinMax());

                MEFUtilities.AddPart(new TypeHandlerROOT());
                MEFUtilities.AddPart(new TypeHandlerHelpers());
                MEFUtilities.AddPart(new TypeHandlerCache());
                MEFUtilities.AddPart(new TypeHandlerCPPCode());
                MEFUtilities.AddPart(new TypeHandlerTranslationClass());
            }

            var qv = new QueryVisitor(Result, null, MEFUtilities.MEFContainer);
            MEFUtilities.Compose(qv);

            qv.VisitQueryModel(queryModel);

            FinalResult = Result;
        }

        /// <summary>
        /// Allow a single guy to run as well.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryModel"></param>
        /// <param name="returnDefaultWhenEmpty"></param>
        /// <returns></returns>
        public T ExecuteSingle<T>(QueryModel queryModel, bool returnDefaultWhenEmpty)
        {
            CommonExecute(queryModel);
            return default(T);
        }
    }
}
