using System;
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
        IVariableScopeHolder Add(string varName, Expression replacementName);

        /// <summary>
        /// Add a vairable mapping to an expression. Used for dealing with parameters and the like.
        /// Use the return object to pop it off the stack when you are done.
        /// </summary>
        /// <param name="indexName"></param>
        /// <param name="indexExpression"></param>
        /// <returns></returns>
        IVariableScopeHolder Add(string indexName, Expression indexExpression);

        /// <summary>
        /// Lookup a replacement
        /// </summary>
        /// <param name="varname"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Expression GetReplacement(string varname, Type type);

        /// <summary>
        /// Returns the expression that has been stored under this name.
        /// Returns null if the translation does not exist.
        /// </summary>
        /// <param name="varname"></param>
        /// <returns></returns>
        Expression GetReplacement(string varname);

        /// <summary>
        /// Get the current index loop variable.
        /// </summary>
        IVariable LoopVariable { get; }

        /// <summary>
        /// Set the current loop variable to be something new
        /// </summary>
        /// <param name="v"></param>
        void SetLoopVariable(IVariable v);


    }
}
