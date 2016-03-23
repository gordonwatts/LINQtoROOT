using LinqToTTreeInterfacesLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LINQToTTreeLib.Utils
{
    public static class StatementUtils
    {
        /// <summary>
        /// Return the list of parents until we hit null.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static IEnumerable<IStatementCompound> WalkParents(this IStatement s, bool includeThisStatment = false)
        {
            bool skipAStatement = !includeThisStatment;
            while (s != null)
            {
                if (!skipAStatement)
                {
                    if (s is IStatementCompound)
                    yield return s as IStatementCompound;
                }
                skipAStatement = false;

                s = s.Parent;
            }
        }

        /// <summary>
        /// Find a parent that is a booking block. Return null otherwise.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static IBookingStatementBlock FindBookingParent(this IStatement s)
        {
            return s.WalkParents(true)
                .Where(m => m is IBookingStatementBlock)
                .Cast<IBookingStatementBlock>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Run a standard check to see if we can do the proper replacement.
        /// </summary>
        /// <param name="resultVariable1"></param>
        /// <param name="resultVariable2"></param>
        /// <param name="expression1"></param>
        /// <param name="expression2"></param>
        /// <param name="dependents1"></param>
        /// <param name="dependents2"></param>
        /// <param name="replaceFirst"></param>
        /// <returns></returns>
        public static Tuple<bool, IEnumerable<Tuple<string, string>>> MakeEquivalentSimpleExpressionAndResult(
            string resultVariable1, string resultVariable2,
            string expression1, string expression2,
            ISet<string> dependents1, ISet<string> dependents2,
            IEnumerable<Tuple<string, string>> replaceFirst)
        {
            // First, do all replacements
            var otherResultValue = resultVariable2.ReplaceVariableNames(replaceFirst);
            var expr = expression2.ReplaceVariableNames(replaceFirst);

            // Track the renames we need to do.
            var renames = new List<Tuple<string, string>>();

            // Look at the result and see if we there is a simple translation.
            if (resultVariable1 != otherResultValue)
            {
                renames.Add(Tuple.Create(otherResultValue, resultVariable1));
                expr = expr.Replace(otherResultValue, resultVariable1);
            }

            if (expr == expression1)
            {
                return Tuple.Create(true, renames as IEnumerable<Tuple<string, string>>);
            }

            // Now we have to go through the dependent variables. If there are common dependent variables, then we
            // can ignore them. The rest we have to do the translation for.
            var otherDependentVarialbesEnum = dependents2.Replace(renames);
            if (replaceFirst != null)
            {
                otherDependentVarialbesEnum = otherDependentVarialbesEnum.Replace(replaceFirst);
            }
            var otherDependentVarialbes = otherDependentVarialbesEnum.ToArray();
            var dependentUs = dependents1.Except(otherDependentVarialbes).ToArray();
            var dependentThem = otherDependentVarialbes.Except(dependents1).ToArray();

            if (dependentUs.Length != dependentThem.Length)
            {
                return Tuple.Create(false, Enumerable.Empty<Tuple<string, string>>());
            }

            var dependentThemInOrder = dependentThem
                .Where(i => expr.IndexOf(i) >= 0)
                .OrderBy(i => expr.IndexOf(i))
                .ToArray();
            var dependentUsInOrder = dependentUs
                .Where(i => expression1.IndexOf(i) >= 0)
                .OrderBy(i => expression1.IndexOf(i))
                .ToArray();

            foreach (var dependent in dependentThemInOrder.Zip(dependentUsInOrder, (o, t) => Tuple.Create(o, t)))
            {
                var exprNew = expr.Replace(dependent.Item1, dependent.Item2);
                if (exprNew != expr)
                {
                    renames.Add(dependent);
                    if (exprNew == expression1)
                    {
                        return Tuple.Create(true, renames as IEnumerable<Tuple<string, string>>);
                    }
                    expr = exprNew;
                }
            }

            // If we are here, then we have failed!
            return Tuple.Create(false, Enumerable.Empty<Tuple<string, string>>());
        }
    }
}
