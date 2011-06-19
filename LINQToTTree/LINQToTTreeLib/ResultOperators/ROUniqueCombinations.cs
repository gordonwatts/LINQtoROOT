using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.relinq;
using Remotion.Linq;
using Remotion.Linq.Clauses;

namespace LINQToTTreeLib.ResultOperators
{
    /// <summary>
    /// Deal with the UniqueCombinations operator - which takes all possible pairs where j1 != j2, and order doesn't
    /// matter (i.e. if it returns (j1,j2) then it shouldn't return (j2, j1)).
    /// </summary>
    [Export(typeof(IQVCollectionResultOperator))]
    class ROUniqueCombinations : IQVCollectionResultOperator
    {
        /// <summary>
        /// There is, really, only one guy that we deal with! Thankgoodness it is pretty simple! :-)
        /// </summary>
        /// <param name="resultOperatorType"></param>
        /// <returns></returns>
        public bool CanHandle(Type resultOperatorType)
        {
            return resultOperatorType == typeof(UniqueCombinationsResultOperator);
        }

        /// <summary>
        /// Take the incoming stream of items, and send them along! :-)
        /// </summary>
        /// <param name="resultOperator"></param>
        /// <param name="queryModel"></param>
        /// <param name="_codeEnv"></param>
        /// <param name="_codeContext"></param>
        /// <param name="container"></param>
        public void ProcessResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, IGeneratedQueryCode gc, ICodeContext cc, CompositionContainer container)
        {
            //
            // Some basic checks on the input.
            //

            if (cc == null)
                throw new ArgumentNullException("cc");
            if (gc == null)
                throw new ArgumentNullException("gc");

            if (cc.LoopVariable.NodeType != System.Linq.Expressions.ExpressionType.ArrayIndex)
                throw new ArgumentException("Unable to generate unique combinations of a non-array indexed sequence");
        }
    }
}
