using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Variables;

namespace LINQToTTreeLib.Expressions
{
    /// <summary>
    /// Holds the smarts to deal with an array info that is represented as vector in the C++ code.
    /// </summary>
    internal class ArrayInfoVector : IArrayInfo
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
        public System.Linq.Expressions.Expression AddLoop(IGeneratedCode env, ICodeContext context, CompositionContainer container)
        {
            ///
            /// First, we will need to know the length of this array
            /// 

            var lenExpression = Expression.ArrayLength(_arrayExpression);
            var lenTranslation = ExpressionVisitor.GetExpression(lenExpression, env, context, container);

            ///
            /// Next, generate the expression that forms the basis of the index lookup. We don't
            /// translate this - that only gets to happen when one is actually looking at a final result.
            /// 

            var loopVariable = Expression.Variable(typeof(int), typeof(int).CreateUniqueVariableName());
            var indexExpression = Expression.MakeBinary(ExpressionType.ArrayIndex, _arrayExpression, loopVariable);

            ///
            /// Ok, now we are ready to actually generate some code here! First, cache the size
            /// 

            var arraySizeVar = new VarSimple(typeof(int)) { Declare = true };
            env.Add(arraySizeVar);
            env.Add(new StatementAssign(arraySizeVar, lenTranslation));

            ///
            /// Now the for loop statement!
            /// 

            env.Add(new StatementVectorLoop(loopVariable, arraySizeVar));

            ///
            /// Return the index expression - the thing that can be used to replace all expressions and
            /// reference the item we are looping over.
            /// 

            return indexExpression;
        }

        /// <summary>
        /// A local class to implement the looping statements to work over this array.
        /// </summary>
        private class StatementVectorLoop : StatementInlineBlock
        {
            /// <summary>
            /// The loop that we will run over
            /// </summary>
            private string _forLoop;

            /// <summary>
            /// Create a for loop statement.
            /// </summary>
            /// <param name="loopVariable"></param>
            /// <param name="arraySizeVar"></param>
            public StatementVectorLoop(ParameterExpression loopVariable, VarSimple arraySizeVar)
            {
                var bld = new StringBuilder();
                bld.AppendFormat("for (int {0}=0; {0} < {1}; {0}++)", loopVariable.Name, arraySizeVar.RawValue);
                _forLoop = bld.ToString();
            }

            /// <summary>
            /// Generate the code to do the looping. No need to generate anything if there is nothing to do! :-)
            /// </summary>
            /// <returns></returns>
            public override System.Collections.Generic.IEnumerable<string> CodeItUp()
            {
                if (Statements.Any())
                {
                    yield return _forLoop;
                    foreach (var l in base.CodeItUp())
                    {
                        yield return l;
                    }
                }
            }
        }
    }
}
