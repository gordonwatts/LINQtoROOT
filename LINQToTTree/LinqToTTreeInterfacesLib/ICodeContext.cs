﻿using Remotion.Linq;
using Remotion.Linq.Clauses;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LinqToTTreeInterfacesLib
{
    /// <summary>
    /// When we do a parameter replacement, use this to pop out the
    /// value that we put in and replace with a previous definition, if this
    /// guy hid the old one.
    /// </summary>
    public interface IVariableScopeHolder
    {
        void Pop();
    }

    /// <summary>
    /// Context for coding - this is the temporary stuff that follows along during code generation... but doesn't
    /// need to be kept around once we are done.
    /// </summary>
    public interface ICodeContext
    {
        /// <summary>
        /// Add a variable mapping to an expression. Used for dealing with parameters and the like.
        /// Use the return object to pop it off the stack when you are done.
        /// </summary>
        /// <param name="indexName"></param>
        /// <param name="indexExpression"></param>
        /// <returns></returns>
        IVariableScopeHolder Add(string indexName, Expression indexExpression);

        /// <summary>
        /// Add a query for later lookup. We use the query as a lookup
        /// here b/c the user may use the same index variable in a
        /// query (scoping).
        /// </summary>
        /// <param name="query"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        IVariableScopeHolder Add(IQuerySource query, Expression expression);

        /// <summary>
        /// We've parsed a sub-query and gotten a result. Cache it incase the sub-query
        /// appears again in later code.
        /// </summary>
        /// <param name="queryModel"></param>
        /// <param name="result"></param>
        IVariableScopeHolder Add(QueryModel queryModel, Expression result);

        /// <summary>
        /// Returns the expression that has been stored under this name.
        /// Returns null if the translation does not exist.
        /// </summary>
        /// <param name="varname"></param>
        /// <returns></returns>
        Expression GetReplacement(string varname);

        /// <summary>
        /// A query reference - can't use a name (as in an argument). Instead, we must use 
        /// </summary>
        /// <param name="exprName"></param>
        /// <returns></returns>
        Expression GetReplacement(IQuerySource exprName);

        /// <summary>
        /// If we have seen this query model before, return a sub-expression for it.
        /// </summary>
        /// <param name="queryModel"></param>
        /// <returns>the expression the contains the result, or null if we don't know about it yet</returns>
        Expression GetReplacement(QueryModel queryModel);

        /// <summary>
        /// Return true if the QM passed in is the top level query model.
        /// </summary>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        bool IsInTopLevelQueryModel(QueryModel queryModel);

        /// <summary>
        /// Returns a list of query expressions that have been referenced via GetReplacement (IQuerySource).
        /// </summary>
        /// <returns></returns>
        IQuerySource[] GetAndResetQuerySourceLookups();

        /// <summary>
        /// Place the list of query sources back on the internal list.
        /// </summary>
        /// <param name="qsList"></param>
        void RestoreQuerySourceLookups(IEnumerable<IQuerySource> qsList);

        /// <summary>
        /// Remove an expression from the repository - allows restoration via the
        /// scope holder
        /// </summary>
        /// <param name="indexName"></param>
        /// <returns></returns>
        IVariableScopeHolder Remove(string indexName);

        /// <summary>
        /// Get the current index loop variable - evaluates to
        /// whatever it is our current expression.
        /// </summary>
        Expression LoopVariable { get; }

        /// <summary>
        /// Returns the index variable that we are using to run the current loop variable.
        /// </summary>
        IDeclaredParameter LoopIndexVariable { get; }

        /// <summary>
        /// Set the current loop variable to be something new
        /// </summary>
        /// <param name="loopVariable">The current value of the expression we are using in the loop</param>
        /// <param name="indexVariable">The integer expression that is running the above loop variable</param>
        void SetLoopVariable(Expression loopVariable, IDeclaredParameter indexVariable);

        /// <summary>
        /// Keep track of cookies that should be taken into account when
        /// calculating the cache.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        List<string> CacheCookies { get; }

        /// <summary>
        /// Returns the type that we are looping over - the base type. This is an implied loop - so we
        /// just avoid making an actual loop over it. :-)
        /// </summary>
        Type BaseNtupleObjectType { get; }

        /// <summary>
        /// Add a variable to the cache that should be eliminated in the end.
        /// </summary>
        /// <param name="variableScopeHolder"></param>
        void CacheVariableToEliminate(IVariableScopeHolder variableScopeHolder);

        /// <summary>
        /// Reset the cached variable list to zero - also return the current list.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IVariableScopeHolder> ResetCachedVariableList();

        /// <summary>
        /// Pop all the variables on the cached list out of the code context.
        /// </summary>
        void PopCachedVariableList();

        /// <summary>
        /// Given a list, add it to the cache.
        /// </summary>
        /// <param name="cachedScopedVariables"></param>
        void LoadCachedVariableList(IEnumerable<IVariableScopeHolder> cachedScopedVariables);

        /// <summary>
        /// Return a query result cache key. This is a promise to return it. It will only
        /// succeed after it has been set (after the complete expression has been visited).
        /// </summary>
        /// <returns></returns>
        Func<IQueryResultCacheKey> CacheKeyFuture { get; set; }
    }
}
