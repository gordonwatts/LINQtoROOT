using System;
using System.Collections.Generic;
using LINQToTTreeLib.ResultOperators;
using LINQToTTreeLib.TypeHandlers;
using LINQToTTreeLib.TypeHandlers.ROOT;
using LINQToTTreeLib.Utils;
using Remotion.Data.Linq;

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
            LastQueryModel = queryModel;

            Result = new GeneratedCode();

            if (!GlobalInitalized)
            {
                GlobalInitalized = true;
                MEFUtilities.AddPart(new QVResultOperators());
                MEFUtilities.AddPart(new ROCount());
                MEFUtilities.AddPart(new ROTakeSkipOperators());
                MEFUtilities.AddPart(new ROAggregate());

                MEFUtilities.AddPart(new TypeHandlerROOT());
                MEFUtilities.AddPart(new TypeHandlerHelpers());
                ExpressionVisitor.TypeHandlers = new TypeHandlerCache();
                MEFUtilities.AddPart(ExpressionVisitor.TypeHandlers);
            }

            var qv = new QueryVisitor(Result);
            MEFUtilities.Compose(qv);

            qv.VisitQueryModel(queryModel);

            FinalResult = Result;
            return default(T);
        }

        public T ExecuteSingle<T>(QueryModel queryModel, bool returnDefaultWhenEmpty)
        {
            throw new NotImplementedException();
        }
    }
}
