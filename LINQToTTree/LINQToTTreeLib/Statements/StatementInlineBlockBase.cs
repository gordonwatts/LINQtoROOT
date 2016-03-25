using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Optimization;
using LINQToTTreeLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LINQToTTreeLib.Statements
{
    /// <summary>
    /// Implements a block of code and the ability to combine, render, etc. Meant to
    /// be used as a base object however. The class that surrounds this one
    /// does not add extra statements inside this block - but it might have something like
    /// an "if" statement around the contents of the block.
    /// </summary>
    public abstract class StatementInlineBlockBase : IBookingStatementBlock, ICMCompoundStatementInfo, ICMStatementInfo
    {
        /// <summary>
        /// The list of statements in this block.
        /// </summary>
        public IEnumerable<IStatement> Statements
        {
            get { return _statements; }
        }

        /// <summary>
        /// Keep track of the statements we know about!
        /// </summary>
        private List<IStatement> _statements = new List<IStatement>();

        /// <summary>
        /// Adds a statement to the end of the list of statements that we know about.
        /// </summary>
        /// <param name="statement"></param>
        public void Add(IStatement statement)
        {
            if (statement == null)
                throw new ArgumentNullException("Cannot add a null statement");
            if (statement.Parent != null)
                throw new ArgumentException("Statement is already in another statement");

            _statements.Add(statement);
            statement.Parent = this;
        }

        /// <summary>
        /// Remove a statement that is in our current list.
        /// </summary>
        /// <param name="statement"></param>
        public void Remove(IStatement statement)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");

            if (!_statements.Remove(statement))
                throw new ArgumentException("statement is not in this compound statement");

            statement.Parent = null;
        }

        /// <summary>
        /// Add a statement before another statement.
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="beforeThisStatement"></param>
        public void AddBefore(IStatement statement, IStatement beforeThisStatement)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");
            if (statement.Parent != null)
                throw new ArgumentException("Statement is already in another statement!");
            if (beforeThisStatement == null)
                throw new ArgumentNullException("beforeThisStatement");

            var index = _statements.IndexOf(beforeThisStatement);
            if (index < 0)
                throw new ArgumentException("Statement does not exist in this compound statement");

            _statements.Insert(index, statement);
            statement.Parent = this;
        }

        /// <summary>
        /// Track the list of variables we keep for others.
        /// </summary>
        private List<IDeclaredParameter> _variables = new List<IDeclaredParameter>();

        /// <summary>
        /// Add a variable to the list.
        /// </summary>
        /// <param name="variableToDeclare"></param>
        public void Add(IDeclaredParameter variableToDeclare, bool failIfAlreadyThere = true)
        {
            if (variableToDeclare == null)
                throw new ArgumentNullException("Must not declare a null variable");

            var findOld = from v in AllDeclaredVariables
                          where v.ParameterName == variableToDeclare.ParameterName
                          select v;
            if (findOld.FirstOrDefault() != null)
            {
                if (!failIfAlreadyThere)
                    return;
                throw new ArgumentException("Variable '" + variableToDeclare.ParameterName + "' has already been declared in this block!");
            }

            _variables.Add(variableToDeclare);
        }

        /// <summary>
        /// Remove a variable. No failure if it isn't in there.
        /// </summary>
        /// <param name="var"></param>
        public void Remove(IDeclaredParameter var)
        {
            _variables.Remove(var);
        }

        /// <summary>
        /// Return the variables in the block
        /// </summary>
        public IEnumerable<IDeclaredParameter> DeclaredVariables
        {
            get { return _variables; }
        }

        /// <summary>
        /// Return every single declared variable in the whole hierarchy!
        /// </summary>
        public IEnumerable<IDeclaredParameter> AllDeclaredVariables
        {
            get
            {
                var b = FindBookingParent(Parent);
                if (b != null)
                    return _variables.Concat(b.AllDeclaredVariables);
                return _variables;
            }
        }

        /// <summary>
        /// See if we can climb chain to find a booking guy
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private IBookingStatementBlock FindBookingParent(IStatement p)
        {
            while (p != null)
            {
                if (p is IBookingStatementBlock)
                    return p as IBookingStatementBlock;
                p = p.Parent;
            }
            return null;
        }

        /// <summary>
        /// Returns the lines of code - must be implemented by the surrounding guy.
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<string> CodeItUp();

        /// <summary>
        /// Return the lines of code that we are keeping in the internal list. We make sure that
        /// we surround it with a bunch of curly braces.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> RenderInternalCode()
        {
            if (_statements.Count > 0)
            {
                yield return "{";

                foreach (var v in _variables)
                {
                    string varDecl = Variables.VarUtils.AsCPPType(v.Type) + " " + v.ParameterName;
                    var defaultValue = GenerateDefaultValue(v);
                    if (!string.IsNullOrWhiteSpace(defaultValue))
                        varDecl = varDecl + "=" + defaultValue;
                    varDecl += ";";
                    yield return "  " + varDecl;
                }

                var sublines = from s in _statements
                               from l in s.CodeItUp()
                               select l;
                foreach (var l in sublines)
                {
                    yield return "  " + l;
                }
                yield return "}";
            }
        }

        /// <summary>
        /// Generate a default value for a variable.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        private string GenerateDefaultValue(IDeclaredParameter v)
        {
            if (v == null)
                throw new ArgumentNullException("v");

            if (v.InitialValue != null)
                return v.InitialValue.RawValue;

            if (v.Type.IsNumberType())
                return "0";

            if (v.Type == typeof(bool))
                return "false";

            if (v.Type.IsArray)
                return "";

            if (v.Type.IsClass || v.Type.IsGenericType)
            {
                return "";
            }

            throw new NotSupportedException(string.Format("Don't know how to do default value for C++ variable of type {0}.", v.Type.ToString()));
        }

        /// <summary>
        /// Implement to do the combining of two blocks of code.
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        public abstract bool TryCombineStatement(IStatement statement, ICodeOptimizationService opt);

        /// <summary>
        /// Helper class - when a statement shows up with no context.
        /// </summary>
        class FailingCodeOptimizer : ICodeOptimizationService
        {
            /// <summary>
            /// We can't rename one level up - we have no context with which to do that!
            /// </summary>
            /// <param name="oldName"></param>
            /// <param name="newVariable"></param>
            /// <returns></returns>
            public bool TryRenameVarialbeOneLevelUp(string oldName, IDeclaredParameter newVariable)
            {
                return false;
            }

            /// <summary>
            /// We can't rename anything. So just return.
            /// </summary>
            /// <param name="originalName"></param>
            /// <param name="newName"></param>
            public void ForceRenameVariable(string originalName, string newName)
            {
            }
        }

        /// <summary>
        /// Given a list of statements, attempt to combine them with the ones we already have
        /// internally. If we can't, then just append them. This is like our "Add" above, but we
        /// first check to see if any of the statements can be added in. This always
        /// succeeds (no need for a bool return) because we just add things onto the end.
        /// </summary>
        /// <param name="statements">List of statements that we need to combine</param>
        /// <remarks>Assume that the ordering given in the statements must be obeyed.
        ///     - Never insert a later statement before an earlier one!
        /// </remarks>
        private List<IStatement> CombineInternal(IEnumerable<IStatement> statements, IBookingStatementBlock parent, bool appendIfCantCombine = true, bool moveIfIdentical = false)
        {
            bool didAllCombine = true;
            ICodeOptimizationService myopt;
            var mergedIntoList = new List<IStatement>();
            if (parent != null)
            {
                myopt = new BlockRenamer(parent, this);
            }
            else
            {
                myopt = new FailingCodeOptimizer();
            }

            // when we move through this we have to be careful - the try combine has side effects!
            var currentStatements = Statements.ToArray();
            var firstStatement = currentStatements.Length == 0 ? null : currentStatements[0];

            var statementsToRemove = new List<IStatement>();
            foreach (var s in statements)
            {
                var firstGood = currentStatements.SkipWhile(sinner => !sinner.TryCombineStatement(s, myopt)).ToArray();

                // If we couldn't find a way to combine this guy, then we will put it as the first statement in our list.
                if (firstGood.Length == 0)
                {
                    if (appendIfCantCombine)
                    {
                        s.Parent = null;
                        if (firstStatement != null)
                        {
                            AddBefore(s, firstStatement);
                        }
                        else
                        {
                            Add(s);
                        }
                    }
                    else
                    {
                        didAllCombine = false;
                    }
                }
                else
                {
                    // Add to our list of merged statements. Ignore this guy if it is the same one.
                    // Remove this guy from its old parent.
                    currentStatements = firstGood;
                    if (currentStatements[0] != s)
                    {
                        mergedIntoList.Add(currentStatements[0]);
                        if (moveIfIdentical)
                        {
                            statementsToRemove.Add(s);
                        }
                    }
                    currentStatements = currentStatements.Skip(1).ToArray();
                    firstStatement = currentStatements.Length == 0 ? null : currentStatements[0];
                }
            }

            // Remove any statements that were combined
            foreach (var s in statementsToRemove)
            {
                (s.Parent as IBookingStatementBlock).Remove(s);
            }

            return didAllCombine ? mergedIntoList : null;
        }

        /// <summary>
        /// Combine a list of statements with a common parent into the this block.
        /// </summary>
        /// <param name="statements">List of statements to combine</param>
        /// <param name="parent">The common parent of the list of statements</param>
        /// <param name="appendIfCantCombine">If true always add the statements onto the end of the block</param>
        /// <returns>True if the statements were merged or appended onto the end of the block</returns>
        public bool Combine(IEnumerable<IStatement> statements, IBookingStatementBlock parent, bool appendIfCantCombine = true, bool moveIfIdentical = false)
        {
            return CombineInternal(statements, parent, appendIfCantCombine, moveIfIdentical) != null || appendIfCantCombine;
        }

        /// <summary>
        /// Try to combine a single statement. Return the statement that it was combined with. If appendIfNoCombine
        /// is true, then add it on, and still return null.
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="appendIfNoCombine">The statement will always be added on the end.</param>
        /// <returns>The statement it was merged with, if merging took place.</returns>
        public IStatement CombineAndMark(IStatement statement, IBookingStatementBlock parent, bool appendIfNoCombine = true)
        {
            var s = CombineInternal(new IStatement[] { statement }, parent, appendIfNoCombine);
            return s == null ? null : s.FirstOrDefault();
        }

        /// <summary>
        /// Absorb all the info from this combined block into this one.
        /// </summary>
        /// <param name="block"></param>
        protected bool Combine(StatementInlineBlockBase block, ICodeOptimizationService opt, bool appendIfCantCombine = true, bool moveIfIdentical = false)
        {
            var combineSucceeded = Combine(block.Statements, block, appendIfCantCombine: appendIfCantCombine, moveIfIdentical: moveIfIdentical);
            Combine(block.DeclaredVariables);
            return combineSucceeded;
        }

        /// <summary>
        /// Add the variables into this block.
        /// </summary>
        /// <param name="vars"></param>
        protected void Combine(IEnumerable<IDeclaredParameter> vars)
        {
            foreach (var v in vars)
            {
                // It is possible that the variables might be the same, so don't
                // add them if that is the case. This happens b/c the various statements are combined and that may cause
                // some variable renaming.

                if (_variables.Find(intv => intv.ParameterName == v.ParameterName) == null)
                    Add(v);
            }
        }

        /// <summary>
        /// Implement to rename variables in this blocked code.
        /// </summary>
        /// <param name="origName"></param>
        /// <param name="newName"></param>
        public abstract void RenameVariable(string origName, string newName);

        /// <summary>
        /// Rename all our guys in our internal statements.
        /// </summary>
        /// <param name="originalName"></param>
        /// <param name="newName"></param>
        public void RenameBlockVariables(string originalName, string newName)
        {
#if false
            // And remove any declarations. When we rename, we are doing it because this block
            // is being made to look like an already existing other block - so the variable will
            // have been declared over there already.
            var decl = DeclaredVariables.Where(d => d.RawValue == originalName).FirstOrDefault();
            if (decl != null)
                Remove(decl);

#else
            // Rename the declared variables as requested. While these variables may be used again,
            // they need to appear here or if the combine fails, they will not be declared (and they
            // will be used).
            foreach (var v in DeclaredVariables.Where(d => d.RawValue == originalName))
            {
                v.RenameRawValue(originalName, newName);
            }
#endif
            foreach (var s in _statements)
            {
                s.RenameVariable(originalName, newName);
            }
        }

        /// <summary>
        /// Points to the statement that holds onto us.
        /// </summary>
        public IStatement Parent { get; set; }

        /// <summary>
        /// Override to decide how to bubble up items
        /// </summary>
        public abstract bool AllowNormalBubbleUp { get; }

        /// <summary>
        /// Get a list of all dependent variables used in this code block.
        /// If you add some new things (like the test expression for an if statement),
        /// then override this.
        /// </summary>
        public virtual ISet<string> DependentVariables
        {
            get
            {
                var dependents = Statements
                    .Where(s => s is ICMStatementInfo)
                    .Cast<ICMStatementInfo>()
                    .SelectMany(s => s.DependentVariables)
                    .Where(v => !DeclaredVariables.Select(p => p.RawValue).Contains(v))
                    ;
                return new HashSet<string>(dependents);
            }
        }

        /// <summary>
        /// Return the list of variables that are altered by this code block. Override if
        /// there is another source somewhere!
        /// </summary>
        public ISet<string> ResultVariables
        {
            get
            {
                var results = Statements
                    .Where(s => s is ICMStatementInfo)
                    .Cast<ICMStatementInfo>()
                    .SelectMany(s => s.ResultVariables)
                    .Where(v => !DeclaredVariables.Select(p => p.RawValue).Contains(v));
                return new HashSet<string>(results);
            }
        }

        /// <summary>
        /// As long as the dependent/result stuff is satisfied, then a lifting can occur no problem.
        /// </summary>
        public bool NeverLift
        {
            get { return false; }
        }

        /// <summary>
        /// If there is no expression surrounding, then return true. Otherwise, one will
        /// have to carefully double check!
        /// </summary>
        /// <param name="followStatement"></param>
        /// <returns></returns>
        public abstract bool CommutesWithGatingExpressions(ICMStatementInfo followStatement);

        /// <summary>
        /// Figure out which statement occurs first in our sequence.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public bool IsBefore(IStatement first, IStatement second)
        {
            var whoIsFirst = Statements.Where(s => s == first || s == second).FirstOrDefault();
            if (whoIsFirst == null)
                throw new ArgumentException("Unable to find either the first or second statement in the list");

            return whoIsFirst == first;
        }

        /// <summary>
        /// We can't determine directly if we can combine.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="replaceFirst"></param>
        /// <returns></returns>
        public virtual Tuple<bool, IEnumerable<Tuple<string, string>>> RequiredForEquivalence(ICMStatementInfo other, IEnumerable<Tuple<string, string>> replaceFirst = null)
        {
            return Tuple.Create(false, Enumerable.Empty<Tuple<string, string>>());
        }

        /// <summary>
        /// Helper routine. Pass it all renames relavent, and it will do all the testing.
        /// It will return the FULL rename list, including everything you have passed in!
        /// </summary>
        /// <param name="other"></param>
        /// <param name="replaceFirst"></param>
        /// <returns></returns>
        protected virtual Tuple<bool, IEnumerable<Tuple<string, string>>> RequiredForEquivalenceForBase(ICMStatementInfo other, Tuple<bool, IEnumerable<Tuple<string, string>>> renames)
        {
            if (!renames.Item1)
                return renames;

            // We assume this works at this point!
            var s2 = other as StatementInlineBlockBase;

            // If the number of statements isn't the same, then this doesn't matter.
            if (Statements.Count() != s2.Statements.Count())
            {
                return Tuple.Create(false, Enumerable.Empty<Tuple<string, string>>());
            }

            // Loop through the statements, accumulating renames as we go.
            foreach (var s in Statements.Zip(s2.Statements, (st1, st2) => Tuple.Create(st1, st2)))
            {
                renames = renames
                    .RequireForEquivForExpression(s.Item1 as ICMStatementInfo, s.Item2 as ICMStatementInfo);
            }

            // If we make it here, then we are good. The last thing to do before returning the result is to remove
            // any renames and any declared variables
            var declaredVariables = s2.DeclaredVariables;

            return renames
                .FilterRenames(i => !declaredVariables.Select(p => p.RawValue).Contains(i.Item1));
        }
    }
}
