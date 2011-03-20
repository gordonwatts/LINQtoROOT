using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using LinqToTTreeInterfacesLib;

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
        Dictionary<string, Expression> _parameterReplacement = new Dictionary<string, Expression>();

        /// <summary>
        /// Get the # of parameter replacements we know about.
        /// </summary>
        public int NumberOfParams { get { return _parameterReplacement.Count; } }

        /// <summary>
        /// Add a parameter replacement to the list.
        /// </summary>
        /// <param name="varName"></param>
        /// <param name="replacementName"></param>
        public IVariableScopeHolder Add(string varName, Expression replacementName)
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
                _codeContext.Delete(_vName);
            }
        }

        /// <summary>
        /// Keep track of a context
        /// </summary>
        private class CCReplacement : IVariableScopeHolder
        {
            private CodeContext _context;
            private string _varName;
            private Expression _oldVal;

            public CCReplacement(CodeContext codeContext, string varName, Expression iValue)
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
        /// Get a replacement substitution - given the name and expected type.
        /// </summary>
        /// <param name="varname"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public Expression GetReplacement(string varname, Type type)
        {
            if (varname == null)
                throw new ArgumentNullException("Can't lookup a null var name!");

            if (type == null)
                throw new ArgumentNullException("Can't lookup a var name with a null type!");

            if (varname.Length == 0)
                throw new ArgumentException("Variables must be non-zero length!");

            if (!_parameterReplacement.ContainsKey(varname))
                return Expression.Variable(type, varname);
            var result = _parameterReplacement[varname];

            if (result.Type != type)
                throw new InvalidOperationException("Can't convert parameter from type '" + result.Type.Name + "' to '" + type.Name + "'.");

            return result;
        }

        /// <summary>
        /// Delete a variable name if it is in there.
        /// </summary>
        /// <param name="_vName"></param>
        internal void Delete(string _vName)
        {
            _parameterReplacement.Remove(_vName);
        }

        /// <summary>
        /// Get the current loop variable. Is null only at the very start!
        /// </summary>
        public IVariable LoopVariable { get; private set; }

        /// <summary>
        /// Set the loop variable to a new value
        /// </summary>
        /// <param name="v"></param>
        public void SetLoopVariable(IVariable v)
        {
            if (v == null)
                throw new ArgumentNullException("can not set a null loop variable");

            LoopVariable = v;
        }
    }
}
