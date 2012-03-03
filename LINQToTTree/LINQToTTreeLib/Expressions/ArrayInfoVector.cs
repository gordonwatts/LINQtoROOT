using System;
using System.ComponentModel.Composition.Hosting;
using System.Linq.Expressions;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Utils;

namespace LINQToTTreeLib.Expressions
{
    /// <summary>
    /// Holds the smarts to deal with an array info that is represented as vector in the C++ code.
    /// </summary>
    public class ArrayInfoVector : IArrayInfo
    {
        /// <summary>
        /// Holds onto the array expression that we are going to be working against.
        /// </summary>
        private Expression _arrayExpression;

        /// <summary>
        /// Create a new array info, using expr as the base for our work.
        /// </summary>
        /// <param name="expr"></param>
        public ArrayInfoVector(Expression expr)
        {
            this._arrayExpression = expr;
        }

        /// <summary>
        /// Generate the code that we will use to access this array. Loop symantics in this framework are basically "foreach" rather than "for" - so
        /// we return an object that can be used to reference each array element.
        /// </summary>
        /// <param name="env"></param>
        /// <param name="context"></param>
        /// <param name="indexName"></param>
        /// <param name="popVariableContext"></param>
        /// <returns></returns>
        public Tuple<Expression, Expression> AddLoop(IGeneratedQueryCode env, ICodeContext context, CompositionContainer container)
        {
            ///
            /// First, we will need to know the length of this array
            /// 

            var lenExpression = Expression.ArrayLength(_arrayExpression);
            var lenTranslation = ExpressionToCPP.GetExpression(lenExpression, env, context, container);

            ///
            /// Next, generate the expression that forms the basis of the index lookup. We don't
            /// translate this - that only gets to happen when one is actually looking at a final result.
            /// 

            var loopVariable = Expression.Variable(typeof(int), typeof(int).CreateUniqueVariableName());
            var indexExpression = Expression.MakeBinary(ExpressionType.ArrayIndex, _arrayExpression, loopVariable);

            ///
            /// Now the for loop statement!
            /// 

            env.Add(new StatementForLoop(loopVariable.Name, lenTranslation));

            ///
            /// Return the index expression - the thing that can be used to replace all expressions and
            /// reference the item we are looping over.
            /// 

            return Tuple.Create<Expression, Expression>(indexExpression, loopVariable);
        }
    }
}
