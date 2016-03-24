using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LINQToTTreeLib.Statements
{
    /// <summary>
    /// We record a value (like an int, etc.) when it comes by. We also mark a value as true once we've seen
    /// it. Finally, if asked, we also will do a break once we've seen the value once.
    /// We can deal with saving multiple values, not just a single one.
    /// </summary>
    public class StatementRecordValue : IStatement, ICMStatementInfo
    {
        /// <summary>
        /// List of the values and parameters we should stuff them into when we fire.
        /// </summary>
        private List<Tuple<IDeclaredParameter, IValue, IDeclaredParameter[]>> _savers = new List<Tuple<IDeclaredParameter, IValue, IDeclaredParameter[]>>();

        /// <summary>
        /// Set this to true when we have seen a first value.
        /// </summary>
        private IDeclaredParameter _valueWasSeen;

        /// <summary>
        /// If true, then only record the first value we see.
        /// </summary>
        private bool _recordOnlyFirstValue;

        /// <summary>
        /// Create the statement block
        /// </summary>
        /// <param name="indexSeen"></param>
        /// <param name="indexValue"></param>
        /// <param name="valueWasSeen"></param>
        /// <param name="recordOnlyFirstValue"></param>
        public StatementRecordValue(IDeclaredParameter indexSaveLocation,
            IValue indexExpression, IDeclaredParameter[] dependents,
            IDeclaredParameter markWhenSeen, bool recordOnlyFirstValue)
        {
            if (indexSaveLocation == null)
                throw new ArgumentNullException("_indexSeen");
            if (indexExpression == null)
                throw new ArgumentNullException("indexExpression");
            if (markWhenSeen == null)
                throw new ArgumentNullException("markWhenSeen");

            AddNewSaver(indexSaveLocation, indexExpression, dependents);
            this._recordOnlyFirstValue = recordOnlyFirstValue;
            this._valueWasSeen = markWhenSeen;
        }

        /// <summary>
        /// Add to the list of variables that are to be recorded.
        /// </summary>
        /// <param name="saver"></param>
        /// <param name="loopIndexVar"></param>
        public void AddNewSaver(IDeclaredParameter saver, IValue loopIndexVar, IDeclaredParameter[] dependents)
        {
            _savers.Add(Tuple.Create(saver, loopIndexVar, dependents));
        }

        /// <summary>
        /// Render the code for this guy!
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> CodeItUp()
        {
            string indent = "";
            if (_recordOnlyFirstValue)
            {
                yield return string.Format("if (!{0}) {{", _valueWasSeen.RawValue);
                indent = "  ";
            }
            foreach (var p in _savers)
            {
                yield return string.Format("{2}{0} = {1};", p.Item1.RawValue, p.Item2.RawValue, indent);
            }
            yield return string.Format("{1}{0} = true;", _valueWasSeen.RawValue, indent);
            if (_recordOnlyFirstValue)
            {
                yield return "}";
            }
        }

        /// <summary>
        /// Rename our variables
        /// </summary>
        /// <param name="originalName"></param>
        /// <param name="newName"></param>
        public void RenameVariable(string originalName, string newName)
        {
            foreach (var p in _savers)
            {
                p.Item1.RenameRawValue(originalName, newName);
                p.Item2.RenameRawValue(originalName, newName);
            }
            _valueWasSeen.RenameRawValue(originalName, newName);
        }

        /// <summary>
        /// Combine these statements.
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="optimize"></param>
        /// <returns></returns>
        public bool TryCombineStatement(IStatement statement, ICodeOptimizationService optimize)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");

            var other = statement as StatementRecordValue;
            if (other == null)
                return false;

            if (other._recordOnlyFirstValue != _recordOnlyFirstValue)
                return false;

            if (other._savers.Count != _savers.Count)
                return false;

            foreach (var o in other._savers)
            {
                var ms = _savers.Any(s => s.Item2.RawValue == o.Item2.RawValue);
                if (!ms)
                    return false;
            }

            if (optimize == null)
                throw new ArgumentNullException("optimize");

            foreach (var o in other._savers)
            {
                var ms = _savers.Where(s => s.Item2.RawValue == o.Item2.RawValue).First();
                optimize.TryRenameVarialbeOneLevelUp(o.Item1.RawValue, ms.Item1);
            }
            optimize.TryRenameVarialbeOneLevelUp(other._valueWasSeen.RawValue, _valueWasSeen);

            return true;
        }

        /// <summary>
        /// See if we can't make everything the same. Since we have so many expressions to manage, this isn't totally "fun".
        /// </summary>
        /// <param name="other"></param>
        /// <param name="replaceFirst"></param>
        /// <returns></returns>
        public Tuple<bool, IEnumerable<Tuple<string, string>>> RequiredForEquivalence(ICMStatementInfo other, IEnumerable<Tuple<string, string>> replaceFirst = null)
        {
            if (!(other is StatementRecordValue))
            {
                return Tuple.Create(false, Enumerable.Empty<Tuple<string, string>>());
            }
            var s2 = other as StatementRecordValue;

            if (s2._recordOnlyFirstValue != _recordOnlyFirstValue || s2._savers.Count != _savers.Count)
            {
                return Tuple.Create(false, Enumerable.Empty<Tuple<string, string>>());
            }

            // Now, just look at all the expressions. Yes this is a monad. Yes I should use something real.
            var renames = Tuple.Create(true, replaceFirst)
                .RequireForEquivForExpression(_valueWasSeen.RawValue, s2._valueWasSeen.RawValue);

            foreach (var s in _savers.Zip(s2._savers, (u,t) => Tuple.Create(u, t)))
            {
                renames = renames.RequireForEquivForExpression(s.Item1.Item1.RawValue,
                    s.Item2.Item1.RawValue);

                renames = renames.RequireForEquivForExpression(s.Item1.Item2.RawValue, s.Item1.Item3.Select(p => p.RawValue),
                    s.Item2.Item2.RawValue, s.Item2.Item3.Select(p => p.RawValue));
            }

            return renames.ExceptFor(replaceFirst);
        }

        /// <summary>
        /// Get/Set the compound statement this is embedded in.
        /// </summary>
        public IStatement Parent { get; set; }

        /// <summary>
        /// What are the vars that we need as input.
        /// </summary>
        public ISet<string> DependentVariables
        {
            get {
                var h = new HashSet<string>();
                foreach (var s in _savers)
                {
                    h.AddRange(s.Item3.Select(v => v.RawValue));
                }

                if (_recordOnlyFirstValue)
                {
                    h.Add(_valueWasSeen.RawValue);
                }

                return h;
            }
        }

        /// <summary>
        /// What are the variables that are a result. This is everything we are going to be setting.
        /// </summary>
        public ISet<string> ResultVariables
        {
            get {
                var h = new HashSet<string>();

                h.Add(_valueWasSeen.RawValue);
                foreach (var s in _savers)
                {
                    h.Add(s.Item1.RawValue);
                }

                return h;
            }
        }


        /// <summary>
        /// This particular statement should never be lifted. Ever.
        /// </summary>
        public bool NeverLift
        {
            get { return true; }
        }
    }
}
