﻿using System.Linq;
using System.Linq.Expressions;
using Remotion.Linq;
using Remotion.Linq.Parsing.Structure;

namespace LINQToTTreeLib.Tests
{
    /// <summary>
    /// The queriable object we will use to drive this testing!
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class QueriableDummy<T> : QueryableBase<T>
    {
        /// <summary>
        /// Get ourselves setup!
        /// </summary>
        public QueriableDummy()
            : base(QueryParser.CreateDefault(), new DummyQueryExectuor())
        {
        }

        public QueriableDummy(IQueryProvider provider, Expression expr)
            : base(provider, expr)
        {
        }
    }
}
