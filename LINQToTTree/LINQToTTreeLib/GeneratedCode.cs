using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Utils;

namespace LINQToTTreeLib
{
    /// <summary>
    /// Holds onto the code that we generate for a single query.
    /// </summary>
    public class GeneratedCode : IGeneratedQueryCode, IExecutableCode
    {
        public GeneratedCode()
        {
            CodeBody = new StatementInlineBlock();
            CurrentScopePointer = CodeBody;
            CurrentDeclarationScopePointer = CodeBody;
            PreviousDeclarationScopePointer = null;
            Depth = 1;
        }

        public int Depth { get; private set; }

        /// <summary>
        /// The final result of this query.
        /// </summary>
        public Expression ResultValue { get; private set; }

        /// <summary>
        /// Get the final result as a variable.
        /// </summary>
        public IDeclaredParameter ResultValueAsVaraible { get; private set; }

        /// <summary>
        /// We have only a single result value, so the return is pretty easy...
        /// </summary>
        public IEnumerable<IDeclaredParameter> ResultValues
        {
            get { yield return ResultValueAsVaraible; }
        }

        /// <summary>
        /// The code body is basically a bunch of statements, which starts from our top level
        /// statement.
        /// </summary>
        public IBookingStatementBlock CodeBody { get; private set; }

        /// <summary>
        /// Keeps track of the level where we are adding statements
        /// </summary>
        private IStatementCompound CurrentScopePointer;

        /// <summary>
        /// Keep track of where we are declaring variables.
        /// </summary>
        private IBookingStatementBlock CurrentDeclarationScopePointer;

        /// <summary>
        /// One level up from where we are now...
        /// </summary>
        private IBookingStatementBlock PreviousDeclarationScopePointer;

        public void SetResult(Expression r)
        {
            if (r == null)
                throw new ArgumentNullException("Cannot set the result to be null");

            ResultValue = r;

            if (r is IDeclaredParameter)
                ResultValueAsVaraible = r as IDeclaredParameter;
        }

        /// <summary>
        /// Reset the result to be null
        /// </summary>
        public void ResetResult()
        {
            ResultValue = null;
            ResultValueAsVaraible = null;
        }

        /// <summary>
        /// Keeps track of the the current scope
        /// </summary>
        private class CurrentScopeInfo : IScopeInfo
        {
            public IStatementCompound Scope;
            public IBookingStatementBlock BookingScope;
            public IBookingStatementBlock PreviousBookingScope;
            public int oldDepth;
        }

        /// <summary>
        /// Get/set the current scope pointer.
        /// </summary>
        public IScopeInfo CurrentScope
        {
            get
            {
                return new CurrentScopeInfo() { Scope = CurrentScopePointer, BookingScope = CurrentDeclarationScopePointer, PreviousBookingScope = PreviousDeclarationScopePointer, oldDepth = Depth };
            }
            set
            {
                CurrentScopeInfo info = value as CurrentScopeInfo;
                if (info == null)
                    throw new ArgumentNullException("Can't reset current scope to be null");

                CurrentScopePointer = info.Scope;
                CurrentDeclarationScopePointer = info.BookingScope;
                PreviousDeclarationScopePointer = info.PreviousBookingScope;
                Depth = info.oldDepth;
            }
        }

        /// <summary>
        /// Add a statement to the current "cursor" point.
        /// </summary>
        /// <param name="s"></param>
        public void Add(IStatement s)
        {
            if (s == null)
            {
                throw new ArgumentNullException("Cannot add a statement that is null");
            }

            CurrentScopePointer.Add(s);

            ///
            /// Change the statement scope if there something that goes down a level.
            /// 

            if (s is IBookingStatementBlock)
            {
                _scopeState.Push(CurrentScope);
            }

            if (s is IStatementCompound)
                CurrentScopePointer = s as IStatementCompound;
            if (s is IBookingStatementBlock)
            {
                PreviousDeclarationScopePointer = CurrentDeclarationScopePointer;
                CurrentDeclarationScopePointer = s as IBookingStatementBlock;
                Depth++;
            }
        }

        /// <summary>
        /// Book a variable at the inner most scoping that is accepting variable
        /// declarations.
        /// </summary>
        /// <param name="v"></param>
        public void Add(IDeclaredParameter v)
        {
            if (v == null)
                throw new ArgumentNullException("Cannot add a null variable!");
            CurrentDeclarationScopePointer.Add(v);
        }

        /// <summary>
        /// Add a variable one level up from the current scope. Fail if we can't!
        /// </summary>
        /// <param name="valSimple"></param>
        public void AddOneLevelUp(IDeclaredParameter valSimple)
        {
            if (valSimple == null)
                throw new ArgumentNullException("cannot add null variable!");

            if (PreviousDeclarationScopePointer == null)
                throw new InvalidOperationException("Can't declare one varaible one level up when one level up doesn't exist!");

            PreviousDeclarationScopePointer.Add(valSimple);
        }

        /// <summary>
        /// Get the list of variables that need to be transfered over the wire.
        /// </summary>
        public IEnumerable<KeyValuePair<string, object>> VariablesToTransfer { get { return _variablesToTransfer; } }

        /// <summary>
        /// Hold onto the list of items that we will have to send over the wire
        /// </summary>
        private Dictionary<string, object> _variablesToTransfer = new Dictionary<string, object>();

        /// <summary>
        /// Some variables need to be shipped over the wire to PROOF or the version of root that is
        /// actually doing the work. This is where we
        /// keep track of those. This is as opposed to the "result" which is going to come back
        /// to the source with what we need in it.
        /// We generate the name on the fly, and also make sure that we don't duplicate anything!
        /// </summary>
        /// <param name="v"></param>
        public string QueueForTransfer(object val)
        {
            if (val == null)
                throw new ArgumentNullException("val");

            var isthere = (from o in _variablesToTransfer
                           where o.Value == val
                           select o.Key).FirstOrDefault();


            if (isthere != null)
            {
                return isthere;
            }
            else
            {
                string name = val.GetType().CreateUniqueVariableName();

                _variablesToTransfer[name] = val;

                return name;
            }
        }

        /// <summary>
        /// Keep track of a list of include files that
        /// we need to pull in at the top of the header file!
        /// </summary>
        private HashSet<string> _includeFiles = new HashSet<string>();

        /// <summary>
        /// Add an include file to the list... if it is already included, it won't be done twice.
        /// </summary>
        /// <param name="includeName"></param>
        public void AddIncludeFile(string includeName)
        {
            _includeFiles.Add(includeName);
        }

        /// <summary>
        /// Gets the list of include files that should be included at the top of this.
        /// </summary>
        public IEnumerable<string> IncludeFiles
        {
            get { return _includeFiles; }
        }

        /// <summary>
        /// We are a single query - so we return a single query block. Always. Someone else is smart
        /// enough to try to combine the blocks.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IStatementCompound> QueryCode()
        {
            yield return CodeBody;
        }

        /// <summary>
        /// Keep a unique list of all leaves that this expression references.
        /// </summary>
        private HashSet<string> _referencedLeavs = new HashSet<string>();

        public void AddReferencedLeaf(string leafName)
        {
            _referencedLeavs.Add(leafName);
        }

        /// <summary>
        /// Returns a set of leaves that can be accessed.
        /// </summary>
        public IEnumerable<string> ReferencedLeafNames
        {
            get { return _referencedLeavs; }
        }

        /// <summary>
        /// Keep track of where our current scope is.
        /// </summary>
        private Stack<IScopeInfo> _scopeState = new Stack<IScopeInfo>();

        /// <summary>
        /// Pop one level of booking statements off our pointer - where our current scope is!
        /// </summary>
        public void Pop()
        {
            if (_scopeState.Count == 0)
                throw new InvalidOperationException("Unable to pop a level up in generated code when we've not gone down a level");
            CurrentScope = _scopeState.Pop();
        }
    }
}
