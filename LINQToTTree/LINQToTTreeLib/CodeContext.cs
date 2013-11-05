using LinqToTTreeInterfacesLib;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace LINQToTTreeLib
{
    /// <summary>
    /// Keeps track of some scoping stuff - like names of parameters, etc.
    /// </summary>
    public class CodeContext : ICodeContext
    {
        /// <summary>
        /// Get the # of parameter replacements we know about.
        /// </summary>
        public int NumberOfParams { get { return _expressionReplacement.Count; } }

        /// <summary>
        /// Scope holder that will pop the stuff off we need popped off.
        /// </summary>
        private class CCReplacementQuery : IVariableScopeHolder
        {
            private CodeContext codeContext;
            private IQuerySource query;
            private Expression expression;

            /// <summary>
            /// Keep track of what to do to pop this guy off
            /// </summary>
            /// <param name="codeContext"></param>
            /// <param name="query"></param>
            /// <param name="expression"></param>
            public CCReplacementQuery(CodeContext codeContext, IQuerySource query, Expression expression)
            {
                this.codeContext = codeContext;
                this.query = query;
                this.expression = expression;
            }

            /// <summary>
            /// Remove the variable.
            /// </summary>
            public void Pop()
            {
                if (expression == null)
                {
                    codeContext.DeleteValue(query);
                }
                else
                {
                    codeContext.Add(query, expression);
                }
            }
        }


        /// <summary>
        /// Keep track of a context
        /// </summary>
        private class CCReplacementExpression : IVariableScopeHolder
        {
            private CodeContext _context;
            private string _varName;
            private Expression _oldVal;

            public CCReplacementExpression(CodeContext codeContext, string varName, Expression iValue)
            {
                _context = codeContext;
                _varName = varName;
                _oldVal = iValue;
            }

            /// <summary>
            /// Go back to what we had!
            /// </summary>
            public void Pop()
            {
                if (_oldVal != null)
                    _context.Add(_varName, _oldVal);
                else
                    _context.DeleteExpression(_varName);
            }
        }

        /// <summary>
        /// Cache for later use a query model change.
        /// </summary>
        private class CQReplacementExpression : IVariableScopeHolder
        {
            private CodeContext _context;
            private QueryModel _model;
            private QueryModelCacheLine _oldVal;

            public CQReplacementExpression(CodeContext context, QueryModel model, QueryModelCacheLine val)
            {
                _context = context;
                _model = model;
                _oldVal = val;
            }

            public void Pop()
            {
                if (_oldVal != null)
                    _context.AddInternal(_model, _oldVal);
                else
                    _context.DeleteValue(_model);
            }
        }

        /// <summary>
        /// Delete a variable name if it is in there.
        /// </summary>
        /// <param name="vName"></param>
        internal void Delete(string vName)
        {
            DeleteExpression(vName);
        }

        /// <summary>
        /// Delete an expression mapping.
        /// </summary>
        /// <param name="vName"></param>
        internal void DeleteExpression(string vName)
        {
            _expressionReplacement.Remove(vName);
        }

        /// <summary>
        /// Delete a previously held query.
        /// </summary>
        /// <param name="query"></param>
        internal void DeleteValue(IQuerySource query)
        {
            _queryReplacement.Remove(query);
        }

        /// <summary>
        /// Get the current loop variable. Is null only at the very start!
        /// </summary>
        public Expression LoopVariable { get; private set; }

        /// <summary>
        /// Returns the current loop index - the integer thing we are walking over.
        /// </summary>
        public Expression LoopIndexVariable { get; private set; }

        /// <summary>
        /// Set the loop variable to a new value
        /// </summary>
        /// <param name="loopExpression">Evaluate to the current value, looping over whatever we are looping over</param>
        /// <param name="indexVariable">The current index - this is what we are walking through right now. Null if this index variable makes no sense.</param>
        public void SetLoopVariable(Expression loopExpression, Expression indexVariable)
        {
            if (loopExpression == null)
                throw new ArgumentNullException("can not set a null loop variable");

            Debug.WriteLine("SetLoopVariable: Index changing from {0} => {1}", LoopIndexVariable == null ? "null" : LoopIndexVariable.ToString(), indexVariable == null ? "null" : indexVariable.ToString());
            LoopVariable = loopExpression;
            LoopIndexVariable = indexVariable;

        }

        private Dictionary<string, Expression> _expressionReplacement = new Dictionary<string, Expression>();

        /// <summary>
        /// Add a new expression to our holding library for later replacement.
        /// </summary>
        /// <param name="indexName"></param>
        /// <param name="indexExpression"></param>
        /// <returns></returns>
        public IVariableScopeHolder Add(string indexName, Expression indexExpression)
        {
            if (indexExpression == null)
                throw new ArgumentException("expr can't be null");

            return AddInternal(indexName, indexExpression);
        }

        /// <summary>
        /// Save a query source for later lookup.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public IVariableScopeHolder Add(IQuerySource query, Expression expression)
        {
            if (expression == null)
                throw new ArgumentNullException("ecpr can't be null");
            if (query == null)
                throw new ArgumentNullException("query");

            return AddInternal(query, expression);
        }

        /// <summary>
        /// Tracks the information for a query model lookup
        /// </summary>
        class QueryModelCacheLine
        {
            /// <summary>
            /// The expression this QM will resolve to.
            /// </summary>
            public Expression _value;

            /// <summary>
            /// The Query Sources that are referenced by this guy
            /// </summary>
            public IQuerySource[] _referencedQS;
        }

        /// <summary>
        /// Returns the query model cache.
        /// </summary>
        private Dictionary<QueryModel, QueryModelCacheLine> _queryModelCache = new Dictionary<QueryModel, QueryModelCacheLine>();

        /// <summary>
        /// Save a query model result for later lookup.
        /// </summary>
        /// <param name="queryModel"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        /// <remarks>Cache any QueryReferences that have been looked up in addtion to the result so we know when to
        /// invalidate this.</remarks>
        public IVariableScopeHolder Add(Remotion.Linq.QueryModel queryModel, Expression result)
        {
            var scope = new CQReplacementExpression(this, queryModel, _queryModelCache.ContainsKey(queryModel) ? _queryModelCache[queryModel] : null);
            var v = new QueryModelCacheLine() { _value = result, _referencedQS = _QSReferencedSet.ToArray() };
            _QSReferencedSet.Clear();
            AddInternal(queryModel, v);
            return scope;
        }

        private void AddInternal(Remotion.Linq.QueryModel queryModel, QueryModelCacheLine v)
        {
            _queryModelCache[queryModel] = v;
            Debug.WriteLine("Caching: QM {0} => {1}", queryModel.ToString(), v._value.ToString());
        }

        /// <summary>
        /// Delete a query model cached expression.
        /// </summary>
        /// <param name="_model"></param>
        public void DeleteValue(QueryModel _model)
        {
            if (_queryModelCache.ContainsKey(_model))
                _queryModelCache.Remove(_model);
        }

        /// <summary>
        /// Remove the definition of a internal variable, and return a popper to allow us
        /// to restore the magic! :-)
        /// </summary>
        /// <param name="varName"></param>
        /// <returns></returns>
        public IVariableScopeHolder Remove(string varName)
        {
            return AddInternal(varName, null);
        }

        /// <summary>
        /// Replace the the variable name with the requested expression.
        /// </summary>
        /// <param name="varName"></param>
        /// <param name="replacementExpr"></param>
        /// <returns></returns>
        public IVariableScopeHolder AddInternal(string varName, Expression replacementExpr)
        {
            Debug.WriteLine("Cache Expression: {0} => {1}", varName, replacementExpr);

            ///
            /// Somethign to get us back to this state
            /// 

            IVariableScopeHolder popper = null;
            if (_expressionReplacement.ContainsKey(varName))
            {
                popper = new CCReplacementExpression(this, varName, _expressionReplacement[varName]);
            }
            else
            {
                popper = new CCReplacementExpression(this, varName, null);
            }

            ///
            /// And save the expression for future lookup
            /// 

            if (replacementExpr != null)
            {
                _expressionReplacement[varName] = replacementExpr;
            }
            else
            {
                _expressionReplacement.Remove(varName);
            }

            return popper;
        }

        /// <summary>
        /// Keep track of the queries we are storing.
        /// </summary>
        private Dictionary<IQuerySource, Expression> _queryReplacement = new Dictionary<IQuerySource, Expression>();

        /// <summary>
        /// Replace or add the variable as requested.
        /// </summary>
        /// <param name="varName"></param>
        /// <param name="replacementExpr"></param>
        /// <returns></returns>
        public IVariableScopeHolder AddInternal(IQuerySource query, Expression replacementExpr)
        {
            Debug.WriteLine("Caching QS {0} => {1}", query.ToString(), replacementExpr.ToString());

            // If this QS is referenced by a QM cache line, then we need to invalidate that QM.

            if (_queryReplacement.ContainsKey(query))
            {
                var badQM = (from q in _queryModelCache
                             where q.Value._referencedQS.Contains(query)
                             select q.Key).ToArray();
                Debug.Indent();
                foreach (var qm in badQM)
                {
                    Debug.WriteLine("Removing QM due to change in QS: {0}", qm.ToString());
                    _queryModelCache.Remove(qm);
                }
                Debug.Unindent();
            }

            ///
            /// Somethign to get us back to this state
            /// 

            IVariableScopeHolder popper = null;
            if (_queryReplacement.ContainsKey(query))
            {
                popper = new CCReplacementQuery(this, query, _queryReplacement[query]);
            }
            else
            {
                popper = new CCReplacementQuery(this, query, null);
            }

            ///
            /// And save the expression for future lookup
            /// 

            if (replacementExpr != null)
            {
                _queryReplacement[query] = replacementExpr;
            }
            else
            {
                _queryReplacement.Remove(query);
            }

            return popper;
        }

        /// <summary>
        /// Return a replacement item. If it doesn't exist, return null.
        /// </summary>
        /// <param name="varname"></param>
        /// <returns></returns>
        public System.Linq.Expressions.Expression GetReplacement(string varname)
        {
            if (!_expressionReplacement.ContainsKey(varname))
                return null;
            Debug.WriteLine("Cache Lookup {0} => {1}", varname, _expressionReplacement[varname]);
            return _expressionReplacement[varname];
        }

        /// <summary>
        /// Return the value for a query reference
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public Expression GetReplacement(IQuerySource query)
        {
            if (!_queryReplacement.ContainsKey(query))
                return null;
            Debug.WriteLine("Cache Lookup QS {0} => {1}", query.ToString(), _queryReplacement[query]);
            _QSReferencedSet.Add(query);
            return _queryReplacement[query];
        }

        /// <summary>
        /// Get back a query replacement. Null we can't find it.
        /// </summary>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        public Expression GetReplacement(QueryModel queryModel)
        {
            QueryModelCacheLine result = null;
            if (_queryModelCache.TryGetValue(queryModel, out result))
            {
                Debug.WriteLine("Cache Lookup QM Would have returned {0} => {1}", queryModel.ToString(), result.ToString());
                Debug.Indent();
                foreach (var qs in _queryReplacement)
                {
                    Debug.WriteLine("QS {0} => {1}", qs.Key, qs.Value);
                }
                Debug.Unindent();
                return result._value;
            }
            return null;
        }

        /// <summary>
        /// Track the list of IQuerySource's that were looked up since we got started.
        /// </summary>
        private HashSet<IQuerySource> _QSReferencedSet = new HashSet<IQuerySource>();

        /// <summary>
        /// Returns the list of query sources that have been referenced, and then zero's out the internal
        /// list.
        /// </summary>
        /// <returns></returns>
        public IQuerySource[] GetAndResetQuerySourceLookups()
        {
            var r = _QSReferencedSet.ToArray();
            _QSReferencedSet.Clear();
            return r;
        }

        /// <summary>
        /// Restore the referenced list of QS with the input list.
        /// </summary>
        /// <param name="qsList"></param>
        public void RestoreQuerySourceLookups(IEnumerable<IQuerySource> qsList)
        {
            foreach (var qs in qsList)
            {
                _QSReferencedSet.Add(qs);
            }
        }

        private List<string> _cachedCookies = new List<string>();

        /// <summary>
        /// Cookies left behind from the translation that should be used when we
        /// calculate a cache hit for this item.
        /// </summary>
        public List<string> CacheCookies
        {
            get { return _cachedCookies; }
        }

        /// <summary>
        /// The base type for the ntuple we are looping over.
        /// </summary>
        public Type BaseNtupleObjectType { get; set; }

        #region Cached Varaible Scoping

        private List<IVariableScopeHolder> _cachedScopeVars = new List<IVariableScopeHolder>();

        /// <summary>
        /// Add a variable that should be eliminated.
        /// </summary>
        /// <param name="variableScopeHolder"></param>
        public void CacheVariableToEliminate(IVariableScopeHolder variableScopeHolder)
        {
            if (variableScopeHolder == null)
                throw new ArgumentNullException("variableScopeHolder");

            _cachedScopeVars.Add(variableScopeHolder);
        }

        /// <summary>
        /// Return the current list, and elminate the current list.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IVariableScopeHolder> ResetCachedVariableList()
        {
            var r = _cachedScopeVars;
            _cachedScopeVars = new List<IVariableScopeHolder>();
            return r;
        }

        /// <summary>
        /// Load variables from the list given.
        /// </summary>
        /// <param name="cachedScopedVariables"></param>
        public void LoadCachedVariableList(IEnumerable<IVariableScopeHolder> cachedScopedVariables)
        {
            _cachedScopeVars.AddRange(cachedScopedVariables);
        }
        /// <summary>
        /// Pop all the variables on the cached list.
        /// </summary>
        public void PopCachedVariableList()
        {
            foreach (var v in _cachedScopeVars)
            {
                //v.Pop();
            }
            _cachedScopeVars.Clear();
        }
        #endregion

    }
}
