using System;
using System.Collections.Generic;
using Remotion.Data.Linq;

namespace LINQToTTreeLib.Tests
{
    class DummyQueryExectuor : IQueryExecutor
    {
        public IEnumerable<T> ExecuteCollection<T>(QueryModel queryModel)
        {
            throw new NotImplementedException();
        }
#if false
        /// <summary>
        /// The result of the query
        /// </summary>
        public GeneratedCode Result { get; private set; }

        public static GeneratedCode FinalResult { get; private set; }


        public static QueryModel LastQueryModel { get; private set; }
#endif
        public static bool GlobalInitalized = false;

        /// <summary>
        /// The only thing that we are going to *even* allow!
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        public T ExecuteScalar<T>(QueryModel queryModel)
        {
#if false
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
                MEFUtilities.AddPart(new TypeHandlerCache());
            }

            var qv = new QueryVisitor(Result, null, MEFUtilities.MEFContainer);
            MEFUtilities.Compose(qv);

            qv.VisitQueryModel(queryModel);

            FinalResult = Result;
#endif
            return default(T);
        }

        public T ExecuteSingle<T>(QueryModel queryModel, bool returnDefaultWhenEmpty)
        {
            throw new NotImplementedException();
        }
    }
}
