using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Remotion.Linq.Clauses;

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
    /// Context for coding - this is the temprorary stuff that follows along during code generation... but doesn't
    /// need to be kept around once we are done.
    /// </summary>
    public interface ICodeContext
    {
        /// <summary>
        /// Add a variable mapping. Used most often for dealing with parameters and the like
        /// </summary>
        /// <param name="varName"></param>
        /// <param name="replacementName"></param>
        IVariableScopeHolder Add(string varName, IValue replacementName);

        /// <summary>
        /// Add a vairable mapping to an expression. Used for dealing with parameters and the like.
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
        /// Lookup a replacement
        /// </summary>
        /// <param name="varname"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        IValue GetReplacement(string varname, Type type);

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
        /// Remove an expression from the repository - allows restoration via the
        /// scope holder
        /// </summary>
        /// <param name="indexName"></param>
        /// <returns></returns>
        IVariableScopeHolder Remove(string indexName);

        /// <summary>
        /// Get the current index loop variable.
        /// </summary>
        Expression LoopVariable { get; }

        /// <summary>
        /// Set the current loop variable to be something new
        /// </summary>
        /// <param name="v"></param>
        void SetLoopVariable(Expression v);

        /// <summary>
        /// Keep track of cookies that should be taken into account when
        /// calculating the cache.
        /// </summary>
        List<string> CacheCookies { get; }
    }
}
