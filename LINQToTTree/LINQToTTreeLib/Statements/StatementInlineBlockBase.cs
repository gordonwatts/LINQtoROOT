using System;
using System.Collections.Generic;
using System.Linq;
using LinqToTTreeInterfacesLib;

namespace LINQToTTreeLib.Statements
{
    /// <summary>
    /// Implements a block of code and the ability to combine, render, etc. Meant to
    /// be used as a base objec,t however. The class that surrounds this one
    /// does not add extra statements inside this block - but it might have something like
    /// an "if" statement around the contents of the block.
    /// </summary>
    public abstract class StatementInlineBlockBase : IBookingStatementBlock
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
        /// Track the list of variables we keep for others.
        /// </summary>
        private List<IVariable> _variables = new List<IVariable>();

        /// <summary>
        /// Add a variable to the list.
        /// </summary>
        /// <param name="variableToDeclare"></param>
        public void Add(IVariable variableToDeclare)
        {
            if (variableToDeclare == null)
                throw new ArgumentNullException("Must not declare a null variable");

            var findOld = from v in _variables
                          where v.VariableName == variableToDeclare.VariableName
                          select v;
            if (findOld.FirstOrDefault() != null)
                throw new ArgumentException("Variable '" + variableToDeclare.VariableName + "' has already been declared in this block!");

            _variables.Add(variableToDeclare);
        }

        /// <summary>
        /// Return the variables in the block
        /// </summary>
        public IEnumerable<IVariable> DeclaredVariables
        {
            get { return _variables; }
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
            var goodVars = (from v in _variables
                            where v.Declare
                            select v).ToArray();

            if (_statements.Count > 0 || goodVars.Length > 0)
            {
                yield return "{";

                foreach (var v in goodVars)
                {
                    string varDecl = Variables.VarUtils.AsCPPType(v.Type) + " " + v.VariableName;
                    if (v.InitialValue != null)
                    {
                        varDecl = varDecl + "=" + v.InitialValue.RawValue;
                    }
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
        /// Implement to do the combining of two blocks of code.
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        public abstract bool TryCombineStatement(IStatement statement, ICodeOptimizationService opt);

        /// <summary>
        /// If someone wants to rename statements downstream, we take care of it.
        /// </summary>
        private class BlockRenamer : ICodeOptimizationService
        {
            private IBookingStatementBlock _holderBlock;

            public BlockRenamer(IBookingStatementBlock holder)
            {
                if (holder == null)
                    throw new ArgumentNullException("holder");
                this._holderBlock = holder;
            }

            /// <summary>
            /// Rename succeeds if we can find the declared variable, amone other things.
            /// </summary>
            /// <param name="oldName"></param>
            /// <param name="newName"></param>
            /// <returns></returns>
            public bool TryRenameVarialbeOneLevelUp(string oldName, IVariable newName)
            {
                var vr = _holderBlock.DeclaredVariables.Where(v => v.RawValue == oldName).FirstOrDefault();
                if (vr == null)
                    return false;

                // Check that its initialization is the same!
                bool initValueSame = (vr.InitialValue == null && newName.InitialValue == null)
                    || (vr.InitialValue != null && (vr.InitialValue.RawValue == newName.InitialValue.RawValue));
                if (!initValueSame)
                    return false;

                // Rename the variable!
                _holderBlock.RenameVariable(oldName, newName.RawValue);

                return true;
            }
        }

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
            public bool TryRenameVarialbeOneLevelUp(string oldName, IVariable newVariable)
            {
                return false;
            }
        }

        /// <summary>
        /// Given a list of statements, attempt to combine them with the ones we already have
        /// internaly. If we can't, then just append them. This is like our "Add" above, but we
        /// first check to see if any of the statements can be added in. This always
        /// succeeds (no need for a bool return) because we just add things onto the end.
        /// </summary>
        /// <param name="statements">List of statements that we need to combine</param>
        protected void Combine(IEnumerable<IStatement> statements, IBookingStatementBlock parent)
        {
            ICodeOptimizationService myopt;
            if (parent != null)
            {
                myopt = new BlockRenamer(parent);
            }
            else
            {
                myopt = new FailingCodeOptimizer();
            }
            foreach (var s in statements)
            {
                bool didCombine = false;
                foreach (var sinner in Statements)
                {
                    if (sinner.TryCombineStatement(s, myopt))
                    {
                        didCombine = true;
                        break;
                    }
                }
                if (!didCombine)
                {
                    s.Parent = null;
                    Add(s);
                }
            }
        }

        /// <summary>
        /// Absorbe all the info from this combined block into this one.
        /// </summary>
        /// <param name="block"></param>
        protected void Combine(StatementInlineBlockBase block, ICodeOptimizationService opt)
        {
            Combine(block.Statements, block);
            Combine(block.DeclaredVariables);
        }

        /// <summary>
        /// Add the variables into this block
        /// </summary>
        /// <param name="vars"></param>
        protected void Combine(IEnumerable<IVariable> vars)
        {
            foreach (var v in vars)
            {
                Add(v);
            }
        }

        /// <summary>
        /// Implement so same statement detection can be done
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        public abstract bool IsSameStatement(IStatement statement);

        /// <summary>
        /// Compare our statement lists to that in another block to see
        /// if they are the same.
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        public virtual bool IsSameStatement(StatementInlineBlockBase statements)
        {
            // Must be same statements in the same order

            if (statements == null)
                throw new ArgumentNullException("statement must not be null");

            if (_statements.Count != statements._statements.Count)
                return false;

            return _statements.Zip(statements._statements, (s1, s2) => s1.IsSameStatement(s2)).All(test => test);
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
            foreach (var s in _statements)
            {
                s.RenameVariable(originalName, newName);
            }
        }

        /// <summary>
        /// Points to the statement that holds onto us.
        /// </summary>
        public IStatement Parent { get; set; }
    }
}
