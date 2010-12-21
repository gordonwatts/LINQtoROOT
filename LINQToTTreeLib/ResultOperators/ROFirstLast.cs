using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqToTTreeInterfacesLib;
using System.ComponentModel.Composition;
using Remotion.Data.Linq.Clauses.ResultOperators;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq;

namespace LINQToTTreeLib.ResultOperators
{
    /// <summary>
    /// The user wants either the first or the last item in this list. Also, if we have to have a value, then throw if we don't!
    /// </summary>
    [Export(typeof(IQVResultOperator))]
    public class ROFirstLast : IQVResultOperator
    {
        /// <summary>
        /// We support the first/last guys...
        /// </summary>
        /// <param name="resultOperatorType"></param>
        /// <returns></returns>
        public bool CanHandle(Type resultOperatorType)
        {
            return resultOperatorType == typeof(FirstResultOperator)
                || resultOperatorType == typeof(LastResultOperator);

        }

        /// <summary>
        /// Process the First/last. This means adding a pointer (or not if we are looking at a plane type) and
        /// then filling it till it is full or filling it till the loop is done. Bomb out if we are asked to at the end!!
        /// </summary>
        /// <param name="resultOperator"></param>
        /// <param name="queryModel"></param>
        /// <param name="_codeEnv"></param>
        /// <returns></returns>
        public IVariable ProcessResultOperator (ResultOperatorBase resultOperator, QueryModel queryModel, IGeneratedCode _codeEnv)
        {
            ///
            /// First, do data normalization
            /// 

            var asFirst = resultOperator as FirstResultOperator;
            var asLast = resultOperator as LastResultOperator;

            if (asFirst == null && asLast == null)
            {
                throw new ArgumentNullException("First/Last operator must be either first or last, and not null!");
            }

            bool isFirst = asFirst != null;
            bool bombIfNothing = true;
            if (isFirst)
            {
                bombIfNothing = !asFirst.ReturnDefaultWhenEmpty;
            }
            else
            {
                bombIfNothing = !asLast.ReturnDefaultWhenEmpty;
            }

            ///
            /// Now, create the upper level pointer (or whatever), along with a "bool" that marks this thing as being
            /// valid.
            /// 

            return null;
        }
    }
}
