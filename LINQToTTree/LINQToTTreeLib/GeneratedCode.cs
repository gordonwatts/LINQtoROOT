using System;
using System.Collections.Generic;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Statements;

namespace LINQToTTreeLib
{
    /// <summary>
    /// Holds onto the code that we generate for a single query.
    /// </summary>
    public class GeneratedCode : IGeneratedCode
    {
        public GeneratedCode()
        {
            CodeBody = new StatementInlineBlock();
            CurrentScopePointer = CodeBody;
            CurrentDeclarationScopePointer = CodeBody;
        }

        /// <summary>
        /// The final result of this query.
        /// </summary>
        public IVariable ResultValue { get; private set; }

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

        public void SetResult(IVariable r)
        {
            if (r == null)
                throw new ArgumentNullException("Cannot set the result to be null");

            ResultValue = r;
        }

        /// <summary>
        /// Keeps track of the the current scope
        /// </summary>
        private class CurrentScopeInfo
        {
            public IStatementCompound Scope;
            public IBookingStatementBlock BookingScope;
        }

        /// <summary>
        /// Get/set the current scope pointer.
        /// </summary>
        public object CurrentScope
        {
            get
            {
                return new CurrentScopeInfo() { Scope = CurrentScopePointer, BookingScope = CurrentDeclarationScopePointer };
            }
            set
            {
                CurrentScopeInfo info = value as CurrentScopeInfo;
                if (info == null)
                    throw new ArgumentNullException("Can't reset current scope to be null");

                CurrentScopePointer = info.Scope;
                CurrentDeclarationScopePointer = info.BookingScope;
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

            if (s is IStatementCompound)
                CurrentScopePointer = s as IStatementCompound;
            if (s is IBookingStatementBlock)
                CurrentDeclarationScopePointer = s as IBookingStatementBlock;
        }

        /// <summary>
        /// Book a variable at the inner most scoping that is accepting variable
        /// declarations.
        /// </summary>
        /// <param name="v"></param>
        public void Add(IVariable v)
        {
            if (v == null)
                throw new ArgumentNullException("Cannot add a null variable!");
            CurrentDeclarationScopePointer.Add(v);
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
        /// </summary>
        /// <param name="v"></param>
        public void QueueForTransfer(string name, object val)
        {
            if (val == null)
                throw new ArgumentNullException("val");
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("name");

            _variablesToTransfer[name] = val;
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
    }
}
