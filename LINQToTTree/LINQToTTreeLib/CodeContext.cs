using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using LinqToTTreeInterfacesLib;
using Remotion.Linq;
using Remotion.Linq.Clauses;

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
        /// Returns the query model cache
        /// </summary>
        private Dictionary<QueryModel, Expression> _queryModelCache = new Dictionary<QueryModel, Expression>();

        /// <summary>
        /// Save a query model result for later lookup.
        /// </summary>
        /// <param name="queryModel"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public IVariableScopeHolder Add(Remotion.Linq.QueryModel queryModel, Expression result)
        {
            _queryModelCache[queryModel] = result;
            return null;
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
            return _queryReplacement[query];
        }

        /// <summary>
        /// Get back a query replacement. Null we can't find it.
        /// </summary>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        public Expression GetReplacement(QueryModel queryModel)
        {
            Expression result = null;
            if (_queryModelCache.TryGetValue(queryModel, out result))
                return result;
            return null;
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
    }
}
