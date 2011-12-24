using System;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
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

            env.Add(new StatementVectorLoop(loopVariable.Name, lenTranslation));

            ///
            /// Return the index expression - the thing that can be used to replace all expressions and
            /// reference the item we are looping over.
            /// 

            return Tuple.Create<Expression, Expression>(indexExpression, loopVariable);
        }

        /// <summary>
        /// A local class to implement the looping statements to work over this array.
        /// </summary>
        public class StatementVectorLoop : StatementInlineBlockBase, IStatementLoop
        {
            public IValue ArrayLength { get; set; }
            string _loopVariable;

            /// <summary>
            /// Create a for loop statement.
            /// </summary>
            /// <param name="loopVariable"></param>
            /// <param name="arraySizeVar"></param>
            public StatementVectorLoop(string loopVariable, IValue arraySizeVar)
            {
                ArrayLength = arraySizeVar;
                _loopVariable = loopVariable;
            }

            /// <summary>
            /// Generate the code to do the looping. No need to generate anything if there is nothing to do! :-)
            /// </summary>
            /// <returns></returns>
            public override System.Collections.Generic.IEnumerable<string> CodeItUp()
            {
                if (Statements.Any())
                {
                    var arrIndex = typeof(int).CreateUniqueVariableName();
                    yield return string.Format("int {0} = {1};", arrIndex, ArrayLength.RawValue);
                    yield return string.Format("for (int {0}=0; {0} < {1}; {0}++)", _loopVariable, arrIndex);
                    foreach (var l in RenderInternalCode())
                    {
                        yield return l;
                    }
                }
            }

            /// <summary>
            /// We need to try to combine statements here.
            /// </summary>
            /// <param name="statement"></param>
            /// <returns></returns>
            public override bool TryCombineStatement(IStatement statement, ICodeOptimizationService opt)
            {
                if (statement == null)
                    throw new ArgumentNullException("statement");

                var other = statement as StatementVectorLoop;
                if (other == null)
                    return false;

                // If we are looping over the same thing, then we can combine.

                if (other.ArrayLength.RawValue != ArrayLength.RawValue)
                    return false;

                // We need to rename the loop variable in the second guy

                other.RenameVariable(other._loopVariable, _loopVariable);

                // Combine everything

                Combine(other, opt);

                return true;
            }

            /// <summary>
            /// Rename all the variables in this block
            /// </summary>
            /// <param name="origName"></param>
            /// <param name="newName"></param>
            public override void RenameVariable(string origName, string newName)
            {
                ArrayLength.RenameRawValue(origName, newName);
                _loopVariable = _loopVariable.ReplaceVariableNames(origName, newName);
                RenameBlockVariables(origName, newName);
            }
        }
    }
}
