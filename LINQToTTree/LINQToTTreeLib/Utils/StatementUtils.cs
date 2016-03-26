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
                    {
                        yield return s as IStatementCompound;
                    }
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
        /// Find a parent that is a booking block. Return null otherwise.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static IStatementCompound FindCompoundParent(this IStatement s)
        {
            return s.WalkParents(false)
                .FirstOrDefault();
        }

        /// <summary>
        /// Return the parents of this statement.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="stopBeforeBookingParent"></param>
        /// <returns></returns>
        public static IEnumerable<IStatement> Parents(this IStatement s, bool stopBeforeBookingParent = true)
        {
            if (stopBeforeBookingParent)
            {
                return s.WalkParents(false)
                    .TakeWhile(st => !(st is IBookingStatementBlock));
            } else
            {
                return s.WalkParents(false);
            }
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
            var r = Tuple.Create(true, replaceFirst)
                .RequireForEquivForExpression(resultVariable1, resultVariable2)
                .RequireForEquivForExpression(expression1, dependents1, expression2, dependents2)
                .ExceptFor(replaceFirst);

            return r;
        }

        /// <summary>
        /// Do the rename when we have two variables. This is a special case, and will always succeed.
        /// </summary>
        /// <param name="renames"></param>
        /// <param name="var1"></param>
        /// <param name="var2"></param>
        /// <returns></returns>
        public static Tuple<bool, IEnumerable<Tuple<string, string>>> RequireForEquivForExpression(this Tuple<bool, IEnumerable<Tuple<string, string>>> renames,
            string var1,
            string var2)
        {
            // Monad option - quick check.
            if (!renames.Item1)
                return renames;

            var v2 = var2.ReplaceVariableNames(renames.Item2);
            if (var1 == v2)
            {
                return renames.CheckForListNull();
            }

            return Tuple.Create(true, renames.CheckForListNull().Item2.Concat(new Tuple<string, string>[] { new Tuple<string, string>(var2, var1) }));
        }

        /// <summary>
        /// Given two IValue's, do the expression conversion.
        /// </summary>
        /// <param name="renames"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static Tuple<bool, IEnumerable<Tuple<string,string>>> RequireForEquivForExpression (this Tuple<bool, IEnumerable<Tuple<string, string>>> renames, IDeclaredParameter p1, IDeclaredParameter p2)
        {
            return renames
                .RequireForEquivForExpression(p1.RawValue, p1.Dependants.Select(p => p.RawValue),
                p2.RawValue, p2.Dependants.Select(p => p.RawValue));
        }

        /// <summary>
        /// Given two IValue's, do the expression conversion.
        /// </summary>
        /// <param name="renames"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static Tuple<bool, IEnumerable<Tuple<string, string>>> RequireForEquivForExpression(this Tuple<bool, IEnumerable<Tuple<string, string>>> renames, IValue p1, IValue p2)
        {
            return renames
                .RequireForEquivForExpression(p1.RawValue, p1.Dependants.Select(p => p.RawValue),
                p2.RawValue, p2.Dependants.Select(p => p.RawValue));
        }

        /// <summary>
        /// Two statements should become common. Take care of everything.
        /// </summary>
        /// <param name="renames"></param>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public static Tuple<bool, IEnumerable<Tuple<string, string>>> RequireForEquivForExpression(this Tuple<bool, IEnumerable<Tuple<string, string>>> renames,
            ICMStatementInfo s1, ICMStatementInfo s2)
        {
            if (!renames.Item1)
                return renames;

            if (s1 == null || s2 == null)
            {
                return Tuple.Create(false, Enumerable.Empty<Tuple<string, string>>());
            }

            // Run the require for the two statements.
            var r = s1.RequiredForEquivalence(s2, renames.Item2);
            if (!r.Item1)
            {
                return Tuple.Create(false, Enumerable.Empty<Tuple<string, string>>());
            }

            if (r.Item2 == null || r.Item2.Count() == 0)
            {
                return renames;
            }

            return Tuple.Create(true, renames.Item2.Concat(r.Item2));
        }

        public static Tuple<bool, IEnumerable<Tuple<string,string>>> FilterRenames (this Tuple<bool, IEnumerable<Tuple<string, string>>> renames, Func<Tuple<string,string>, bool> filter)
        {
            if (!renames.Item1 || renames.Item2 == null)
                return renames;

            return Tuple.Create(true, renames.Item2.Where(filter));
        }

        /// <summary>
        /// Can we make two expressions with a set of dependents look a like? What needs to be done to do it?
        /// </summary>
        /// <param name="renames"></param>
        /// <param name="e1"></param>
        /// <param name="dependents1"></param>
        /// <param name="e2"></param>
        /// <param name="dependents2"></param>
        /// <returns></returns>
        public static Tuple<bool, IEnumerable<Tuple<string,string>>> RequireForEquivForExpression (this Tuple<bool, IEnumerable<Tuple<string,string>>> renames,
            string e1, IEnumerable<string> dependents1,
            string e2, IEnumerable<string> dependents2)
        {
            // Monad option - quick check.
            if (!renames.Item1)
                return renames;

            // Now, build a new list of renames
            var rn = renames.Item2 == null ? new List<Tuple<string,string>>() : new List<Tuple<string, string>>(renames.Item2);

            // First, apply everything to our second expression.
            var exp2 = e2.ReplaceVariableNames(rn);
            if (exp2 == e1)
            {
                return renames.CheckForListNull();
            }

            // Now, sort by what we see in dependent variables, and see what we can replace here.
            var dependent2InOrder = dependents2
                .Where(i => exp2.IndexOf(i) >= 0)
                .OrderBy(i => exp2.IndexOf(i))
                .ToHashSet();

            var dependent1InOrder = dependents1.Except(renames.CheckForListNull().Item2.Select(i => i.Item2))
                .Where(i => e1.IndexOf(i) >= 0)
                .OrderBy(i => e1.IndexOf(i))
                .ToHashSet();

            var dep1InOrder = dependent1InOrder.Except(dependent2InOrder);
            var dep2InOrder = dependent2InOrder.Except(dependent1InOrder);

            // Loop through them now
            foreach (var dependent in dep2InOrder.Zip(dep1InOrder, (o, t) => Tuple.Create(o, t)))
            {
                var exprNew = exp2.Replace(dependent.Item1, dependent.Item2);
                if (exprNew != exp2)
                {
                    rn.Add(dependent);
                    if (exprNew == e1)
                    {
                        return Tuple.Create(true, rn as IEnumerable<Tuple<string, string>>);
                    }
                    exp2 = exprNew;
                }
            }

            return Tuple.Create(false, Enumerable.Empty<Tuple<string, string>>());
        }

        /// <summary>
        /// Prevent nulls
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static Tuple<bool, IEnumerable<Tuple<string,string>>> CheckForListNull (this Tuple<bool, IEnumerable<Tuple<string, string>>> s)
        {
            if (s.Item2 == null)
            {
                return Tuple.Create(s.Item1, new Tuple<string, string>[0] as IEnumerable<Tuple<string,string>>);
            }
            return s;
        }

        /// <summary>
        /// Filter a list out.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="exceptList"></param>
        /// <returns></returns>
        internal static Tuple<bool, IEnumerable<Tuple<string, string>>> ExceptFor(this Tuple<bool, IEnumerable<Tuple<string, string>>> s, IEnumerable<Tuple<string,string>> exceptList)
        {
            if (!s.Item1 || s.Item2 == null || exceptList == null)
                return s;

            var h = new HashSet<Tuple<string, string>>(exceptList);
            var filteredList = s.Item2.Where(p => !h.Contains(p)).ToArray();
            return Tuple.Create(true, filteredList as IEnumerable<Tuple<string, string>>);
        }

        /// <summary>
        /// Return all statements before this one. we march to the start of each block, and then up, and then all ones previous to that. Each time we go
        /// up a level we ignore that one.
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        internal static IEnumerable<IStatement> AllStatementsPrevious (this IStatement statement)
        {
            var parent = statement.FindCompoundParent();
            if (parent != null)
            {
                var statements = parent.Statements.TakeWhile(s => s != statement).Reverse();

                foreach (var s in statements)
                {
                    yield return s;
                }

                // Now, go up one.
                foreach (var s in statement.Parent.AllStatementsPrevious())
                {
                    yield return s;
                }
            }
        }

        /// <summary>
        /// Return all statements after this one
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        internal static IEnumerable<IStatement> AllStatementsAfter (this IStatement statement)
        {
            var parent = statement.FindCompoundParent();
            if (parent != null)
            {
                var statements = parent.Statements.SkipWhile(s => s != statement).Skip(1);

                foreach (var s in statements)
                {
                    yield return s;
                }

                // Now, go up one.
                foreach (var s in statement.Parent.AllStatementsAfter())
                {
                    yield return s;
                }
            }
        }

        /// <summary>
        /// Look to see if any of the variables names that are results are being used by these folks.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="varnames"></param>
        /// <returns></returns>
        internal static bool CheckForVariableAsReult (this IStatement s, IEnumerable<string> varnames)
        {
            // If we can't look, assume the worst, and that it is a result.
            if (!(s is ICMStatementInfo))
                return true;

            var resultVariables = (s as ICMStatementInfo).ResultVariables;
            var common = resultVariables.Intersect(varnames);
            return common.Count() != 0;
        }
    }
}
