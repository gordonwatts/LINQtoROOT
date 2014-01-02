using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.QMFunctions;
using LINQToTTreeLib.QueryVisitors;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Utils;
using Remotion.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

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
            Debug.WriteLine("SetResult: {0}{1}", r.ToString(), "");

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
            public IBookingStatementBlock ResultScope;
            public IDictionary<string, IValue> RememberedExpressions;
            public int oldDepth;
        }

        /// <summary>
        /// Get/set the current scope pointer.
        /// </summary>
        public IScopeInfo CurrentScope
        {
            get
            {
                return new CurrentScopeInfo() { Scope = CurrentScopePointer, BookingScope = CurrentDeclarationScopePointer, PreviousBookingScope = PreviousDeclarationScopePointer, oldDepth = Depth, ResultScope = CurrentResultScope, RememberedExpressions = new Dictionary<string, IValue>(CurrentRememberedExpressions) };
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
                CurrentResultScope = info.ResultScope;
                CurrentRememberedExpressions = new Dictionary<string, IValue>(info.RememberedExpressions);
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
        /// Remove a statement from the current statement block where we are adding statements.
        /// </summary>
        /// <param name="s">The statement to remove. Throw if we can't find it in the current block.</param>
        public void Remove(IStatement s)
        {
            CurrentScopePointer.Remove(s);
        }

        /// <summary>
        /// Book a variable at the inner most scoping that is accepting variable
        /// declarations.
        /// </summary>
        /// <param name="v"></param>
        public void Add(IDeclaredParameter v, bool failIfALreadyThere = true)
        {
            if (v == null)
                throw new ArgumentNullException("Cannot add a null variable!");
            CurrentDeclarationScopePointer.Add(v, failIfALreadyThere);
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
        /// Keep track of the current result scope
        /// </summary>
        public IBookingStatementBlock CurrentResultScope { get; private set; }

        /// <summary>
        /// When we are in a result operator and the result of its work needs to be visible "outside" we need
        /// to make sure the declaration happens at the right spot. That is what this guy does.
        /// </summary>
        /// <param name="p"></param>
        public void AddAtResultScope(IDeclaredParameter p)
        {
            if (CurrentResultScope == null)
                throw new InvalidOperationException("Unable to add a parameter at the result scope - none is set");

            CurrentResultScope.Add(p);
        }

        /// <summary>
        /// Add a statement at result scoping.
        /// </summary>
        /// <param name="p"></param>
        public void AddAtResultScope(IStatement p)
        {
            if (CurrentResultScope == null)
                throw new InvalidOperationException("Unable to add a parameter at the result scope - none is set");

            CurrentResultScope.Add(p);
        }

        /// <summary>
        /// The current booking scope is set to be the result scope.
        /// </summary>
        public void SetCurrentScopeAsResultScope()
        {
            CurrentResultScope = CurrentDeclarationScopePointer;
        }

        /// <summary>
        /// Pop out the stack till we hit results level.
        /// </summary>
        public void PopToResultsLevel()
        {
            while (CurrentDeclarationScopePointer != CurrentResultScope)
            {
                Pop();
            }
        }

        /// <summary>
        /// Book a variable up above the current scope pointer - just outside something that
        /// is a loop construct.
        /// </summary>
        /// <param name="v"></param>
        public void AddOutsideLoop(IDeclaredParameter v)
        {
            FindFirstBookingForSurroundingLoop().Add(v);
        }

        /// <summary>
        /// Find a booking construct outside a loop.
        /// </summary>
        /// <returns></returns>
        private IBookingStatementBlock FindFirstBookingForSurroundingLoop()
        {
            //
            // First, look at the top level scope to see if we are sitting
            // at a loop or similar.
            //

            bool foundLoop = CurrentScopePointer is IStatementLoop;

            //
            // Run through the scope and make sure we get it right...
            //

            foreach (var s in _scopeState)
            {
                var sinfo = s as CurrentScopeInfo;
                if (foundLoop)
                {
                    return sinfo.BookingScope;
                }

                if (sinfo.Scope is IStatementLoop)
                {
                    foundLoop = true;
                }
            }

            throw new InvalidOperationException("Unable to add anything before a loop construct as there is not loop construct found!");
        }


        /// <summary>
        /// Add a statement up above the current scope pointer - just outside somethign that is a loop
        /// construct.
        /// </summary>
        /// <param name="s"></param>
        public void AddOutsideLoop(IStatement s)
        {
            FindFirstBookingForSurroundingLoop().Add(s);
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
        public void Pop(bool popPastLoopStatement = false)
        {
            while (true)
            {
                if (_scopeState.Count == 0)
                    throw new InvalidOperationException("Unable to pop a level up in generated code when we've not gone down a level");

                bool isLoopStatement = CurrentScopePointer is IStatementLoop;

                CurrentScope = _scopeState.Pop();

                if (!popPastLoopStatement || isLoopStatement)
                    return;
            }
        }

        /// <summary>
        /// Keep track of all the sub-expressions we are trying to remember.
        /// </summary>
        private Dictionary<string, IValue> CurrentRememberedExpressions = new Dictionary<string, IValue>();

        /// <summary>
        /// Remember a sub-expression. We will hide a previously made association!
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="r"></param>
        public void RememberSubExpression(Expression expr, IValue r)
        {
            var key = GetSubExpressionKey(expr);
            if (key == null)
                return;

            CurrentRememberedExpressions[key] = r;
            Debug.WriteLine("RememberSubExpression: {0} => {1}", key, r.ToString());
        }

        /// <summary>
        /// Return the key for doing sub-expression storage in a table.
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        private string GetSubExpressionKey(Expression expr)
        {
            return ExpressionStringConverter.Format(expr, true);
        }

        /// <summary>
        /// See if we have the sub-expression in our DB. If it isn't there, return null.
        /// </summary>
        /// <param name="expr">The expression that is being searched for</param>
        /// <returns>A value if the expression was cached, or null if not</returns>
        /// <remarks>This gets tricky if this is a constant expression. In that case, we use the Hash as the
        /// key to doing the lookup (GetHash).</remarks>
        public IValue LookupSubExpression(Expression expr)
        {
            string key = GetSubExpressionKey(expr);
            if (key == null)
                return null;

            if (CurrentRememberedExpressions.ContainsKey(key))
            {
                Debug.WriteLine("LookupSubExpression: {0} => {1}", key, CurrentRememberedExpressions[key]);
                return CurrentRememberedExpressions[key];
            }
            return null;
        }

        /// <summary>
        /// Is a variable declared in the given scope visible at the current level?
        /// </summary>
        /// <param name="scope">Scope to see if still visible</param>
        /// <returns></returns>
        /// <remarks>Check the booking scope - anywhere on the stack?</remarks>
        public bool InScopeNow(IScopeInfo scope)
        {
            var s = scope as CurrentScopeInfo;
            var bs = CurrentDeclarationScopePointer;

            while (bs != null)
            {
                if (bs == s.BookingScope)
                    return true;
                var p = bs.Parent;
                while (p != null && !(p is IBookingStatementBlock))
                    p = p.Parent;
                bs = p as IBookingStatementBlock;
            }
            return false;
        }

        /// <summary>
        /// Find top most booking block where all the variables are declared. We assume that all are
        /// declared at some point. Return may be invalid if these are from different scope "branches".
        /// </summary>
        /// <param name="variables">List of variables that must be valid in the block returned</param>
        /// <returns>The upper most booking block where all the variables listed are declared.</returns>
        public IBookingStatementBlock FirstAllInScopeFromNow(IEnumerable<IDeclaredParameter> variables)
        {
            var bb = CurrentDeclarationScopePointer;
            while (bb != null)
            {
                if (bb.DeclaredVariables.Any(v => variables.Contains(v)))
                {
                    return bb;
                }

                var p = bb.Parent;
                while (p != null && !(p is IBookingStatementBlock))
                {
                    p = p.Parent;
                }
                bb = p as IBookingStatementBlock;
            }

            // If we are here, then nothing was declared the whole way up.

            //throw new ArgumentException("Can't find a booking block for a set of variables that are never declared.");
            return null;
        }

        /// <summary>
        /// Debugging routine to dump the code to a given file. Can be called directly from within
        /// the debugger.
        /// </summary>
        /// <param name="output"></param>
        internal void DumpToFile(System.IO.FileInfo output)
        {
            using (var writer = output.CreateText())
            {
                foreach (var l in CodeBody.CodeItUp())
                {
                    writer.WriteLine(l);
                }
                writer.Close();
            }
        }

        /// <summary>
        /// Track the list of functions we know about
        /// </summary>
        private List<QMFuncSource> _qmFunctions = new List<QMFuncSource>();

        /// <summary>
        /// All QM Methods that are going to be written up for this code.
        /// </summary>
        public IEnumerable<QMFuncSource> QMFunctions { get { return _qmFunctions; } }

        /// <summary>
        /// Add a new QueryModel function.
        /// </summary>
        /// <param name="qMFuncSource"></param>
        internal void Add(QMFuncSource qMFuncSource)
        {
            _qmFunctions.Add(qMFuncSource);
        }

        /// <summary>
        /// Return a QM function if we can find a list.
        /// </summary>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        public IQMFunctionSource FindQMFunction(QueryModel queryModel)
        {
            var qmText = FormattingQueryVisitor.Format(queryModel);
            return QMFunctions.Where(ff => ff.Matches(qmText)).FirstOrDefault();
        }

        /// <summary>
        /// Get the list of functions, as exectuable sources.
        /// </summary>
        public IEnumerable<IQMFuncExecutable> Functions
        {
            get { return QMFunctions; }
        }
    }
}
