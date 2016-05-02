using System;
using System.Collections.Generic;
using System.Linq;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Utils;

namespace LINQToTTreeLib.Statements
{
    /// <summary>
    /// Deal with a "Where"-like clause. Basically, we have an expression which we evaluate, and we make sure that
    /// it goes!
    /// </summary>
    public class StatementFilter : StatementInlineBlockBase, ICMStatementInfo
    {
        /// <summary>
        /// Get the expression we are going to test
        /// </summary>
        public IValue TestExpression { get; private set; }

        /// <summary>
        /// We don't want to bubble up statements normally from here. Protected by an if statement usually means
        /// that we are doing it for a good reason.
        /// </summary>
        public override bool AllowNormalBubbleUp { get { return false; } }

        /// <summary>
        /// testExpression is what we test against to see if we should fire!
        /// </summary>
        /// <param name="testExpression"></param>
        public StatementFilter(IValue testExpression)
        {
            if (testExpression == null)
                throw new ArgumentNullException("testExpression");
            TestExpression = testExpression;
        }

        /// <summary>
        /// Return the code for this if statement.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<string> CodeItUp()
        {
            ///
            /// If there are no held statements, then we can optimize away the
            /// if statement!
            /// 

            if (Statements.Any() && TestExpression.RawValue != "false")
            {

                ///
                /// Now, emit the if statement and work from there
                /// 

                if (TestExpression.RawValue != "true")
                    yield return "if (" + TestExpression.RawValue + ")";
                foreach (var l in RenderInternalCode())
                {
                    yield return l;
                }
            }
        }

        /// <summary>
        /// Return the index variables for this loop.
        /// </summary>
        public override IEnumerable<IDeclaredParameter> InternalResultVarialbes
        {
            get
            {
                return new IDeclaredParameter[] { };
            }
        }

        /// <summary>
        /// We filter on one simple thing. If it is the case that the tests are the same,
        /// (identical), we do the combination, stealing the statements from the second one
        /// for ourselves. No renaming is required as this is a simple test!
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        public override bool TryCombineStatement(IStatement statement, ICodeOptimizationService opt)
        {
            if (statement == null)
                throw new ArgumentException("statement");
            var other = statement as StatementFilter;
            if (other == null)
                return false;

            if (other.TestExpression.RawValue != TestExpression.RawValue)
            {
                return false;
            }

            // The combine is going to go (on some level). Exactly how it goes depends on the context
            // that the statement and this guy sit in. Specifically - if they are buried in different
            // if statements then we can't move non-identical things.
            // Here are the rules:
            // -> If both if's have the same parents, then we can combine them without worry.
            //    This is a little funny because we will often be combining them from different
            //    tree's (though the same). So we have to be a little careful looking backwards.
            //    This combination means that we can do this even if the statements in the body of the if
            //    are different.
            // -> If one is hidden behind another if statement, then we can combine iff:
            //      1) The one we are combining into is not the one hidden
            //      2) The bodies of the if statement are identical
            // -> If there is some other case, then we can't combine at all.
            //
            // Further notes:
            // -> This is not a general optimization. For example, if .Fill is called, that has side effects.
            //    But our code is such that we don't have to worry about that - .Fill is only going to get
            //    called once, and so those statements will never be identical.

            // Understand if the context of the two is the same or not (or how different).
            var itsContext = statement.WalkParents(includeThisStatment: true).Where(s => s is StatementFilter).Cast<StatementFilter>().ToLookup(s => s.TestExpression.RawValue);
            var myContext = this.WalkParents(includeThisStatment: true).Where(s => s is StatementFilter).Cast<StatementFilter>().ToLookup(s => s.TestExpression.RawValue);

            if (itsContext.Count < myContext.Count)
            {
                // It is at a higher level than us, so we can't replicate it.
                return false;
            }

            bool sameContext = false;
            if (itsContext.Count == myContext.Count)
            {
                sameContext = itsContext.Select(c => myContext.Contains(c.Key)).All(p => p);
            }

            // Since the if statements are the same, we can combine the interiors!
            return Combine(other, opt, appendIfCantCombine: sameContext);
        }

        /// <summary>
        /// Make an attempt to combine if statements.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="replaceFirst"></param>
        /// <returns></returns>
        public override Tuple<bool, IEnumerable<Tuple<string, string>>> RequiredForEquivalence(ICMStatementInfo other, IEnumerable<Tuple<string, string>> replaceFirst = null)
        {
            // Quick check.
            if (!(other is StatementFilter))
            {
                return Tuple.Create(false, Enumerable.Empty<Tuple<string, string>>());
            }
            var s2 = other as StatementFilter;

            // Do the test expression.
            var renames = Tuple.Create(true, replaceFirst)
                .RequireAreSame(TestExpression, s2.TestExpression);

            // And do everything in the block
            return RequiredForEquivalenceForBase(other, renames)
                .ExceptFor(replaceFirst);
        }

        /// <summary>
        /// Rename our variables
        /// </summary>
        /// <param name="origName"></param>
        /// <param name="newName"></param>
        public override void RenameVariable(string origName, string newName)
        {
            TestExpression.RenameRawValue(origName, newName);
            RenameBlockVariables(origName, newName);
        }

        /// <summary>
        /// We have a statement that we want to move out of this if statement. Make sure it isn't going to alter
        /// anything we are looking at in our if statement!
        /// </summary>
        /// <param name="followStatement"></param>
        /// <returns></returns>
        public override bool CommutesWithGatingExpressions(ICMStatementInfo followStatement)
        {
            var varsImpacted = followStatement.ResultVariables.Intersect(TestExpression.Dependants.Select(s => s.RawValue));
            return !varsImpacted.Any();
        }

        /// <summary>
        /// Return the list of dependent variables, which includes ones in our test
        /// expression.
        /// </summary>
        public override IEnumerable<string> DependentVariables
        {
            get
            {
                return base.DependentVariables
                    .Concat(TestExpression.Dependants.Select(p => p.RawValue))
                    .ToHashSet();
            }
        }
    }
}
