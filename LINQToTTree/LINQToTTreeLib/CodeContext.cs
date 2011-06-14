using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Variables;

namespace LINQToTTreeLib
{
    /// <summary>
    /// Keeps track of some scoping stuff - like names of parameters, etc.
    /// </summary>
    public class CodeContext : ICodeContext
    {
        /// <summary>
        /// Keep track of all parameters we need to know about.
        /// </summary>
        Dictionary<string, IValue> _parameterReplacement = new Dictionary<string, IValue>();

        /// <summary>
        /// Get the # of parameter replacements we know about.
        /// </summary>
        public int NumberOfParams { get { return _parameterReplacement.Count; } }

        /// <summary>
        /// Add a parameter replacement to the list.
        /// </summary>
        /// <param name="varName"></param>
        /// <param name="replacementName"></param>
        public IVariableScopeHolder Add(string varName, IValue replacementName)
        {
            if (varName == null || replacementName == null)
                throw new ArgumentNullException("Can't setup an Add that is null!");

            if (varName.Length == 0)
                throw new ArgumentException("var and replacement must be real strings");

            ///
            /// See if there was somethign there already that we are going to replace
            /// 

            IVariableScopeHolder popper = null;
            if (_parameterReplacement.ContainsKey(varName))
            {
                popper = new CCReplacement(this, varName, _parameterReplacement[varName]);
            }
            else
            {
                popper = new CCReplacementNull(this, varName);
            }

            ///
            /// Ok, now do the replacement
            /// 

            _parameterReplacement[varName] = replacementName;

            return popper;
        }

        /// <summary>
        /// We have no context to keep track of.
        /// </summary>
        private class CCReplacementNull : IVariableScopeHolder
        {
            private string _vName;
            private CodeContext _codeContext;

            public CCReplacementNull(CodeContext codeContext, string varName)
            {
                // TODO: Complete member initialization
                _codeContext = codeContext;
                _vName = varName;
            }
            public void Pop()
            {
                _codeContext.DeleteValue(_vName);
            }
        }

        /// <summary>
        /// Keep track of a context
        /// </summary>
        private class CCReplacement : IVariableScopeHolder
        {
            private CodeContext _context;
            private string _varName;
            private IValue _oldVal;

            public CCReplacement(CodeContext codeContext, string varName, IValue iValue)
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
                _context.Add(_varName, _oldVal);
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
        /// Get a replacement substitution - given the name and expected type.
        /// </summary>
        /// <param name="varname"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public IValue GetReplacement(string varname, Type type)
        {
            if (varname == null)
                throw new ArgumentNullException("Can't lookup a null var name!");

            if (type == null)
                throw new ArgumentNullException("Can't lookup a var name with a null type!");

            if (varname.Length == 0)
                throw new ArgumentException("Variables must be non-zero length!");

            if (!_parameterReplacement.ContainsKey(varname))
                return new ValSimple(varname, type);
            var result = _parameterReplacement[varname];

            if (result.Type != type)
                throw new InvalidOperationException("Can't convert parameter from type '" + result.Type.Name + "' to '" + type.Name + "'.");

            return result;
        }

        /// <summary>
        /// Delete a variable name if it is in there.
        /// </summary>
        /// <param name="vName"></param>
        internal void Delete(string vName)
        {
            DeleteExpression(vName);
            DeleteValue(vName);
        }

        internal void DeleteExpression(string vName)
        {
            _expressionReplacement.Remove(vName);
        }

        internal void DeleteValue(string vName)
        {
            _parameterReplacement.Remove(vName);
        }

        /// <summary>
        /// Get the current loop variable. Is null only at the very start!
        /// </summary>
        public Expression LoopVariable { get; private set; }

        /// <summary>
        /// Set the loop variable to a new value
        /// </summary>
        /// <param name="v"></param>
        public void SetLoopVariable(Expression v)
        {
            if (v == null)
                throw new ArgumentNullException("can not set a null loop variable");

            LoopVariable = v;
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

        private List<string> _cachedCookies = new List<string>();

        /// <summary>
        /// Cookies left behind from the translation that should be used when we
        /// calculate a cache hit for this item.
        /// </summary>
        public List<string> CacheCookies
        {
            get { return _cachedCookies; }
        }
    }
}
