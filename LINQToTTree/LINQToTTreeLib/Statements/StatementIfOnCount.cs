using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqToTTreeInterfacesLib;

namespace LINQToTTreeLib.Statements
{
    /// <summary>
    /// A if statement that will fire depending on the relationship between a variable and a value.
    /// Is a complete scoping declaration.
    /// </summary>
    public class StatementIfOnCount : StatementInlineBlock
    {
        /// <summary>
        /// What operation are we going to be performing here?
        /// </summary>
        public enum ComparisonOperator
        {
            GreaterThan, LessThan, EqualTo, GreaterThanEqual, LessThanEqual
        }

        /// <summary>
        /// The left hand side of the comparison
        /// </summary>
        public IValue ValLeft { get; private set; }

        /// <summary>
        /// The right hand side of the comparison
        /// </summary>
        public IValue ValRight { get; private set; }

        /// <summary>
        /// The comparison operator
        /// </summary>
        public ComparisonOperator Comparison { get; private set; }

        /// <summary>
        /// Create with value1 comp value2 - if that is true, then we will execute our
        /// inner statements and declarations.
        /// </summary>
        /// <param name="valueLeft"></param>
        /// <param name="IValue"></param>
        public StatementIfOnCount(IValue valueLeft, IValue valueRight, ComparisonOperator comp)
        {
            ///
            /// Make sure that nothing is crazy here!
            /// 

            if (valueLeft == null)
                throw new ArgumentNullException("Can't have a lefthand value that is null");
            if (valueRight == null)
                throw new ArgumentNullException("Cant have a righthand value that is null!");

            ///
            /// Remember!
            /// 

            ValLeft = valueLeft;
            ValRight = valueRight;
            Comparison = comp;
        }
    }
}
