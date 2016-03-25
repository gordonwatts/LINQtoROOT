using System;
using System.Collections.Generic;
using System.Linq;
using LinqToTTreeInterfacesLib;

namespace LINQToTTreeLib.Statements
{
    /// <summary>
    /// A if statement that will fire depending on the relationship between a variable and a value.
    /// Is a complete scoping declaration.
    /// </summary>
    public class StatementIfOnCount : StatementInlineBlockBase
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
        public IDeclaredParameter Counter { get; private set; }

        /// <summary>
        /// The right hand side of the comparison
        /// </summary>
        public IValue Limit { get; private set; }

        /// <summary>
        /// The comparison operator
        /// </summary>
        public ComparisonOperator Comparison { get; private set; }

        /// <summary>
        /// Don't let statements just buble up on their own as they are protected by an if statement.
        /// </summary>
        public override bool AllowNormalBubbleUp { get { return false; } }

        /// <summary>
        /// Create with value1 comp value2 - if that is true, then we will execute our
        /// inner statements and declarations.
        /// </summary>
        /// <param name="counter"></param>
        /// <param name="IValue"></param>
        public StatementIfOnCount(IDeclaredParameter counter, IValue limit, ComparisonOperator comp)
        {
            ///
            /// Make sure that nothing is crazy here!
            /// 

            if (counter == null)
                throw new ArgumentNullException("Can't have a lefthand value that is null");
            if (limit == null)
                throw new ArgumentNullException("Cant have a righthand value that is null!");

            ///
            /// Remember!
            /// 

            Counter = counter;
            Limit = limit;
            Comparison = comp;
        }

        /// <summary>
        /// Translate from the operation into the operation
        /// </summary>
        private Dictionary<ComparisonOperator, string> ComparisonCodeTranslation = new Dictionary<ComparisonOperator, string>()
        { 
            {ComparisonOperator.EqualTo, "=="},
            {ComparisonOperator.GreaterThan, ">"},
            {ComparisonOperator.GreaterThanEqual, ">="},
            {ComparisonOperator.LessThan, "<"},
            {ComparisonOperator.LessThanEqual, "<="}
        };

        /// <summary>
        /// Emit the code for this test statement
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<string> CodeItUp()
        {
            ///
            /// If no statements, then we can optimize away!
            /// 

            if (Statements.Any())
            {
                yield return string.Format("{0}++;", Counter.RawValue);
                yield return "if (" + Counter.RawValue + " " + ComparisonCodeTranslation[Comparison] + " " + Limit.RawValue + ")";
                foreach (var l in RenderInternalCode())
                {
                    yield return l;
                }
            }
        }

        /// <summary>
        /// We don't have the code to do the combination yet, so we have to bail!
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        public override bool TryCombineStatement(IStatement statement, ICodeOptimizationService opt)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");

            if (opt == null)
                throw new ArgumentNullException("opt");

            var other = statement as StatementIfOnCount;
            if (other == null)
                return false;

            var issame = Comparison == other.Comparison
                && Limit.RawValue == other.Limit.RawValue;

            if (!issame)
                return false;

            //
            // Now we have to rename the right, just in case...
            //

            if (!opt.TryRenameVarialbeOneLevelUp(other.Counter.RawValue, Counter))
                return false;

            Combine(other, opt);

            return true;
        }

        /// <summary>
        /// Rename everything
        /// </summary>
        /// <param name="origName"></param>
        /// <param name="newName"></param>
        public override void RenameVariable(string origName, string newName)
        {
            Counter.RenameRawValue(origName, newName);
            Limit.RenameRawValue(origName, newName);
            RenameBlockVariables(origName, newName);
        }

        /// <summary>
        /// If the statement doesn't alter anything in the block, then no problem.
        /// </summary>
        /// <param name="followStatement"></param>
        /// <returns></returns>
        public override bool CommutesWithGatingExpressions(ICMStatementInfo followStatement)
        {
            var varsAffected = followStatement.ResultVariables.Intersect(Limit.Dependants.Select(s => s.RawValue));
            return !varsAffected.Any();
        }
    }
}
