using LinqToTTreeInterfacesLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LINQToTTreeLib.Expressions
{
    /// <summary>
    /// An expression that holds onto a value.
    /// </summary>
    public class ValueExpression : Expression
    {
        public ValueExpression(IValue v)
        {
            Value = v;
        }

        /// <summary>
        /// Get the value of the IValue we are holding onto.
        /// </summary>
        public IValue Value { get; private set; }

        /// <summary>
        /// TO make debugging, etc., easier.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Value.RawValue;
        }

        /// <summary>
        /// The expression type for testing to see if it is a declared variable.
        /// </summary>
        public const ExpressionType ExpressionType = (ExpressionType)110003;

        /// <summary>
        /// Return the expression type
        /// </summary>
        public override ExpressionType NodeType { get { return ExpressionType; } }

        /// <summary>
        /// The type of the thing we are holding.
        /// </summary>
        public override Type Type { get { return Value.Type; } }

        /// <summary>
        /// We don't have to do anything. So let it go.
        /// </summary>
        /// <param name="visitor"></param>
        /// <returns></returns>
        protected override Expression VisitChildren(ExpressionVisitor visitor)
        {
            return this;
        }
    }
}
